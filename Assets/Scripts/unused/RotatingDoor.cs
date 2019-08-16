using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingDoor : MonoBehaviour {

    public float m_RotationAngle = 90;
    private bool m_IsOpen = false;

    private float m_OpenTimestamp;
    private float m_OpenDelay = .1f;
    private float m_RotationSpeed = .2f;
    private Quaternion m_DoorRotation;


    void FixedUpdate() {
        transform.rotation = Quaternion.Lerp(transform.rotation, m_DoorRotation, m_RotationSpeed);
    }

    void OnTriggerStay(Collider other) {
        if(Input.GetButtonDown(Strings.Controls.Action.ToString()) && Time.time > m_OpenTimestamp + m_OpenDelay) {
            m_OpenTimestamp = Time.time;
            m_IsOpen = !m_IsOpen;
            // transform.Rotate(0, m_IsOpen ? m_RotationAngle : -m_RotationAngle, 0);

            float rotationAngle = m_IsOpen ? m_RotationAngle : -m_RotationAngle;
            m_DoorRotation = transform.rotation * Quaternion.Euler(0, rotationAngle, 0);
            
        }
    }

}
