using UnityEngine;
using System.Collections;

public class VendingMachineScript : MonoBehaviour {

	public AudioClip vend;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			Stamina staminaBar = GameObject.FindWithTag ("GameController").GetComponent<Stamina> ();

			staminaBar.setToFull();

			if (!audio.isPlaying)
			{
				audio.PlayOneShot(vend, 1.0f);
			}
			ScreenFadingScript fadingManager = GameObject.Find("ScreenFadingManager").GetComponent<ScreenFadingScript> ();
			fadingManager.FadeToWhite();
		}

	}
}
