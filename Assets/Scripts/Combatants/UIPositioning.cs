using UnityEngine;
using UnityEngine.UI;

public class UIPositioning : MonoBehaviour {

    private float m_VerticalOffset = 2.5f;
    private Quaternion m_RelativeRotation;
    private EnemyAttack m_EnemyAttack; 
    private GameObject m_AlertIndicator;

    void Start() {
        m_RelativeRotation = transform.localRotation;
        m_EnemyAttack = GetComponentInParent<EnemyAttack>();
        if(m_EnemyAttack)
            m_AlertIndicator = transform.Find("AlertStatus").gameObject;
    }

    void Update() {
        Vector3 playerPosition = transform.parent.position;
        transform.position = new Vector3(playerPosition.x, transform.position.y, playerPosition.z + m_VerticalOffset);
        transform.rotation = m_RelativeRotation;

        if(m_AlertIndicator) {
            if(m_EnemyAttack.IsAlerted()) {
                m_AlertIndicator.SetActive(true);
            }
            else {
                m_AlertIndicator.SetActive(false);
            }
        }

    }
}