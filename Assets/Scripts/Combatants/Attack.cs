using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Attack : MonoBehaviour {
    
    public Rigidbody m_Bullet;
    public float m_FireRate = .3f; // for automic firing mode
    public AudioClip m_GunSound;
    public AudioClip m_MeleeAttackSound;

    protected Animator m_Animator;
    private float m_FireTimestamp;
    private AudioSource m_AudioSource;
    private Transform m_BulletSpawn;
    
    private float m_MeleeDamage = 100;
    private float m_MeleeRange = 2.5f;
    private float m_MeleeTimestamp;
    private float m_MeleeTime = .35f;
    private float m_MaxMeleeAngle = 60;
    private bool m_PlayingMeleeAnimation = false;


    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_GunMiddle");
        m_BulletSpawn = transform.Find("BulletSpawn");
        m_AudioSource = GetComponent<AudioSource>();
    }

    protected void Fire() {
        Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
        if(tag == "Player") {
            m_Animator.Play("Shoot_single", 0, .25f);
        }
        else
            m_Animator.PlayInFixedTime("Shoot_single", 0, m_FireRate);
        m_AudioSource.clip = m_GunSound;
        m_AudioSource.Play();
    }


    /// <summary>
    /// Automatic firing bound by a fire rate. Use for enemy attacks 
    /// </summary>
    protected void ContinuousFire() {
         if(Time.time > m_FireTimestamp + m_FireRate) {
            Fire();
            m_FireTimestamp = Time.time;
         }
    }

    protected void StopContinuousFire() {
        m_Animator.Play("Idle_Shoot");
    }

    protected void MeleeAttack() { // external interface. TODO not used. animation didn't look good while sprinting and not calling PerformMeleeAttack immediately made it feel a bit unresponsive
        if(!m_PlayingMeleeAnimation) {
            StartCoroutine(PlayMeleeAnimation());
        }
    }

    private IEnumerator PlayMeleeAnimation() { //TODO  not used
        m_PlayingMeleeAnimation = true;
        float timeTaken = 0;
        m_Animator.Play("Melee_Attack");
        while(timeTaken < m_MeleeTime) {
            timeTaken += Time.deltaTime;
            m_Animator.SetFloat("meleeSpeed", timeTaken / m_MeleeTime);
            if(timeTaken / m_MeleeTime > .85) {
                PerformMeleeAttack();
            }
            yield return new WaitForEndOfFrame();
        }
        m_PlayingMeleeAnimation = false;
    }

    protected void PerformMeleeAttack() {
        m_MeleeTimestamp = Time.time;
        m_Animator.Play("basic_Melee_Attack", 0, .1f);
        m_AudioSource.clip = m_MeleeAttackSound;
        m_AudioSource.Play();

        LayerMask enemyMask = LayerMask.GetMask("Enemies");
        LayerMask obstacleMask = LayerMask.GetMask("Obstacles");

        // Find all enemy colliders overlapping a sphere around the player
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_MeleeRange, enemyMask);
        foreach(Collider collider in colliders) {
            Vector3 directionToTarget = collider.transform.position - transform.position;
            
            // filter colliders by those that are in an arc in front of the combatant (player)
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
            if(angleToTarget < m_MaxMeleeAngle / 2) {
                Debug.DrawRay(transform.position, directionToTarget, Color.white, .1f);
                // check if an obstacle is in the way
                if(!Physics.Raycast(transform.position, directionToTarget, m_MeleeRange, obstacleMask)) {
                    collider.gameObject.GetComponent<Combatant>().TakeDamage(m_MeleeDamage);
                }
            }
        }
    }

}





