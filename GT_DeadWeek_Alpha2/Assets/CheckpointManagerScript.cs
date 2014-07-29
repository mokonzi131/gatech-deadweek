using UnityEngine;
using System.Collections;

public class CheckpointManagerScript : MonoBehaviour {

	[HideInInspector]
	public int lastCheckpointIndex;

	[HideInInspector]
	public float lastCheckpointTime;

	
	public float fadeSpeed = 2.5f;          // Speed that the screen fades to and from black.
	public GUITexture guiTexture;

	private bool fadingIn;
	private bool fadingOut;

	
	void Awake ()
	{
		
		lastCheckpointIndex = 0;
		lastCheckpointTime = GameObject.Find ("Timer").GetComponent<Timer> ().time;

		fadingOut = false;
		fadingIn = false;

		// Set the texture so that it is the the size of the screen and covers it.
		guiTexture.texture = new Texture2D (Screen.width, Screen.height);
		guiTexture.color = Color.clear;
		guiTexture.transform.position = new Vector3 (0, 0, 100);
		guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}

	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	
	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}




	// Update is called once per frame
	void Update () {
	
		if (fadingOut)
		{
			FadeToBlack();
			Debug.Log(guiTexture.color.ToString());
			if (guiTexture.color.a >= 0.95f)
			{

				fadingOut = false;
				fadingIn = true;

				SetPlayerAndTimer();

			}
		}
		else if (fadingIn)
		{
			FadeToClear();
			Debug.Log(guiTexture.color.ToString());
			if (guiTexture.color.a <= 0.05f)
			{
				guiTexture.color = Color.clear;
				guiTexture.enabled = false;
				fadingIn  = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.P))
			ResetToLastCheckpoint();
			//GameObject.FindGameObjectWithTag("GameController").GetComponent<FadeInOut>().EndScene() ;

	}



	public void ResetToLastCheckpoint()
	{
		// Make sure the texture is enabled.
		guiTexture.enabled = true;

		fadingOut = true;
	}

	void SetPlayerAndTimer()
	{
		
		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
		GameObject targetCP = cps[0];
		foreach (GameObject cp in cps)
		{
			if (cp.GetComponent<CheckpointScript>().index == lastCheckpointIndex)
			{
				targetCP = cp;
				break;
			}
		}

		GameObject.FindGameObjectWithTag ("Player").transform.position = targetCP.transform.position;
		GameObject.Find ("Timer").GetComponent<Timer> ().time = lastCheckpointTime;

	}


}
