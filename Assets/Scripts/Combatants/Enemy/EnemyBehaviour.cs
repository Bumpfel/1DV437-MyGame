using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Combatant {
    public GameObject m_FOVVisualization;
    public GameObject m_FOVHelper;
    public Transform m_Model; // the part of the enemy that should rotate (to avoid rotating the character gui)
    public GameObject m_AlertedIndicator;
    public GameObject m_AsleepIndicator;
    public bool m_IsAsleep = false;
    
    private int ShotsToFireWhenShootingBlindly = 3;
    private const float WakeUpDelayOnTakingDmg = .4f;
    private const float WakeUpDelayOnCollider = 1;
    private const float AlertedTime = 7;
    private const float ReactionTime = .2f;
    private const float ReactionTurnDuration = 1.5f;
    private const float SlowReactionTurnDuration = ReactionTurnDuration * 3;
    private const float FollowTargetSpeed = .2f;
    private const float MaxAngleToTargetBeforeShooting = 15;
    private EnemyPatrol m_EnemyPatrol;
    private FieldOfView m_FOV;
    private Transform m_Target;
    private Quaternion m_StartRotation;
    private Attack m_Attack;
 
    // an enemy becomes alerted if the player has been spotted recently or if the enemy took dmg
    private bool m_IsAlerted = false;
    public bool IsAlerted {
        get => m_IsAlerted;
        private set {
            m_IsAlerted = value;
            SetAlertStatus(value);
        }
    }
    private bool m_IsWakingUp = false;
    private bool m_RecentlyDetectedPlayer = false;
    private float m_LastReaction;
    private float m_AngleToTarget;

    private bool IsCloseEnoughToShoot => m_AngleToTarget < MaxAngleToTargetBeforeShooting;
    private bool HasTargetInSight => m_FOV.HasTargetInSight;
    private bool AlertedTimedOut => Time.time > m_LastReaction + AlertedTime;

    private new void Start() {
        base.Start();
        m_Attack = GetComponent<Attack>();
        m_FOV = GetComponent<FieldOfView>();
        m_EnemyPatrol = GetComponent<EnemyPatrol>();

        SetAlertStatus(false);
        if(m_IsAsleep) {
            m_AsleepIndicator.SetActive(true);
            foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
                if(script != this) {
                    script.enabled = false;
                }
            }
        }
        m_StartRotation = m_Model.rotation;
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
                
                m_EnemyPatrol.Halt();
                m_RecentlyDetectedPlayer = true;
            }
            StopAllCoroutines();
            FollowTarget();
            m_LastReaction = Time.time;

            if(IsCloseEnoughToShoot)
                m_Attack.Fire();
        }
        else if(m_RecentlyDetectedPlayer && AlertedTimedOut) {
            m_RecentlyDetectedPlayer = false;
            IsAlerted = false;
            m_EnemyPatrol.ReturnToPatrol();
        }
    }

    // FollowTarget vars
    private Quaternion targetRotation;
    private void FollowTarget() {
        targetRotation = Quaternion.LookRotation(m_Target.position - m_Model.position, Vector3.up);
        m_AngleToTarget = Quaternion.Angle(m_Model.rotation, targetRotation);

        m_Model.rotation = Quaternion.Lerp(m_Model.rotation, targetRotation, FollowTargetSpeed);
    }

    public override void TakeDamage(float amount, Vector3 dmgSource) {
        base.TakeDamage(amount, dmgSource);
        if(!m_IsWakingUp && !HasTargetInSight && !IsDead) {
            if(m_IsAsleep)
                StartCoroutine(WakeUp(WakeUpDelayOnTakingDmg, true, dmgSource));
            else {
                StopAllCoroutines();
                StartCoroutine(ReactToHit(dmgSource, true));
            }
        }
    }

    // wakes an enemy if bumped by something (like the player or an object). if awake, turns slowly towards the source if not alerted, otherwise quickly 
    private void OnCollisionEnter(Collision collision) { 
        if(collision.collider.tag != "Ignored" && collision.collider.tag != "Door" && !m_IsWakingUp) {
            if(m_IsAsleep)
                StartCoroutine(WakeUp(WakeUpDelayOnCollider, false, collision.collider.transform.position));
            else {
                StopAllCoroutines();
                StartCoroutine(ReactToHit(collision.collider.transform.position, IsAlerted));
            }
        }
    }

    private IEnumerator WakeUp(float delay, bool tookDmg, Vector3 hitSource) {
        m_AsleepIndicator.SetActive(false);
        m_IsWakingUp = true;
        yield return new WaitForSeconds(delay);
        foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
            script.enabled = true;
        }
        m_IsAsleep = false;
        m_IsWakingUp = false;
        
        // turns towards dmg source if took dmg
        if(tookDmg)
            StartCoroutine(ReactToHit(hitSource, true));
    }

   private IEnumerator ReactToHit(Vector3 hitSource, bool tookDmg) {
        m_LastReaction = Time.time;
        if(!IsAlerted) {
            if(tookDmg)
                IsAlerted = true;
            m_EnemyPatrol.Halt();
            yield return new WaitForSeconds(ReactionTime);
        }
        yield return RoughlyTurnTowards(hitSource, tookDmg ? ReactionTurnDuration : SlowReactionTurnDuration);
        
        // blindly fire against source if alerted
        if(IsAlerted && Time.time < m_LastReaction + ReactionTime) { 
            yield return new WaitForSeconds(ReactionTime);
            yield return FireBlindly(ShotsToFireWhenShootingBlindly);
        }

        yield return new WaitForSeconds(AlertedTime);
        IsAlerted = false;
        m_EnemyPatrol.ReturnToPatrol();
    }
    
    private IEnumerator RoughlyTurnTowards(Vector3 point, float fullTurnDuration) {
        float timeTaken = 0;
        Quaternion startRotation = m_Model.rotation;

        point.y = 0; // normallizing y-position
        float exactAngle = Vector3.Angle(m_Model.position, point);
        Quaternion targetRotation = Quaternion.LookRotation(point - transform.position, Vector3.up);

        // adding a random angle to avoid enemy being 100% precise when not seeing the player. becomes more accurate the smaller the angle is between the hit source and its own rotation
        float randomAngle = Random.Range(-exactAngle / 4, exactAngle / 4);
        targetRotation *= Quaternion.Euler(0, randomAngle, 0);
        
        float angle = Quaternion.Angle(startRotation, targetRotation);
        float turnDuration = fullTurnDuration * angle / 360;

        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            m_Model.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
    }

    private IEnumerator FireBlindly(int shots) {
        for(int i = 0; i < shots; i ++) {
            m_Attack.Fire();
            yield return new WaitForSeconds(m_Attack.m_FireRate);
        }
    }

    protected override void Die() {
        m_FOVVisualization.SetActive(false);
        m_FOVHelper.SetActive(false);
        m_GameController.m_PlayerStats.AddKill();
        base.Die();
        // StopAllCoroutines();
    }

    private void SetAlertStatus(bool enabled) {
        m_AlertedIndicator.SetActive(enabled);
    }
}