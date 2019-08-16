using UnityEngine;

public class PlayerBehaviour : Combatant {
  
    private new void Start() {
        base.Start();
    }
    
    public void Heal(float amount) {
        m_Health = Mathf.Min(100, m_Health + amount);
        UpdateHealthBar();
    }

    protected override void Die() {
        base.Die();
        m_GameController.m_PlayerStats.AddPlayerDeath();
        m_GameController.SetGameOver();
    }
    
    public void AddArmourPiercingRounds(int rounds) {
        m_ArmourPiercingRounds += rounds;
        m_APRoundsBuffUI.SetActive(true);
        UpdateAPRBuff();
    }

    public void AddArmour(float amount) {
        m_Armour += amount;
        m_ArmourBuffUI.SetActive(true);
        UpdateHealthBar();
    }
    
}