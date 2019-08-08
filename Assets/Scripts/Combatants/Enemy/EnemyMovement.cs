using System.Collections;
using UnityEngine;
using UnityEditor;

public class EnemyMovement : MonoBehaviour {

    public float m_PatrolDistance = 0f;
    public float m_PatrolEndWaitTime = 3f;
    private const float TurnEndWaitTime = .5f;
    private const float NormalTurnDuration = 1;
    private float m_TurnDuration;
    private const float ReactionTime = .2f;
    private const float m_MovementSpeed = 3;

    private Vector3 m_StartingPosition;
    private Vector3 m_EndPosition;
    private Quaternion m_StartingRotation;
    private Combatant m_Combatant;
    private Animator m_Animator;
    private float m_StopTime;
    private bool m_IsHeadingBack = false; // heading back to starting position
    // private float m_CheckIfStuckTimestamp = 0;
    private bool m_IsTurning = false;
    private bool m_IsFirstTurn;

    // tanke - kunna ange koordinator som karaktären ska gå emellan, istället för bara fram/tillbaka
    // skulle kunna skapa game objects för koordinator, för att få en enklare, visuell justering av dessa koordinater. får dock inte flyttas med fienden (får inte vara barn av fiendemodellen)

    void Start() {
        m_Combatant = GetComponent<Combatant>();
        m_StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();

        StartCoroutine(Patrol());

        m_EndPosition = transform.position + transform.forward * m_PatrolDistance;
    }


    public Vector3 GetStartingPosition() {
        return m_StartingPosition;
    }

    public Vector3 GetEndPosition() {
        return m_EndPosition;
    }

    void OnEnable() {
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    void OnDisable() {
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        StopAllCoroutines();
    }

    public void OnAfterAssemblyReload() {
        if(!GetComponent<EnemyAttack>().IsAlerted())
            ReturnToPatrol();
    }

    private bool ShouldPatrol() {
        return m_PatrolDistance > 0;
    }

    private IEnumerator Patrol() {
        if(!ShouldPatrol())
            yield break;
        yield return new WaitForSeconds(.5f);
        m_Animator.Play("WalkForward_Shoot");

        while(true) {
            if((!m_IsHeadingBack && Vector3.Distance(m_StartingPosition, transform.position) >= m_PatrolDistance) || 
            (m_IsHeadingBack && Vector3.Distance(transform.position, m_EndPosition) >= m_PatrolDistance)) {
            // (m_IsHeadingBack && Vector3.Distance(transform.position, m_StartingPosition) < .2f)) {
                m_IsHeadingBack = !m_IsHeadingBack;
                yield return Wait(m_PatrolEndWaitTime);
                yield return Turn(true);
                yield return Wait(TurnEndWaitTime);
                m_Animator.Play("WalkForward_Shoot");
            }
            else {
                Walk();
                yield return new WaitForFixedUpdate();
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
        StartCoroutine(ResetPatrol());
    }

    private IEnumerator ResetPatrol() {
        yield return Turn(true);
        yield return Patrol();
    }

    // Public helper method
    public void ReactToTakingDamage() {
        if(!m_IsTurning && !GetComponent<EnemyAttack>().IsAlerted()) {
            Halt();
            StartCoroutine(DiscoverySpin());
        }
    }

    private IEnumerator DiscoverySpin() {
        m_IsFirstTurn = true;
        yield return Turn(false);
        m_IsFirstTurn = false;
        yield return Turn(false);
        yield return Wait(m_PatrolEndWaitTime);
        ReturnToPatrol();
    }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void Halt() {
        m_Animator.Play("Idle_Shoot");
        StopAllCoroutines();
    }

    private IEnumerator Wait(float time) {
        m_Animator.Play("Idle_GunMiddle");
        yield return new WaitForSeconds(time);
    }

    private IEnumerator Turn(bool isPatrolTurn) {
        Quaternion targetRotation;
        if(isPatrolTurn) {
            targetRotation = m_IsHeadingBack ? m_StartingRotation * Quaternion.Euler(0, 180, 0) : m_StartingRotation;
            m_TurnDuration = NormalTurnDuration;
        }
        else {
            targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            m_TurnDuration = NormalTurnDuration / 2;
            if(m_IsFirstTurn)
                yield return new WaitForSeconds(ReactionTime);
        }

        m_IsTurning = true;
        m_Animator.Play("WalkLeft_Shoot");
        
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;

        while(timeTaken < m_TurnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / m_TurnDuration);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); //TODO nödlösning för att en gubbe verkar ibland ändra y-position och därmed ser fov fel ut
        m_Animator.Play("Idle_GunMiddle");
        m_IsTurning = false;
    }

    // public void StopCoroutinesIfDead() {
    //     if(m_Combatant.IsDead()) {
    //         // if(m_ActiveRoutine != null)
    //         //     StopCoroutine(m_ActiveRoutine);
    //         StopAllCoroutines();
    //         enabled = false;
    //     }
    // }
}
