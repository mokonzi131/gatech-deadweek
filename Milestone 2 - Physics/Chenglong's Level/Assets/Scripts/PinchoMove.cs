using UnityEngine;
using System.Collections;

public class PinchoMove : MonoBehaviour {

	public float upSpeed = 10.0f;
	public float downSpeed = 2.0f;

	public float maxMovementHeight = 2.0f;

	bool moveDirection = true;
	float initialY;


	// Use this for initialization
	void Start () {
		initialY = transform.position.y;
		moveDirection = true;
	}
	
	// Update is called once per frame
	void Update () {
		PinchoUp ();
	}

	void PinchoUp () 
	{
		if (moveDirection)
		{
			transform.position += new Vector3(0, upSpeed * Time.deltaTime, 0);
			if (transform.position.y >= initialY + maxMovementHeight)
				moveDirection = false;
		}
		else
		{
			transform.position -= new Vector3(0, downSpeed * Time.deltaTime, 0);
			if (transform.position.y <= initialY)
				moveDirection = true;
		}

	}


	void PinchoDown ()
	{



	}


}
