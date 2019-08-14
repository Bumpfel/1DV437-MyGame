using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Combatant {
    public bool m_IsAsleep = false;

    private const float WakeUpDelayOnTakingDmg = .4f;
    private const float WakeUpDelayOnCollider = 1;
    private const float AlertedTime = 5;
    private const float ReactionTime = .2f;
    private const float ReactionTurnDuration = .4f;
    private GameObject m_AsleepIndicator;
    private EnemyMovement m_EnemyMovement;
    private FieldOfView m_FOV;
    private Transform m_Target;
    private Quaternion m_StartRotation;
    private Attack m_Attack;
 
    private bool m_IsWakingUp = false;
    private bool m_IsAlerted = false;
    // private bool m_IsInvestigating;
    private bool m_RecentlyDetectedPlayer = false;
    private float m_LastReaction;
    private const float FollowTargetSpeed = 0.2f;
    private const float MaxAngleDifferenceToTargetBeforeShooting = 15;
    private float m_AngleDifferenceToTarget;

    private new void Start() {
        base.Start();
        m_Attack = GetComponent<Attack>();
        m_FOV = GetComponent<FieldOfView>();
        m_EnemyMovement = GetComponent<EnemyMovement>();

        m_AsleepIndicator = m_CharacterGUI.transform.Find("Asleep").gameObject;
        if(m_IsAsleep) {
            m_AsleepIndicator.SetActive(true);
            foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
                if(script != this) {
                    script.enabled = false;
                }
            }
        }
        m_StartRotation = transform.rotation;
    }

    private void Update() {
        ReactToVisibleTargets();
    }

    private void ReactToVisibleTargets() {
        if(HasTargetInSight()) {
            if(!m_RecentlyDetectedPlayer) {
                m_Target = m_FOV.m_VisibleTargets[0];
                m_IsAlerted = true;
                print("spotted target. stopping all coroutines");
                
                m_EnemyMovement.Halt();
                m_RecentlyDetectedPlayer = true;
                // print(name + " detected " + m_Target.name); // debug
            }
            StopAllCoroutines();
            FollowTarget();
            m_LastReaction = Time.time;

            if(CloseEnoughToShoot())
                m_Attack.Fire();
        }
        else if(m_RecentlyDetectedPlayer && Time.time > m_LastReaction + AlertedTime) {
            m_RecentlyDetectedPlayer = false;
            m_IsAlerted = false;
            print(name + "reacted to visible target. returning to patrol");
            m_EnemyMovement.ReturnToPatrol();
            // StopAutomaticFire(); // just to stop firing animation
            // print(name + " is returning to patrol"); // debug
        }
    }

    // TurnTowardsTarget vars
    Quaternion orgRotation;
    Quaternion targetRotation;

    private void FollowTarget() {
        targetRotation = Quaternion.LookRotation(m_Target.position - transform.position, Vector3.up);
        m_AngleDifferenceToTarget = Mathf.Abs(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, FollowTargetSpeed);
    }

    public IEnumerator TurnTowards(Vector3 point, float turnDuration) {
        float timestamp = Time.time;
        float timeTaken = 0;

        point.y = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(point - transform.position, Vector3.up);

        // m_IsInvestigating = true;
        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
        // transform.position = new Vector3(transform.position.x, 0, transform.position.z); //TODO nödlösning för att en gubbe verkar ibland ändra y-position och därmed ser fov fel ut
    }
  
    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag != "Ignored" && m_IsAsleep && !m_IsWakingUp) // && !IsDead()
            StartCoroutine(WakeUp(WakeUpDelayOnCollider, false, transform.forward));
    }

    public override void TakeDamage(float amount, Vector3 dmgSource) {
        if(!m_IsWakingUp) {  // && !IsDead()
            // base.TakeDamage(amount, dmgSource);
            if(!HasTargetInSight()) {
                if(m_IsAsleep)
                    StartCoroutine(WakeUp(WakeUpDelayOnTakingDmg, true, dmgSource));
                else {
                    StopAllCoroutines();
                    StartCoroutine(ReactToTakingDamage(dmgSource));
                }
            }
        }
    }
    // Coroutine m_ActiveCoroutine;

    private IEnumerator WakeUp(float delay, bool tookDmg, Vector3 dmgSource) {
        print("WakeUp()");
        m_AsleepIndicator.SetActive(false);
        m_IsWakingUp = true;
        yield return new WaitForSeconds(delay);
        foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
            script.enabled = true;
        }
        m_IsAsleep = false;
        m_IsWakingUp = false;
        if(tookDmg) {
            StartCoroutine(ReactToTakingDamage(dmgSource));
        }
    }

    private IEnumerator ReactToTakingDamage(Vector3 dmgSource) {
        print("ReactToTakingDamage(). ");
        m_LastReaction = Time.time;
        if(!m_IsAlerted) {
            m_IsAlerted = true;
            m_EnemyMovement.Halt();
            yield return new WaitForSeconds(ReactionTime);
        }
        yield return TurnTowards(dmgSource, ReactionTurnDuration);
        // if(!m_IsInvestigating)
            // yield return FireBlindly(3);
        // m_IsInvestigating = false;
        yield return new WaitForSeconds(AlertedTime);
        m_IsAlerted = false;
        print(name + " reacted to dmg. returning to patrol");
        m_EnemyMovement.ReturnToPatrol();

    }

    private IEnumerator FireBlindly(int shots) {
        for(int i = 0; i < shots; i ++) {
            m_Attack.Fire();
            yield return new WaitForSeconds(m_Attack.m_FireRate);
        }
    }

    protected override void Die() {
        base.Die();
        transform.Find("FOVVisualization").gameObject.SetActive(false);
        m_GameController.m_PlayerStats.AddKill();
        
        foreach(MonoBehaviour component in GetComponents<MonoBehaviour>()) {
            // component.enabled = false;
            Destroy(component);
        }
    }
    
    public bool IsAlerted() {
        return m_IsAlerted;
    }

    private bool CloseEnoughToShoot() {
        return m_AngleDifferenceToTarget < MaxAngleDifferenceToTargetBeforeShooting;
    }

    private bool HasTargetInSight() {
         return m_FOV && m_FOV.m_VisibleTargets.Count > 0;
    }
}