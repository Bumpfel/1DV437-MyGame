using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform m_Player;
    public float m_UnitsInFrontOfPlayer = 3;
    // private Vector3 velocity;

    void Start() {
    }

    public void SetPlayer(Transform player) {
        m_Player = player;
    }

    void Update() {
        if(m_Player) {
            // Makes the camera follow the player with focus slighly in front of the player 
            Vector3 inFrontOfPlayer = m_Player.forward * m_UnitsInFrontOfPlayer;
            transform.position = new Vector3(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
        }
    }
}
