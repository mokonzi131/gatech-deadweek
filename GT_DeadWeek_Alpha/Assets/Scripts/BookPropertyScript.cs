using UnityEngine;
using System.Collections;

public class BookPropertyScript : MonoBehaviour {

	public bool isJustThrowed;
	public float attractionTimeout = 15.0f;
	float lastThrowedTime = -100.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (lastThrowedTime + attractionTimeout < Time.time)
		{
			isJustThrowed = false;
		}
	
	}


	public void BeingThrowed()
	{
		isJustThrowed = true;
		lastThrowedTime = Time.time;
	}
}
