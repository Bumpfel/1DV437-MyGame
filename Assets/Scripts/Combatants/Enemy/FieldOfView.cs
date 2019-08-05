using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Most of this code is taken from https://github.com/SebLague/Field-of-View
public class FieldOfView : MonoBehaviour {

    [Range(0, 100)]
    public float m_ViewRadius = 20f;

    [Range(0, 360)]
    public float m_ViewAngle = 120f;

    private LayerMask m_PlayerMask;
    private LayerMask m_ObstacleMask;
    private LayerMask m_EnemyMask;
    private LayerMask m_CombinedMask;

    [HideInInspector]
    public List<Transform> m_VisibleTargets = new List<Transform>();

    // [Range(0.1f, 2)]
    private const float m_MeshResolution = 1;
    private const int m_EdgeResolveIterations = 4;
    private const float m_EdgeDistanceTreshold = .5f;

    private Mesh m_ViewMesh;
    private Combatant m_Combatant;
    private float m_LastSearched;
    private const float m_SearchInterval = .2f;
    private Vector3 m_RaycastOrigin;
    private float m_EyePosition = 1.7f;

    private MeshRenderer m_HelperRenderer;


    // temp variables for FindVisibleTargets
    private Collider[] targetsInViewRadius;
    private Transform target;
    private Vector3 dirToTarget;
    private RaycastHit hitinfo;
    private float distToTarget;


    // temp variables for DrawFieldOfView
    private int stepCount;
    private float stepAngleSize;
    private List<Vector3> viewPoints = new List<Vector3>();
    private ViewCastInfo oldViewCast = new ViewCastInfo();
    private float angle;
    private ViewCastInfo newViewCast;
    private bool edgeDistanceThresholdExceeded;
    private EdgeInfo edge;
    private int vertexCount;
    private List<Vector3> verticesList = new List<Vector3>();
    private List<int> trianglesList = new List<int>();

    // vars for viewcastinfo
    Vector3 dir;
    RaycastHit hit;
    RaycastHit hit2;

    void Start() {
        m_PlayerMask = LayerMask.GetMask("Players");
        m_ObstacleMask = LayerMask.GetMask("Obstacles");
        m_EnemyMask = LayerMask.GetMask("Enemies");

        m_CombinedMask = LayerMask.GetMask("Enemies", "Obstacles");

        MeshFilter viewMeshFilter = transform.Find("FOVVisualization").GetComponent<MeshFilter>();
        m_ViewMesh = new Mesh();
        m_ViewMesh.name = "View Mesh";
        viewMeshFilter.mesh = m_ViewMesh;

        Transform FOVHelper = transform.Find("FOVHelper");
        float increasedRange = 1.1f;
        FOVHelper.localScale += new Vector3((m_ViewRadius - FOVHelper.localScale.x) * increasedRange, 0, (m_ViewRadius - FOVHelper.localScale.z) * increasedRange);
        m_HelperRenderer = FOVHelper.GetComponent<MeshRenderer>();

        m_Combatant = GetComponent<Combatant>();
    }

    void LateUpdate() {
        // if(!m_Combatant.IsDead()) {
            if(m_HelperRenderer.isVisible && !m_Combatant.m_IsAsleep) {
                DrawFieldOfView();
                if(Time.time > m_LastSearched + m_SearchInterval)
                    FindVisibleTarget();
            }
        // }
        // else {
        //     m_ViewMesh.Clear();
        //     m_VisibleTargets.Clear();
        // }
        
    }

    public bool HasTargetInView() {
        return m_VisibleTargets.Count > 0;
    }

    private void FindVisibleTarget() {
        m_VisibleTargets.Clear();
        m_LastSearched = Time.time;
        
        // Finds all colliders in a sphere with the center at the enemy combatant
        targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_PlayerMask);
        foreach(Collider targetInView in targetsInViewRadius) {
            target = targetInView.transform;
            dirToTarget = (target.position - transform.position).normalized;

            // ...check if the collider is in front of the enemy combatant within a specified view angle
            if(Vector3.Angle(transform.forward, dirToTarget) < m_ViewAngle / 2) {
                distToTarget = Vector3.Distance(transform.position, target.position);
                
                m_RaycastOrigin = transform.position + Vector3.up * m_EyePosition;

                // Check if sight is obstructed by obstacles or other enemies
                
                // if(!Physics.Raycast(m_RaycastOrigin, dirToTarget, out hitinfo, distToTarget)) {
                //     if(hitinfo.collider && hitinfo.collider.tag == "Player")
                //         m_VisibleTargets.Add(target);
                // }

                if(!Physics.Raycast(m_RaycastOrigin, dirToTarget, out hitinfo, distToTarget, m_CombinedMask)) {
                    m_VisibleTargets.Add(target);
                }
            }
        }
    }

    private void DrawFieldOfView() { // draws the FoV mesh consisting of many triangles originating from the transform it's attached to
        stepCount = Mathf.RoundToInt(m_ViewAngle * m_MeshResolution); // how many endpoints there are. if set to m_ViewAngle it means 1 point per view angle degree
        stepAngleSize = m_ViewAngle / stepCount; // distance between two end points
        viewPoints.Clear();
        oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i ++) {
            angle = transform.eulerAngles.y - m_ViewAngle / 2 + stepAngleSize * i;
            // Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * m_ViewRadius, Color.red); // debug
            newViewCast = ViewCast(angle);
            if(i > 0) {
                edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > m_EdgeDistanceTreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)) {
                    edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }
                    if(edge.pointB != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }

                }
            }
            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        vertexCount = viewPoints.Count + 1;
        verticesList.Clear();
        trianglesList.Clear();

        verticesList.Add(Vector3.zero);

        for(int i = 0; i < vertexCount - 1; i ++) {
            verticesList.Add(transform.InverseTransformPoint(viewPoints[i]));
            
            if(i < vertexCount - 2) {
                trianglesList.Add(0);
                trianglesList.Add(i + 1);
                trianglesList.Add(i + 2);
            }
        }

        m_ViewMesh.Clear();
        m_ViewMesh.vertices = verticesList.ToArray();
        m_ViewMesh.triangles = trianglesList.ToArray();
        m_ViewMesh.RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i = 0; i < m_EdgeResolveIterations; i ++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast (angle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > m_EdgeDistanceTreshold;
            if(newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    public Vector3 DirFromAngle(float angle, bool angleIsGlobal) {
        if(!angleIsGlobal) {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private ViewCastInfo ViewCast(float globalAngle) {
        dir = DirFromAngle(globalAngle, true);
        
        if(Physics.Raycast(transform.position, dir, out hit, m_ViewRadius, m_CombinedMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else {
            return new ViewCastInfo(false, transform.position + dir * m_ViewRadius, m_ViewRadius, globalAngle);
        }
    }

    private struct ViewCastInfo {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _distance, float _angle) {
            hit = _hit;
            point = _point;
            distance = _distance;
            angle = _angle;
        }
    }

    private struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
