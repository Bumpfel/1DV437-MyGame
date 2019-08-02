using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Attack : MonoBehaviour {
    
    public Rigidbody m_Bullet;
    public float m_FireRate = .3f; // for automatic firing mode
    public AudioClip m_GunSound;
    public AudioClip m_MeleeAttackSound;

    protected Animator m_Animator;
    protected Combatant m_Combatant;
    private AudioSource m_GunAudioSource;
    private AudioSource m_MeleeAudioSource;
    private Transform m_BulletSpawn;

    private float m_AttackTimestamp;
    private const float MeleeDamage = 100;
    private const float MeleeRange = 2.5f;
    private const float MeleeTime = .35f;
    private const float MaxMeleeAngle = 60;
    private bool m_PlayingMeleeAnimation = false;

    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_GunMiddle");
        m_BulletSpawn = transform.Find("BulletSpawn");

        AudioSource[] audios = GetComponents<AudioSource>(); // There are two different audio sources to make they sure an attack does not interrupt another
        m_GunAudioSource = audios[0];
        m_GunAudioSource.clip = m_GunSound;
        m_MeleeAudioSource = audios[1];
        m_MeleeAudioSource.clip = m_MeleeAttackSound;

        m_Combatant = GetComponent<Combatant>();
    }

    protected void Fire() {
        Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
        bool armourPiercing = false;
        if(armourPiercing = m_Combatant.UseArmourPiercingRounds())
            bullet.gameObject.GetComponent<Bullet>().SetArmorPiercing();
        
        if(tag == "Player") {
            m_Animator.Play("Shoot_single", 0, .25f);
        }
        else
            m_Animator.PlayInFixedTime("Shoot_single", 0, m_FireRate);

        if(armourPiercing)
            m_GunAudioSource.pitch = .85f;
        else
            m_GunAudioSource.pitch = 1;
        m_GunAudioSource.Play();
    }


    /// <summary>
    /// Automatic firing bound by a fire rate. Use for enemy attacks 
    /// </summary>
    protected void ContinuousFire() {
         if(Time.time > m_AttackTimestamp + m_FireRate) {
            Fire();
            m_AttackTimestamp = Time.time;
         }
    }

    protected void StopContinuousFire() {
        m_Animator.Play("Idle_Shoot");
    }

    protected void PerformMeleeAttack() {
        if(Time.time > m_AttackTimestamp + MeleeTime) {
            m_AttackTimestamp = Time.time;
            m_Animator.Play("basic_Melee_Attack", 0, .1f);
            m_MeleeAudioSource.Play();

            LayerMask enemyMask = LayerMask.GetMask("Enemies");
            LayerMask obstacleMask = LayerMask.GetMask("Obstacles");

            // Find all enemy colliders overlapping a sphere around the player
            Collider[] colliders = Physics.OverlapSphere(transform.position, MeleeRange, enemyMask);
            foreach(Collider collider in colliders) {
                Vector3 directionToTarget = collider.transform.position - transform.position;
                
                // filter colliders by those that are in an arc in front of the combatant (player)
                float angleToTarget = Vector3.Angle(transform.forward + transform.right * .2f, directionToTarget); // adjusting what I consider to be "in front" of the combatant for a melee attack 
                if(angleToTarget < MaxMeleeAngle / 2) {
                    // Debug.DrawRay(transform.position, directionToTarget, Color.white, .1f);
                    // check if an obstacle is in the way
                    if(!Physics.Raycast(transform.position, directionToTarget, MeleeRange, obstacleMask)) {
                        collider.gameObject.GetComponent<Combatant>().TakeDamage(MeleeDamage);
                    }
                }
            }
        }
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
        while(timeTaken < MeleeTime) {
            timeTaken += Time.deltaTime;
            m_Animator.SetFloat("meleeSpeed", timeTaken / MeleeTime);
            if(timeTaken / MeleeTime > .85) {
                PerformMeleeAttack();
            }
            yield return new WaitForEndOfFrame();
        }
        m_PlayingMeleeAnimation = false;
    }

}





