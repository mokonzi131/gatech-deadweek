using UnityEngine;
using System.Collections;

public class FadeInOut : MonoBehaviour {

	public float fadeSpeed = 2.5f;          // Speed that the screen fades to and from black.
	public GUITexture guiTexture;
	
	private bool sceneStarting;      // Whether or not the scene is still fading in.
	private bool sceneEnding;
	
	void Awake ()
	{
		sceneStarting = true;
		sceneEnding = false;
		// Set the texture so that it is the the size of the screen and covers it.
		guiTexture.texture = new Texture2D (Screen.width, Screen.height);
		guiTexture.color = Color.black;
		guiTexture.transform.position = new Vector3 (0, 0, 100);
		guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}
	
	
	void Update ()
	{
		if (sceneStarting)
			StartScene ();
		else if (sceneEnding)
			EndScene ();
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
	
	
	void StartScene ()
	{
		// Fade the texture to clear.
		FadeToClear();
		
		// If the texture is almost clear...
		if(guiTexture.color.a <= 0.05f)
		{
			// ... set the colour to clear and disable the GUITexture.
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
			
			// The scene is no longer starting.
			sceneStarting = false;
		}
	}
	
	
	public void EndScene ()
	{
		sceneEnding = true;
		sceneStarting = false;
		// Make sure the texture is enabled.
		guiTexture.enabled = true;
		
		// Start fading towards black.
		FadeToBlack();
		
		// If the screen is almost black...
		if(guiTexture.color.a >= 0.95f)
			// ... reload the level.
			Application.LoadLevel(0);
	}
}
