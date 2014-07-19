/*
Created by Team "GT Dead Week"
	Chenglong Jiang
	Arnaud Golinvaux	
	Michael Landes
	Josephine Simon
	Chuan Yao
*/

using UnityEngine;
using System.Collections;

public class CameraControllerSimple : MonoBehaviour {

	public Transform playerTransform;

	Vector3 offset;

	// Use this for initialization
	void Start () {
		offset = transform.position - playerTransform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = playerTransform.position + offset;
	}
}
