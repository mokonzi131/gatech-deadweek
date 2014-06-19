/*
Created by Team "GT Dead Week"
	Arnaud Golinvaux	
	Chenglong Jiang
	Michael Landes
	Josephine Simon
	Chuan Yao
*/

using UnityEngine;
using System.Collections;

public class CylinderController : MonoBehaviour {

	private float maxHeight;
	private float minHeight;
	private float vertSpeed;
	private Vector3 moveDirection;
	private float timeDamp;

	// Use this for initialization
	void Start () 
	{
		maxHeight = 30.0f;
		minHeight = 10.0f;
		vertSpeed = -5.0f;
		moveDirection = new Vector3 (1.0f, 0.0f, 0.0f);
		timeDamp = 3.0f;

	}
	
	// Update is called once per frame
	void LateUpdate () {
		updateSpeed();

		transform.Translate( moveDirection * vertSpeed * Time.deltaTime);
	}

	void updateSpeed()
	{
		if (timeDamp > 3.0f && (transform.position.y <= minHeight || transform.position.y >= maxHeight))
		{
			vertSpeed = -vertSpeed;
			timeDamp = 0;
		}
		else
			timeDamp += Time.deltaTime;
	}

	void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player")
		{
			Debug.Log ("Player collision with ice");
			collisionInfo.rigidbody.drag = 0.5f;//0.05f;
			collisionInfo.rigidbody.mass = 1.0f;
			collisionInfo.rigidbody.constraints = RigidbodyConstraints.None;
		}
	}
}
