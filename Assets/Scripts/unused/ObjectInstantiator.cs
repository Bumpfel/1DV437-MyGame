using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiator : MonoBehaviour {
    public GameObject[] m_Object;
    public int[] m_PoolSize;
    private static ObjectInstantiator singleton;
    private static GameObject nextObject;
    private Dictionary<int, Stack<GameObject>> m_Dictionary = new Dictionary<int, Stack<GameObject>>();

    private void Start() {
            Initialize();
    }
    
    private void OnEnable() {
        Initialize();
    }

    private void OnDisable() {
        foreach(Stack<GameObject> pool in m_Dictionary.Values) {
            while(pool.Count > 0) {
                Destroy(pool.Pop());
            }
        }
    }

    private void Initialize() {
        for(int i = 0 ; i < m_Object.Length; i ++) {
            CreateObjectPool(m_Object[i], m_PoolSize[i]);
        }
        singleton = this;
    }

    private void CreateObjectPool(GameObject prefabSource, int size) {
        m_Dictionary.Add(prefabSource.GetInstanceID(), new Stack<GameObject>());
        print("creating pool for " + prefabSource.name + ": " + prefabSource.GetInstanceID());
        for(int i = 0; i < size; i ++) {
            AddObjectToPool(prefabSource);
        }
    }

    private GameObject AddObjectToPool(GameObject prefabSource) {
        var obj = Instantiate(prefabSource);
        // obj.hideFlags = HideFlags.HideInHierarchy;
        obj.SetActive(false);
        m_Dictionary[prefabSource.GetInstanceID()].Push(obj);
        return obj;
    }

    public static GameObject GetNextObject(GameObject prefabSource) {
        print("getting next object for " + prefabSource.name + ": " + prefabSource.GetInstanceID());
        Stack<GameObject> pool = singleton.m_Dictionary[prefabSource.GetInstanceID()];
        nextObject = pool.Count == 0 ? null : pool.Pop();

        if(nextObject == null)
            nextObject = singleton.AddObjectToPool(prefabSource);
        return nextObject;
    }

    public static void PutBackInPool(GameObject bullet) {
        bullet.SetActive(false);
        singleton.m_Dictionary[bullet.GetInstanceID()].Push(bullet);
    }


}