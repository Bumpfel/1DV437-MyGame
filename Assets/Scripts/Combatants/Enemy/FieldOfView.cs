﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour {

    [Range(0, 100)]
    public float m_ViewRadius = 20f;

    [Range(0, 360)]
    public float m_ViewAngle = 120f;

    private LayerMask m_TargetMask;
    private LayerMask m_ObstacleMask;

    [HideInInspector]
    public List<Transform> m_VisibleTargets = new List<Transform>();

    public float m_MeshResolution = 5;
    public int m_EdgeResolveIterations = 3;
    public float m_EdgeDistanceTreshold = .5f;

    private Mesh m_ViewMesh;
    private Health m_Health;

    private float m_LastSearched;
    private float m_SearchInterval = .2f;
    public Vector3 m_RayCastOrigin; // TODO public for debug

    private float m_EyePosition = 1.7f; // TODO not sure if I'll use

    public Vector3 m_DebugDirToTarget; // TODO debug

    public bool m_DrawDebugLine = true; // TODO debug


    void Start() {
        m_TargetMask = LayerMask.GetMask("Players");
        m_ObstacleMask = LayerMask.GetMask("Obstacles");

        MeshFilter viewMeshFilter = GetComponentInChildren<MeshFilter>();
        m_ViewMesh = new Mesh();
        m_ViewMesh.name = "View Mesh";
        viewMeshFilter.mesh = m_ViewMesh;
        
        m_Health = GetComponent<Health>();
    }

    void LateUpdate() {
        if(!m_Health.IsDead()) {
            DrawFieldOfView();
            if(Time.time > m_LastSearched + m_SearchInterval)
                FindVisibleTarget();
        }
        else {
            m_ViewMesh.Clear();
            m_VisibleTargets.Clear();
        }
    }
    void FindVisibleTarget() {
        new WaitForEndOfFrame();
        m_VisibleTargets.Clear();
        m_LastSearched = Time.time;
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, m_ViewRadius, m_TargetMask);

        foreach(Collider targetInView in targetsInViewRadius) {
            Transform target = targetInView.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            
            m_DebugDirToTarget = target.position; // TODO debug

            if(Vector3.Angle(transform.forward, dirToTarget) < m_ViewAngle / 2) {
                float distToTarget = Vector3.Distance(transform.position, target.position);
                
                m_RayCastOrigin = transform.position + Vector3.up * m_EyePosition;

                // Player located within search arc. Check if sight is obstructed
                if(!Physics.Raycast(m_RayCastOrigin, dirToTarget, distToTarget, m_ObstacleMask)) {
                    m_VisibleTargets.Add(target);
                }
                // RaycastHit hitinfo;
                // if(Physics.Raycast(m_RayCastOrigin, dirToTarget, out hitinfo)) {
                    
                //     if(hitinfo.collider && hitinfo.collider.tag == "Player") {
                //         m_DrawDebugLine =  true;
                //         m_VisibleTargets.Add(target);
                //     }
                //     else 
                //         m_DrawDebugLine = false;

                //     if(hitinfo.collider)
                //         print(name + " detected " + hitinfo.collider.name);
                // }
            }
        }
    }

    void DrawFieldOfView() {
        int stepCount = Mathf.RoundToInt(m_ViewAngle * m_MeshResolution);
        float stepAngleSize = m_ViewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i <= stepCount; i ++) {
            float angle = transform.eulerAngles.y - m_ViewAngle / 2 + stepAngleSize * i;
            // Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * m_ViewRadius, Color.red); // debug
            ViewCastInfo newViewCast = ViewCast(angle);
            if(i > 0) {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > m_EdgeDistanceTreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded)) {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
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

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for(int i = 0; i < vertexCount - 1; i ++) {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            
            if(i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        m_ViewMesh.Clear();
        m_ViewMesh.vertices = vertices;
        m_ViewMesh.triangles = triangles;
        m_ViewMesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
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
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, dir, out hit, m_ViewRadius, m_ObstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else {
            return new ViewCastInfo(false, transform.position + dir * m_ViewRadius, m_ViewRadius, globalAngle);
        }
    }

    public struct ViewCastInfo {
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

    public struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB) {
            pointA = _pointA;
            pointB = _pointB;
        }
    }

}
