using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolling : MonoBehaviour {

    public float m_PatrolDistance = 10;
    // public float m_Angle = 0;

    private Vector3 m_StartingPosition;
    // private Quaternion m_StartingRotation;
    private float m_MovementSpeed = 3;
    private float m_TurnSpeed = .1f;
    private float m_StopTime;
    private bool m_Turned = false;
    private Health m_Health;

    private Animator m_Animator;


    // tanke - kunna ange koordinator som karaktären ska gå emellan, istället för bara fram/tillbaka
    // skulle kunna skapa game objects för koordinator, för att få en enklare, visuell justering av dessa koordinater. får dock inte flyttas med fienden (får inte vara barn av fiendemodellen)

    void Start() {
        m_Health = GetComponent<Health>();
        m_StartingPosition = transform.position;
        // m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();
        // m_Animator.Play("Idle_Shoot");

        StartCoroutine("Patrol");
    }

    private IEnumerator Patrol() {
        m_Animator.Play("WalkForward_Shoot");
        while(true) {
            yield return new WaitForFixedUpdate();
            if(!m_Health.IsDead()) { // && is not engaging player

                if(Vector3.Distance(m_StartingPosition, transform.position) >= m_PatrolDistance || (m_Turned && Vector3.Distance(transform.position, m_StartingPosition) < .2f )) { // TODO add if collided against object? check i endpoint reached not watertight
                        print("reached patrol endpoint");
                        m_Animator.Play("Idle_Shoot");
                        yield return Wait();
                        yield return Turn();
                        m_Animator.Play("WalkForward_Shoot");
                        Walk(); // to make sure not to immediately trigger another turn in the Patrol routine
                }
                else
                    Walk();
            }
        }
    }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime; // not sure fixedDeltaTime is correct in a coroutine
    }

    private IEnumerator Wait() {
        yield return new WaitForSeconds(3);
    }

    private IEnumerator Turn() {
        m_Animator.Play("WalkLeft_Shoot");
       
        Quaternion targetRotation = Quaternion.Euler(0, (m_Turned ? 180 : 0), 0);
        float turnLeeway = 0.05f;
        float turnLeeway2 = 0.001f; // for some reason, the slerp gets closer to its rotation target when turning towards 1 than 0 
        while(Mathf.Abs(transform.rotation.y) + turnLeeway2 < targetRotation.y || Mathf.Abs(transform.rotation.y) - turnLeeway > targetRotation.y) { // interpolate until the transform gets within a specified range of its target
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed);
        }
        m_Turned = !m_Turned;
        transform.rotation = targetRotation; // do the last exact rotation, since slerp will only get close

        m_Animator.Play("Idle_Shoot");
        yield break;
    }


}