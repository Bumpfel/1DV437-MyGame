using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public GameObject ConcreteImpactEffect;
    public GameObject MetalImpactEffect;
    public GameObject WoodImpactEffect;
    public GameObject BodyImpactEffect;
    private GameObject m_UsedImpactEffect;
    private ParticleSystem m_Impact;
    private const float BulletVelocity = 500;
    private const float MaximumBulletSurvivalTime = 3;
    private const float BulletDmgMin = 20;
    private const float BulletDmgMax = 25;
    private const float ArmourPiercingMultiplier = 2;
    private  float m_DmgMultiplier = 1;
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
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") {
                Combatant target = m_Hitinfo.collider.gameObject.GetComponentInParent<Combatant>();
                target.TakeDamage(Random.Range(BulletDmgMin, BulletDmgMax) * m_DmgMultiplier, m_Origin);
                ShowImpactEffects();
                Destroy(gameObject);
            }
            else if(m_Hitinfo.collider.tag != "Ignored") {
                m_Body.MovePosition(m_Hitinfo.point);
                ShowImpactEffects();
                Destroy(gameObject);
            }
        }
        m_PrevPosition = transform.position;
    }

    private void ShowImpactEffects() {
        if(ConcreteImpactEffect != null) {
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy")
                m_UsedImpactEffect = BodyImpactEffect;
            else if(m_Hitinfo.collider.tag == "Metal")
                m_UsedImpactEffect = MetalImpactEffect;
            else if(m_Hitinfo.collider.tag == "Wood")
                m_UsedImpactEffect = WoodImpactEffect;
            else
                m_UsedImpactEffect = ConcreteImpactEffect;

            m_Impact = Instantiate(m_UsedImpactEffect.GetComponent<ParticleSystem>(), m_Hitinfo.point, Quaternion.LookRotation(m_Hitinfo.normal), m_Hitinfo.collider.transform);
            // print("impact duration: " + m_Impact.main.duration);
            // Destroy(m_Impact, m_Impact.main.duration);
        }
    }

    public void SetArmorPiercing() {
        m_DmgMultiplier = ArmourPiercingMultiplier;
    }

    private int i = 0;
    private Color[] rayColors = { Color.red, Color.green, Color.blue };
    private void DrawDebugLines() {
        Debug.DrawLine(transform.position, m_Hitinfo.collider.transform.position, rayColors[i % 3], 5);
            i ++;
    }

}
