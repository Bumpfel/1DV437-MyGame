using System.Collections;
using UnityEngine;
using UnityEditor;

public class EnemyPatrol : MonoBehaviour {

    public float m_PatrolDistance = 0;
    public float m_PatrolEndWaitTime = 3; // time the enemy waits before turning upon reaching patrol end/start position
    
    private const float TurnEndWaitTime = .5f; // time the enemy waits before walking after making a patrol turn
    private const float PatrolTurnDuration = 1;
    private const float MovementSpeed = 3;
    public Vector3 StartingPosition { get; private set; }
    public Vector3 EndPosition { get; private set; }
    private Quaternion m_StartingRotation;
    private Animator m_Animator;
    private bool m_IsHeadingBack = false; // heading back to starting position

    void Start() {
        StartingPosition = transform.position;
        m_StartingRotation = transform.rotation;

        m_Animator = GetComponent<Animator>();

        StartCoroutine(Patrol());

        EndPosition = transform.position + transform.forward * m_PatrolDistance;
    }

    void OnEnable() {
        #if UNITY_EDITOR
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        #endif
    }

    void OnDisable() {
        #if UNITY_EDITOR
            StopAllCoroutines();
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
        #endif
    }

    public void OnAfterAssemblyReload() {
        if(!GetComponent<EnemyBehaviour>().IsAlerted)
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
            if((!m_IsHeadingBack && Vector3.Distance(StartingPosition, transform.position) >= m_PatrolDistance) || 
            (m_IsHeadingBack && Vector3.Distance(transform.position, EndPosition) >= m_PatrolDistance)) {
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

    private void Walk() {
        transform.position = transform.position + transform.forward * MovementSpeed * Time.fixedDeltaTime;
    }
    
    public void Halt() {
        m_Animator.Play("Idle_Shoot");
        StopAllCoroutines();
    }

    public IEnumerator Wait(float time) {
        m_Animator.Play("Idle_GunMiddle");
        yield return new WaitForSeconds(time);
    }

    private IEnumerator PatrolTurn() {
        Quaternion targetRotation;
        float turnDuration;
        targetRotation = m_IsHeadingBack ? m_StartingRotation * Quaternion.Euler(0, 180, 0) : m_StartingRotation;
        turnDuration = PatrolTurnDuration;

        m_Animator.Play("WalkLeft_Shoot");
        
        float timestamp = Time.time;
        float timeTaken = 0;
        Quaternion startRotation = transform.rotation;

        while(timeTaken < turnDuration) {
            timeTaken += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeTaken / turnDuration);
        }
        // transform.position = new Vector3(transform.position.x, 0, transform.position.z); //TODO nödlösning för att en gubbe verkar ibland ändra y-position och därmed ser fov fel ut
        m_Animator.Play("Idle_GunMiddle");
    }

}
