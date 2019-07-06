using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {
    
    public int m_PlayerNumber = 1; // TODO temp
    public Rigidbody m_Bullet;
    private Transform m_BulletSpawn;
    private Animator m_Animator;
    private Rigidbody m_Player;
    public readonly float m_BULLET_VELOCITY = 100;
    private string m_FireButton;
    private Input Fire;
 
    protected void Start() {
        m_Animator = GetComponent<Animator>();
        m_Animator.Play("Idle_Shoot");
        m_Player = GetComponent<Rigidbody>();

        m_BulletSpawn = transform.Find("BulletSpawn");

        m_FireButton = "Fire1_Player" + m_PlayerNumber;
    }


    void Update() {
        Shoot();
    }

    private void Shoot() {

        if(Input.GetButtonDown(m_FireButton)) {
            Rigidbody bullet = Instantiate(m_Bullet, m_BulletSpawn.position, m_BulletSpawn.rotation);
            m_Animator.Play("Shoot_single");
            bullet.velocity = bullet.transform.forward * m_BULLET_VELOCITY;
            Destroy(bullet.gameObject, 3f); // destroys bullet within a certain time
            // TODO play audio
            
            //crap
            // AnimationClip clip = GetComponent<Animation>().GetClip("Shoot single");
            // clip.
        }
    }

}
