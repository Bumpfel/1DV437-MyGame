using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    
    public LayerMask m_CombatantsMask;
    public float dmg = 25f;
    public readonly float m_BULLET_VELOCITY = 100;
    private RaycastHit m_Hitinfo;
    private float m_CalculatedHitTimeStamp;

    void FixedUpdate() {
        //for every frame cast a raycast forwards with a distance equal to the distance the bullet travels per frame (and see if there is a collision)
        Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, m_BULLET_VELOCITY * Time.fixedDeltaTime);
        
        if(m_Hitinfo.collider && m_Hitinfo.collider.tag != "Ignored") {
            Destroy(gameObject);

            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                m_Hitinfo.collider.gameObject.GetComponent<Health>().TakeDamage(dmg); //TODO should I bother with different dmg  on different colliders, i.e. less dmg if hit an arm vs e.g. torso
            }
        }      
    }
}
