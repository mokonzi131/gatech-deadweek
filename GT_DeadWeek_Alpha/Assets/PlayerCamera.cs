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

	public LayerMask hitLayer;

	private Vector3 cPos;

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

	private float targetDistance;
	private Vector3 camDir;
	private float targetHeight;

	public float minShakeSpeed;
	public float maxShakeSpeed;

	public float minShake;
	public float maxShake = 2.0f;

	public int minShakeTimes;
	public int maxShakeTimes;

	public float maxShakeDistance;

	private bool shake;
	private float shakeSpeed = 2.0f;
	private float cShakePos;
	private int shakeTimes = 8;
	private float cShake;
	private float cShakeSpeed;
	private int cShakeTimes;

	public Transform radar;
	public Transform radarCamera;

	
	//private DepthOfField _depthOfFieldEffect;

	// Use this for initialization
	void Start () {
	
		cShakeTimes = 0;
		cShake = 0.0f;
		cShakeSpeed = shakeSpeed;

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

	}

	void GoToOrbitMode(bool state)
	{
		orbit = state;

		playerController.idleTimer = 0.0f;
	}

	// Update is called once per frame
	void Update () {
		//if (GameManager.pause || GameManager.scores) return;

		if(orbit && (Input.GetKeyDown(KeyCode.O) || Input.GetAxis("Horizontal") != 0.0 || Input.GetAxis("Vertical") != 0.0 || playerController.aim || playerController.fire))
		{
			GoToOrbitMode(false);
		}
		
		if(!orbit && playerController.idleTimer > 0.1)
		{
			GoToOrbitMode(true);
		}
	}


	void LateUpdate ()
	{
		//if (GameManager.scores) return;

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
		
		HandleCameraShake();
		
		cPos = player.position + new Vector3(0, targetHeight, 0);

		RaycastHit hit;
		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, hitLayer))
		{
			float t = hit.distance - 0.1f;
			t -= minDistance;
			t /= (targetDistance - minDistance);
			
			targetHeight = Mathf.Lerp(maxHeight, targetHeight, Mathf.Clamp(t, 0.0f, 1.0f));
			cPos = player.position + new Vector3(0, targetHeight, 0); 
		}
		
		if(Physics.Raycast(cPos, camDir, out hit, targetDistance + 0.2f, hitLayer))
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
		
		camTransform.position = cPos + (camDir * targetDistance);
		camTransform.LookAt(lookPoint);
		
		target.position = cPos;
		target.rotation = Quaternion.Euler(y, x, 0);
	}
	
	void HandleCameraShake()
	{
		if(shake)
		{
			cShake += cShakeSpeed * deltaTime;
			
			if(Mathf.Abs(cShake) > cShakePos)
			{
				cShakeSpeed *= -1.0f;
				cShakeTimes++;
				
				if(cShakeTimes >= shakeTimes)
				{
					shake = false;
				}
				
				if(cShake > 0.0f)
				{
					cShake = maxShake;
				}
				else
				{
					cShake = -maxShake;
				}
			}
			
			targetHeight += cShake;
		}
	}
	
	void StartShake(float distance)
	{
		float proximity = distance / maxShakeDistance;
		
		if(proximity > 1.0f) return;
		
		proximity = Mathf.Clamp(proximity, 0.0f, 1.0f);
		
		proximity = 1.0f - proximity;
		
		cShakeSpeed = Mathf.Lerp(minShakeSpeed, maxShakeSpeed, proximity);
		shakeTimes = Mathf.RoundToInt( Mathf.Lerp(minShakeTimes, (float)maxShakeTimes, proximity));
		cShakeTimes = 0;
		cShakePos = Mathf.Lerp(minShake, maxShake, proximity);
		
		shake = true;
	}
	
	void GetInput()
	{
		Vector2 a = playerController.aim ? aimSpeed : speed;
		x += Mathf.Clamp(Input.GetAxis("Mouse X") * a.x, -maxSpeed.x, maxSpeed.x) * deltaTime;
		y -= Mathf.Clamp(Input.GetAxis("Mouse Y") * a.y, -maxSpeed.y, maxSpeed.y) * deltaTime;
		y = ClampAngle(y, yMinLimit, yMaxLimit);
	}

	/*
	void DepthOfFieldControl()
	{
		if(_depthOfFieldEffect == null) return;
		if(soldierController == null) return;
		
		if(soldierController.aim && GameQualitySettings.depthOfField)
		{
			if(!_depthOfFieldEffect.enabled)
			{
				_depthOfFieldEffect.enabled = true;
			}
		}
		else
		{
			if(_depthOfFieldEffect.enabled)
			{
				_depthOfFieldEffect.enabled = false;
			}
		}
	}
	*/
	
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
