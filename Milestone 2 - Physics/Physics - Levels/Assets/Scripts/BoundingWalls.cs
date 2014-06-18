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
