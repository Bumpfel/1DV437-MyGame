using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, Observer {

    public Slider m_Healthbar;
    public AudioClip m_DeathSound;
    private float m_Health = 100;
    private Animator animator;
    private bool m_IsDead = false;

    private List<Observer> observers = new List<Observer>();

    private PlayerStats m_PlayerStats;

    public void ObserverUpdate() {
        m_PlayerStats = GetComponentInParent<GameController>().m_PlayerStats; // needed because this script might be loaded before the playerstats script in gamecontroller
    }

    void Start() {
        GetComponentInParent<GameController>().AddObserver(this);
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
        if(tag == "Player") {
            m_PlayerStats.AddPlayerDeath();

            //Debug
            m_Health = 100;                
            UpdateHealthBar();
            return;
        }
        else {
            m_PlayerStats.AddKill();
        }
        m_IsDead = true;
        animator.Play("Die");
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = m_DeathSound;
        audio.Play();

        Destroy(m_Healthbar.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
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
