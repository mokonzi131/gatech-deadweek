using UnityEngine;
using System.Collections;

public class PerlinShake : MonoBehaviour {
	
	public float duration = 0.5f;
	public float speed = 4.0f;
	public float magnitude = 0.1f;

	public Transform playerTransform;

	private PlayerCamera camController;
	
	//set camera position relative to player
	float cameraXOffset;
	float cameraYOffset;
	float cameraDistanceFromPlayer;
	Quaternion cameraRotationFull;

	float lastShakeTime = 0.0f;

	[HideInInspector]
	public bool isShaking = false;
	
	public bool test = false;

	public void Start()
	{
		cameraRotationFull = transform.rotation;

		camController = gameObject.GetComponent<PlayerCamera> ();

//		cameraXOffset = Camera.main.GetComponent<OrbitingCamera> ().cameraXOffset;
//		cameraYOffset = Camera.main.GetComponent<OrbitingCamera> ().cameraYOffset;
//		cameraDistanceFromPlayer = Camera.main.GetComponent<OrbitingCamera> ().cameraDistanceFromPlayer;
//		cameraRotationFull = Camera.main.GetComponent<OrbitingCamera> ().cameraRotationFull;

	}

	// -------------------------------------------------------------------------
	public void PlayShake() {
		gameObject.GetComponent<PlayerCamera> ().orbit = false;
		isShaking = true;
		//Camera.main.GetComponent<OrbitingCamera> ().orbitIsActive = false;
		lastShakeTime = Time.time;
		StopAllCoroutines();
		StartCoroutine("Shake");
	}
	
	// -------------------------------------------------------------------------
	void Update() {

		if (Input.GetKeyDown(KeyCode.P))
		{
			PlayShake();
		}

		if (test) {
			test = false;
			PlayShake();
		}
		if (lastShakeTime + duration < Time.time)
		{
			gameObject.GetComponent<PlayerCamera> ().orbit = true;
			isShaking  =false;
		}
			//Camera.main.GetComponent<OrbitingCamera> ().orbitIsActive = true;
	}
	
	// -------------------------------------------------------------------------
	IEnumerator Shake() {
		
		float elapsed = 0.0f;

		float randomStart = Random.Range(-1000.0f, 1000.0f);
		
		while (elapsed < duration) {
			
			elapsed += Time.deltaTime;			
			
			float percentComplete = elapsed / duration;			
			
			// We want to reduce the shake from full power to 0 starting half way through
			float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);
			
			// Calculate the noise parameter starting randomly and going as fast as speed allows
			float alpha = randomStart + speed * percentComplete;
			
			// map noise to [-1, 1]
			float x = Util.Noise.GetNoise(alpha, 0.0f, 0.0f) * 2.0f - 1.0f;
			float y = Util.Noise.GetNoise(0.0f, alpha, 0.0f) * 2.0f - 1.0f;
			float z = Util.Noise.GetNoise(0.0f, 0.0f, alpha) * 2.0f - 1.0f;
			
			x *= magnitude * damper;
			y *= magnitude * damper;
			z *= magnitude * damper;


			cameraRotationFull = transform.rotation;

			transform.position = camController.cPos + (camController.camDir * camController.targetDistance) + new Vector3(x, y, z);


//			Camera.main.transform.position = cameraRotationFull * new Vector3(cameraXOffset, cameraYOffset, -cameraDistanceFromPlayer)
//				+ playerTransform.position + new Vector3(x, y, z);

			yield return null;
		}

		
		transform.position = camController.cPos + (camController.camDir * camController.targetDistance);

	}
}
