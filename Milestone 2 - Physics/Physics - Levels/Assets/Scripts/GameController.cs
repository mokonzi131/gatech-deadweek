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
	public int StaminaMaxValue = 100;

	//Audio Clips
	public AudioClip AudioBounce;
	public AudioClip AudioRollingGrass;
	public AudioClip AudioRollingConcrete;
	public AudioClip AudioHitting;
	public AudioClip AudioHittingMetal;
	public AudioClip AudioJumping; 
	public AudioClip AudioGrav;

	private bool death;

	// Use this for initialization
	void Start () {

		death = false;

		Stamina_bar = new HealthSystem(StaminaBarDimens, VerticleStaminaBar, StaminaBubbleTexture, StaminaTexture, StaminaBubbleTextureRotation);
		
		Stamina_bar.Initialize(StaminaMaxValue);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > lastWeightTextStartTime + weightTextLasting)
			ClearWeightText ();

		if(playerController.rigidbody.velocity.magnitude > 0.1)
		{
			if (playerController.status == 2)
				Stamina_bar.IncrimentBar(- StaminaConsumption * Time.deltaTime);
			else
				Stamina_bar.IncrimentBar(StaminaRecovery * Time.deltaTime);
		}

		Stamina_bar.Update ();

		if (Stamina_bar.getCurrentValue() < 3.0f)
			playerController.canRun = false;

		if (Stamina_bar.getCurrentValue() > 15.0f)
			playerController.canRun = true;

		//weightText.transform.position = playerController.target.transform.position;


		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Application.LoadLevel("Hidden Valley");

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
			Application.LoadLevel ("Gravity");
		}

	}

	public void OnGUI() 
	{
		Stamina_bar.DrawBar ();


		if (death)
		{
			GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 200, 40), "You're Dead!");
			if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 130, 200, 40), "Try Again"))
			{            
				Application.LoadLevel(Application.loadedLevelName);
			}
		}

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

	public void StopPlayingAudioRolling()
	{
		if (audio.clip == AudioRollingGrass || audio.clip == AudioRollingConcrete)
		{
			audio.Stop();
			audio.clip = null;
		}
	}

	public void PlayAudioBounce(float mass, float speed)
	{
		Debug.Log("hit trampoline");
		if(audio.clip != AudioBounce)
		{
			if(audio.isPlaying)
				return;

			audio.clip = AudioBounce;
			audio.loop = false;
			audio.Play();
		}
		audio.volume = mass / 2;

	}

	public void PlayAudioGrav()
	{
		//if (audio.clip != AudioGrav)
		//{
			if (audio.isPlaying)
				return;

			audio.clip = AudioGrav;
			audio.loop = false;
		audio.volume *= 2;
			audio.Play ();
		//}
	}

	public void PlayAudioRollingGrass(float mass, float speed)
	{
		//Debug.Log ("Playing Grass!");
		if (audio.clip != AudioRollingGrass)
		{
			if (audio.isPlaying)
				return;

			audio.clip = AudioRollingGrass;
			audio.loop = true;
			audio.Play();
		}
		audio.volume = mass / 100;

		//High pitch audioclip, hard to manipulate the pitch only for good effects
		audio.volume *= speed/10;

		audio.pitch = Mathf.InverseLerp(0, 10, speed) * 0.15f + 0.85f;

	}

	public void PlayAudioRollingConcrete(float mass, float speed)
	{
		//Debug.Log ("Playing Concrete!");

		if (audio.clip != AudioRollingConcrete)
		{
			if (audio.isPlaying)
				return;

			audio.clip = AudioRollingConcrete;
			audio.loop = true;
			audio.Play();
		}
		audio.volume = mass / 200;
		audio.pitch = speed / 5;

	}

	public void PlayAudioDeath()
	{
		audio.clip = AudioHitting;
		audio.loop = false;
		
		audio.volume = 1;

		audio.pitch = 0.1f;
		
		audio.Play ();
		
	}

	public void PlayAudioHitMetal(float mass, float speed)
	{
		audio.clip = AudioHittingMetal;
		audio.loop = false;

		audio.volume = mass / 40;
		
		//High pitch audioclip, hard to manipulate the pitch only for good effects
		audio.volume *= (speed/10 + 0.1f);
		audio.pitch = 0.8f;
		
		audio.Play ();

	}

	public void SetDeath()
	{
		death = true;
		PlayAudioDeath ();

	}

	public void PlayAudioJumping()
	{
		
		audio.clip = AudioJumping;
		audio.loop = false;
		
		audio.volume = 0.05f;
		audio.pitch = 1;

		audio.Play ();
	}


}
