using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour {
    
    public enum Type { Heal, Armour };
    public Type m_Type = Type.Heal;
    private float m_RotationSpeed = 0.5f;
    private float m_HealAmount = 50;

    void Update() {
        Spin();
    }

    private void Spin() {
        transform.Rotate(0, 360 * m_RotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            if(m_Type == Type.Heal) {
                Health player = other.GetComponent<Health>();
                if(player.GetHealth() < 100) {
                    player.Heal(m_HealAmount);
                    AudioSource audio = GetComponent<AudioSource>();
                    audio.Play();
                    Destroy(GetComponent<MeshRenderer>()); // need to destroy this first to hide the object on trigger
                    Destroy(gameObject, 1f);
                }
            }
            else {
                //not a heal item
            }
        }
    }
}
