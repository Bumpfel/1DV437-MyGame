using UnityEngine;

public class PlayerAttack : Attack {
    
    public int m_PlayerNumber = 1;
    private string m_FireButton;
    private string m_FireButton2;
    private Input m_Fire;
    
    [HideInInspector]
    public bool m_GamePaused = false;

    private PlayerMovement m_PlayerMovement;
 
    new void Start() {
        base.Start();
        m_FireButton = "Fire1_Player" + m_PlayerNumber;
        m_FireButton2 = "Fire2_Player" + m_PlayerNumber;
        m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if(!m_GamePaused) {
            FireIfTriggered();
            MeleeIfTriggered();
        }
    }

    protected override void Fire() {
        base.Fire();
        m_Animator.Play("Shoot_single", 0, .25f);
    }


    private void FireIfTriggered() {
        if(Input.GetButtonDown(m_FireButton) && !m_PlayerMovement.IsRunning()) {
            Fire();
        }
    }

    private void MeleeIfTriggered() {
        if(Input.GetButtonDown(m_FireButton2)) {
            // PerformMeleeAttack();
            MeleeAttack();
        }
    }

}
