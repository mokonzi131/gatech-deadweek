/*
Created by Team "GT Dead Week"
	Arnaud Golinvaux	
	Chenglong Jiang
	Michael Landes
	Josephine Simon
	Chuan Yao
*/


using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public ThirdPersonController playerController;

	private HealthSystem Stamina_bar;

	public Rect StaminaBarDimens;
	public bool VerticleStaminaBar;
	public Texture StaminaBubbleTexture;
	public Texture StaminaTexture;
	public float StaminaBubbleTextureRotation;
	public float StaminaConsumption = 3.0f;
	public float StaminaRecovery = 1.0f;
	public float StaminaRecoveryTimeout = 2.0f;
	int StaminaMaxValue = 100;

	float lastActingTime = -10.0f;

	// Use this for initialization
	void Start () {

		Stamina_bar = new HealthSystem(StaminaBarDimens, VerticleStaminaBar, StaminaBubbleTexture, StaminaTexture, StaminaBubbleTextureRotation);
		
		Stamina_bar.Initialize(StaminaMaxValue);
	
	}
	
	// Update is called once per frame
	void Update () {
//
//		if (Time.time > lastWeightTextStartTime + weightTextLasting)
//			ClearWeightText ();
//
		if(playerController.rigidbody.velocity.magnitude > 0.1)
		{
			if (playerController.isActing)
			{
				Stamina_bar.IncrimentBar(- StaminaConsumption * Time.deltaTime);
				lastActingTime = Time.time;
			}
			else
			{
				if (lastActingTime + StaminaRecoveryTimeout < Time.time)
					Stamina_bar.IncrimentBar(StaminaRecovery * Time.deltaTime);
			}
		}

		Stamina_bar.Update ();

		if (Stamina_bar.getCurrentValue() < 3.0f)
			playerController.canRun = false;

		if (Stamina_bar.getCurrentValue() > 15.0f)
			playerController.canRun = true;

		//weightText.transform.position = playerController.target.transform.position;

	}

	public void OnGUI() 
	{
		Stamina_bar.DrawBar ();


//		if (death)
//		{
//			GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 200, 40), "You're Dead!");
//			if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 130, 200, 40), "Try Again"))
//			{            
//				Application.LoadLevel(Application.loadedLevelName);
//			}
//		}

	}

}
