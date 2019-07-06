using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarPosition : MonoBehaviour {

    public float m_VerticalOffset = 2.5f;
    private Quaternion m_RelativeRotation;

    private void Start() {
        m_RelativeRotation = transform.localRotation;
    }

    void Update() {
        Vector3 playerPosition = transform.parent.position;
        transform.position = new Vector3(playerPosition.x, transform.position.y, playerPosition.z + m_VerticalOffset);
        transform.rotation = m_RelativeRotation;
    }
}
