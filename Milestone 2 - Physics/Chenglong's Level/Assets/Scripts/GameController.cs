using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public ThirdPersonController3 playerController;

	public GUIText weightText;
	public float weightTextLasting = 3.0f;
	float lastWeightTextStartTime;

	private HealthSystem Stamina_bar;

	public Rect StaminaBarDimens;
	public bool VerticleStaminaBar;
	public Texture StaminaBubbleTexture;
	public Texture StaminaTexture;
	public float StaminaBubbleTextureRotation;

	public float StaminaConsumption = 3.0f;
	public float StaminaRecovery = 1.0f;

	public int maxValue;

	// Use this for initialization
	void Start () {
		
		Stamina_bar = new HealthSystem(StaminaBarDimens, VerticleStaminaBar, StaminaBubbleTexture, StaminaTexture, StaminaBubbleTextureRotation);
		
		Stamina_bar.Initialize(maxValue);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > lastWeightTextStartTime + weightTextLasting)
			ClearWeightText ();


		if (playerController.status == 2)
			Stamina_bar.IncrimentBar(- StaminaConsumption * Time.deltaTime);
		else
			Stamina_bar.IncrimentBar(StaminaRecovery * Time.deltaTime);


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

//		// These are the example Incriment and Deincriment buttons, mainly for demonstration purposes only
//		if (GUI.Button(new Rect(health_bar.getScrollBarRect().x + (health_bar.getScrollBarRect().width / 2) - (128/2), health_bar.getScrollBarRect().y + (health_bar.getScrollBarRect().height / 2) - 30, 128, 20), "Increase Health"))
//		{
//			health_bar.IncrimentBar(Random.Range(1, 6));
//		}
//		else if (GUI.Button(new Rect(health_bar.getScrollBarRect().x + (health_bar.getScrollBarRect().width / 2) - (128 / 2), health_bar.getScrollBarRect().y + (health_bar.getScrollBarRect().height / 2) + (20/2), 128, 20), "Decrease Health"))
//		{
//			health_bar.IncrimentBar(Random.Range(-6, -1));
//		}
	}

	
	void ClearWeightText()
	{
		weightText.text = "";
	}

	public void UpdateWeightText (float mass) 
	{
		weightText.text = "Weight: " + mass.ToString ();
		lastWeightTextStartTime = Time.time;
	}

}
