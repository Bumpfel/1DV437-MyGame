using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform m_Player;
    public float m_UnitsInFrontOfPlayer = 3;
    private float m_MaxDistanceFromCamera = 12;

    public bool m_FollowPlayer = true;

    public void SetPlayer(Transform player) {
        m_Player = player;
    }

    void Update() {
        if(m_Player && m_FollowPlayer) {
            // Makes the camera follow the player with focus slighly in front of the player 
            Vector3 inFrontOfPlayer = m_Player.forward * m_UnitsInFrontOfPlayer;
            transform.position = new Vector3(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
            // transform.position = m_Player.position + Vector3.up * transform.position.y;

            if(Input.GetKey(KeyCode.LeftControl)) {
                Vector3 mousePos = GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.y));
                Vector3 maxVector = m_Player.position + m_Player.forward * m_MaxDistanceFromCamera;
                float mousePosDistance = Vector3.Distance(m_Player.position, mousePos);
                float maxVectorDistance = Vector3.Distance(m_Player.position, maxVector);

                if(mousePosDistance > maxVectorDistance) {
                    transform.position = maxVector + Vector3.up * transform.position.y;
                }
                else {
                    transform.position = mousePos + Vector3.up * transform.position.y;
                }
                
            }
        }
    }

}
