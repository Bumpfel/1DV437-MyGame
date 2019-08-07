using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public LayerMask m_CombatantsMask;
    private const float BulletVelocity = 100;
    private const float DestroyBulletAfterSeconds = 3;
    private float m_BulletDmg = 25;
    private RaycastHit m_Hitinfo;
    private float m_CalculatedHitTimeStamp;

    private Vector3 m_Origin;

    void Start() {
        m_Origin = transform.position;
        Destroy(gameObject, DestroyBulletAfterSeconds);
    }
    
    void FixedUpdate() {
        // cast a ray forwards with a distance equal to the distance the bullet travels in one fixed udpate and see if there is a collision
        if(Physics.Raycast(transform.position, transform.forward, out m_Hitinfo, BulletVelocity * Time.fixedDeltaTime)) {
            // DrawDebugLines();
            if(m_Hitinfo.collider.tag != "Ignored") {
                if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                    Combatant target = m_Hitinfo.collider.gameObject.GetComponentInParent<Combatant>();
                    target.TakeDamage(m_BulletDmg, m_Origin);
                }
                // if(m_Hitinfo.collider.tag == "Enemy") {
                    // var tempObj = new GameObject("temp");
                    // tempObj.transform.position = m_Origin;
                    // m_Hitinfo.collider.GetComponentInParent<FieldOfView>().m_VisibleTargets.Add(tempObj.transform);
                // }
            }
            Destroy(gameObject);
            
        }
        MoveBullet();
    }

    private void MoveBullet() {
        transform.position = transform.position + transform.forward * BulletVelocity * Time.fixedDeltaTime;
    }

//     private void MoveRigidbodyBullet() {
//         GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * BulletVelocity * Time.fixedDeltaTime);
// ;
//     }

    public void SetArmorPiercing() {
        m_BulletDmg *= 2;
    }


    private int i = 0;
    private Color[] rayColors = { Color.red, Color.green, Color.blue };
    private void DrawDebugLines() {
        Debug.DrawLine(transform.position, m_Hitinfo.collider.transform.position, rayColors[i % 3], 5);
            i ++;
    }

}
