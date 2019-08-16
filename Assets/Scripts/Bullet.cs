using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public GameObject ConcreteImpactEffect;
    public GameObject MetalImpactEffect;
    public GameObject WoodImpactEffect;
    public GameObject NonBlockingImpactEffect;
    public GameObject BodyImpactEffect;
    private GameObject m_UsedImpactEffect;
    
    private ParticleSystem m_Impact;
    private const float BulletVelocityMultiplier = 4000;
    private const float MaximumBulletSurvivalTime = 3;
    private const float BulletDmgMin = 23;
    private const float BulletDmgMax = 27;
    private const float ArmourPiercingMultiplier = 2;
    private float m_DmgMultiplier = 1;
    private RaycastHit m_Hitinfo;
    private Rigidbody m_Body;
    private float m_CalculatedHitTimeStamp;
    private Vector3 m_Origin;
    private Quaternion m_OriginalRotation;
    private Vector3 m_PrevPosition;
    private Collider m_IgnoreCollider;
    private float m_FiredTimestamp;

    private bool m_UsePreInstantiatedEffects = false;

    public void SetStartingPoint(Vector3 startPoint, Quaternion spawnRotation) {
        transform.position = startPoint;
        transform.rotation = spawnRotation;
        AddMomentum();
    }

    public void AddMomentum() {
        m_FiredTimestamp = Time.time;
        m_Origin = m_PrevPosition = transform.position;
        m_OriginalRotation = transform.rotation;
        gameObject.SetActive(true);
        AddForceToBullet();
    }


    private void AddForceToBullet() {
        m_Body = GetComponent<Rigidbody>();
        m_Body.velocity = Vector3.zero;
        m_Body.angularVelocity = Vector3.zero; 
        float force = m_Body.mass * BulletVelocityMultiplier; // to get same velocity regardless of mass (makes mass adjustable)
        m_Body.AddForce(transform.forward * force, ForceMode.Force);
    }

    private void FixedUpdate() {
        CheckCollision();
    }

    private void CheckCollision() {
        if(m_IgnoreCollider != null) { // if bullet has collided with e.g. a cardboard box, it will go through the box
            Physics.IgnoreCollision(GetComponent<Collider>(), m_IgnoreCollider);
            // m_Body.MovePosition(m_Hitinfo.point);
            transform.rotation = m_OriginalRotation;
            AddForceToBullet(); // add new force to bullet after collision
            m_IgnoreCollider = null;
        }
        // cast a ray forwards from its previous position with a distance equal to the distance the bullet has travelled since previous check, and see if there is a collision
        if(Physics.Raycast(m_PrevPosition, transform.forward, out m_Hitinfo, Vector3.Distance(m_PrevPosition, transform.position)) && m_Hitinfo.collider != GetComponent<Collider>()) {
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy") { // hit a combatant
                Combatant target = m_Hitinfo.collider.gameObject.GetComponentInParent<Combatant>();
                target.TakeDamage(Random.Range(BulletDmgMin, BulletDmgMax) * m_DmgMultiplier, m_Origin);
                ShowImpactEffects();
                RemoveBullet();
            }
            else if(m_Hitinfo.collider.tag == "Semiblocking") { // hit e.g. a cardboard box. lets some bullets through
                if(!Physics.GetIgnoreCollision(GetComponent<Collider>(), m_Hitinfo.collider)) {
                    m_Body.MovePosition(m_Hitinfo.point);
                    ShowImpactEffects();
                    if(RandomizePenetration()) {
                        m_IgnoreCollider = m_Hitinfo.collider;
                    }
                    else {
                        RemoveBullet();
                    }
                }
            }
            else if(m_Hitinfo.collider.tag != "Ignored") { // bullet collided with a wall, box, or door or similar
                m_Body.MovePosition(m_Hitinfo.point); // used to guarantee effect on potentially attached rigidbody
                ShowImpactEffects();
                RemoveBullet();
            }
        }
        else if(Time.time > m_FiredTimestamp + MaximumBulletSurvivalTime) { // remove after timeout if no hit
            RemoveBullet();
        }
        
        m_PrevPosition = transform.position;
    }

    private void RemoveBullet() { // used this since I switched between using a pre-instantiated pool of bullets and instantiating new bullet for every shot
        if(m_UsePreInstantiatedEffects)
            ObjectInstantiator.PutBackInPool(gameObject);
        else
            Destroy(gameObject);
    }

    private bool RandomizePenetration() { // used to decide whether a bullet should penetrate an object or not
        return Random.Range(1, 3) == 1;
    }

    private void ShowImpactEffects() {
        if(ConcreteImpactEffect != null) {
            if(m_Hitinfo.collider.tag == "Player" || m_Hitinfo.collider.tag == "Enemy")
                m_UsedImpactEffect = BodyImpactEffect;
            else if(m_Hitinfo.collider.tag == "Metal")
                m_UsedImpactEffect = MetalImpactEffect;
            else if(m_Hitinfo.collider.tag == "Wood")
                m_UsedImpactEffect = WoodImpactEffect;
            else if(m_Hitinfo.collider.tag == "Concrete")
                m_UsedImpactEffect = ConcreteImpactEffect;
            else
                m_UsedImpactEffect = NonBlockingImpactEffect;

            if(m_UsedImpactEffect != null) {
                if(m_UsePreInstantiatedEffects) {
                    // m_Impact = CFX_SpawnSystem.GetNextObject(m_UsedImpactEffect).GetComponent<ParticleSystem>();
                    m_Impact = ObjectInstantiator.GetNextObject(m_UsedImpactEffect).GetComponent<ParticleSystem>();
                    m_Impact.transform.position = m_Hitinfo.point;
                    m_Impact.transform.rotation = Quaternion.LookRotation(m_Hitinfo.normal);
                    m_Impact.transform.SetParent(m_Hitinfo.collider.transform);
                }
                else {
                    m_Impact = Instantiate(m_UsedImpactEffect.GetComponent<ParticleSystem>(), m_Hitinfo.point, Quaternion.LookRotation(m_Hitinfo.normal));

                    // attach bullet hole to collider so it follows the collider transform. does not work well with cfx_spawnsystem
                    Transform bulletHole = m_Impact.transform.Find("Bullet Hole");
                    if(bulletHole != null) {
                        bulletHole.SetParent(m_Hitinfo.collider.transform); 
                        Destroy(bulletHole.gameObject, 10);
                    }
                }

            }
        }
    }

    public void SetArmorPiercing() {
        m_DmgMultiplier = ArmourPiercingMultiplier;
    }

    private int i = 0;
    private Color[] rayColors = { Color.red, Color.green, Color.blue };
    private void DrawDebugLines() {
        // Debug.DrawLine(transform.position, m_Hitinfo.point, rayColors[i % 3], 5);
        Debug.DrawLine(m_PrevPosition, transform.position, rayColors[i % 3], 5);
            i ++;
    }

}
