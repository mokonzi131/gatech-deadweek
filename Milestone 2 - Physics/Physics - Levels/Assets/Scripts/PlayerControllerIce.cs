using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	public float Speed; // speed scaling parameter without acceleration
	private float distToGround;
	private float timeElapsed, noJumpTime;

	void Start()
	{
		distToGround = collider.bounds.extents.y;
		timeElapsed = 0.0f;
		noJumpTime = 1;
	}
	void Update()
	{
		if(Input.GetButton("Jump") && timeElapsed == 0)
		{
			Debug.Log("Juuuump");
			Vector3 jump = new Vector3(0.0f, 250.0f, 0.0f);
			rigidbody.AddForce(jump);
			timeElapsed = Time.deltaTime;
		}
		else
		{
			if (timeElapsed > 0)
			{
				if(timeElapsed > noJumpTime)
					timeElapsed = 0.0f;
				else
					timeElapsed += Time.deltaTime;
			}
		}
	}


	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// create movement from directional keys
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

		// apply movement scaled by the speed parameter
		rigidbody.AddForce(movement * Speed * Time.deltaTime);
	}

	bool IsGrounded() 
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround);
	}

}