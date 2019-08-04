using System.Collections;
using UnityEngine;
using UnityEditor;

public class GameButton : MonoBehaviour {
    public SlidingDoor[] m_AffectedDoors = null;

    private const float CameraSpeed = 40;
    private GameController m_GameController;
    private AudioSource m_AudioSource;
    private bool m_IsInsideTrigger = false;

    private GameObject m_Triggerer;

    private void Start() {
        m_GameController = FindObjectOfType<GameController>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if(Input.GetButtonDown(m_GameController.m_ActionKey) && m_IsInsideTrigger) {
            m_AudioSource.Play();
            bool wasLocked = false;
            foreach(SlidingDoor door in m_AffectedDoors) {
                if(door.m_IsLocked) {
                    wasLocked = true;
                }
            }
            // if(wasLocked) {
                StartCoroutine(ShowUnlockedDoor(m_AffectedDoors));
                m_GameController.DisplayMessage((m_AffectedDoors.Length > 1 ? "Doors" : "Door") + " unlocked");
            // }
        }
    }

    private IEnumerator ShowUnlockedDoor(SlidingDoor[] doors) {
        m_GameController.TogglePlayerControl(true);
        Camera camera = Camera.main;
        CameraController cameraController = camera.GetComponent<CameraController>();
        cameraController.m_FollowPlayer = false;
        Vector3 initialCameraPosition = camera.transform.position;
        Vector3 targetCameraPosition = doors[0].transform.position;
        targetCameraPosition.y = camera.transform.position.y;

        yield return cameraController.MoveCamera(camera, targetCameraPosition, CameraSpeed);

        yield return new WaitForSeconds(.5f);
        foreach(SlidingDoor door in doors) {
            door.Unlock();
        }
        yield return new WaitForSeconds(2);
        yield return cameraController.MoveCamera(camera, initialCameraPosition, CameraSpeed);
        Camera.main.GetComponent<CameraController>().m_FollowPlayer = true;
        m_GameController.TogglePlayerControl(false);
    }

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

    public bool IsAssigned() {
        return m_AffectedDoors != null;
    }
}

[CustomEditor (typeof(GameButton))]
public class AffectedDoors : Editor {

    void OnSceneGUI() {
        GameButton button = (GameButton) target;
    
        if(button.IsAssigned()) {
            Handles.color = Color.grey;
            foreach(SlidingDoor door in button.m_AffectedDoors) {
                if(door != null)
                    Handles.DrawLine(button.transform.position, door.transform.position);
            }
        }

    }

}