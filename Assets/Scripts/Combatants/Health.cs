using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour { // TODO döpa om till något annat? Combatant / CombatantStatus?

    private GameObject m_CharacterGUI;
    private Slider m_HealthBar;
    public AudioClip m_DeathSound;
    private float m_Health = 100;
    private Animator animator;
    private bool m_IsDead = false;
    private GameController m_GameController;

    void Start() {
        m_GameController = GetComponentInParent<GameController>();
        animator = GetComponent<Animator>();

        m_CharacterGUI = transform.Find("CharacterGUI").gameObject;
        m_HealthBar = m_CharacterGUI.GetComponentInChildren<Slider>();
    }

    public void TakeDamage(float amount) {
        if(!m_IsDead) {
            m_Health -= amount;
            UpdateHealthBar();
            if(m_Health <= 0) {
                Die();
            }
            else if(tag == "Enemy") {
                GetComponent<EnemyMovement>().ReactToTakingDamage();
            }
        }
    }

    public void Heal(float amount) {
        if(!m_IsDead) {
            m_Health = Mathf.Min(100, m_Health + amount);
            UpdateHealthBar();
        }
    }
    
    private void Die() {
        m_IsDead = true;
        animator.Play("Die");
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = m_DeathSound;
        audio.Play();

        Destroy(m_CharacterGUI.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
        
        if(tag == "Player") {
            m_GameController.m_PlayerStats.AddPlayerDeath();

            m_GameController.SetGameOver();
            //Debug
            // m_Health = 100;
            // UpdateHealthBar();
        }
        else {
            m_GameController.m_PlayerStats.AddKill();
        }
    }

    public bool IsDead() {
        return m_IsDead;
    }

    private void UpdateHealthBar() {
        m_HealthBar.value = m_Health;
    }

    public float GetHealth() {
        return m_Health;
    }

}
