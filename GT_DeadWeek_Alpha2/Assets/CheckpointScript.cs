using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {

	public int index = 1;
	private CheckpointManagerScript checkpointManager;

	bool newCheckpointFlash = false;

	
	public float fadeSpeed = 2.5f;          // Speed that the screen fades to and from black.
	public GUITexture guiTexture;

	// Use this for initialization
	void Start () {
		checkpointManager = GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>();
	}

	void Awake () {
		
		// Set the texture so that it is the the size of the screen and covers it.
		guiTexture.texture = new Texture2D (Screen.width, Screen.height);
		guiTexture.color = Color.clear;
		guiTexture.transform.position = new Vector3 (0, 0, 100);
		guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);

	}

	
	// Update is called once per frame
	void Update () {
		if (newCheckpointFlash)
		{
			FadeToWhite();
			Debug.Log(guiTexture.color.ToString());
			if (guiTexture.color.a >= 0.65f)
			{
				guiTexture.color = Color.clear;
				guiTexture.enabled = false;
				newCheckpointFlash  = false;
			}

		}
	}


	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.tag == "Player")
		{
			if (index > checkpointManager.lastCheckpointIndex)
			{
				Debug.Log("Visit Checkpoint #" + index.ToString());
				newCheckpointFlash = true;
				guiTexture.enabled = true;
				checkpointManager.lastCheckpointIndex = index;
				checkpointManager.lastCheckpointTime = GameObject.Find("Timer").GetComponent<Timer>().time;
			}
		}
		
	}

	
	void FadeToWhite ()
	{
		// Lerp the colour of the texture between itself and black.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.white, fadeSpeed * Time.deltaTime);
	}
}
