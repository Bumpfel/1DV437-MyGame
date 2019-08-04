using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(PlayerAttack))]
public class PlayerAttackEditor : Editor {
    private float MeleeRange = 2;
    private PlayerAttack attacker;

    void OnSceneGUI() {
        attacker = (PlayerAttack) target;

        Handles.color = Color.magenta;
        Handles.DrawWireArc(attacker.transform.position + attacker.transform.forward * .7f, Vector3.up, Vector3.forward, 360, MeleeRange / 2);
        // Handles.DrawLine(attacker.transform.position, attacker.targetPosition);
    }


}
