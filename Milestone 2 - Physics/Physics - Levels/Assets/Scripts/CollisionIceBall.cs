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

public class CollisionIceBall : MonoBehaviour {

	void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player")
		{
			Debug.Log ("Player collision with ice");
			collisionInfo.rigidbody.drag = 0.5f;//0.05f;
			collisionInfo.rigidbody.mass = 0.15f;
			collisionInfo.rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		}
	}

	void OnCollisionStay(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player")
		{
			Debug.Log ("Player collision with ice");
			collisionInfo.rigidbody.drag = 0.25f;//0.5f;//0.05f;
			collisionInfo.rigidbody.angularDrag = 0.0f;
			collisionInfo.rigidbody.mass = 0.7f;
			collisionInfo.rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		}
	}
	void OnCollisionExit(Collision collisionInfo)
	{
		collisionInfo.rigidbody.constraints = RigidbodyConstraints.None;
		collisionInfo.rigidbody.mass = 1.0f;
		collisionInfo.rigidbody.drag = 0;//4.0f;
		collisionInfo.rigidbody.angularDrag = 0.5f;
		//collisionInfo.gameObject. = (PhysicMaterial)Resources.Load("snow_Material");
	}
}
