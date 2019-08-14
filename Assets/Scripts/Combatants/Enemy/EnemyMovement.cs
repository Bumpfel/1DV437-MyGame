using System.Collections;
using UnityEngine;
using UnityEditor;

public class EnemyMovement : MonoBehaviour {

    public float m_PatrolDistance = 0f;
    public float m_PatrolEndWaitTime = 3;
    private const float TurnEndWaitTime = .5f;
    private const float PatrolTurnDuration = 1;
    private const float m_MovementSpeed = 3;
    private Vector3 m_StartingPosition;
    private Vector3 m_EndPosition;
    private Quaternion m_StartingRotation;
    private Animator m_Animator;
    private float m_StopTime;
    private bool m_IsHeadingBack = false; // heading back to starting position
    private bool m_IsFirstTurn; // TODO delete. ugly solution to turn the character 2x 180 degrees

    void Start() {
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
        StopAllCoroutines();
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    public void OnAfterAssemblyReload() {
        if(enabled && !GetComponent<EnemyBehaviour>().IsAlerted())
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
                m_IsHeadingBack = !m_IsHeadingBack;
                yield return Wait(m_PatrolEndWaitTime);
                yield return PatrolTurn();
                yield return Wait(TurnEndWaitTime);
                m_Animator.Play("WalkForward_Shoot");
            }
            else {
                Walk();
                yield return new WaitForFixedUpdate();
            }
        }
    }

    // Public helper method
    public void ReturnToPatrol() {
        Halt();
        StartCoroutine(ResetPatrol());
    }

    private IEnumerator ResetPatrol() {
        yield return PatrolTurn();
        yield return Patrol();
    }

    // private IEnumerator DiscoverySpin() {
    //     m_IsFirstTurn = true;
    //     yield return Turn(false);
    //     m_IsFirstTurn = false;
    //     yield return Turn(false);
    //     yield return Wait(m_PatrolEndWaitTime);
    //     ReturnToPatrol();
    // }

    private void Walk() {
        transform.position = transform.position + transform.forward * m_MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void Halt() {
        m_Animator.Play("Idle_Shoot");
        StopAllCoroutines();
    }

    public IEnumerator Wait(float time) {
        m_Animator.Play("Idle_GunMiddle");
        yield return new WaitForSeconds(time);
    }

    private IEnumerator PatrolTurn() { //TODO ta bort eller använd bara för patrol turn. borde iaf kunna gå att ha lerpen gemensam
        Quaternion targetRotation;
        float turnDuration;
        targetRotation = m_IsHeadingBack ? m_StartingRotation * Quaternion.Euler(0, 180, 0) : m_StartingRotation;
        turnDuration = PatrolTurnDuration;

        // m_IsTurning = true;
        m_Animator.Play("WalkLeft_Shoot");
        
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;

        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); //TODO nödlösning för att en gubbe verkar ibland ändra y-position och därmed ser fov fel ut
        m_Animator.Play("Idle_GunMiddle");
        // m_IsTurning = false;
    }

    // public bool IsTurning() {
    //     return m_IsTurning;
    // }


}
