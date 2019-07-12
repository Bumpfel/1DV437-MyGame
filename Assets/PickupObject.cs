using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour {

    private float m_RotationSpeed = 0.5f;

    private float m_HealAmount = 50;
    void Start() {
        
    }

    void Update() {
        BounceAndRotate();
    }

    private void BounceAndRotate() {
        transform.Rotate(0, 360 * m_RotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            Health player = other.GetComponent<Health>();
            if(player.GetHealth() < 100) {
                player.Heal(m_HealAmount);
                Destroy(gameObject);
            }
        }
    }
}
