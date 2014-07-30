/*
Created by Team "GT Dead Week"
	Chenglong Jiang
	Arnaud Golinvaux	
	Michael Landes
	Josephine Simon
	Chuan Yao
*/

using UnityEngine;
using System.Collections;

public class Stamina : MonoBehaviour {

	private PlayerController playerController;

	public float StaminaConsumption = 3.0f;
	public float StaminaRecovery = 1.0f;
	public float StaminaRecoveryTimeout = 2.0f;
	
	float lastActingTime = -10.0f;
	
	private StaminaBar staminaBar;
	
	public int maxStamina = 100;
	private float stamina;
	// Use this for initialization
	void Start () {
		playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

		staminaBar = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().energy;
		staminaBar.setMaxStamina (maxStamina);
		stamina = maxStamina;
	}

	public void deltaStamina(float ds){
		stamina += ds;
		stamina = Mathf.Max(0, Mathf.Min(stamina, (float)maxStamina));
	}
	
	// Update is called once per frame
	void Update () {
//		if (playerController.hasJump) {
//			deltaStamina(- (playerController.rigidbody.mass - 1) * StaminaConsumption);
//			playerController.hasJump = false;
//		}

		if (playerController.controller.velocity.magnitude > 0.1 && !playerController.walk)
		{
			deltaStamina(-StaminaConsumption * Time.deltaTime);
			lastActingTime = Time.time;
		}
		else
		{
			if (lastActingTime + StaminaRecoveryTimeout < Time.time){
				deltaStamina(StaminaRecovery * Time.deltaTime);
			}
		}

//		if (stamina < 0.03*maxStamina)
//			playerController.canRun = false;
//		
//		if (stamina > 0.15*maxStamina)
//			playerController.canRun = true;
		
		
		staminaBar.setStamina (stamina);
	}
}
