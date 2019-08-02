﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    public bool m_IsLocked = false;
    public bool m_IsExitDoor = false;
    // public bool m_AutoCloses = false;
    
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
    
    private const float OpenDelay = .2f;
    private const float SlideTime = 10;
    private float m_OpenTimestamp;
    private bool m_IsInsideTrigger = false; // used because can't have Input check in OnTriggerStay. it can call methods twice


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
            if(Input.GetButtonDown(m_GameController.m_ActionKey) && Time.time > m_OpenTimestamp + OpenDelay) {
                OpenClose();
            }
            if(Input.GetKeyDown(KeyCode.U)) { //TODO for testing
                Unlock();
            }
        }
    }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Player") {
            m_IsInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            m_IsInsideTrigger = false;
        }
    }


    public void Unlock() {
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
                StopCoroutine(m_RunningCoroutine);
                m_RunningCoroutine = null;
                m_IsOpen = !m_IsOpen;
                m_AudioSource.Stop();
            }
            
            m_AudioSource.timeSamples = m_IsOpen ? 0 : m_AudioSource.clip.samples - 1;
            m_AudioSource.pitch = 1.7f * (m_IsOpen ? 1 : -1);
            m_AudioSource.Play();
            m_RunningCoroutine = StartCoroutine(ToggleOpenDoor());
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
        float estimatedTime = SlideTime * Time.fixedDeltaTime;

        m_LeftDoorTargetPosition = m_IsOpen ? m_OriginalLeftDoorBladePosition : m_OriginalLeftDoorBladePosition + m_LeftDoorBlade.right * (m_LeftDoorBlade.localScale.x) * -.9f;
        m_RightDoorTargetPosition = m_IsOpen ? m_OriginalRightDoorBladePosition : m_OriginalRightDoorBladePosition + m_RightDoorBlade.right * (m_RightDoorBlade.localScale.x) * .9f;
        while(Time.time < m_OpenTimestamp + estimatedTime) {
            yield return new WaitForFixedUpdate();
            m_LeftDoorBlade.position = Vector3.Lerp(m_LeftDoorBlade.position, m_LeftDoorTargetPosition, SlideTime * Time.fixedDeltaTime);
            m_RightDoorBlade.position = Vector3.Lerp(m_RightDoorBlade.position, m_RightDoorTargetPosition, SlideTime * Time.fixedDeltaTime);
        }
        m_LeftDoorBlade.position = m_LeftDoorTargetPosition;
        m_RightDoorBlade.position = m_RightDoorTargetPosition;

        m_IsOpen = !m_IsOpen;
        m_RunningCoroutine = null;
    }


}
