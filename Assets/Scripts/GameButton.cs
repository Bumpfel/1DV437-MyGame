using System.Collections;
using UnityEngine;
using UnityEditor;

public class GameButton : MonoBehaviour {
    public SlidingDoor[] m_AffectedDoors = null;

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

    public float cameraVelocity = 30;
    private IEnumerator ShowUnlockedDoor(SlidingDoor[] doors) {
        Camera camera = Camera.main;
        camera.GetComponent<CameraController>().m_FollowPlayer = false;
        Vector3 initialCameraPosition = camera.transform.position;
        Vector3 targetCameraPosition = doors[0].transform.position;
        targetCameraPosition.y = camera.transform.position.y;

        yield return MoveCamera(camera, initialCameraPosition, targetCameraPosition);

        yield return new WaitForSeconds(.5f);
        foreach(SlidingDoor door in doors) {
            door.Unlock();
        }
        yield return new WaitForSeconds(2);
        Camera.main.GetComponent<CameraController>().m_FollowPlayer = true;
    }

    private IEnumerator MoveCamera(Camera camera, Vector3 from, Vector3 to) {
        float distance = Vector3.Distance(from, to);

        float duration = distance / cameraVelocity;
        // float timeTaken = 0;
        // while(timeTaken < duration) {
        //     timeTaken += Time.deltaTime;
        while(camera.transform.position != to) {
            if(Input.GetKeyDown(KeyCode.F12)) {
                break;
            }
            camera.transform.position = Vector3.Lerp(from, to, cameraVelocity * Time.deltaTime);
            // camera.transform.position = Vector3.Lerp(from, to, timeTaken / duration);
            yield return new WaitForEndOfFrame();
        }
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