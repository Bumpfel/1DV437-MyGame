using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript_bkup : MonoBehaviour {
    
    public LayerMask m_CombatantsMask;
    public float dmg = 25f;
    public readonly float m_BULLET_VELOCITY = 100;
    private RaycastHit m_Hitinfo;
    private float m_CalculatedHitTimeStamp;

    void Start() {
        if(Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, Mathf.Infinity)) {
            float travelTime = m_Hitinfo.distance / m_BULLET_VELOCITY;

            m_CalculatedHitTimeStamp = Time.time + travelTime;
        }
        // else if(Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, Mathf.Infinity)) {
        //     print("bullet hit " + m_Hitinfo.collider.name);
        // }
    }


    void FixedUpdate() {
        if(m_Hitinfo.collider && Time.time > m_CalculatedHitTimeStamp) {
            RaycastHit hitinfo;
            if(Physics.Raycast(transform.position, transform.forward, out hitinfo, Mathf.Infinity))
                print("bullet hit " + hitinfo.collider.gameObject.name);
            // else
            //     print("bullet missed");

            // Debug.Log("bullet hit " + m_Hitinfo.collider.name);
            if(m_Hitinfo.collider.tag == "")
            Destroy(gameObject);

            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                m_Hitinfo.collider.gameObject.GetComponent<Health>().TakeDamage(dmg); //TODO should I bother with different dmg  on different colliders, i.e. less dmg if hit an arm vs e.g. torso
            }
        }      
    }
}
