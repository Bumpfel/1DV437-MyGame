using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Combatant : MonoBehaviour {
    public AudioClip m_DeathSound;
    public bool m_IsAsleep = false;

    private AudioSource m_AudioSource;
    private GameObject m_CharacterGUI;
    private Slider m_HealthBar;
    private GameObject m_ArmourBuffUI;
    private GameObject m_APRoundsBuffUI;
    private GameObject m_AsleepIndicator;
    private Animator m_Animator;
    private GameController m_GameController;
    private const float m_ArmourResistanceMultiplier = 2;
    private float m_Health = 100;
    public float m_Armour = 0;
    public int m_ArmourPiercingRounds = 0;

    void Start() {
        m_GameController = GetComponentInParent<GameController>();
        m_Animator = GetComponent<Animator>();

        m_CharacterGUI = transform.Find("CharacterGUI").gameObject;
        m_HealthBar = m_CharacterGUI.GetComponentInChildren<Slider>();

        // if(tag == "Player") {
            GameObject buffBar = m_CharacterGUI.transform.Find("BuffBar").gameObject;
            for(int i = 0; i < buffBar.transform.childCount; i ++) {
                GameObject child = buffBar.transform.GetChild(i).gameObject;
                if(child.tag == "ArmourBuff")
                    m_ArmourBuffUI = child;
                else if(child.tag == "APRBuff")
                    m_APRoundsBuffUI = child;
            }

            UpdateAPRBuff();
            UpdateHealthBar();
        // }
        // else {
        if(tag == "Enemy") {
            m_AsleepIndicator = m_CharacterGUI.transform.Find("Asleep").gameObject;
            if(m_IsAsleep)
                m_AsleepIndicator.SetActive(true);
        }
        
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float amount) {
        // if(!IsDead()) {
            WakeUp();

            float unmitigatedDmg = Mathf.Max(0, amount - m_Armour);
            float mitigatedDmg = amount - unmitigatedDmg;
            float incomingDamage = unmitigatedDmg + mitigatedDmg / m_ArmourResistanceMultiplier;
            m_Armour = Mathf.Max(0, m_Armour - incomingDamage);

            m_Health = Mathf.Max(m_Health - incomingDamage, 0);
            UpdateHealthBar();
            if(IsDead()) {
                Die();
                return;
            }
            else if(tag == "Enemy") {
                GetComponent<EnemyMovement>().ReactToTakingDamage();
            }
        // }
    }

    private void WakeUp() {
        m_IsAsleep = false;
        if(tag == "Enemy")
            m_AsleepIndicator.SetActive(false);
    }

    public void Heal(float amount) {
        if(!IsDead()) {
            m_Health = Mathf.Min(100, m_Health + amount);
            UpdateHealthBar();
        }
    }

    private void Die() {
        m_Animator.Play("Die");
        Destroy(m_Animator, 2); // make sure the animator doesn't reset when unity reloads scripts (in editor)
        
        m_AudioSource.clip = m_DeathSound;
        m_AudioSource.Play();

        Destroy(m_CharacterGUI.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
        
        if(tag == "Player") {
            m_GameController.m_PlayerStats.AddPlayerDeath();
            m_GameController.SetGameOver();
        }
        else {
            transform.Find("FOVVisualization").gameObject.SetActive(false);
            m_GameController.m_PlayerStats.AddKill();
        }
        foreach(MonoBehaviour component in GetComponents<MonoBehaviour>()) {
            component.enabled = false;
        }
        // m_Animator.enabled = false;
    }

    public bool IsDead() {
        return m_Health <= 0;
    }

    private void UpdateHealthBar() {
        m_HealthBar.value = m_Health;
        // if(tag == "Player") {
            if(m_Armour == 0)
                m_ArmourBuffUI.SetActive(false);
            else
                m_ArmourBuffUI.GetComponentInChildren<TextMeshProUGUI>().SetText("" + m_Armour);
        // }
    }
    private void UpdateAPRBuff() {
        if(m_ArmourPiercingRounds == 0)
            m_APRoundsBuffUI.SetActive(false);
        else
            m_APRoundsBuffUI.GetComponentInChildren<TextMeshProUGUI>().SetText("" + m_ArmourPiercingRounds);
    }

    public float GetHealth() {
        return m_Health;
    }

    ///<summary>
    /// A combatant gets the specified nr of rounds that deals increased damage
    ///</summary>
    public void AddArmourPiercingRounds(int rounds) {
        m_ArmourPiercingRounds += rounds;
        m_APRoundsBuffUI.SetActive(true);
        UpdateAPRBuff();
    }

    public bool UseArmourPiercingRounds() {
        if(m_ArmourPiercingRounds > 0) {
            m_ArmourPiercingRounds = Mathf.Max(0, m_ArmourPiercingRounds - 1);
            UpdateAPRBuff();
            return true;
        }
        return false;
    }


    ///<summary>
    /// Armour reduces damage taken. Taking damage reduces the amount of armour until it reaches 0
    ///</summary>
    public void AddArmour(float amount) {
        m_Armour += amount;
        m_ArmourBuffUI.SetActive(true);
        UpdateHealthBar();
    }

}
