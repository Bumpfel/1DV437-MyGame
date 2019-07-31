using UnityEngine;
using UnityEditor;

public class GameButton : MonoBehaviour {
    public SlidingDoor[] m_Triggers;

    private GameController m_GameController;

    void Start() {
        m_GameController = FindObjectOfType<GameController>();
    }

    void OnTriggerStay(Collider other) {
        if(other.tag == "Player" && Input.GetButtonDown(m_GameController.m_ActionKey)) {
            bool wasLocked = false;
            foreach(SlidingDoor door in m_Triggers) {
                if(door.m_IsLocked) {
                    wasLocked = true;
                    door.Unlock();
                }
            }
            if(wasLocked) {
                m_GameController.DisplayMessage((m_Triggers.Length > 1 ? "Doors" : "Doors") + " unlocked");
            }
        }
    }
}

[CustomEditor (typeof(GameButton))]
public class AffectedDoors : Editor {

    void OnSceneGUI() {
        GameButton button = (GameButton) target;
    
        if(button.m_Triggers != null) {
            foreach(SlidingDoor door in button.m_Triggers) {
                Handles.color = Color.grey;
                Handles.DrawLine(button.transform.position, door.transform.position);
            }
        }

    }

}