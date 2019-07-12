using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public Slider m_Healthbar;
    public AudioClip m_DeathSound;
    private float m_Health = 100;
    // private float m_despawnAfterDeathSeconds = 2f;
    private Animator animator;
    private bool m_IsDead = false;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount) {
        if(!m_IsDead) {
            m_Health -= amount;
            UpdateHealthBar();
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
        //Debug
        if(tag == "Player") {
            m_Health = 100;
            UpdateHealthBar();
            return;
        }
        m_IsDead = true;
        animator.Play("Die");
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = m_DeathSound;
        audio.Play();

        Destroy(m_Healthbar.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
        
        // Destroy(gameObject, m_despawnAfterDeathSeconds);
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
