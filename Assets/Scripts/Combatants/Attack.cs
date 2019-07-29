using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    
    public Rigidbody m_Bullet;
    public float m_FireRate = .3f;
    public AudioClip m_GunSound;
    public AudioClip m_MeleeAttackSound;

    protected Animator m_Animator;
    private float m_FireTimestamp;
    private AudioSource m_AudioSource;
    private float m_DestroyBulletAfterSeconds = 3f;
    private Transform m_BulletSpawn;
    
    private float m_MeleeDamage = 100;
    private float m_MeleeRange = 2.5f;
    private float m_MeleeTimestamp;
    private float m_MeleeDelay = .35f;


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
        bullet.velocity = bullet.transform.forward * m_Bullet.GetComponent<BulletScript>().m_BULLET_VELOCITY;
        Destroy(bullet.gameObject, m_DestroyBulletAfterSeconds);
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

    protected void MeleeAttack() {
        if(Time.time > m_MeleeTimestamp + m_MeleeDelay) {
            m_MeleeTimestamp = Time.time;
            m_Animator.PlayInFixedTime("Melee_Attack", 0, .1f);
            m_AudioSource.clip = m_MeleeAttackSound;
            m_AudioSource.Play();
            RaycastHit hitInfo;

            Vector3 origin = transform.position + Vector3.up * 1.5f;
            Debug.DrawLine(origin, origin + transform.forward * m_MeleeRange, Color.white, .2f);
            // Debug.DrawLine(origin, origin + transform.right * -1 + transform.forward * m_MeleeRange, Color.white, .2f);
            // Debug.DrawLine(origin, origin + transform.right * 1 + transform.forward * m_MeleeRange, Color.white, .2f);

            if(Physics.Raycast(origin, transform.forward, out hitInfo, m_MeleeRange) || 
            Physics.Raycast(origin, transform.right * -.4f + transform.forward, out hitInfo, m_MeleeRange) || 
            Physics.Raycast(origin, transform.right * .4f + transform.forward, out hitInfo, m_MeleeRange)) {
                if(hitInfo.collider.tag != tag) {
                    // print("melee attack hit " + hitInfo.collider.name);
                    hitInfo.collider.gameObject.GetComponent<Health>().TakeDamage(m_MeleeDamage);
                }
            }
        }
    }
}
