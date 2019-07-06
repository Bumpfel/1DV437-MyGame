using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public Slider m_Healthbar;
    private float m_Health = 100;
    private Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount) {
        m_Health -= amount;
        m_Healthbar.value = m_Health;
        if(m_Health <= 0) {
            Die();
        }
    }
    
    private void Die() {
        animator.Play("Die");
    }


}
