using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Most of this code is taken from https://github.com/SebLague/Field-of-View
public class FieldOfView : MonoBehaviour {

    [Range(0, 100)]
    public float m_ViewRadius = 20;

    [Range(0, 360)]
    public float m_ViewAngle = 120;

    private LayerMask m_PlayerMask;
    private LayerMask m_ObstacleMask;
    private LayerMask m_EnemyMask;
    private LayerMask m_CombinedMask; // decides what the FoV collides against

    private List<Transform> m_VisibleTargets = new List<Transform>();

    private const float MeshResolution = 1f;
    private const int EdgeResolveIterations = 5; 
    private const float EdgeDistanceTreshold = .8f;

    private Mesh m_ViewMesh;
    private float m_LastSearched;
    private const float SearchInterval = .15f;
    private const float DrawMeshInterval = .02f;

    private const float EyeHeight = 1.6f;
    private readonly Vector3 EyePosition = Vector3.up * EyeHeight; // the height from which the enemy detects objects
    private Vector3 m_SightOrigin;
    private MeshRenderer m_HelperRenderer;


    // variables for FindVisibleTargets
    private Collider[] targetsInViewRadius;
    private Transform target;
    private Vector3 dirToTarget;
    private RaycastHit hitinfo;
    private float distToTarget;


    // variables for DrawFieldOfView
    private int stepCount; // how many endpoints there are. if set to m_ViewAngle it means 1 point per view angle degree
    private float stepAngleSize; // distance between two end points
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

    //Public properties
    public Transform VisibleTarget => m_VisibleTargets[0];
    public bool HasTargetInSight => m_VisibleTargets.Count > 0;

    private void Start() {
        float FOVHeightFromGround = 1 - EyeHeight;
        transform.Find("FOVVisualization").transform.position += Vector3.up * FOVHeightFromGround; // to raise the fov from the ground. don't want it in eye height, since then it covers many objects

        m_PlayerMask = LayerMask.GetMask("Players");
        m_ObstacleMask = LayerMask.GetMask("Obstacles");
        m_EnemyMask = LayerMask.GetMask("Enemies");

        MeshFilter viewMeshFilter = transform.Find("FOVVisualization").GetComponent<MeshFilter>();
        m_ViewMesh = new Mesh();
        m_ViewMesh.name = "View Mesh";
        viewMeshFilter.mesh = m_ViewMesh;

        MeshFilter FOVHelper = transform.Find("FOVHelper").GetComponent<MeshFilter>();
        m_HelperRenderer = FOVHelper.GetComponent<MeshRenderer>();
        Mesh helpMesh = new Mesh();
        helpMesh.name = "Help Mesh";
        FOVHelper.mesh = helpMesh;

        stepCount = 4; // a simple mesh is good enough for the helpmesh
        stepAngleSize = m_ViewAngle / stepCount;
        DrawFieldOfView(helpMesh);

        m_CombinedMask = LayerMask.GetMask("Enemies", "Obstacles"); // must not set mask before the help mesh is drawn
        stepCount = Mathf.RoundToInt(m_ViewAngle * MeshResolution);
        stepAngleSize = m_ViewAngle / stepCount;

        // Transform FOVHelper = transform.Find("FOVHelper");
        // float increasedRange = 1.1f;
        // FOVHelper.localScale += new Vector3((m_ViewRadius - FOVHelper.localScale.x) * increasedRange, 0, (m_ViewRadius - FOVHelper.localScale.z) * increasedRange);
    }

    private void LateUpdate() {
        if(Time.timeScale == 0)
            return;
        
        if(m_HelperRenderer.isVisible) {
            if(Time.time > m_LastSearched + DrawMeshInterval)
                DrawFieldOfView(m_ViewMesh);
            if(Time.time > m_LastSearched + SearchInterval)
                SearchForTargets();
        }
    }

    private void SearchForTargets() {
        m_VisibleTargets.Clear();
        m_LastSearched = Time.time;
        
        // Finds all colliders in a sphere with the center at the transform (enemy combatant)
        targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_PlayerMask);
        foreach(Collider targetInView in targetsInViewRadius) {
            target = targetInView.transform;
            dirToTarget = (target.position - transform.position).normalized;

            // ...check if the collider is in front of the enemy combatant within a specified view angle
            if(Vector3.Angle(transform.forward, dirToTarget) < m_ViewAngle / 2) {
                distToTarget = Vector3.Distance(transform.position, target.position);
                
                // Check if sight is obstructed by obstacles or other enemies
                if(!Physics.Raycast(m_SightOrigin, dirToTarget, out hitinfo, distToTarget, m_CombinedMask)) {
                    m_VisibleTargets.Add(target);
                }
            }
        }
    }

    private void DrawFieldOfView(Mesh mesh) { // draws the FoV mesh consisting of many triangles originating from the transform it's attached to
        m_SightOrigin = transform.position + EyePosition;
        viewPoints.Clear();
        oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i ++) {
            angle = transform.eulerAngles.y - m_ViewAngle / 2 + stepAngleSize * i;
            // Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * m_ViewRadius, Color.red); // debug
            newViewCast = ViewCast(angle);
            if(i > 0) {
                edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > EdgeDistanceTreshold;
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

        verticesList.Add(Vector3.zero + EyePosition);

        for(int i = 0; i < vertexCount - 1; i ++) {
            verticesList.Add(transform.InverseTransformPoint(viewPoints[i]));
            
            if(i < vertexCount - 2) {
                trianglesList.Add(0);
                trianglesList.Add(i + 1);
                trianglesList.Add(i + 2);
            }
        }

        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
    }


    private float minAngle;
    private float maxAngle;
    private Vector3 minPoint;
    private Vector3 maxPoint;
    private ViewCastInfo tempViewCast;
    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        minAngle = minViewCast.angle;
        maxAngle = maxViewCast.angle;
        minPoint = Vector3.zero;
        maxPoint = Vector3.zero;

        for(int i = 0; i < EdgeResolveIterations; i ++) {
            angle = (minAngle + maxAngle) / 2;
            tempViewCast = ViewCast (angle);

            edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - tempViewCast.distance) > EdgeDistanceTreshold;
            if(tempViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = tempViewCast.point;
            }
            else {
                maxAngle = angle;
                maxPoint = tempViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }

    private Vector3 tempVector; 
    public Vector3 DirFromAngle(float angle, bool angleIsGlobal) {
        if(!angleIsGlobal) {
            angle += transform.eulerAngles.y;
        }

        tempVector.Set(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        return tempVector;
    }

    private ViewCastInfo ViewCast(float globalAngle) {
        dir = DirFromAngle(globalAngle, true);
        
        // Debug.DrawRay(m_SightOrigin);
        if(Physics.Raycast(m_SightOrigin, dir, out hit, m_ViewRadius, m_CombinedMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else {
            return new ViewCastInfo(false, m_SightOrigin + dir * m_ViewRadius, m_ViewRadius, globalAngle);
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
