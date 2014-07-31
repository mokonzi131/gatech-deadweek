using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {

	public int index = 1;
	private CheckpointManagerScript checkpointManager;
	private ScreenFadingScript fadingManager;

	bool newCheckpointFlash = false;



	// Use this for initialization
	void Start () {
		checkpointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>();
		fadingManager = GameObject.Find("ScreenFadingManager").GetComponent<ScreenFadingScript> ();

	}

	// Update is called once per frame
	void Update () {
//		if (newCheckpointFlash)
//		{
//			FadeToWhite();
//			Debug.Log(guiTexture.color.ToString());
//			if (guiTexture.color.a >= 0.65f)
//			{
//				guiTexture.color = Color.clear;
//				guiTexture.enabled = false;
//				newCheckpointFlash  = false;
//			}
//
//		}
	}


	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.tag == "Player")
		{
			if (index == checkpointManager.lastCheckpointIndex + 1)
			{
				Debug.Log("Visit Checkpoint #" + index.ToString());
				newCheckpointFlash = true;

				fadingManager.FadeToWhite();

				checkpointManager.UpdateCheckpoint(index);

				GameObject.FindGameObjectWithTag("GameController").GetComponentInChildren<Timer>().setTime(900);
				GameObject.FindGameObjectWithTag("GameController").GetComponent<Stamina>().setToFull();

//				checkpointManager.lastCheckpointIndex = index;
//				checkpointManager.lastCheckpointTime = GameObject.Find("Timer").GetComponent<Timer>().time;
			}
		}
		
	}

	
	void FadeToWhite ()
	{
		// Lerp the colour of the texture between itself and black.
//		guiTexture.color = Color.Lerp(guiTexture.color, Color.white, fadeSpeed * Time.deltaTime);
	}
}
