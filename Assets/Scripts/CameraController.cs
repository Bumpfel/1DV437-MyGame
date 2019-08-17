using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform m_Player;
    public float m_UnitsInFrontOfPlayer = 4;
    private float m_MaxDistanceFromCamera = 12; // when holding ctrl

    private float m_FollowSpeed = .1f;
    public bool m_FollowPlayer = true;
    private string m_FreeLookKey;

    private Vector3 mousePos;
    private Vector3 inputVector;
    private Vector3 maxVector;
    private Vector3 targetPosition;
    
    private Vector3 m_CameraHeightPosition;
    private Vector3 inFrontOfPlayer;
    private float mousePosDistance;
    private float maxVectorDistance;

    private void Start() {
        m_FreeLookKey = Controls.FreeLook.ToString();
        m_CameraHeightPosition = Vector3.up * transform.position.y;
    }

    public void SetPlayer(Transform player) {
        m_Player = player;
    }

    void Update() {
        if(Time.timeScale == 0)
            return;
        // CheckIfWantsToZoom();

        if(m_Player && m_FollowPlayer) {
            if(Input.GetButton(m_FreeLookKey)) {
                inputVector.Set(Input.mousePosition.x, Input.mousePosition.y, transform.position.y);
                mousePos = GetComponent<Camera>().ScreenToWorldPoint(inputVector);
                maxVector = m_Player.position + m_Player.forward * m_MaxDistanceFromCamera;
                mousePosDistance = Vector3.Distance(m_Player.position, mousePos);
                maxVectorDistance = Vector3.Distance(m_Player.position, maxVector);

                if(mousePosDistance > maxVectorDistance) {
                    targetPosition = maxVector + m_CameraHeightPosition;
                }
                else {
                    targetPosition = mousePos + m_CameraHeightPosition;
                }
            }
            else {
                // Makes the camera follow the player with focus slighly in front of the player 
                inFrontOfPlayer = m_Player.forward * m_UnitsInFrontOfPlayer;
                targetPosition.Set(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
            }
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_FollowSpeed);
        }
    }






    // not used

    private float cameraZoom;
    private const float ZoomSpeed = 3;
    private const float MinZoom = 40;
    private const float MaxZoom = 80;
    public void CheckIfWantsToZoom() {
        if(Input.mouseScrollDelta.y != 0) {
            cameraZoom = Mathf.Max(Mathf.Min(GetComponent<Camera>().fieldOfView - Input.mouseScrollDelta.y * ZoomSpeed, MaxZoom), MinZoom);
            GetComponent<Camera>().fieldOfView = cameraZoom;
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
