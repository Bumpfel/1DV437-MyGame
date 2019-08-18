using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {

    private FieldOfView fov;
    Vector3 viewAngleA;
    Vector3 viewAngleB;


    void OnSceneGUI() {
        fov = (FieldOfView) target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.m_ViewRadius);
        viewAngleA = fov.DirFromAngle(-fov.m_ViewAngle / 2, false);
        viewAngleB = fov.DirFromAngle(fov.m_ViewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.m_ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.m_ViewRadius);

        Handles.color = Color.red;
        // foreach(Transform visibleTargets in fov.m_VisibleTargets) {
        //     // Handles.DrawLine(fov.transform.position, visibleTargets.position);

        //     // Handles.color = Color.green;
        //     // Handles.DrawLine(fov.m_RayCastOrigin, fov.m_DebugDirToTarget);
        // }
    }

}
