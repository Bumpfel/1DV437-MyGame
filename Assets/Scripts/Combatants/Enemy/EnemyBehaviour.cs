using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Combatant {
    public bool m_IsAsleep = false;

    private const float WakeUpDelayOnTakingDmg = .4f;
    private const float WakeUpDelayOnCollider = 1;
    private const float AlertedTime = 5;
    private const float ReactionTime = .2f;
    private const float ReactionTurnDuration = 1.5f;
    private const float FollowTargetSpeed = .2f;
    private const float MaxAngleToTargetBeforeShooting = 15;
    private GameObject m_AsleepIndicator;
    private EnemyPatrol m_EnemyMovement;
    private FieldOfView m_FOV;
    private Transform m_Target;
    private Quaternion m_StartRotation;
    private Attack m_Attack;
 
    private bool m_IsWakingUp = false;
    public bool IsAlerted { get; private set; }

    private bool m_IsTurning = false;
    private bool m_RecentlyDetectedPlayer = false;
    private float m_LastReaction;
    private float m_AngleToTarget;

    private bool IsCloseEnoughToShoot => m_AngleToTarget < MaxAngleToTargetBeforeShooting;
    private bool HasTargetInSight => m_FOV.HasTargetInSight;
    private bool EnoughTimeHasGoneBySinceLastReaction => m_RecentlyDetectedPlayer && Time.time > m_LastReaction + AlertedTime;

    private new void Start() {
        base.Start();
        m_Attack = GetComponent<Attack>();
        m_FOV = GetComponent<FieldOfView>();
        m_EnemyMovement = GetComponent<EnemyPatrol>();

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
        if(Time.timeScale == 0)
            return;
        ReactToVisibleTargets();
    }

    private void ReactToVisibleTargets() {
        if(HasTargetInSight) {
            if(!m_RecentlyDetectedPlayer) {
                m_Target = m_FOV.VisibleTarget;
                IsAlerted = true;
                
                m_EnemyMovement.Halt();
                m_RecentlyDetectedPlayer = true;
            }
            StopAllCoroutines();
            FollowTarget();
            m_LastReaction = Time.time;

            if(IsCloseEnoughToShoot)
                m_Attack.Fire();
        }
        else if(EnoughTimeHasGoneBySinceLastReaction) {
            m_RecentlyDetectedPlayer = false;
            IsAlerted = false;
            m_EnemyMovement.ReturnToPatrol();
        }
    }

    // FollowTarget vars
    Quaternion targetRotation;
    private void FollowTarget() {
        targetRotation = Quaternion.LookRotation(m_Target.position - transform.position, Vector3.up);
        m_AngleToTarget = Quaternion.Angle(transform.rotation, targetRotation);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, FollowTargetSpeed);
    }

    public override void TakeDamage(float amount, Vector3 dmgSource) {
        base.TakeDamage(amount, dmgSource);
        if(!m_IsWakingUp && !HasTargetInSight) {
            if(m_IsAsleep)
                StartCoroutine(WakeUp(WakeUpDelayOnTakingDmg, true, dmgSource));
            else {
                StopAllCoroutines();
                StartCoroutine(ReactToTakingDamage(dmgSource));
            }
        }
    }

    // wakes an enemy if hit by something
    private void OnCollisionEnter(Collision collision) { 
        if(collision.collider.tag != "Ignored" && m_IsAsleep && !m_IsWakingUp) // && !IsDead()
            StartCoroutine(WakeUp(WakeUpDelayOnCollider, false, transform.forward));
    }

    private IEnumerator WakeUp(float delay, bool tookDmg, Vector3 dmgSource) {
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
        m_LastReaction = Time.time;
        if(!IsAlerted) {
            IsAlerted = true;
            m_EnemyMovement.Halt();
            yield return new WaitForSeconds(ReactionTime);
        }
        yield return RoughlyTurnTowards(dmgSource, ReactionTurnDuration);
        if(Time.time < m_LastReaction + ReactionTime) {
            yield return new WaitForSeconds(ReactionTime);
            yield return FireBlindly(2);
        }
        yield return new WaitForSeconds(AlertedTime);
        IsAlerted = false;
        m_EnemyMovement.ReturnToPatrol();
    }
    
    private IEnumerator RoughlyTurnTowards(Vector3 point, float fullTurnDuration) {
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;

        point.y = 0; // normallizing y-position
        float exactAngle = Vector3.Angle(transform.position, point);
        Quaternion targetRotation = Quaternion.LookRotation(point - transform.position, Vector3.up);

        // adding a random direction to avoid enemy being 100% precise when not seeing enemy. becomes more accurate when turned towards target
        float randomAngle = Random.Range(-exactAngle / 4, exactAngle / 4);
        targetRotation *= Quaternion.Euler(0, randomAngle, 0);
        
        float angle = Quaternion.Angle(startRotation, targetRotation);
        float turnDuration = fullTurnDuration * angle / 360;

        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
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
        transform.Find("FOVHelper").gameObject.SetActive(false);
        m_GameController.m_PlayerStats.AddKill();
        // StopAllCoroutines();
        
        foreach(MonoBehaviour component in GetComponents<MonoBehaviour>()) {
            Destroy(component); // need to destroy since coroutines ain't stopping
        }
    }
}