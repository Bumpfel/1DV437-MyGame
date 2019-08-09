using UnityEngine;
using UnityEditor;

public class PlayerAttack : Attack {
    
    public int m_PlayerNumber = 1;
    public bool m_AutomaticFire = false;
    private string m_FireButton;
    private string m_FireButton2;
    private Input m_Fire;
    
    [HideInInspector]
    public bool m_ControlsEnabled = true;

    private PlayerMovement m_PlayerMovement;
 
    new void Start() {
        base.Start();
        m_FireButton = Strings.Controls.Fire1_Player.ToString() + m_PlayerNumber;
        m_FireButton2 = Strings.Controls.Fire2_Player.ToString() + m_PlayerNumber;

        m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if(m_ControlsEnabled) {
            if(m_AutomaticFire)
                AutoFireIfTriggered();
            else
                FireIfTriggered();
            MeleeIfTriggered();
        }
    }
    
    private void FireIfTriggered() {
        if(Input.GetButtonDown(m_FireButton) && !m_PlayerMovement.IsRunning()) {
            SingleFire();
        }
    }

    private void AutoFireIfTriggered() {
        if(Input.GetButton(m_FireButton) && !m_PlayerMovement.IsRunning()) {
            AutomaticFire();
        }
        else if(Input.GetButtonUp(m_FireButton)) {
            StopAutomaticFire();
        }
    }

    private void MeleeIfTriggered() {
        if(Input.GetButtonDown(m_FireButton2)) {
            MeleeAttack();
        }
    }

}

// [CustomEditor (typeof(PlayerAttack))]
// public class PlayerAttackEditor : Editor {
//     private float MeleeRange = 2;
//     private PlayerAttack attacker;

//     void OnSceneGUI() {
//         attacker = (PlayerAttack) target;

//         Handles.color = Color.magenta;
//         // Handles.DrawWireArc(attacker.transform.position + attacker.transform.forward * .7f, Vector3.up, Vector3.forward, 360, MeleeRange / 2);
//         // Handles.DrawLine(attacker.transform.position, attacker.targetPosition);
//     }


// }