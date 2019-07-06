using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    
    public Rigidbody m_Bullet;
    public AudioClip m_GunSound;
    private float m_DestroyBulletAfterSeconds = 3f;
    private Transform m_BulletSpawn;
    private Animator m_Animator;
 
    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_Shoot");
        m_BulletSpawn = transform.Find("BulletSpawn");
    }

    public void Fire() {
        Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
        m_Animator.Play("Shoot_single");
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = m_GunSound;
        audio.Play();
        bullet.velocity = bullet.transform.forward * m_Bullet.GetComponent<BulletScript>().m_BULLET_VELOCITY;
        Destroy(bullet.gameObject, m_DestroyBulletAfterSeconds);
    }

}
