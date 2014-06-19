using UnityEngine;
using System.Collections;

public class GameControllerIce : MonoBehaviour {


	public ThirdPersonControllerIce playerController;

	public GUIText weightText;
	public float weightTextLasting = 3.0f;
	float lastWeightTextStartTime;

	public AudioClip snowAudio;
	public AudioClip bouncingAudio;
	public AudioClip stoneAudio;

	private HealthSystem Stamina_bar;
	
	public Rect StaminaBarDimens;
	public bool VerticleStaminaBar;
	public Texture StaminaBubbleTexture;
	public Texture StaminaTexture;
	public float StaminaBubbleTextureRotation;
	public float StaminaConsumption = 3.0f;
	public float StaminaRecovery = 1.0f;
	public int StaminaMaxValue = 100;


	void Start () {

		Stamina_bar = new HealthSystem(StaminaBarDimens, VerticleStaminaBar, StaminaBubbleTexture, StaminaTexture, StaminaBubbleTextureRotation);
		
		Stamina_bar.Initialize(StaminaMaxValue);

		UpdateWeightText (1.0f); // weight doesn't change in this level
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > lastWeightTextStartTime + weightTextLasting)
			ClearWeightText ();

		if(playerController.rigidbody.velocity.magnitude > 0.1)
		{
			if (playerController.terrainStatus == 2)
			{
				if(playerController.moveStatus == 2)
					Stamina_bar.IncrimentBar(- 4.0f * StaminaConsumption * Time.deltaTime);
				else
					Stamina_bar.IncrimentBar(- StaminaConsumption * Time.deltaTime);
			}
			else if (playerController.terrainStatus == 0)
				Stamina_bar.IncrimentBar(5.0f * StaminaRecovery * Time.deltaTime);
			else if(playerController.moveStatus == 2)
				Stamina_bar.IncrimentBar(- StaminaConsumption * Time.deltaTime);
			else
				Stamina_bar.IncrimentBar(StaminaConsumption * Time.deltaTime);
			
			
			Stamina_bar.Update ();
		}
		
		if (Stamina_bar.getCurrentValue() < 3.0f)
			playerController.canRun = false;
		
		if (Stamina_bar.getCurrentValue() > 15.0f)
			playerController.canRun = true;
		
		//weightText.transform.position = playerController.target.transform.position;
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Application.LoadLevel("Treasure Hunting");
			
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Application.LoadLevel("SnowScene");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Application.LoadLevel("Bounce");
			
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Application.LoadLevel("WindyScene");
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			
		}
	}

	public void refillEnergy()
	{
		//Debug.Log("REFILL");
		Stamina_bar.refill ();
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
	
	public void OnGUI() 
	{
		Stamina_bar.DrawBar ();
		
	}


	public void StopAudio()
	{
		if(audio.isPlaying)
		{
			if(!(audio.clip == bouncingAudio))
			{
			   audio.Stop ();
			   //Debug.Log ("STOP");
			}
		}
	}

	public void PlayAudioSnow(float speed)
	{
		//Debug.Log ("Playing Snow from :" + audio.clip.name);
		if (audio.clip != snowAudio)
		{
			audio.clip = snowAudio;
			audio.loop = true;
			audio.Play();
		}
		else
		{
			if (audio.isPlaying)
				return;
			else
			{
				audio.Play();
				Debug.Log("PLAYYYY");
			}

		}

		//Debug.Log ("Speed :" + speed);
		
	}

	public void StopAudioSnow()
	{
		if(audio.clip == snowAudio)
		{
			Debug.Log("stopping snow");
			audio.Stop ();
		}
	}

	public void PlayAudioRollingStone(float speed)
	{
		Debug.Log ("Playing Cylinder from :" + audio.clip.name);
		Debug.Log("isplaying = " + audio.isPlaying);
		if (audio.clip != stoneAudio)
		{
			//audio.Stop();
			audio.clip = stoneAudio;
			audio.loop = true;
			audio.Play();
		}
		else
		{
			if (audio.isPlaying)
				return;
			else
				audio.Play();
			
		}
		
	}

	public void PlayBouncingAudio(float speed)
	{

		float val = speed/30.0f;
		audio.volume = val;
		audio.clip = bouncingAudio;
		audio.loop = false;
		audio.Play ();

	}

	public void updateRollingAudio(float speed, bool isGrounded)
	{
		if (!audio.isPlaying)
			return;
		else
		{
			if(isGrounded)
			{
				if(audio.clip == snowAudio)
				{
					//Debug.Log("SPEED UPDATE");
					float val = speed/25.0f;
					audio.volume = val;
					audio.pitch = Mathf.Clamp (val*10.0f, 0.1f, 1.0f);
				}
				else if(audio.clip == stoneAudio)
				{
					float val = speed/33.0f;
					audio.volume = val;
					audio.pitch = Mathf.Clamp (val*10.0f, 0.1f, 2.0f);
				}
			}
			else
			{
				//Debug.Log("is not grounded");
				audio.volume = 0.0f;
			}
		}
	}
}
