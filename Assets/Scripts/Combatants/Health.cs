using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour { // TODO döpa om till något annat? Combatant / CombatantStatus?

    public Slider m_Healthbar;
    public AudioClip m_DeathSound;
    private float m_Health = 100;
    private Animator animator;
    private bool m_IsDead = false;
    private List<Observer> observers = new List<Observer>();
    private GameController m_GameController;

    void Start() {
        m_GameController = GetComponentInParent<GameController>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount) {
        if(!m_IsDead) {
            m_Health -= amount;
            UpdateHealthBar();
            // NotifySubscribers(); // TODO my thought here was for this to enable the enemy turning and shooting in the direction it took dmg
            if(m_Health <= 0) {
                Die();
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

        Destroy(m_Healthbar.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
        
        if(tag == "Player") {
            m_GameController.m_PlayerStats.AddPlayerDeath();

            m_GameController.SetGameOver();
            //Debug
            // m_Health = 100;                
            // UpdateHealthBar();
            // return;
        }
        else {
            m_GameController.m_PlayerStats.AddKill();
        }
    }

    public bool IsDead() {
        return m_IsDead;
    }


    private void UpdateHealthBar() {
        m_Healthbar.value = m_Health;
    }

    public float GetHealth() {
        return m_Health;
    }

}
