using UnityEngine;

public class PickupObject : MonoBehaviour {
    
    public enum Type { Heal, ArmourPiercingRounds, Armour };
    public Type m_Type = Type.Heal;
    public AudioClip m_AudioClip;
    [Range(0, 1)]
    public float m_AudioClipVolume = 1;

    // private AudioSource m_Audiosource;

    private const float HealAmount = 50;
    private const float ArmourAmount = 25;
    private const int ArmourPiercingRoundsAmount = 10;
    private const float RotationSpeed = .5f;

    void Start() {
        // m_AudioSource = GetComponent<AudioSource>();
    }

    void Update() {
        Spin();
    }

    private void Spin() {
        transform.Rotate(0, 360 * RotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Combatant player = other.GetComponent<Combatant>();

            if(m_Type == Type.Heal) {
                if(player.GetHealth() < 100) {
                    player.Heal(HealAmount);
                    PickUpObject();
                }
            }
            else {
                if(m_Type == Type.ArmourPiercingRounds) {
                    player.AddArmourPiercingRounds(ArmourPiercingRoundsAmount);
                }
                else if(m_Type == Type.Armour) {
                    player.AddArmour(ArmourAmount);
                }
                PickUpObject();
            }
        }
    }
    private void PickUpObject() {
        // m_AudioSource.Play();
        AudioSource.PlayClipAtPoint(m_AudioClip, transform.position, m_AudioClipVolume);
        // GetComponent<BoxCollider>().enabled = false;
        // // need to destroy this first to hide the object on trigger without stopping the audio clip
        // Destroy(GetComponent<Renderer>());
        // foreach(Renderer rend in GetComponentsInChildren<Renderer>()) {
        //     print("destroying " + rend.name);
        //     Destroy(rend);
        // }
        Destroy(gameObject);
    }
}
