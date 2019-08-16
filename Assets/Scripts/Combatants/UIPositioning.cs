using UnityEngine;
using UnityEngine.UI;

public class UIPositioning : MonoBehaviour {

    private Quaternion m_RelativeRotation;
    private EnemyBehaviour m_EnemyAttack; 
    private GameObject m_AlertIndicator;

    private Vector3 playerPosition;
    private const float m_VerticalOffset = 2;

    void Start() {
        m_RelativeRotation = transform.localRotation;
        m_EnemyAttack = GetComponentInParent<EnemyBehaviour>();
        if(m_EnemyAttack)
            m_AlertIndicator = transform.Find("AlertStatus").gameObject;
    }

    void Update() {
        if(Time.timeScale == 0)
            return;
    
        playerPosition = transform.parent.position;
        transform.position = new Vector3(playerPosition.x, transform.position.y , playerPosition.z + m_VerticalOffset);
        transform.rotation = m_RelativeRotation;

        if(m_AlertIndicator) {
            if(m_EnemyAttack.IsAlerted) {
                m_AlertIndicator.SetActive(true);
            }
            else {
                m_AlertIndicator.SetActive(false);
            }
        }

    }
}