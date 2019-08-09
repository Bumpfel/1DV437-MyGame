using System.Collections;
using UnityEngine;

public class EnemyBehaviour : Combatant {
    public bool m_IsAsleep = false;

    private const float WakeUpDelayOnTakingDmg = .4f;
    private const float WakeUpDelayOnCollider = 1;
    private GameObject m_AsleepIndicator;
    private MonoBehaviour m_FieldOfView;
    private EnemyMovement m_EnemyMovement;


    private new void Start() {
        base.Start();
        m_EnemyMovement = GetComponent<EnemyMovement>();

        m_AsleepIndicator = m_CharacterGUI.transform.Find("Asleep").gameObject;
        if(m_IsAsleep) {
            m_AsleepIndicator.SetActive(true);
            foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
                if(script != this) {
                    script.enabled = false;
                }
            }
            // GetComponent<FieldOfView>().enabled = false;
            // GetComponent<FieldOfView>().enabled = false;
        }
    }
    
    private void OnCollisionEnter(Collision collision) {
        if(collision.collider.tag != "Ignored" && m_IsAsleep)
          StartCoroutine(WakeUp(WakeUpDelayOnCollider, false));
    }

    public override void TakeDamage(float amount, Vector3 dmgSource) {
        base.TakeDamage(amount, dmgSource);
        if(m_IsAsleep) {
            StartCoroutine(WakeUp(WakeUpDelayOnTakingDmg, true));
        }
        else if(!IsDead()) {
            m_EnemyMovement.ReactToTakingDamage();
        }
    }

    private IEnumerator WakeUp(float delay, bool tookDmg) {
        m_AsleepIndicator.SetActive(false);
        m_IsAsleep = false;
        yield return new WaitForSeconds(delay);
        if(!IsDead()) {
            foreach(MonoBehaviour script in GetComponents<MonoBehaviour>()) {
                script.enabled = true;
            }
            if(tookDmg) {
                m_EnemyMovement.ReactToTakingDamage();
            }
        }
    }

    protected override void Die() {
        base.Die();
        transform.Find("FOVVisualization").gameObject.SetActive(false);
        m_GameController.m_PlayerStats.AddKill();
        
        foreach(MonoBehaviour component in GetComponents<MonoBehaviour>()) {
            component.enabled = false;
        }
    }   

}