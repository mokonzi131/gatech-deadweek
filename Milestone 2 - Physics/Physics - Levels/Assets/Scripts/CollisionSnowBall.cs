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

public class CollisionSnowBall : MonoBehaviour {

	void OnCollisionStay(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.tag == "Player")
		{
			Debug.Log ("Player collision with snow");
			collisionInfo.rigidbody.drag = 4.0f;
			collisionInfo.rigidbody.mass = 1.5f;
			collisionInfo.rigidbody.constraints = RigidbodyConstraints.None;
		}
	}
}
