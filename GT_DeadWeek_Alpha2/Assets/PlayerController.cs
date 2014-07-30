using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {

	
	// Public variables shown in inspector
	
	public float runSpeed = 4.6f;
	public float runStrafeSpeed = 3.07f;
	public float walkSpeed  = 1.22f;
	public float walkStrafeSpeed  = 1.22f;
	public float crouchRunSpeed  = 5;
	public float crouchRunStrafeSpeed  = 5;
	public float crouchWalkSpeed = 1.8f;
	public float crouchWalkStrafeSpeed = 1.8f;

	public GameObject radarObject;
	
	public float maxRotationSpeed = 540;
	
	//public GunManager weaponSystem;
	public float minCarDistance;
	
	static public bool dead;
	
	// Public variables hidden in inspector
	
	[HideInInspector]
	public bool walk;
	
	[HideInInspector]
	public bool crouch;
	
	[HideInInspector]
	public bool inAir;
	
	[HideInInspector]
	public bool fire;
	
	[HideInInspector]
	public bool aim;
	
	[HideInInspector]
	public bool reloading;
	
	[HideInInspector]
	public bool currentWeaponName;
	
	[HideInInspector]
	public bool currentWeapon;
	
	[HideInInspector]
	public bool grounded;
	
	[HideInInspector]
	public float targetYRotation;

	[HideInInspector]
	public CharacterController controller;
	// Private variables
	
	private Transform playerTransform;
	//private HeadLookController headLookController;
	private CharacterMotor motor;
	
	private bool firing;
	private float firingTimer;
	public float idleTimer;
	
	public Transform enemiesRef;
	public Transform enemiesShootRef;
	
	static public Transform enemiesReference;
	static public Transform enemiesShootReference;
	
	[HideInInspector]
	public Vector3 moveDir;
	
	private bool _useIK;


	
	public Animator animator;
	private Vector3 lastPosition;
	private Vector3 lastForward;
	private int updateAnim;


	void Awake()
	{
		if (enemiesRef != null) enemiesReference = enemiesRef;
		if (enemiesRef != null) enemiesShootReference = enemiesShootRef;
	}

	// Use this for initialization
	void Start () {
	
		idleTimer = 0.0f;

		playerTransform = transform;

		walk = true;
		aim = false;
		reloading = false;

		controller = gameObject.GetComponent<CharacterController> ();
		motor = gameObject.GetComponent<CharacterMotor> ();
	}

	void OnEnable()
	{
		if (radarObject != null)
			radarObject.SetActive(true);

		moveDir = Vector3.zero;
		// headLookController = gameObject.GetComponent<headLookController> ();
		// headLookController.enabled = true;

		walk = true;
		aim = false;
		reloading = false;

	}

	void OnDisable()
	{
		if (radarObject != null)
			radarObject.SetActive(false);

		moveDir = Vector3.zero;
		//headLookController.enabled = false;

		walk = true;
		aim = false;
		reloading = false;

	}


	// Update is called once per frame
	void Update () {
	
		//if (GameManager.pause || GameManager.scores)
		if (1==0)
		{
			moveDir = Vector3.zero;
			motor.canControl = false;
		}
		else
		{
			GetUserInputs();

			if (!motor.canControl)
				motor.canControl = true;

			if (!dead)
			{
				//moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				moveDir = new Vector3(0, 0, Input.GetAxis("Vertical"));
			}
			else
			{
				moveDir = Vector3.zero;
				motor.canControl = false;
			}

		}

		//Check the player move direction
		if (moveDir.sqrMagnitude > 1)
			moveDir = moveDir.normalized;

		motor.inputMoveDirection = transform.TransformDirection (moveDir);
		motor.inputJump = Input.GetButton ("Jump") && !crouch;
		
		motor.movement.maxForwardSpeed = ((walk) ? ((crouch) ? crouchWalkSpeed : walkSpeed) : ((crouch) ? crouchRunSpeed : runSpeed));
		motor.movement.maxBackwardsSpeed = motor.movement.maxForwardSpeed;
		//Debug.Log (motor.movement.maxForwardSpeed.ToString () + " " + motor.movement.maxBackwardsSpeed.ToString () + " " + controller.velocity.magnitude.ToString());
		motor.movement.maxSidewaysSpeed = ((walk) ? ((crouch) ? crouchWalkStrafeSpeed : walkStrafeSpeed) : ((crouch) ? crouchRunStrafeSpeed : runStrafeSpeed));

		
		if(moveDir != Vector3.zero)
		{
			idleTimer = 0.0f;
		}
		
		inAir = !motor.grounded;
		
		var currentAngle = playerTransform.localRotation.eulerAngles.y;
		var delta = Mathf.Repeat ((targetYRotation - currentAngle), 360);
		if (delta > 180)
			delta -= 360;

		Quaternion tmpQuaterion = playerTransform.localRotation;
		Vector3 tmpEulerAngles = tmpQuaterion.eulerAngles;
		tmpEulerAngles.y = Mathf.MoveTowards(currentAngle, currentAngle + delta, Time.deltaTime * maxRotationSpeed);
		tmpQuaterion.eulerAngles = tmpEulerAngles;
		playerTransform.localRotation = tmpQuaterion;

	}


	void LateUpdate()
	{
		if(updateAnim == 0)
		{
			// Set linear speed
			float linearSpeed = Mathf.Sign (Input.GetAxis ("Vertical"))*(lastPosition - transform.position).magnitude/Time.deltaTime/5.0f;
			animator.SetFloat ("linear_speed", linearSpeed);
			
			// Set angular speed
			float angularSpeed = Mathf.Sign (Input.GetAxis ("Sidestep"))*Vector3.Angle(lastForward, transform.forward)/Time.deltaTime/5.0f;
			animator.SetFloat("angular_speed", angularSpeed/30.0f);
			//Debug.Log ("linear_speed = " + animator.GetFloat("linear_speed"));
			//Debug.Log ("angular_speed = " + animator.GetFloat("angular_speed"));
			
			// Update variables
			lastPosition = transform.position;
			lastForward = transform.forward;
		}
		updateAnim = (updateAnim + 1) % 5;

		if(Input.GetButton ("Push"))
		{
			animator.SetTrigger("pushTrigger");
		}
	}



	void GetUserInputs()
	{
		//Check if the user if firing the weapon
		//fire = Input.GetButton("Fire1") && weaponSystem.currentGun.freeToShoot && !dead && !inAir;
		fire = Input.GetButton("Fire1") && !dead && !inAir;

		//Check if the user is aiming the weapon
		aim = (Input.GetButton("Fire2") || Input.GetAxis("Aim") >0.5) && !dead;
		
		idleTimer += Time.deltaTime;
		
		if(aim || fire)
		{
			firingTimer -= Time.deltaTime;
			idleTimer = 0.0f;
		}
		else
		{
			firingTimer = 0.3f;
		}
		
		firing = (firingTimer <= 0.0f && fire);

//		//Check if the user wants the soldier to crouch
//		if(Input.GetKeyDown(KeyCode.LeftControl))
//		{
//			crouch = !crouch;
//			idleTimer = 0.0f;
//		}
		
		crouch |= dead;
		
		//Check if the user wants the soldier to walk
		walk = (!Input.GetButton("Run") && !dead || moveDir == Vector3.zero || crouch || moveDir.z<=0);

	}

}
