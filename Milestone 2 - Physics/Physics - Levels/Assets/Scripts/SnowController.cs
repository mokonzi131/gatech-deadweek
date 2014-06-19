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

public class SnowController : MonoBehaviour {

	public GameObject mainCamera;
	private Vector3 offset;

	// Use this for initialization
	void Start () 
	{
		offset = transform.position - mainCamera.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = mainCamera.transform.position + offset;
	}
}
