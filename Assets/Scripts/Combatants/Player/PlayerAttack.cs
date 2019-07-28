using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : Attack {
    
    public int m_PlayerNumber = 1; // TODO temp
    private string m_FireButton;
    private Input m_Fire;
    private Health m_Health;
    
    [HideInInspector]
    public bool m_GamePaused = false;

    private PlayerMovement m_PlayerMovement;
 
    new void Start() {
        base.Start();
        m_FireButton = "Fire1_Player" + m_PlayerNumber;
        m_Health = GetComponent<Health>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if(!m_Health.IsDead() && !m_GamePaused) {
            AllowShoot();
        }
    }

    private void AllowShoot() {
        if(Input.GetButtonDown(m_FireButton) && !m_PlayerMovement.IsRunning()) {
            Fire();
        }
    }


}
