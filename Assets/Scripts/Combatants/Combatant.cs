using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Combatant : MonoBehaviour {
    public AudioClip m_DeathSound;
    public float m_Armour = 0;
    public int m_ArmourPiercingRounds = 0;
    public GameObject m_CharacterGUI;
    private float GUIVerticalOffset = 1.5f;

    protected float m_Health = 100;
    protected GameController m_GameController;
    protected GameObject m_ArmourBuffUI;
    protected GameObject m_APRoundsBuffUI;

    private AudioSource m_AudioSource;
    private Slider m_HealthBar;
    private Animator m_Animator;
    private const float m_ArmourResistanceMultiplier = 2;

    protected bool IsDead => m_Health <= 0;

    protected void Start() {
        m_GameController = FindObjectOfType<GameController>();
        m_Animator = GetComponent<Animator>();

        m_CharacterGUI.transform.position += Vector3.forward * GUIVerticalOffset;
        m_CharacterGUI.transform.rotation = transform.parent.localRotation * Quaternion.Euler(90, 0, 0);
        m_HealthBar = m_CharacterGUI.GetComponentInChildren<Slider>();

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
      
        m_AudioSource = GetComponent<AudioSource>();
    }

    public virtual void TakeDamage(float amount, Vector3 dmgSource) {
        float unmitigatedDmg = Mathf.Max(0, amount - m_Armour);
        float mitigatedDmg = amount - unmitigatedDmg;
        float incomingDamage = unmitigatedDmg + mitigatedDmg / m_ArmourResistanceMultiplier;
        m_Armour = Mathf.Max(m_Armour - incomingDamage, 0);

        m_Health = Mathf.Max(m_Health - incomingDamage, 0);
        UpdateHealthBar();

        if(IsDead) {
            Die();
            return;
        }
    }

    protected virtual void Die() {
        m_Animator.Play("Die");
        // float animLen = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        Destroy(m_Animator, 1.5f);

        m_AudioSource.clip = m_DeathSound;
        m_AudioSource.Play();

        m_CharacterGUI.gameObject.SetActive(false);
        GetComponent<CapsuleCollider>().enabled = false;

        foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
            // script.enabled = false;
            Destroy(script);
        }
    }

    public float GetHealth() {
        return m_Health;
    }

    protected void UpdateHealthBar() {
        m_HealthBar.value = m_Health;
        if(m_Armour == 0)
            m_ArmourBuffUI.SetActive(false);
        else
            m_ArmourBuffUI.GetComponentInChildren<TextMeshProUGUI>().SetText("" + Mathf.Round(m_Armour));
    }
    protected void UpdateAPRBuff() {
        if(m_ArmourPiercingRounds == 0)
            m_APRoundsBuffUI.SetActive(false);
        else
            m_APRoundsBuffUI.GetComponentInChildren<TextMeshProUGUI>().SetText("" + m_ArmourPiercingRounds);
    }

    public bool UseArmourPiercingRounds() {
        if(m_ArmourPiercingRounds > 0) {
            m_ArmourPiercingRounds = Mathf.Max(0, m_ArmourPiercingRounds - 1);
            UpdateAPRBuff();
            return true;
        }
        return false;
    }

}
