﻿using UnityEngine;

public class PlayerAttack : Attack {
    public bool m_AutomaticFire = true;
    private string m_FireButton;
    private string m_FireButton2;
    private string m_FiringModeSwitchButton;
    private Input m_Fire;

    private PlayerMovement m_PlayerMovement;
 
    new void Start() {
        base.Start();
        m_FireButton = Controls.Fire1.ToString();
        m_FireButton2 = Controls.Fire2.ToString();
        m_FiringModeSwitchButton = Controls.SwitchFiringMode.ToString();

        m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    void Update() {
        if(Time.timeScale == 0)
            return;
        CheckIfWantsToSwitchFiringMode();
        CheckIfWantsToFire();
        CheckIfWantsToMelee();
    }

    private void CheckIfWantsToFire() {
        if(Input.GetButton(m_FireButton) && !m_PlayerMovement.IsRunning) {
            if(m_AutomaticFire && Input.GetButton(m_FireButton))
                Fire();
            else if(!m_AutomaticFire && Input.GetButtonDown(m_FireButton))
                Fire();
        }
    }

    private void CheckIfWantsToMelee() {
        if(Input.GetButtonDown(m_FireButton2)) {
            MeleeAttack();
        }
    }

    private void CheckIfWantsToSwitchFiringMode() {
        if(Input.GetButtonDown(m_FiringModeSwitchButton)) {
            m_AutomaticFire = !m_AutomaticFire;
            string msg = m_AutomaticFire ? Strings.GetMessage(Message.SwitchedToAutoFire) : Strings.GetMessage(Message.SwitchedToSingleFire);
            ScreenUI.DisplayMessage(msg);
        }
    }

}