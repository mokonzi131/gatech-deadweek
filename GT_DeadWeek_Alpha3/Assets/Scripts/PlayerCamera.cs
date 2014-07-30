using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour {

	public Transform target;
	public Transform player;

	public Vector2 speed = new Vector2(135.0f, 135.0f);
	public Vector2 aimSpeed = new Vector2(70.0f, 70.0f);
	public Vector2 maxSpeed = new Vector2(100.0f, 100.0f);

	public float yMinLimit = -90.0f;
	public float yMaxLimit = 90.0f;

	public int normalFOV = 60;
	public int zoomFOV = 30;

	public float lerpSpeed = 8.0f;

	private float distance = 10.0f;
		
	private float x = 0.0f;
	private float y = 0.0f;

	private Transform camTransform;
	private Quaternion rotation;
	private Vector3 position;
	private float deltaTime;
	private Quaternion originalSoldierRotation;

	private PlayerController playerController;

	public bool orbit;

	public LayerMask ignoreLayer;

	[HideInInspector]
	public Vector3 cPos;

	public Vector3 normalDirection;
	public Vector3 aimDirection;
	public Vector3 crouchDirection;
	public Vector3 aimCrouchDirection;

	public float positionLerp;

	public float normalHeight;
	public float crouchHeight;
	public float normalAimHeight;
	public float crouchAimHeight;
	public float minHeight;
	public float maxHeight;
	
	public float normalDistance;
	public float crouchDistance;
	public float normalAimDistance;
	public float crouchAimDistance;
	public float minDistance;
	public float maxDistance;

	[HideInInspector]
	public float targetDistance;

	[HideInInspector]
	public Vector3 camDir;

	private float targetHeight;
	
	private bool shake;
	public float shakeDuration = 0.5f;
	public float shakeSpeed = 4.0f;
	public float shakeMagnitude = 0.3f;
	private float shakeElapse = -10.0f;
	private float shakeStart = -10.0f;
	private float shakeRandomStart = 0.0f;


	public Transform radar;
	public Transform radarCamera;

	
	//private DepthOfField _depthOfFieldEffect;

	// Use this for initialization
	void Start () {
	
		//_depthOfFieldEffect = gameObject.GetComponent<"DepthOfField">() as DepthOfField;

		if (target == null || player == null)
		{
			Destroy(this);
			return;
		}

		target.parent = null;

		camTransform = transform;

	    Vector3 angles = camTransform.eulerAngles;
		x = angles.y;
		y = angles.x;

		originalSoldierRotation = player.rotation;

		playerController = player.GetComponent<PlayerController> ();

		targetDistance = normalDistance;

		cPos = player.position + new Vector3 (0, normalHeight, 0);

		ignoreLayer = ~ignoreLayer;

	}

	void GoToOrbitMode(bool state)
	{
		orbit = state;

		playerController.idleTimer = 0.0f;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.pause || GameManager.scores) return;

		if(orbit && (Input.GetKeyDown(KeyCode.O) || Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0 || playerController.aim || playerController.fire))
		{
			GoToOrbitMode(false);
		}
		
		if(!orbit && playerController.idleTimer > 0.1)
		{
			GoToOrbitMode(true);
		}

//		
//		if (Input.GetKeyDown(KeyCode.P))
//		{
//			StartShake();
//		}

	}


	void LateUpdate ()
	{
		if (GameManager.scores) return;

		deltaTime = Time.deltaTime;

		GetInput();
		
		RotateSoldier();

		CameraMovement();
		
		//DepthOfFieldControl();
	}
	
	void CameraMovement()
	{
		if(playerController.aim)
		{
			//(camera.GetComponent(DepthOfField) as DepthOfField).enabled = true;
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, zoomFOV, deltaTime * lerpSpeed);
			
			if(playerController.crouch)
			{
				camDir = (aimCrouchDirection.x * target.forward) + (aimCrouchDirection.z * target.right);
				targetHeight = crouchAimHeight;
				targetDistance = crouchAimDistance;
			}
			else
			{
				camDir = (aimDirection.x * target.forward) + (aimDirection.z * target.right);
				targetHeight = normalAimHeight;
				targetDistance = normalAimDistance;
			}
		}
		else
		{
			//(camera.GetComponent(DepthOfField) as DepthOfField).enabled = false;
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, normalFOV, deltaTime * lerpSpeed);
			
			if(playerController.crouch)
			{
				camDir = (crouchDirection.x * target.forward) + (crouchDirection.z * target.right);
				targetHeight = crouchHeight;
				targetDistance = crouchDistance;
			}
			else
			{
				camDir = (normalDirection.x * target.forward) + (normalDirection.z * target.right);
				targetHeight = normalHeight;
				targetDistance = normalDistance;
			}
		}
		
		camDir = camDir.normalized;
		
		Vector3 shakeOffest = HandleCameraShake();
		
		cPos = player.position + new Vector3(0, targetHeight, 0);


		RaycastHit hit;
		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, ignoreLayer))
		{
			float t = hit.distance - 0.1f;
			t -= minDistance;
			t /= (targetDistance - minDistance);
			
			targetHeight = Mathf.Lerp(maxHeight, targetHeight, Mathf.Clamp(t, 0.0f, 1.0f));
			cPos = player.position + new Vector3(0, targetHeight, 0); 
		}
		
		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, ignoreLayer))
		{
			targetDistance = hit.distance - 0.1f;
		}


		if(radar != null)
		{
			radar.position = cPos;
			radarCamera.rotation = Quaternion.Euler(90, x, 0);
		}
		
		Vector3 lookPoint = cPos;
		lookPoint += (target.right * Vector3.Dot(camDir * targetDistance, target.right));

		camTransform.position = cPos + (camDir * targetDistance) + shakeOffest;
		camTransform.LookAt(lookPoint);

		target.position = cPos;
		target.rotation = Quaternion.Euler(y, x, 0);
	}


	private Vector3 HandleCameraShake()
	{
		if (!shake)
		{
			shakeElapse = 0;
			return new Vector3 (0, 0, 0);
		}

		shakeElapse += Time.deltaTime;
		if (shakeElapse > shakeDuration)
			shake = false;

		float percentComplete = shakeElapse / shakeDuration;			
		
		// We want to reduce the shake from full power to 0 starting half way through
		float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);
		
		// Calculate the noise parameter starting randomly and going as fast as speed allows
		float alpha = shakeRandomStart + shakeSpeed * percentComplete;
		
		// map noise to [-1, 1]
		float x = Util.Noise.GetNoise(alpha, 0.0f, 0.0f) * 2.0f - 1.0f;
		float y = Util.Noise.GetNoise(0.0f, alpha, 0.0f) * 2.0f - 1.0f;
		float z = Util.Noise.GetNoise(0.0f, 0.0f, alpha) * 2.0f - 1.0f;
		
		x *= shakeMagnitude * damper;
		y *= shakeMagnitude * damper;
		z *= shakeMagnitude * damper;

		return new Vector3 (x, y, z);

	}


	public void StartShake()
	{
		shakeStart = 0.0f;
		shakeElapse = 0.0f;

		if (shake == false)
		{
			shakeRandomStart = Random.Range(-1000.0f, 1000.0f);
		}

		shake = true;
	}

	
	void GetInput()
	{
		Vector2 a = playerController.aim ? aimSpeed : speed;
		x += Mathf.Clamp(Input.GetAxis("Mouse X") * a.x, -maxSpeed.x, maxSpeed.x) * deltaTime;
		y -= Mathf.Clamp(Input.GetAxis("Mouse Y") * a.y, -maxSpeed.y, maxSpeed.y) * deltaTime;
		y = ClampAngle(y, yMinLimit, yMaxLimit);
	}

	
	void RotateSoldier()
	{
		if(!orbit)
			playerController.targetYRotation = x;
	}
	
	static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360)
		{
			angle += 360;
		}
		
		if (angle > 360)
		{
			angle -= 360;
		}
		
		return Mathf.Clamp (angle, min, max);
	}

}
