using UnityEngine;
using System.Collections;

public class PlayerDamageControl : MonoBehaviour {
	public float life;
	
	public Texture2D hitTexture;
	public Texture2D blackTexture;
	
	private float hitAlpha;
	private float blackAlpha;
	
	private float recoverTime;
	
	public AudioClip[] hitSounds;
	public AudioClip dyingSound;

	bool receiveDamage;

	void Start()
	{
		PlayerController.dead = false;
		hitAlpha = 0.0f;
		blackAlpha = 0.0f;
		life = 1.0f;

		receiveDamage = false;
	}
	
	void HitSoldier(string hit)
	{
		if(receiveDamage)
		{

			life -= 0.2f;

			Camera.main.GetComponent<PlayerCamera>().StartShake();
			
			if(!audio.isPlaying)
			{
				//if(life < 0.5f && (Random.Range(0, 100) < 30))
				if(life < 0.5f)
				{
					audio.clip = dyingSound;
				}
				else
				{
					audio.clip = hitSounds[Random.Range(0, hitSounds.Length)];
				}
				
				audio.Play();
			}
			
			recoverTime = (1.0f - life + 0.1f) * 10.0f;

			if(life <= 0.0f)
			{
				PlayerController.dead = true;
			}

			receiveDamage = false;
		}
	}
	
	void Update()
	{
		if (PlayerController.dead)
		{
			SetToHealthy();
			//StartCoroutine ("SetToHealthy");
			GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>().ResetToLastCheckpoint();

			return;
		}

		recoverTime -= Time.deltaTime;
		
		if(recoverTime <= 0.0f)
		{
			life += life * Time.deltaTime;
			
			life = Mathf.Clamp(life, 0.0f, 1.0f);
			
			hitAlpha = 0.0f;
		}
		else
		{
			hitAlpha = recoverTime / ((1.0f - life + 0.1f) * 10.0f);
		}


		if(!PlayerController.dead) return;

		//SetToHealthy ();

		SetToHealthy ();
		GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>().ResetToLastCheckpoint();

//		blackAlpha += Time.deltaTime;
//		
//		if(blackAlpha >= 1.0f)
//		{
//			GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>().ResetToLastCheckpoint();
//			//Application.LoadLevel(0);
//		}
	}

	void SetToHealthy()
	{
		PlayerController.dead = false;
		hitAlpha = 0.0f;
		blackAlpha = 0.0f;
		life = 1.0f;
		recoverTime = 0;
		
		receiveDamage = false;

	}
	
	void OnGUI()
	{
		//if(!receiveDamage) return;

		Color oldColor;
		Color auxColor;
		oldColor = auxColor = GUI.color;
		
		auxColor.a = hitAlpha;
		GUI.color = auxColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitTexture);
		
		auxColor.a = blackAlpha;
		GUI.color = auxColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
		
		GUI.color = oldColor;

	}	

	public void isMeleeAttacked()
	{
		receiveDamage = true;
		HitSoldier ("dfa");

	}

	public void isRangeAttacked()
	{
		receiveDamage = true;
		HitSoldier ("dfa");
	}

	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Axe")
			isRangeAttacked();
	}



}
