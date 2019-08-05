using System.Collections;
using UnityEngine;
using UnityEditor;

public class EnemyMovement : MonoBehaviour {

    public float m_PatrolDistance = 0f;
    public float m_PatrolEndWaitTime = 3f;

    private const float TurnEndWaitTime = .5f;
    private const float TurnDuration = 1;
    private const float m_MovementSpeed = 3;

    private Vector3 m_StartingPosition;
    private Vector3 m_EndPosition;
    private Quaternion m_StartingRotation;
    private Combatant m_Combatant;
    private Animator m_Animator;
    private IEnumerator m_ActiveRoutine;
    private float m_StopTime;
    private bool m_IsHeadingBack = false; // heading back to starting position
    // private float m_CheckIfStuckTimestamp = 0;
    [HideInInspector]
    public bool m_IsTurning = false;

    // tanke - kunna ange koordinator som karaktären ska gå emellan, istället för bara fram/tillbaka
    // skulle kunna skapa game objects för koordinator, för att få en enklare, visuell justering av dessa koordinater. får dock inte flyttas med fienden (får inte vara barn av fiendemodellen)

    void Start() {
        m_Combatant = GetComponent<Combatant>();
        m_StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();
        // m_Animator.Play("Idle_Shoot");

        m_ActiveRoutine = Patrol();
        StartCoroutine(m_ActiveRoutine);

        m_EndPosition = transform.position + transform.forward * m_PatrolDistance;
    }


    public Vector3 GetStartingPosition() {
        return m_StartingPosition;
    }

    public Vector3 GetEndPosition() {
        return m_EndPosition;
    }

//   void OnEnable() {
//         AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
//     }

//     void OnDisable() {
//         AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
//     }
//     public void OnAfterAssemblyReload() {
//         // StartCoroutine(m_ActiveRoutine);
//     }

    private bool ShouldPatrol() {
        return m_PatrolDistance > 0;
    }

    private IEnumerator Patrol() {
        if(!ShouldPatrol())
            yield break;
        ConfirmIsAlive();
        m_Animator.Play("WalkForward_Shoot");

        while(true) {//!m_Combatant.m_IsAsleep) {
            if((!m_IsHeadingBack && Vector3.Distance(m_StartingPosition, transform.position) >= m_PatrolDistance) || 
            (m_IsHeadingBack && Vector3.Distance(transform.position, m_StartingPosition) < .2f)) {
                m_IsHeadingBack = !m_IsHeadingBack;
                ConfirmIsAlive();
                yield return Wait(m_PatrolEndWaitTime);
                ConfirmIsAlive();
                yield return Turn();
                ConfirmIsAlive();
                yield return Wait(TurnEndWaitTime);
                ConfirmIsAlive();
                m_Animator.Play("WalkForward_Shoot");
            }
            else {
                ConfirmIsAlive();
                Walk();
                yield return new WaitForFixedUpdate();
            }
        }
        
        // m_Animator.Play("Idle_GunMiddle");
    }

    // private bool IsStuck() {
    //     Vector3 prevPosition = transform.position;
    //     float checkIfStuckInterval = .3f;

    //     if(Time.time > m_CheckIfStuckTimestamp + checkIfStuckInterval) {
    //         m_CheckIfStuckTimestamp = Time.time;
    //         prevPosition = transform.position;
    //         return false;
    //     }
    //     else {
    //         if(Vector3.Distance(transform.position, prevPosition) < .2f)
    //             print(name + " got stuck");
    //         return Time.time > m_CheckIfStuckTimestamp + checkIfStuckInterval && transform.position == prevPosition;
    //     }
    // }

    // Public helper method
    public void ReturnToPatrol() {
        m_ActiveRoutine = ResetPatrol();
        StartCoroutine(m_ActiveRoutine);
    }

    private IEnumerator ResetPatrol() {
        ConfirmIsAlive();
        yield return Turn();
        ConfirmIsAlive();
        yield return Patrol();
    }

    // Public helper method
    public void ReactToTakingDamage() {
        if(!m_IsTurning && !GetComponent<EnemyAttack>().IsAlerted()) {
            Halt();
            m_ActiveRoutine = DiscoverySpin();
            StartCoroutine(m_ActiveRoutine);
        }
    }

    private IEnumerator DiscoverySpin() {
        print(name + " was recently shot. Turning");
        // yield return Turn();
        yield return SpinAround();
        yield return Wait(m_PatrolEndWaitTime);
        ReturnToPatrol();
    }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void Halt() {
        ConfirmIsAlive();
        m_Animator.Play("Idle_Shoot");
        // print("halting");
        if(m_ActiveRoutine != null)
            StopCoroutine(m_ActiveRoutine);
    }

    private IEnumerator Wait(float time) {
        m_Animator.Play("Idle_GunMiddle");
        yield return new WaitForSeconds(time);
    }

    private IEnumerator Turn() {
        m_IsTurning = true;
        m_Animator.Play("WalkLeft_Shoot");

        float degrees = 180;

        // Quaternion targetRotation;
        // if(isPatrolTurn)
        Quaternion targetRotation = m_IsHeadingBack ? m_StartingRotation * Quaternion.Euler(0, degrees, 0) : m_StartingRotation;
        // }
        // else {
        //     m_TurnDuration = 2;
        //     targetRotation = Quaternion.Euler(0, transform.rotation.y + 360, 0);
        // }
        
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;
        // float angleDifference = Mathf.Abs(transform.rotation.eulerAngles.y - targetRotation.eulerAngles.y);
        // float duration = angleDifference * (m_NormalTurnDuration / degrees);

        while(timeTaken < TurnDuration) {
            ConfirmIsAlive();
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / TurnDuration);
        }
        //DEBUG
        // if(name == "Enemy (2)")
        //     print("current rotation: " + transform.rotation.y + ", target rotation: " + targetRotation.y);
        // transform.rotation = targetRotation;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); //TODO nödlösning för att en gubbe verkar ibland ändra y-position och därmed ser fov fel ut
        m_Animator.Play("Idle_GunMiddle");
        m_IsTurning = false;
    }

    private IEnumerator SpinAround() {
        m_IsTurning = true;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation - 180;
        float timeTaken = 0.0f;
        float yRotation;
        while(timeTaken < TurnDuration) {
            ConfirmIsAlive();
            timeTaken += Time.fixedDeltaTime;
            yRotation = Mathf.Lerp(startRotation, endRotation, timeTaken / TurnDuration) % 180;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return new WaitForFixedUpdate();
        }
        m_IsTurning = false;
    }

    private void ConfirmIsAlive() {
        if(m_Combatant.IsDead()) {
            StopCoroutine(m_ActiveRoutine);
            StopAllCoroutines();
            enabled = false;
        }
    }
}
