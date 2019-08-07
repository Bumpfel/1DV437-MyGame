using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    public bool m_IsLocked = false;
    public bool m_IsExitDoor = false;
    public bool m_Automatic = false;
    public float m_OpenDuration = 1;

    private const float AutomaticCloseDelay = .3f;
    private bool m_IsOpen = false;
    private Transform m_LeftDoorBlade;
    private Transform m_RightDoorBlade;
    private Vector3 m_OriginalLeftDoorBladePosition;
    private Vector3 m_OriginalRightDoorBladePosition;

    private Vector3 m_LeftDoorTargetPosition;
    private Vector3 m_RightDoorTargetPosition;

    private Coroutine m_RunningCoroutine;

    private AudioSource m_AudioSource;
    private GameController m_GameController;
    private Color m_UnlockedDoorColor = Color.black; // TODO get real color
    private Color m_UnlockedExitDoorColor = Color.green;
    
    private bool m_IsInsideTrigger = false; // used because can't have Input check in OnTriggerStay. it can call methods twice
    private float m_WasLastInsideTrigger;
    

    void Start() {
        m_LeftDoorBlade = transform.Find("DoorBladeLeft");
        m_OriginalLeftDoorBladePosition = m_LeftDoorBlade.transform.position;
        m_RightDoorBlade = transform.Find("DoorBladeRight");
        m_OriginalRightDoorBladePosition = m_RightDoorBlade.transform.position;

        // Material doorMtrl = (Material) Resources.Load("Materials/Door", typeof(Material));
        // m_UnlockedDoorColor = doorMtrl.color;

        m_GameController = FindObjectOfType<GameController>();
        m_AudioSource = GetComponent<AudioSource>();

        // if(m_IsOpen) {
        //     // print(name + " starts open");
        //     m_RunningCoroutine = StartCoroutine("ToggleOpenDoor");
        // }
    }

    void Update() {
        if(m_IsInsideTrigger) {
            if(m_Automatic) {
                if(!m_IsOpen)
                    OpenDoor(true);
            }
            else {
                if(Input.GetButtonDown(m_GameController.m_ActionKey)) {// && Time.time > m_OpenTimestamp + OpenDelay) {
                    OpenDoor(!m_IsOpen);
                }
                else if(Input.GetKeyDown(KeyCode.U)) { //TODO for testing
                    Unlock();
                }
            }
        }
        else {
            if(m_Automatic && m_IsOpen && Time.time > m_WasLastInsideTrigger + AutomaticCloseDelay) {
                OpenDoor(false);
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if(m_Automatic) {
            m_WasLastInsideTrigger = Time.time; // introducing the use of a small delay since ontriggerexit is wonky
        }
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player" || m_Automatic) {
            m_IsInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.tag == "Player" || m_Automatic) {
            m_IsInsideTrigger = false;
        }
    }


    public void Unlock() {
        m_IsLocked = false;
        ChangeDoorColor(m_IsExitDoor ? m_UnlockedExitDoorColor : m_UnlockedDoorColor);
    }


    public void OpenDoor(bool open) {
        if(m_IsLocked) {
            m_GameController.DisplayMessage("This door is locked");
        }
        else {
            if(m_IsExitDoor) {
                m_GameController.EndLevel();
            }
            else if(m_RunningCoroutine != null) {
                StopCoroutine(m_RunningCoroutine);
                m_RunningCoroutine = null;
                m_IsOpen = open;
                // m_AudioSource.Stop();
            }
            
            // m_AudioSource.timeSamples = open ? m_AudioSource.clip.samples - 1 : 0;
            // m_AudioSource.pitch = 1.7f * (open ? -1 : 1);

            float savedVolume = m_GameController.GetSavedVolume(ExposedMixerGroup.SFXVolume);

            Vector3 audioClipPoint = transform.position + Vector3.up * Camera.main.transform.position.y * .9f;
            AudioSource.PlayClipAtPoint(m_AudioSource.clip, audioClipPoint, m_AudioSource.volume * savedVolume);
            m_RunningCoroutine = StartCoroutine(AnimateDoor(open));
        }
    }

    private void ChangeDoorColor(Color newColor) {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers) {
            renderer.material.SetColor("_Color", newColor);
        }
    }

    private IEnumerator AnimateDoor(bool open) {
        yield return new WaitForFixedUpdate();

        m_LeftDoorTargetPosition = !open ? m_OriginalLeftDoorBladePosition : m_OriginalLeftDoorBladePosition + m_LeftDoorBlade.right * (m_LeftDoorBlade.localScale.x) * -.9f;
        m_RightDoorTargetPosition = !open ? m_OriginalRightDoorBladePosition : m_OriginalRightDoorBladePosition + m_RightDoorBlade.right * (m_RightDoorBlade.localScale.x) * .9f;

        float timeTaken = 0;
        while(timeTaken < m_OpenDuration) {
            timeTaken += Time.fixedDeltaTime;
            m_LeftDoorBlade.position = Vector3.Lerp(m_LeftDoorBlade.position, m_LeftDoorTargetPosition, timeTaken / m_OpenDuration);
            m_RightDoorBlade.position = Vector3.Lerp(m_RightDoorBlade.position, m_RightDoorTargetPosition, timeTaken / m_OpenDuration);
            yield return new WaitForFixedUpdate();
        }
        m_LeftDoorBlade.position = m_LeftDoorTargetPosition;
        m_RightDoorBlade.position = m_RightDoorTargetPosition;

        m_IsOpen = open;
        m_RunningCoroutine = null;
    }


}
