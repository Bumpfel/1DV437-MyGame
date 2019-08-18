using System.Collections.Generic;
using UnityEngine;

public class BulletInstantiator : MonoBehaviour {
    public GameObject m_Bullet;
    public int m_PoolSize;
    private Stack<GameObject> bulletPool = new Stack<GameObject>();
    private static BulletInstantiator singleton;
    private static GameObject nextBullet;

    private void Start() {
        Initialize();
    }
    
    private void OnEnable() {
        Initialize();
    }

    private void OnDisable() {
        while(bulletPool.Count > 0) {
            Destroy(bulletPool.Pop());
        }
    }

    private void Initialize() {
        CreateBulletPool(m_PoolSize);
        singleton = this;
    }

    private void CreateBulletPool(int size) {
        for(int i = 0; i < size; i ++) {
            var bullet = Instantiate(m_Bullet);
            bullet.SetActive(false);
            bulletPool.Push(bullet);
        }
    }

    

    public static GameObject GetNextBullet() {
        nextBullet = singleton.bulletPool.Count == 0 ? null : singleton.bulletPool.Pop();

        if(nextBullet == null)
            nextBullet = Instantiate(singleton.m_Bullet);
        return nextBullet;
    }

    public static void PutBackInPool(GameObject bullet) {
        bullet.SetActive(false);
        singleton.bulletPool.Push(bullet);
    }


}