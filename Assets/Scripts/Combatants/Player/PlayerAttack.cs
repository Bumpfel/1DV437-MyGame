using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
    
    public int m_PlayerNumber = 1; // TODO temp
    private string m_FireButton;
    private Input m_Fire;
    private Attack attack;
    private Health m_Health;
 
    protected void Start() {
        attack = GetComponent<Attack>();
        m_FireButton = "Fire1_Player" + m_PlayerNumber;
        m_Health = GetComponent<Health>();
    }

    void Update() {
        if(!m_Health.IsDead()) {
            AllowShoot();
        }
    }

    private void AllowShoot() {
        if(Input.GetButtonDown(m_FireButton)) {
            attack.Fire();
        }
    }

}
