using UnityEngine;
using System.Collections;

public class ThirdPersonControllerIce : MonoBehaviour
{
	public Rigidbody target;
		// The object we're steering


	public float walkSpeed = 15.0f;
	public float backWalkSpeed = 7.0f;
	public float runSpeed = 30.0f;

	public float jumpSpeed = 10.0f;

	public float drag = 1.0f;

	//public float speed = 1.0f, speedChangeScale = 1.5f, turnSpeed = 2.0f, mouseTurnSpeed = 2.0f, jumpSpeed = 2.0f, drag = 1.0f;
		// Tweak to ajust character responsivenesss


	public int moveStatus = 0;
		// 0: walk; 1: back walk; 2: run
	public int terrainStatus = 0;
		// 0: ice; 1: other; 2: snow

	private const float inputThreshold = 0.01f,
		directionalJumpFactor = 0.0f,
		groundOffset = 0.1f;

    public bool canJump = true;

	public float inAirControl = 0.2f;

	public bool canRun = true;

    private float jumpRepeatTime = 0.05f;
    private float jumpTimeout = 0.15f;
    private float lastJumpButtonTime = -10.0f;
    private float lastJumpTime = -1.0f;
	

    private float groundedTimeout = 0.25f;

	private float distToGround;

	GameControllerIce gameController;

	void Start ()
	// Verify setup, configure rigidbody
	{
		if (target == null)
		{
			target = GetComponent<Rigidbody> ();
		}

		
		if (target == null)
		{
			Debug.LogError ("No target assigned. Please correct and restart.");
			enabled = false;
			return;
		}
		
		target.drag = drag;

		distToGround = collider.bounds.extents.y;

		//target.freezeRotation = true;
			// We will be controlling the rotation of the target, so we tell the physics system to leave it be
		moveStatus = 0;
		terrainStatus = 0;
	    
		gameController = GameObject.Find ("Game Controller").GetComponent<GameControllerIce> ();
	}


	void FixedUpdate ()
	{
        if (Input.GetButton("Jump"))
        // Handle jumping
        {
            lastJumpButtonTime = Time.time;
        }

        ApplyJumping();


		Vector3 moveForward = Camera.main.transform.TransformDirection(Vector3.forward);
		moveForward.y = 0;
		moveForward = moveForward.normalized;
		Vector3 moveRight = Camera.main.transform.TransformDirection(Vector3.right);
		moveRight.y = 0;
		moveRight = moveRight.normalized;
		
		Vector3 movement = Input.GetAxis ("Vertical") * moveForward +
			Input.GetAxis ("Horizontal") * moveRight;
		
		//Debug.Log(SidestepAxisInput.ToString());
		
		if (movement.magnitude > inputThreshold)
			// Only apply movement if we have sufficient input
		{
			
			
			float appliedSpeed = walkSpeed;
			// Scale down applied speed if in walk mode
			moveStatus = 0;
			
			
			
			if (Input.GetAxis ("Vertical") < 0.0f)
				// Scale down applied speed if walking backwards
			{
				appliedSpeed = backWalkSpeed;
				moveStatus = 1;
			}
			else if (canRun && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				appliedSpeed = runSpeed;
				moveStatus = 2;
			}
			
			if (isGrounded())
				target.AddForce (movement.normalized * appliedSpeed, ForceMode.Force);
			else
				target.AddForce (movement.normalized * appliedSpeed * inAirControl , ForceMode.Force);
			
		}

		gameController.updateRollingAudio(rigidbody.velocity.magnitude, isGrounded ());
		if(!isGrounded())
		{
			rigidbody.drag = 0.0f;
		}

	}

	bool isGrounded () 
	{
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	void DestroyPlayer()
	{
		gameObject.SetActive (false);
		//Destroy (gameObject);
	}

	void OnCollisionStay(Collision collision) {

		//Debug.Log ("Collision Stay with  " + collision.gameObject.name);
		if (collision.gameObject.name == "DeformableTerrain")
		{
			gameController.PlayAudioSnow(target.velocity.magnitude);
			terrainStatus = 2;
		}
		else if(collision.gameObject.name == "Ramp")
		{
			gameController.PlayAudioRollingStone(target.velocity.magnitude);
			rigidbody.drag = 2.0f;
			terrainStatus = 1;
		}
		else if(collision.gameObject.tag == "Ice")
		{
			terrainStatus = 0;
		}
		else
		{
			gameController.StopAudio ();
			terrainStatus = 1;
		}
	}

	void OnCollisionEnter(Collision collision) {
		
		//Debug.Log ("Collision Enter with  " + collision.gameObject.name);
		if (collision.gameObject.name == "DeformableTerrain")
		{
			gameController.PlayAudioSnow(target.velocity.magnitude);
			rigidbody.drag = 4.0f;
			rigidbody.mass = 1.5f;
			rigidbody.constraints = RigidbodyConstraints.None;
			terrainStatus = 2;
		}
		else if(collision.gameObject.name == "Ramp")
		{
			gameController.PlayAudioRollingStone(target.velocity.magnitude);
			rigidbody.drag = 2.0f;
			terrainStatus = 1;
		}
		else if(collision.gameObject.name == "BouncingWall")
		{
			gameController.PlayBouncingAudio(target.velocity.magnitude);
		}
		else if(collision.gameObject.tag == "Ice")
		{
			terrainStatus = 0;
			gameController.StopAudio ();
		}
		else
		{
			gameController.StopAudio ();
			terrainStatus = 1;
		}
	}


	void OnCollisionExit(Collision collision) {


		if (collision.gameObject.name == "DeformableTerrain")
		{
			gameController.StopAudioSnow();
		}
		else if(collision.gameObject.name == "Ramp")
		{
			//Debug.Log("Collision Exit with = " + collision.gameObject.name);
			gameController.StopAudio ();
		}
		
	}

	void OnTriggerEnter(Collider other) {
		if(other.name == "EnergyRefill")
		{
			Debug.Log("Collision with energy bar");
			other.gameObject.SetActive(false);
			gameController.refillEnergy();
		}
	}



    void ApplyJumping()
    {
        // Prevent jumping too fast after each other
        if (lastJumpTime + jumpRepeatTime > Time.time)
            return;

        if (isGrounded())
        {
            // Jump
            // - Only when pressing the button down
            // - With a timeout so you can press the button slightly before landing		
            if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
            {
				Vector3 curV = target.velocity;
				curV.y = 0;
				target.velocity = curV;
                target.AddForce(
                    jumpSpeed * new Vector3(0, 1, 0) +
                        target.velocity.normalized * directionalJumpFactor,
                    ForceMode.Impulse
                );
                lastJumpTime = Time.time;
                lastJumpButtonTime = -10.0f;
            }

        }

    }

}
