using UnityEngine;

public class PickupObject : MonoBehaviour {
    
    public enum Type { Heal, ArmourPiercingRounds, Armour, AutoFire };
    public Type m_Type = Type.Heal;
    private AudioSource m_AudioSource;
     private const float HealAmount = 50;
    private const float ArmourAmount = 25;
    private const int ArmourPiercingRoundsAmount = 10;
    private const float RotationSpeed = .5f;

    void Start() {
        m_AudioSource = GetComponent<AudioSource>();
    }

    void Update() {
        Spin();
    }

    private void Spin() {
        transform.Rotate(0, 360 * RotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            PlayerBehaviour player = other.GetComponent<PlayerBehaviour>();

            if(m_Type == Type.Heal && player.GetHealth() < 100) {
                float healedAmount = Mathf.Round(Mathf.Min(100 - player.GetHealth(), HealAmount));
                ScreenUI.DisplayMessage("Healed for " + healedAmount + " points");
                player.Heal(HealAmount);
                PickUpObject();
            }
            else if(m_Type == Type.ArmourPiercingRounds) {
                player.AddArmourPiercingRounds(ArmourPiercingRoundsAmount);
                ScreenUI.DisplayMessage("Picked up " + ArmourPiercingRoundsAmount + " Armour Piercing Rounds");
                PickUpObject();
            }
            else if(m_Type == Type.Armour) {
                player.AddArmour(ArmourAmount);
                ScreenUI.DisplayMessage("Picked up " + ArmourAmount + " " + m_Type);
                PickUpObject();
            }
            // else if(m_Type == Type.AutoFire) {
            //     player.GetComponent<PlayerAttack>().m_AutomaticFire = true;
            //     ScreenUI.DisplayMessage("Picked up Automatic Fire");
            //     PickUpObject();
            // }
        }
    }


    private void PickUpObject() {
        // AudioSource.PlayClipAtPoint(m_AudioClip, transform.position, m_AudioClipVolume);
        m_AudioSource.Play();

        // disabling renderers and colliders on the object to not stop the audio clip
        GetComponent<BoxCollider>().enabled = false;
        Renderer singleRenderer = GetComponent<Renderer>();
        if(singleRenderer != null) {
            singleRenderer.enabled = false;
        }
        else {
            foreach(Renderer renderer in GetComponentsInChildren<Renderer>()) {
                renderer.enabled = false;
            }
        }
        Destroy(gameObject, 1); //m_AudioSource.clip.length);
    }
}
