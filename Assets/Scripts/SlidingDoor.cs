using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    // public bool switchAdjustmentSide = false;
    public bool m_IsLocked = false;
    public bool m_IsExitDoor = false;
    public bool m_Automatic = false;
    public float m_OpenDuration = 1;
    public Material m_UnlockedDoorMaterial;
    public AudioClip m_DoorOpenSound;
    public AudioClip m_DoorCloseSound;
    public AudioClip m_DoorLockedSound;

    private float m_SavedVolume;
    private const float AutomaticCloseDelay = .3f;
    private bool m_IsOpen = false;
    private bool m_DoorInMotion;
    private Transform[] m_DoorBlades;
    private AudioSource m_AudioSource;
    private Vector3 m_AudioClipPoint;
    private GameController m_GameController;
    private bool m_IsInsideTrigger = false; // used because can't have Input check in OnTriggerStay. it can call methods twice
    private float m_WasLastInsideTrigger;

    private void Start() {
        m_GameController = FindObjectOfType<GameController>();
        m_AudioSource = GetComponent<AudioSource>();

        transform.position += transform.forward * -.01f; //(switchAdjustmentSide ? .01f : -.01f);

        m_DoorBlades = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i ++) {
            m_DoorBlades[i] = transform.GetChild(i);
        }
    }

    private void Update() {
        if(Time.timeScale == 0) 
            return;

        if(m_IsInsideTrigger) {
            if(m_Automatic) {
                if(!m_IsOpen)
                    OpenDoor(true);
            }
            else {
                if(Input.GetButtonDown(Controls.Action.ToString())) { // && Time.time > m_OpenTimestamp + OpenDelay) {
                    OpenDoor(!m_IsOpen);
                }
                // else if(Input.GetKeyDown(KeyCode.U)) { //TODO for testing
                //     Unlock();
                // }
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
        ChangeDoorColor();
    }

    public void OpenDoor(bool open) {
        if(m_IsLocked) {
            ScreenUI.DisplayMessage("This door is locked");
            PlaySound(m_DoorLockedSound);
        }
        else {
            if(m_IsExitDoor) {
                m_GameController.EndLevel();
            }
            else if(m_DoorInMotion) {
                StopAllCoroutines();
                m_DoorInMotion = false;
                m_IsOpen = open;
            }

            m_AudioSource.clip = m_IsOpen ? m_DoorCloseSound : m_DoorOpenSound;
            m_AudioSource.Play();
            for(int i = 0; i < m_DoorBlades.Length; i ++) {
                StartCoroutine(AnimateDoorBlade(m_DoorBlades[i], open));
            }
        }
    }

    private void ChangeDoorColor() {
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()) {
            renderer.material = m_UnlockedDoorMaterial;
        }
    }

    private IEnumerator AnimateDoorBlade(Transform doorBlade, bool open) {
        m_DoorInMotion = true;

        Vector3 doorBladeStartingPosition = doorBlade.position;
        Vector3 doorBladeTargetPosition = transform.position + (!open ? Vector3.zero : doorBlade.right * doorBlade.localScale.x * .9f);

        float timeTaken = 0;
        while(timeTaken < m_OpenDuration) {
            timeTaken += Time.fixedDeltaTime;
            doorBlade.position = Vector3.Lerp(doorBladeStartingPosition, doorBladeTargetPosition, timeTaken / m_OpenDuration);
            yield return new WaitForFixedUpdate();
        }

        m_IsOpen = open;
        m_DoorInMotion = false;
    }

    private void PlaySound(AudioClip clip) {
        if(clip != null) {
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }


}
