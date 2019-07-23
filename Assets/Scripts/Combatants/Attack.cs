using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    
    public Rigidbody m_Bullet;
    public float m_FireRate = .3f;
    public AudioClip m_GunSound;
    private float m_DestroyBulletAfterSeconds = 3f;
    private Transform m_BulletSpawn;
    protected Animator m_Animator;
    private float m_FireTimestamp;

    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_GunMiddle");
        m_BulletSpawn = transform.Find("BulletSpawn");
    }

    public void Fire() {
        Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
        // m_Animator.Play("Shoot_single");
        m_Animator.PlayInFixedTime("Shoot_single", 0, m_FireRate);
        // m_Animator.ResetTrigger("Relax");
        // m_Animator.SetTrigger("Attack");
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = m_GunSound;
        audio.Play();
        bullet.velocity = bullet.transform.forward * m_Bullet.GetComponent<BulletScript>().m_BULLET_VELOCITY;
        Destroy(bullet.gameObject, m_DestroyBulletAfterSeconds);
    }


    /// <summary>
    /// Automatic firing bound by a fire rate. Use for enemy attacks 
    /// </summary>
    public void ContinuousFire() {
         if(Time.time > m_FireTimestamp + m_FireRate) {
            Fire();
            m_FireTimestamp = Time.time;
         }
    }

    public void StopContinuousFire() {
        m_Animator.Play("Idle_Shoot");
        // m_Animator.ResetTrigger("Attack");
        // m_Animator.SetTrigger("Relax");
    }

}
