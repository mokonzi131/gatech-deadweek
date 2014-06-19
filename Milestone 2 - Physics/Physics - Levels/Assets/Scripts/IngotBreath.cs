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

public class IngotBreath : MonoBehaviour {

	public float timeout = 1.0f;

	float lastChangeTime;
	
	// Use this for initialization
	void Start () {
		lastChangeTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > lastChangeTime + timeout)
		{
			(gameObject.GetComponent("Halo") as Behaviour).enabled = !(gameObject.GetComponent("Halo") as Behaviour).enabled;
			lastChangeTime = Time.time;
		}

	}



}
