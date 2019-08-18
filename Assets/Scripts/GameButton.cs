using System.Collections;
using UnityEngine;
using UnityEditor;

public class GameButton : MonoBehaviour {
    public SlidingDoor[] m_AffectedDoors = null;
    public string m_AreaName = "";

    private const float CameraSpeed = 40;
    private AudioSource m_AudioSource;
    private bool m_IsInsideTrigger = false;
    private GameObject m_Triggerer;

    public bool IsAssigned => m_AffectedDoors != null;
    
    private void Start() {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if(Time.timeScale == 0)
            return;
        if(Input.GetButtonDown(Controls.Action.ToString()) && m_IsInsideTrigger) {
            m_AudioSource.Play();
            bool wasLocked = false;
            foreach(SlidingDoor door in m_AffectedDoors) {
                if(door.m_IsLocked) {
                    wasLocked = true;
                    door.Unlock();
                }
            }
            if(wasLocked) {
                // StartCoroutine(ShowUnlockedDoors());
                string msg = m_AreaName + (m_AffectedDoors.Length > 1 ? Strings.GetMessage(Message.DoorsUnlocked) : Strings.GetMessage(Message.DoorUnlocked));
                ScreenUI.DisplayMessage(msg);
            }
        }
    }

    // private IEnumerator ShowUnlockedDoors() { // not used
    //     m_GameController.SetPlayerControls(false);
    //     Camera camera = Camera.main;
    //     CameraController cameraController = camera.GetComponent<CameraController>();
    //     cameraController.m_FollowPlayer = false;
    //     Vector3 initialCameraPosition = camera.transform.position;

    //     Vector3 targetCameraPosition = new Vector3();
    //     foreach(SlidingDoor door in m_AffectedDoors) {
    //         targetCameraPosition += door.transform.position;
    //     }
    //     targetCameraPosition /= m_AffectedDoors.Length;
    //     targetCameraPosition.y = camera.transform.position.y;

    //     yield return cameraController.MoveCamera(camera, targetCameraPosition, CameraSpeed);
    //     yield return new WaitForSeconds(.5f);

    //     foreach(SlidingDoor door in m_AffectedDoors) {
    //         door.Unlock();
    //     }
    //     yield return new WaitForSeconds(2);
    //     yield return cameraController.MoveCamera(camera, initialCameraPosition, CameraSpeed);
    //     Camera.main.GetComponent<CameraController>().m_FollowPlayer = true;
    //     m_GameController.SetPlayerControls(true);
    // }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Player") {
            m_Triggerer = other.gameObject;
            m_IsInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.tag == "Player") {
            m_IsInsideTrigger = false;
        }
    }
}