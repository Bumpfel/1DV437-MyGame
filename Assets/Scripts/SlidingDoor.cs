using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    public bool m_IsOpen = false;
    public bool m_AutoClose = false;

    private float m_OpenTimestamp;
    private float m_OpenDelay = .2f;
    private float m_SlideTime = 10;
    private string m_ActionKey = "Action_Player1";

    private Transform m_LeftDoorBlade;
    private Transform m_RightDoorBlade;

    private Vector3 m_OriginalLeftDoorBladePosition;
    private Vector3 m_OriginalRightDoorBladePosition;

    private Vector3 m_LeftDoorTargetPosition;
    private Vector3 m_RightDoorTargetPosition;

    private Coroutine m_RunningCoroutine;


    void Start() {
        m_LeftDoorBlade = transform.GetChild(0);
        m_OriginalLeftDoorBladePosition = m_LeftDoorBlade.transform.position;
        m_RightDoorBlade = transform.GetChild(1);
        m_OriginalRightDoorBladePosition = m_RightDoorBlade.transform.position;
    }

    void OnTriggerStay(Collider other) {
        if(Input.GetButtonDown(m_ActionKey) && Time.time > m_OpenTimestamp + m_OpenDelay) {
            if(m_RunningCoroutine != null) {
                m_RunningCoroutine = null;
                m_IsOpen = !m_IsOpen;
                StopCoroutine("ToggleDoor");
            }
            m_RunningCoroutine = StartCoroutine("ToggleOpenDoor");
        }
    }

    private IEnumerator ToggleOpenDoor() {
        yield return new WaitForFixedUpdate();
        m_OpenTimestamp = Time.time;
        float estimatedTime = m_SlideTime * Time.fixedDeltaTime;

        m_LeftDoorTargetPosition = m_IsOpen ? m_OriginalLeftDoorBladePosition : m_LeftDoorBlade.position + Vector3.right * (m_LeftDoorBlade.localScale.z - .1f);
        m_RightDoorTargetPosition = m_IsOpen ? m_OriginalRightDoorBladePosition : m_RightDoorBlade.position + Vector3.left * (m_RightDoorBlade.localScale.z - .1f);
        while(Time.time < m_OpenTimestamp + estimatedTime) {
            yield return new WaitForFixedUpdate();
            m_LeftDoorBlade.position = Vector3.Lerp(m_LeftDoorBlade.position, m_LeftDoorTargetPosition, m_SlideTime * Time.fixedDeltaTime);
            m_RightDoorBlade.position = Vector3.Lerp(m_RightDoorBlade.position, m_RightDoorTargetPosition, m_SlideTime * Time.fixedDeltaTime);
        }
        m_LeftDoorBlade.position = m_LeftDoorTargetPosition;
        m_RightDoorBlade.position = m_RightDoorTargetPosition;

        m_IsOpen = !m_IsOpen;
        m_RunningCoroutine = null;
    }
















    // void Start() {
    //     m_LeftDoorBlade = transform.GetChild(0);
    //     m_OriginalLeftDoorPosition = m_LeftDoorBlade.transform.position;
    //     m_LeftDoorTargetPosition = m_LeftDoorBlade.transform.position;
    //     m_RightDoorBlade = transform.GetChild(1);
    //     m_OriginalRightDoorPosition = m_RightDoorBlade.transform.position;
    //     m_RightDoorTargetPosition = m_RightDoorBlade.transform.position;
    // }

    // void FixedUpdate() {
    //      m_LeftDoorBlade.position = Vector3.Slerp(m_LeftDoorBlade.position, m_LeftDoorTargetPosition, estimatedTime * Time.fixedDeltaTime);
    //     m_RightDoorBlade.position = Vector3.Slerp(m_RightDoorBlade.position, m_RightDoorTargetPosition, estimatedTime * Time.fixedDeltaTime);
    //     // m_LeftDoorBlade.position = m_LeftDoorTargetPosition;
    //     // m_RightDoorBlade.position = m_RightDoorTargetPosition;
    // }

    // void OnTriggerStay(Collider other) {
    //     if(Input.GetButtonDown(m_ActionKey) && Time.time > m_OpenTimestamp + m_OpenDelay) {
    //         // StartCoroutine(OpenDoor());
    //         OpenDoor();
    //     }
    // }

    // private void OpenDoor() {
    //     // leftDoorBlade.position = leftDoorBlade.position * Vector2.up * -.5f;
    //     // rightDoorBlade.position = rightDoorBlade.position * Vector2.up * .5f;
    //     float timestamp = Time.time;
    //     m_LeftDoorTargetPosition = m_IsOpen ? m_LeftDoorBlade.position + Vector3.right * m_LeftDoorBlade.localScale.z : m_OriginalLeftDoorPosition;
    //     m_RightDoorTargetPosition = m_IsOpen ? m_RightDoorBlade.position + Vector3.left * m_RightDoorBlade.localScale.z : m_OriginalRightDoorPosition;
    //     // while(Time.time < timestamp + estimatedTime) {
    //         // yield return new WaitForFixedUpdate();
    //         // m_LeftDoorBlade.position = Vector3.Slerp(m_LeftDoorBlade.position, m_LeftDoorTargetPosition, estimatedTime * Time.fixedDeltaTime);
    //         // m_RightDoorBlade.position = Vector3.Slerp(m_RightDoorBlade.position, m_RightDoorTargetPosition, estimatedTime * Time.fixedDeltaTime);
    //     // }
    //     // m_LeftDoorBlade.position = m_LeftDoorTargetPosition;
    //     // m_RightDoorBlade.position = m_RightDoorTargetPosition;

    //     m_OpenTimestamp = Time.time;
    //     m_IsOpen = !m_IsOpen;
    //     // yield break;
    // }

}
