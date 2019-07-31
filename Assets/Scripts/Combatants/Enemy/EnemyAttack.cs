using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAttack : Attack {

    private FieldOfView m_FOV;
    private Transform m_Target;
    private Combatant m_Combatant;
    private EnemyMovement m_Movement;
    private float m_LastDetectedTargetTime;
    private const float m_TimeToLingerOnTarget = 5f; // how long to keep looking in the direction a target was detected. during this time the enemy is alert and turns faster towards the target
    private Quaternion m_StartRotation;

    private bool m_RecentlyDetectedPlayer = false;

    private float m_FollowTargetSpeed = 0.2f;

    private float m_AngleDifferenceToTarget;
    private readonly float m_AngleDifferenceToTargetBeforeShooting = 15;
    private bool m_IsAlerted = false;

    new void Start() {
        base.Start();
        m_FOV = GetComponent<FieldOfView>();
        m_Combatant = GetComponent<Combatant>();
        m_Movement = GetComponent<EnemyMovement>();

        m_StartRotation = transform.rotation;
    }

    void Update() {
        if(!m_Combatant.IsDead()) {
            SearchForTargets();
            // ShootAtDetectedTarget();
        }
    }

    private void ShootAtDetectedTarget() {
        if(m_FOV.m_VisibleTargets.Count > 0 && CloseEnoughToShoot())
            ContinuousFire();
        else if(m_RecentlyDetectedPlayer)
            StopContinuousFire(); // just to stop firing animation
    }

    private void SearchForTargets() { // TODO (liten) något missledande namn
        if(m_FOV.m_VisibleTargets.Count > 0) {
            m_IsAlerted = true;
            m_Target = m_FOV.m_VisibleTargets[0];

            if(!m_RecentlyDetectedPlayer) {
                m_Movement.Halt();
                // print(name + " detected " + m_Target.name); // debug
            }
            TurnTowardsTarget();

            m_RecentlyDetectedPlayer = true;
            m_LastDetectedTargetTime = Time.time;
        }
        else if(m_RecentlyDetectedPlayer && Time.time > m_LastDetectedTargetTime + m_TimeToLingerOnTarget) {
            m_RecentlyDetectedPlayer = false;

            m_Movement.ReturnToPatrol();
            m_IsAlerted = false;
            // print(name + " is returning to patrol"); // debug
        }
    }

    public bool IsAlerted() {
        return m_IsAlerted;
    }

    private void TurnTowardsTarget() { // TODO flytta till EnemyMovement?
        Quaternion orgRotation = transform.rotation;
        transform.LookAt(m_Target); // TODO ugly solution
        Quaternion targetRotation = transform.rotation;
        transform.rotation = orgRotation;
        m_AngleDifferenceToTarget = Mathf.Abs(targetRotation.eulerAngles.y - transform.rotation.eulerAngles.y);

        // Vector3 targetDir = m_Target.position - transform.position;
        // Quaternion smt = Quaternion.LookRotation(targetDir);
        // m_AngleDifferenceToTarget = Mathf.Abs(Vector3.Angle(targetDir, transform.forward));
        // Quaternion targetRotation = smt * Quaternion.Euler(0, m_AngleDifferenceToTarget, 0);

        // print("difference: " + m_AngleDifferenceToTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_FollowTargetSpeed);
    }

    private bool CloseEnoughToShoot() {
        return m_AngleDifferenceToTarget < m_AngleDifferenceToTargetBeforeShooting;
    }
}