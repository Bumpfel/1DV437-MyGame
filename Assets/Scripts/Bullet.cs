using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public GameObject ConcreteImpact;
    public GameObject MetalImpact;
    public GameObject WoodImpact;
    public GameObject BodyImpact;
    private ParticleSystem m_Impact;
    private const float BulletVelocity = 500;
    private const float MaximumBulletSurvivalTime = 3;
    private float m_BulletDmg = 25;
    private RaycastHit m_Hitinfo;
    private Rigidbody m_Body;
    private float m_CalculatedHitTimeStamp;

    private Vector3 m_Origin;
    private Vector3 m_PrevPosition;

    private bool m_ShowImpactEffects;

    private void Start() {
        m_Origin = transform.position;
        m_PrevPosition = transform.position;
        m_Body = GetComponent<Rigidbody>();
        m_Body.AddForce(transform.forward * BulletVelocity, ForceMode.Force);
        Destroy(gameObject, MaximumBulletSurvivalTime);
    }

    private void FixedUpdate() {
        CheckCollision();
    }

    private void CheckCollision() {
        // cast a ray forwards from its previous position with a distance equal to the distance the bullet has travelled since previous check, and see if there is a collision
        Vector3 direction = (transform.position - m_PrevPosition).normalized;
        if(Physics.Raycast(m_PrevPosition, direction, out m_Hitinfo, Vector3.Distance(m_PrevPosition, transform.position)) && m_Hitinfo.collider != GetComponent<Collider>()) {
            print("hit " + m_Hitinfo.collider.name);
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                Combatant target = m_Hitinfo.collider.gameObject.GetComponentInParent<Combatant>();
                target.TakeDamage(m_BulletDmg, m_Origin);
            }
            else if(m_Hitinfo.collider.tag != "Ignored") {
                m_Body.MovePosition(m_Hitinfo.point);
            }
            ShowImpactEffects();
            Destroy(gameObject);
        }
        m_PrevPosition = transform.position;
    }

    private void ShowImpactEffects() {
        if(ConcreteImpact != null) {
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy")
                m_Impact = Instantiate(BodyImpact.GetComponent<ParticleSystem>());
            else if(m_Hitinfo.collider.tag == "Metal")
                m_Impact = Instantiate(MetalImpact.GetComponent<ParticleSystem>());
            else if(m_Hitinfo.collider.tag == "Wood")
                m_Impact = Instantiate(WoodImpact.GetComponent<ParticleSystem>());
            else
                m_Impact = Instantiate(ConcreteImpact.GetComponent<ParticleSystem>());

            // print("impact duration: " + m_Impact.main.duration);
            // Destroy(m_Impact, m_Impact.main.duration);
            // m_Impact.transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            m_Impact.transform.LookAt(m_Hitinfo.normal);
            m_Impact.transform.position = m_Hitinfo.point;
        }
    }

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
