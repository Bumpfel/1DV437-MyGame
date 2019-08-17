using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingedDoor : MonoBehaviour {

    public bool m_IsLocked = false;
    public bool m_IsExitDoor = false;
    public float m_OpenDuration = 1;

    public Material m_UnlockedDoorMaterial;
    private bool m_IsOpen = false;
    private Vector3 m_OriginalDoorBladePosition;

    private Vector3 m_TargetPosition;

    // private Coroutine m_RunningCoroutine;

    private AudioSource m_AudioSource;
    private GameController m_GameController;    
    private bool m_IsInsideTrigger = false; // used because can't have Input check in OnTriggerStay. it can call methods twice

    void Start() {
        m_GameController = FindObjectOfType<GameController>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(Time.timeScale == 0)
            return;
        if(m_IsInsideTrigger) {
            if(Input.GetButtonDown(Controls.Action.ToString())) {// && Time.time > m_OpenTimestamp + OpenDelay) {
                OpenDoor(!m_IsOpen);
            }
            else if(Input.GetKeyDown(KeyCode.U)) { //TODO for testing
                Unlock();
            }
        }
    }


    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            m_IsInsideTrigger = true;
            print("entered door trigger area");
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            m_IsInsideTrigger = false;
            print("left door trigger area");
        }
    }


    public void Unlock() {
        m_IsLocked = false;
        // ChangeDoorColor();
    }


    public void OpenDoor(bool open) {
        if(m_IsLocked) {
            ScreenUI.DisplayMessage("Door is locked");
        }
        else {
            if(m_IsExitDoor) {
                m_GameController.EndLevel();
            }
            else {
                print("opening door");
                GetComponentInChildren<HingeJoint>().useMotor = open;
            }

            // float savedVolume = m_GameController.GetSavedVolume(ExposedMixerGroup.SFXVolume);
            // Vector3 audioClipPoint = transform.position + Vector3.up * Camera.main.transform.position.y * .9f;
            // AudioSource.PlayClipAtPoint(m_AudioSource.clip, audioClipPoint, m_AudioSource.volume * savedVolume);
            // m_RunningCoroutine = StartCoroutine(AnimateDoor(open));
        }
    }

}
