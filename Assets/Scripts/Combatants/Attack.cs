using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private const float MeleeRange = 2;
    private const float MeleeTime = .2f; //.35f;
    private const float MaxMeleeAngle = 90;
    private bool m_PlayingMeleeAnimation = false;
    private LayerMask m_EnemyMask;
    private LayerMask m_ObstacleMask;

    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_GunMiddle");
        m_BulletSpawn = transform.Find("BulletSpawn");

        AudioSource[] audios = GetComponents<AudioSource>(); // There are two different audio sources to make they sure an attack does not interrupt the sound of the other
        m_GunAudioSource = audios[0];
        m_GunAudioSource.clip = m_GunSound;
        m_MeleeAudioSource = audios[1];
        m_MeleeAudioSource.clip = m_MeleeAttackSound;

        m_Combatant = GetComponent<Combatant>();

        m_EnemyMask = LayerMask.GetMask("Enemies");
        m_ObstacleMask = LayerMask.GetMask("Obstacles");
    }

    protected void Fire() {
        Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
        if(m_Combatant.UseArmourPiercingRounds()) {
            m_GunAudioSource.pitch = .85f;
            bullet.gameObject.GetComponent<Bullet>().SetArmorPiercing();
        }
        else
            m_GunAudioSource.pitch = 1;
        
        if(tag == "Player") {
            m_Animator.Play("Shoot_single", 0, .25f);
        }
        else
            m_Animator.PlayInFixedTime("Shoot_single", 0, m_FireRate);

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

    protected void MeleeAttack() {
        if(Time.time > m_AttackTimestamp + MeleeTime) {
            m_AttackTimestamp = Time.time;
            m_MeleeAudioSource.Play();
            // m_Animator.Play("basic_Melee_Attack", 0, .15f);

            Collider[] colliders = Physics.OverlapSphere(transform.position + transform.forward * .7f, MeleeRange / 2, m_EnemyMask);
            if(colliders.Length > 0) {
                Combatant enemy = colliders[0].gameObject.GetComponent<Combatant>();
                Vector3 directionToTarget = (enemy.transform.position - transform.position).normalized;
                // Debug.DrawRay(transform.position + Vector3.up * 1.5f, directionToTarget, Color.magenta, 2f);

                // checking if there are obstacles in the way. Starting cast from the back of the player collider since starting from center causes problems if too close to the target.
                float colliderRadius = transform.GetComponent<CapsuleCollider>().radius;
                Vector3 origin = transform.position + transform.forward * - colliderRadius / 2;
                if(!Physics.Raycast(origin, directionToTarget, MeleeRange, m_ObstacleMask)) { 
                    enemy.TakeDamage(MeleeDamage, origin);
                }
            }
        }
    }


    // protected void MeleeAttack2() { // external interface. TODO not used. animation didn't look good while sprinting and not calling PerformMeleeAttack immediately made it feel a bit unresponsive
    //     if(!m_PlayingMeleeAnimation) {
    //         StartCoroutine(PlayMeleeAnimation());
    //     }
    // }
    
    // private IEnumerator PlayMeleeAnimation() { // not used
    //     m_PlayingMeleeAnimation = true;
    //     float timeTaken = 0;
    //     m_Animator.Play("animated_Melee_Attack");
    //     while(timeTaken < MeleeTime) {
    //         timeTaken += Time.deltaTime;
    //         m_Animator.SetFloat("meleeSpeed", timeTaken / MeleeTime);
    //         if(timeTaken / MeleeTime > .8) {
    //             MeleeAttack();
    //         }
    //         yield return new WaitForEndOfFrame();
    //     }
    //     m_PlayingMeleeAnimation = false;
    // }

}



