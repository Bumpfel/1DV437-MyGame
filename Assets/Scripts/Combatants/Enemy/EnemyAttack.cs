using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    private FieldOfView m_FOV;
    private Transform target;
    private Attack attack;
    public float fireRate = .3f;
    private float fireTimestamp;
    private Health m_Health;
    private float m_LastSpottedTargetTime;
    private float m_TimeToLingerOnTarget = 3f; // how long to keep looking in the direction a target was spotted
    private Quaternion m_StartRotation;

    private readonly float m_TURN_SPEED = 0.2f;

    void Start() {
        m_FOV = GetComponent<FieldOfView>();
        attack = GetComponent<Attack>();
        m_Health = GetComponent<Health>();

        m_StartRotation = transform.rotation;
    }

    void Update() {
        if(!m_Health.IsDead()) {
            TurnTowardsDetectedTarget();
            ShootAtDetectedTarget();
        }
    }

    private void ShootAtDetectedTarget() {
        if(m_FOV.m_VisibleTargets.Count > 0) {
            if(Time.time > fireTimestamp + fireRate) {
                attack.Fire();
                fireTimestamp = Time.time;
            }
        }
    }

    private void TurnTowardsDetectedTarget() {
        if(m_FOV.m_VisibleTargets.Count > 0) {
            target = m_FOV.m_VisibleTargets[0];
            transform.LookAt(target);
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