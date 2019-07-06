using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableController : MonoBehaviour {

    public Rigidbody Player;
    
    private Collider collider;

    void Start() {
        collider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other) {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0, default);

        foreach(Collider collider in colliders) {
            print("box collided with " + other.name);
        }
    }

}
