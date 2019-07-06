using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float moveSpeed = 6;

	Rigidbody body;
	Camera viewCamera;
	Vector3 velocity;

	void Start () {
		body = GetComponent<Rigidbody> ();
		viewCamera = Camera.main;
	}

	void Update () {
		Vector3 mousePos = viewCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, viewCamera.transform.position.y));
		transform.LookAt(mousePos + Vector3.up * transform.position.y);
		velocity = new Vector3 (Input.GetAxisRaw("Horizontal_Player1"), 0, Input.GetAxisRaw("Vertical_Player1")).normalized * moveSpeed;
	}

	void FixedUpdate() {
		body.MovePosition (body.position + velocity * Time.fixedDeltaTime);
	}
}
