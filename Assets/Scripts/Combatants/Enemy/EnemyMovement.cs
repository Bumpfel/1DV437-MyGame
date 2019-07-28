using System.Collections;
using UnityEngine;
using UnityEditor;

public class EnemyMovement : MonoBehaviour {

    public float m_PatrolDistance = 0f;
    public float m_PatrolEndWaitTime = 3f;
    private float m_TurnEndWaitTime = .5f;

    [HideInInspector]
    public Vector3 m_StartingPosition;

    [HideInInspector]
    public Vector3 m_EndPosition;
    private Quaternion m_StartingRotation;
    private float m_MovementSpeed = 3;
    private float m_TurnDuration = 1f;
    private float m_StopTime;
    private bool m_HasTurnedOnPatrol = false; // förbättra logik för denna. nu sätts denna till true innan man vänt om fienden patrullerar
    private Health m_Health;
    private Animator m_Animator;
    private float m_CheckIfStuckTimestamp = 0;
    private bool m_RecentlyMadeDiscovery = false;
    private IEnumerator m_ActiveRoutine;

    // private bool m_SpunHalfLapOnDiscoveryTurn = false;


    // tanke - kunna ange koordinator som karaktären ska gå emellan, istället för bara fram/tillbaka
    // skulle kunna skapa game objects för koordinator, för att få en enklare, visuell justering av dessa koordinater. får dock inte flyttas med fienden (får inte vara barn av fiendemodellen)

    void Start() {
        m_Health = GetComponent<Health>();
        m_StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();
        // m_Animator.Play("Idle_Shoot");

        m_ActiveRoutine = Patrol();
        StartCoroutine(m_ActiveRoutine);

        m_EndPosition = transform.position + transform.forward * m_PatrolDistance;
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
        m_Animator.Play("WalkForward_Shoot");

        while(true) {
            yield return new WaitForFixedUpdate();
            
            if((!m_HasTurnedOnPatrol && Vector3.Distance(m_StartingPosition, transform.position) >= m_PatrolDistance) || 
            (m_HasTurnedOnPatrol && (m_HasTurnedOnPatrol && Vector3.Distance(transform.position, m_StartingPosition) < .2f))) {
            //IsStuck()) {
                ConfirmIsAlive();
                yield return Wait(m_PatrolEndWaitTime);
                m_HasTurnedOnPatrol = !m_HasTurnedOnPatrol;
                ConfirmIsAlive();
                yield return Turn(true);
                ConfirmIsAlive();
                yield return Wait(m_TurnEndWaitTime);
                ConfirmIsAlive();
                m_Animator.Play("WalkForward_Shoot");
            }
            else {
                ConfirmIsAlive();
                Walk();
            }
        }
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
        yield return Turn(true);
        yield return Patrol();
    }

    // Public helper method
    public void ReactToTakingDamage() {
        // if(!m_RecentlyMadeDiscovery && !GetComponent<EnemyAttack>().IsEngaging()) {
        //     m_RecentlyMadeDiscovery = true;
        //     Halt();
        //     m_ActiveRoutine = DiscoverySpin();
        //     StartCoroutine(m_ActiveRoutine);
        // }
    }

    private IEnumerator DiscoverySpin() {
        print(name + " was recently shot. Turning");
        // yield return Turn(false);
        yield return Rotate(3f);
        m_RecentlyMadeDiscovery = false;
        yield return Wait(m_PatrolEndWaitTime);
        ReturnToPatrol();
    }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void Halt() {
        m_Animator.Play("Idle_Shoot");
        // print("halting");
        StopCoroutine(m_ActiveRoutine);
    }

    private IEnumerator Wait(float time) {
        m_Animator.Play("Idle_GunMiddle");
        yield return new WaitForSeconds(time);
    }

    private IEnumerator Turn(bool isPatrolTurn) {
        m_Animator.Play("WalkLeft_Shoot");

        Quaternion targetRotation;
        if(isPatrolTurn) {
            targetRotation = m_HasTurnedOnPatrol ? m_StartingRotation * Quaternion.Euler(0, 180f, 0) : m_StartingRotation;
        }
        else {
            m_TurnDuration = 2; //TODO hardcoded
            // targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            targetRotation = Quaternion.Euler(0, transform.rotation.y + 360, 0);
        }
        
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;

        while(timeTaken < m_TurnDuration) {
            timeTaken += Time.fixedDeltaTime;
            ConfirmIsAlive();
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeTaken / m_TurnDuration);
        }
        //DEBUG
        // if(name == "Enemy (2)")
        //     print("current rotation: " + transform.rotation.y + ", target rotation: " + targetRotation.y);
        // transform.rotation = targetRotation;
        m_Animator.Play("Idle_GunMiddle");
        yield break;
    }

    private IEnumerator Rotate(float duration) {
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation - 360.0f;
        float timeTaken = 0.0f;
        while(timeTaken < duration) {
            timeTaken += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotation, timeTaken / duration) % 360.0f;
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);
            yield return null;
        }
    }

    private void ConfirmIsAlive() {
        if(m_Health.IsDead()) {
            StopCoroutine(m_ActiveRoutine);
            Destroy(this);
        }
    }
}


// Gives an indication of patrol path by drawing a cyan line which helps when you place enemies designing levels
[CustomEditor (typeof (EnemyMovement))]
public class EnemyMovementEditor : Editor {

    void OnSceneGUI() {
        EnemyMovement enemy = (EnemyMovement) target;
        Handles.color = Color.cyan;
        if(enemy.m_EndPosition == Vector3.zero)
            Handles.DrawLine(enemy.transform.position, enemy.transform.position + enemy.transform.forward * enemy.m_PatrolDistance);
        else
            Handles.DrawLine(enemy.m_StartingPosition, enemy.m_EndPosition);
    }

}
