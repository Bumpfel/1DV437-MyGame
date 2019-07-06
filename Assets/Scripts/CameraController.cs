using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform player;
    public float unitsInFrontOfPlayer = 3;
    // private Vector3 velocity;

    void Start() {
        
    }

    void Update() {
        // Makes the camera follow the player with focus slighly in front of the player 
        Vector3 inFrontOfPlayer = player.forward * unitsInFrontOfPlayer;
        transform.position = new Vector3(player.position.x + inFrontOfPlayer.x, transform.position.y, player.position.z + inFrontOfPlayer.z);
    }
}
