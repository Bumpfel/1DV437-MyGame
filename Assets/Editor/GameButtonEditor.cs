using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(GameButton))]
public class AffectedDoors : Editor {

    void OnSceneGUI() {
        GameButton button = (GameButton) target;
    
        if(button.IsAssigned) {
            Handles.color = Color.grey;
            foreach(SlidingDoor door in button.m_AffectedDoors) {
                if(door != null)
                    Handles.DrawLine(button.transform.position, door.transform.position);
            }
        }

    }

}