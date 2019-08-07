using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform m_Player;
    public float m_UnitsInFrontOfPlayer = 3;
    private float m_MaxDistanceFromCamera = 12; // when holding ctrl

    private float m_FollowSpeed = .1f;
    public bool m_FollowPlayer = true;

    public void SetPlayer(Transform player) {
        m_Player = player;
    }

    void FixedUpdate() {
        if(m_Player && m_FollowPlayer) {
            if(!Input.GetKey(KeyCode.LeftControl)) {
                // Makes the camera follow the player with focus slighly in front of the player 
                Vector3 inFrontOfPlayer = m_Player.forward * m_UnitsInFrontOfPlayer;
                
                
                //standard
                // transform.position = new Vector3(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
                

                //smooth
                Vector3 to = new Vector3(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
                transform.position = Vector3.Lerp(transform.position, to, m_FollowSpeed);


                // transform.position = m_Player.position + Vector3.up * transform.position.y;
            }
            else {
            // if(Input.GetKey(KeyCode.LeftControl)) {
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

    public IEnumerator MoveCamera(Camera camera, Vector3 to, float cameraSpeed) {
        Vector3 from = camera.transform.position;
        float distance = Vector3.Distance(from, to);

        float duration = distance / cameraSpeed;
        float timeTaken = 0;
        while(timeTaken < duration) {
            timeTaken += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(from, to, timeTaken / duration);
            yield return new WaitForEndOfFrame();
        }
    }

}
