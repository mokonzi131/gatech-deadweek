using UnityEngine;
using System.Collections;

public class Stamina : MonoBehaviour {

	private ThirdPersonController playerController;
	
	public float StaminaConsumption = 3.0f;
	public float StaminaRecovery = 1.0f;
	public float StaminaRecoveryTimeout = 2.0f;
	
	float lastActingTime = -10.0f;
	
	private StaminaBar staminaBar;
	
	public int maxStamina = 100;
	private float stamina;
	// Use this for initialization
	void Start () {
		playerController = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();

		staminaBar = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().energy;
		staminaBar.setMaxStamina (maxStamina);
		stamina = maxStamina;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerController.rigidbody.velocity.magnitude > 0.1 && playerController.isActing) {
			stamina -= playerController.rigidbody.mass * StaminaConsumption * Time.deltaTime;
			lastActingTime = Time.time;
		} else {
			if (lastActingTime + StaminaRecoveryTimeout < Time.time)
				stamina += StaminaRecovery * Time.deltaTime;
		}
		
		if (stamina < 0.03*maxStamina)
			playerController.canRun = false;
		
		if (stamina > 0.15*maxStamina)
			playerController.canRun = true;
		
		
		staminaBar.setStamina (stamina);
	}
}
