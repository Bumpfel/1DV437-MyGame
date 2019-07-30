using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    
    public LayerMask m_CombatantsMask;
    public float dmg = 25;
    private readonly float m_BULLET_VELOCITY = 100;
    private RaycastHit m_Hitinfo;
    private float m_CalculatedHitTimeStamp;
    private float m_DestroyBulletAfterSeconds = 3f;

    void Start() {
        Destroy(gameObject, m_DestroyBulletAfterSeconds);
    }
    
    void FixedUpdate() {
        // cast a ray forwards with a distance equal to the distance the bullet travels in one fixed udpate and see if there is a collision
        if(Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, m_BULLET_VELOCITY * Time.fixedDeltaTime)) {
            // DrawDebugLines();
            Destroy(gameObject);

            if(m_Hitinfo.collider.tag != "Ignored") {
                if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                    m_Hitinfo.collider.gameObject.GetComponent<Combatant>().TakeDamage(dmg); //TODO should I bother with different dmg  on different colliders, i.e. less dmg if hit an arm vs e.g. torso
                }
            }
        }
        MoveBullet();
    }

    private void MoveBullet() {
        transform.position = transform.position + transform.forward * m_BULLET_VELOCITY * Time.fixedDeltaTime;
    }


    private int i = 0;
    private Color[] rayColors = { Color.red, Color.green, Color.blue };
    private void DrawDebugLines() {
        Debug.DrawLine(transform.position, m_Hitinfo.collider.transform.position, rayColors[i % 3], 5);
            i ++;
    }

}
