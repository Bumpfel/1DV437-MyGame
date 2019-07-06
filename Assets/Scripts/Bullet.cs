using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public LayerMask m_CombatantsMask;
    public float dmg = 25f;

    private RaycastHit m_Hitinfo;
    private float m_HitTimeStamp;
    private bool m_Hit = false;

    void Start() {
        if(Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, Mathf.Infinity, m_CombatantsMask)) {
            m_Hit = true;
            float bulletVelocity = m_Hitinfo.collider.GetComponent<PlayerShooting>().m_BULLET_VELOCITY;
            float travelTime = m_Hitinfo.distance / bulletVelocity;

            m_HitTimeStamp = Time.time + travelTime;
            print("bullet hit " + m_Hitinfo.collider.name);
        }
    }


    void Update() {
        if(m_Hit && Time.time > m_HitTimeStamp) {
            Destroy(gameObject);
            m_Hitinfo.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(dmg); //TODO should I bother with different dmg  on different colliders, i.e. less dmg if hit an arm vs e.g. torso
        }
    }

    // private void OnTriggerEnter(Collider other) {
    //     // Collider[] colliders = Physics.OverlapSphere(transform.position, 0);
        
    //     if(other.tag == "Player") {
    //         print("bullet hit " + other.name);
    //     }

    //     Collider[] colliders = Physics.OverlapSphere(transform.position, 0, m_CombatantsMask); //TODO capsule?

    //     // print("bullet collided with colliders " + colliders.Length + " game objects");
    //     foreach(Collider collider in colliders) {
    //         print("bullet hit " + collider.name);
    //     }
    // }
}
