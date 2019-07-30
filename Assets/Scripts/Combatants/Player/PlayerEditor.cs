using UnityEngine;
using UnityEditor;

// [CustomEditor (typeof (PlayerAttack))]
public class PlayerEditor : Editor {
    
    void OnSceneGUI() {
        Attack attacker = (Attack) target;

        float angle = 45;
        Handles.color = Color.green;

        // not correct
        Vector3 relativeAngle = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        Handles.DrawWireArc(attacker.transform.position, Vector3.up, attacker.transform.forward - relativeAngle / 2, angle, 2.5f);
    }


}