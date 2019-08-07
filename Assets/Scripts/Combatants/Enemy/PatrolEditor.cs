using UnityEngine;
using UnityEditor;

// Gives an indication of patrol path by drawing a cyan line which helps when you place enemies designing levels
[CustomEditor (typeof (EnemyMovement))]
public class PatrolEditor : Editor {

    void OnSceneGUI() {
        EnemyMovement enemy = (EnemyMovement) target;
        Handles.color = Color.yellow;
        if(!Application.isPlaying)
            Handles.DrawLine(enemy.transform.position, enemy.transform.position + enemy.transform.forward * enemy.m_PatrolDistance);
        else
            Handles.DrawLine(enemy.GetStartingPosition(), enemy.GetEndPosition());
    }

}