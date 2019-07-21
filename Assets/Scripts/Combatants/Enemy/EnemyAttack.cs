using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    private FieldOfView m_FOV;
    private Transform m_Target;
    private Attack m_Attack;
    public float m_FireRate = .3f;
    private float m_FireTimestamp;
    private Health m_Health;
    private float m_LastSpottedTargetTime;
    private float m_TimeToLingerOnTarget = 3f; // how long to keep looking in the direction a target was spotted
    private Quaternion m_StartRotation;

    private readonly float m_TURN_SPEED = 0.2f;

    void Start() {
        m_FOV = GetComponent<FieldOfView>();
        m_Attack = GetComponent<Attack>();
        m_Health = GetComponent<Health>();

        m_StartRotation = transform.rotation;
    }

    void FixedUpdate() {
        if(!m_Health.IsDead()) {
            // TurnTowardsDetectedTarget(); // temp disabled
            // ShootAtDetectedTarget();
        }
    }

    private void ShootAtDetectedTarget() {
        if(m_FOV.m_VisibleTargets.Count > 0) {
            if(Time.time > m_FireTimestamp + m_FireRate) {
                m_Attack.Fire();
                m_FireTimestamp = Time.time;
            }
        }
    }

    private void TurnTowardsDetectedTarget() {
        if(m_FOV.m_VisibleTargets.Count > 0) {
            m_Target = m_FOV.m_VisibleTargets[0];
            transform.LookAt(m_Target);
            m_LastSpottedTargetTime = Time.time;
        }
        else {
            if(Time.time > m_LastSpottedTargetTime + m_TimeToLingerOnTarget) {
                // transform.rotation = m_StartRotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, m_StartRotation, m_TURN_SPEED);
                //TODO go back to previous routine
            }
        }
    }
}