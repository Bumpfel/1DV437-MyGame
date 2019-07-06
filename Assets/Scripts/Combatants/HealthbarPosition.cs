using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthbarPosition : MonoBehaviour {

    private Quaternion m_RelativeRotation;

    private void Start() {
        m_RelativeRotation = transform.parent.localRotation;
    }

    void Update() { //TODO creating a lot of objects here...
        Vector3 playerPosition = transform.parent.parent.position;
        Vector3 offsetPosition = new Vector3(playerPosition.x, transform.position.y, playerPosition.z + 2f);
        transform.position = offsetPosition;
        transform.rotation = m_RelativeRotation;
    }
}
