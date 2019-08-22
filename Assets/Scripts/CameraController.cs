using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform m_Player;

    
    //NCC = New Camera Control, OCC = Old Camera Control
    private const float NCC_MaxDistanceFromCamera = 35;
    private const float OCC_MaxDistanceFromCamera = 12; // when holding ctrl

    private const float NewControlCameraFollowSpeed = .15f;
    private const float OldControlCameraFollowSpeed = .05f;


    // NCC only
    private const float PlayerToReticlePositionBias = 2.5f;
    private float currentCameraFollowSpeed = NewControlCameraFollowSpeed;
    
    //OCC only
    private const float DefaultUnitsInFrontOfPlayer = 4;
    private float m_UnitsInFrontOfPlayer = DefaultUnitsInFrontOfPlayer;
    private string m_CameraSpecialKey;


    public bool m_FollowPlayer = true;
    private Vector3 mousePos;
    private Vector3 inputVector;
    private Vector3 maxVector;
    private Vector3 targetPosition;

    //NCC vars
    private float reticleRelativeXPos;
    private float reticleRelativeZPos;
    private Vector3 relativeReticlePoint;
    
    private Vector3 m_CameraHeightPosition;
    private Vector3 inFrontOfPlayer;
    private float m_MousePosDistance;
    private float m_MaxVectorDistance = OCC_MaxDistanceFromCamera;


    private void Start() {
        m_CameraSpecialKey = Controls.CameraSpecial.ToString();
        m_CameraHeightPosition = Vector3.up * transform.position.y;

        m_UseNewCameraControl = PlayerPrefs.GetInt(Settings.CameraControl.ToString(), 1) == 1 ? true : false;
    }

    public void SetPlayer(Transform player) {
        m_Player = player.Find("Model");
    }


    private bool m_UseNewCameraControl;
    void LateUpdate() {
        if(Time.timeScale == 0)
            return;

        if(Input.GetKeyDown(KeyCode.C)) {
            m_UseNewCameraControl = !m_UseNewCameraControl;
            ScreenUI.DisplayMessage("Switched to " + (m_UseNewCameraControl ? "new camera control" : "legacy camera control"));
            PlayerPrefs.SetInt(Settings.CameraControl.ToString(), m_UseNewCameraControl ? 1 : 0);
            PlayerPrefs.Save();
        }

        if(!m_UseNewCameraControl) {
            if(Input.mouseScrollDelta.y != 0) {
                m_UnitsInFrontOfPlayer = Mathf.Clamp(m_UnitsInFrontOfPlayer + Input.mouseScrollDelta.y, 0, OCC_MaxDistanceFromCamera);
            }
            if(Input.GetMouseButtonUp(2)) {
                m_UnitsInFrontOfPlayer = DefaultUnitsInFrontOfPlayer;
            }
        }

        if(m_UseNewCameraControl)
            NewCameraControl();
        else
           OldCameraControl();
        
        // CheckIfWantsToZoom();
    }


    private void NewCameraControl() {
        if(m_Player && m_FollowPlayer) {
            
            inputVector.Set(Input.mousePosition.x, Input.mousePosition.y, transform.position.y);
            mousePos = GetComponent<Camera>().ScreenToWorldPoint(inputVector);

            reticleRelativeXPos = Mathf.Clamp(m_Player.position.x - mousePos.x, -NCC_MaxDistanceFromCamera, NCC_MaxDistanceFromCamera);
            reticleRelativeZPos = Mathf.Clamp(m_Player.position.z - mousePos.z, -NCC_MaxDistanceFromCamera, NCC_MaxDistanceFromCamera);
            relativeReticlePoint.Set(reticleRelativeXPos, 0, reticleRelativeZPos);

            currentCameraFollowSpeed = NewControlCameraFollowSpeed;
            if(Input.GetButton(m_CameraSpecialKey)) {
                currentCameraFollowSpeed = NewControlCameraFollowSpeed / 3;
            }

            targetPosition = m_Player.position - relativeReticlePoint / PlayerToReticlePositionBias + m_CameraHeightPosition;
            transform.position = Vector3.Lerp(transform.position, targetPosition, currentCameraFollowSpeed);
        }
    }


    private void OldCameraControl() {
        if(Input.GetButton(m_CameraSpecialKey)) {
            inputVector.Set(Input.mousePosition.x, Input.mousePosition.y, transform.position.y);
            mousePos = GetComponent<Camera>().ScreenToWorldPoint(inputVector);
            maxVector = m_Player.position + m_Player.forward * OCC_MaxDistanceFromCamera;
            m_MousePosDistance = Vector3.Distance(m_Player.position, mousePos);
            m_MaxVectorDistance = Vector3.Distance(m_Player.position, maxVector);

            if(m_MousePosDistance > m_MaxVectorDistance) {
                targetPosition = maxVector + m_CameraHeightPosition;
            }
            else {
                targetPosition = mousePos + m_CameraHeightPosition;
            }
        }
        else {
            inFrontOfPlayer = m_Player.forward * m_UnitsInFrontOfPlayer;
            targetPosition.Set(m_Player.position.x + inFrontOfPlayer.x, transform.position.y, m_Player.position.z + inFrontOfPlayer.z);
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, OldControlCameraFollowSpeed);
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
