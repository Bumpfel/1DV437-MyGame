using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public float m_PatrolDistance = 0;
    public float m_PatrolEndWaitTime = 3;


    private Vector3 m_StartingPosition;
    private Quaternion m_StartingRotation;
    private float m_MovementSpeed = 3;
    private float m_TurnSpeed = .1f;
    private float m_StopTime;
    private bool m_HasTurnedOnPatrol = false; // förbättra logik för denna. nu sätts denna till true innan man vänt om fienden patrullerar
    private Health m_Health;
    private Animator m_Animator;

    public Coroutine m_RunningCoroutine;

    // public enum TurnSpeed { SLOW = .1f, QUICK = .2f};


    // tanke - kunna ange koordinator som karaktären ska gå emellan, istället för bara fram/tillbaka
    // skulle kunna skapa game objects för koordinator, för att få en enklare, visuell justering av dessa koordinater. får dock inte flyttas med fienden (får inte vara barn av fiendemodellen)

    void Start() {
        m_Health = GetComponent<Health>();
        m_StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();
        // m_Animator.Play("Idle_Shoot");

        if(isPatrolling())
            StartCoroutine("Patrol");


        // if(name == "Enemy (1)")
        //     print("starting rotation = "  + m_StartingRotation.eulerAngles.y); // debug
    }

    // public bool isEnabled() {
    //     return m_Health != null;
    // }

    // void OnApplicationFocus() { // TODO prob remove. useful for testing
    //     print("restarting patrol");
    //     if(m_PatrolDistance > 0) {

    //     StopCoroutine("Patrol");
    //     StartCoroutine("Patrol");
    //     }
    // }

    private bool isPatrolling() {
        return m_PatrolDistance > 0;
    }

    private IEnumerator Patrol() {
        m_Animator.Play("WalkForward_Shoot");
        while(true) {
            yield return new WaitForFixedUpdate();
            if(!m_Health.IsDead()) {

                if(Vector3.Distance(m_StartingPosition, transform.position) >= m_PatrolDistance || (m_HasTurnedOnPatrol && Vector3.Distance(transform.position, m_StartingPosition) < .1f )) { // TODO add if collided against object? check i endpoint reached not watertight
                    m_Animator.Play("Idle_Shoot");
                    yield return Wait();
                    m_HasTurnedOnPatrol = !m_HasTurnedOnPatrol;
                    yield return Turn();
                    m_Animator.Play("WalkForward_Shoot");
                    Walk(); // to make sure not to immediately trigger another turn in the Patrol routine
                }
                else
                    Walk();
            }
        }
    }

    public IEnumerator ReturnToPatrol() {
        yield return Turn();
        if(isPatrolling())
            StartCoroutine("Patrol");
    }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void StopPatrol() {
        // if(isPatrolling())
        //     m_HasTurnedOnPatrol = !m_HasTurnedOnPatrol; // ugly fix to make sure the enemy will continue patrolling in the same direction as before
        StopCoroutine("Patrol");
    }


    private IEnumerator Wait() {
        yield return new WaitForSeconds(m_PatrolEndWaitTime);
    }

    public IEnumerator Turn() {
        m_Animator.Play("WalkLeft_Shoot");

        Quaternion targetRotation = Quaternion.Euler(0, (m_HasTurnedOnPatrol ? 0 : m_StartingRotation.eulerAngles.y), 0); // TODO se till att det är 180 grader relativt till nuvarande position som menas 
        // float turnLeeway = 0.05f;
        // float turnLeeway2 = 0.001f; // for some reason, the slerp gets closer to its rotation target when turning towards 1 than 0 
        // while(Mathf.Abs(transform.rotation.y) + turnLeeway2 < targetRotation.y || Mathf.Abs(transform.rotation.y) - turnLeeway > targetRotation.y) { // interpolate until the transform gets within a specified range of its target
          
        float timestamp = Time.time;
        float turn = Mathf.Abs(transform.rotation.y - targetRotation.y);
        float estimatedTurnTime = turn / ((m_TurnSpeed / 4f) * (1 / Time.fixedDeltaTime));

        // print("estimated turn time: " + estimatedTurnTime);

        while(Time.time < timestamp + estimatedTurnTime) {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_TurnSpeed);
        }
        transform.rotation = targetRotation; // do the last exact rotation, since slerp will only get close
        m_Animator.Play("Idle_Shoot");
        yield break;
    }


}