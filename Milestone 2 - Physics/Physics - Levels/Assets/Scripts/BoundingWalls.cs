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

public class BoundingWalls : MonoBehaviour {

	void Start()
	{
		Color color = renderer.material.color;
		Debug.Log (renderer.material.color.a);
		color.a = 0.1f;
		renderer.material.color = color;
		Debug.Log (renderer.material.color.a);
	}
}
