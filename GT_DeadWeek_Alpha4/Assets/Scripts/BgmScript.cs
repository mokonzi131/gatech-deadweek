using UnityEngine;
using System.Collections;

public class BgmScript : MonoBehaviour {

	public AudioClip[] bgm;
	public float bgmVolume = 0.1f;

	public AudioClip bgmFight;
	public float fightVolume = 0.2f;

	private bool isFighting;

	private GameManager gameManager;

	void Awake()
	{
		isFighting = false;
		gameManager = GameObject.FindWithTag ("GameController").GetComponent<GameManager> ();
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log (GameManager.countAttackingZombie.ToString ());
		if (gameManager.countAttackingZombie > 0 && !isFighting)
		{
			isFighting  =true;
			playFightBGM();
		}

		if (gameManager.countAttackingZombie == 0 && isFighting)
		{
			isFighting = false;
			playRandomBGM();
		}

		if (!audio.isPlaying)
			playRandomBGM();
	}

	void playRandomBGM()
	{
		audio.clip = bgm [Random.Range (0, bgm.Length)];
		audio.volume = bgmVolume;
		audio.loop = false;
		audio.Play ();

	}

	void playFightBGM()
	{
		audio.clip = bgmFight;
		audio.volume = fightVolume;
		audio.loop = true;
		audio.Play ();
	}


}
