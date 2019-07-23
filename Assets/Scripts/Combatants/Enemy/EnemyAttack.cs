using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// namespace ObserverPattern {
    public interface Observer {
        void Update();
    }
    
    public class EnemyAttack : Attack, Observer {

        public void Update() {
            // turn towards enemy
            // m_Target = 
        }

        private FieldOfView m_FOV;
        private Transform m_Target;
        private Health m_Health;
        private EnemyMovement m_Movement;
        private float m_LastDetectedTargetTime;
        private const float m_TimeToLingerOnTarget = 5f; // how long to keep looking in the direction a target was detected. during this time the enemy is alert and turns faster towards the target
        private Quaternion m_StartRotation;

        private bool m_RecentlyDetectedPlayer = false;

        private readonly float m_TURN_SPEED = 0.1f;

        private float m_AngleDifferenceToTarget;
        private readonly float m_AngleDifferenceToTargetBeforeShooting = 15;


        new void Start() {
            base.Start();
            m_FOV = GetComponent<FieldOfView>();
            m_Health = GetComponent<Health>();
            m_Movement = GetComponent<EnemyMovement>();

            m_StartRotation = transform.rotation;

            // m_Health.AddObserver(this);
        }

        void FixedUpdate() {
            if(!m_Health.IsDead()) {
                SearchForTargets();
                ShootAtDetectedTarget();
                
                
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
                m_Target = m_FOV.m_VisibleTargets[0];

                if(!m_RecentlyDetectedPlayer) {
                    m_Movement.StopPatrol();
                    print(name + " detected " + m_Target.name); // debug
                }
                TurnTowardsTarget();

                m_RecentlyDetectedPlayer = true;
                m_LastDetectedTargetTime = Time.time;
            }
            else if(m_RecentlyDetectedPlayer && Time.time > m_LastDetectedTargetTime + m_TimeToLingerOnTarget) {
                m_RecentlyDetectedPlayer = false;

                StartCoroutine(m_Movement.ReturnToPatrol());
                print(name + " is returning to patrol");
            }
        }

        private void TurnTowardsTarget() {
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

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_TURN_SPEED); // TODO try using Time.fixedDeltaTime here 
        }

        private bool CloseEnoughToShoot() {
            return m_AngleDifferenceToTarget < m_AngleDifferenceToTargetBeforeShooting;
        }
    }
// }