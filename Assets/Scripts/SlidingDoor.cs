using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    private bool m_IsOpen = false;
    public bool m_IsLocked = false;
    public bool m_IsExitDoor = false;
    // public bool m_AutoClose = false;

    private GameController m_GameController;
    private Color m_UnlockedDoorColor = Color.black; // TODO get real color
    private float m_OpenTimestamp;
    private float m_OpenDelay = .2f;
    private float m_SlideTime = 10;

    private Transform m_LeftDoorBlade;
    private Transform m_RightDoorBlade;
    private Vector3 m_OriginalLeftDoorBladePosition;
    private Vector3 m_OriginalRightDoorBladePosition;

    private Vector3 m_LeftDoorTargetPosition;
    private Vector3 m_RightDoorTargetPosition;

    private Coroutine m_RunningCoroutine;


    void Start() {
        m_LeftDoorBlade = transform.Find("DoorBladeLeft");
        m_OriginalLeftDoorBladePosition = m_LeftDoorBlade.transform.position;
        m_RightDoorBlade = transform.Find("DoorBladeRight");
        m_OriginalRightDoorBladePosition = m_RightDoorBlade.transform.position;

        // Material doorMtrl = (Material) Resources.Load("Materials/Door", typeof(Material));
        // m_UnlockedDoorColor = doorMtrl.color;

        m_GameController = FindObjectOfType<GameController>();

        // if(m_IsOpen) {
        //     // print(name + " starts open");
        //     m_RunningCoroutine = StartCoroutine("ToggleOpenDoor");
        // }
    }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Player" && Input.GetButtonDown(m_GameController.m_ActionKey) && Time.time > m_OpenTimestamp + m_OpenDelay) {
            OpenClose();
        // if(Input.GetKeyDown(KeyCode.U) && other.tag == "Player") { //TODO for testing
        //     Unlock();
            // }
        }
    }

    public void Unlock() {
        print(name + " was unlocked");
        m_IsLocked = false;
        if(!m_IsExitDoor)
            ChangeDoorColor(m_UnlockedDoorColor);
    }

    public void OpenClose() {// TODO bättre namn
        if(m_IsLocked) {
            m_GameController.DisplayMessage("This door is locked");
        }
        else {
            if(m_IsExitDoor) {
                m_GameController.EndLevel();
            }
            else if(m_RunningCoroutine != null) {
                m_RunningCoroutine = null;
                m_IsOpen = !m_IsOpen;
                StopCoroutine("ToggleDoor");
            }
            m_RunningCoroutine = StartCoroutine("ToggleOpenDoor");
        }
    }

    private void ChangeDoorColor(Color newColor) {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers) {
            renderer.material.SetColor("_Color", newColor);
        }
    }

    private IEnumerator ToggleOpenDoor() { // TODO bättre namn
        yield return new WaitForFixedUpdate();
        m_OpenTimestamp = Time.time;
        float estimatedTime = m_SlideTime * Time.fixedDeltaTime;

        m_LeftDoorTargetPosition = m_IsOpen ? m_OriginalLeftDoorBladePosition : m_LeftDoorBlade.position + m_LeftDoorBlade.right * (m_LeftDoorBlade.localScale.x) * -.9f;
        m_RightDoorTargetPosition = m_IsOpen ? m_OriginalRightDoorBladePosition : m_RightDoorBlade.position + m_RightDoorBlade.right * (m_RightDoorBlade.localScale.x) * .9f;
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


}
