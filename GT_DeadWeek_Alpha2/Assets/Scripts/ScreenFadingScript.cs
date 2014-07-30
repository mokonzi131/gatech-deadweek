using UnityEngine;
using System.Collections;

public class ScreenFadingScript : MonoBehaviour {

	
	public float fadeSpeed = 2.5f;          // Speed that the screen fades to and from black.
	public GUITexture guiTexture;

	private bool FadingToClear;

	private bool FadingToBlack;

	private bool FadingToWhite;
	
	void Awake ()
	{
		// Set the texture so that it is the the size of the screen and covers it.
		guiTexture.texture = new Texture2D (Screen.width, Screen.height);
		guiTexture.color = Color.black;
		guiTexture.enabled = false;
		guiTexture.transform.position = new Vector3 (0, 0, 100);
		guiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);

		FadingToClear = false;
		FadingToBlack = false;
		FadingToWhite = false;
	}
	
	
	void Update ()
	{
		if (FadingToClear)
		{
			//Debug.Log(Time.time.ToString() + " is fading clear!");
			FadeClear();

			if (guiTexture.color.a <= 0.05f)
			{
				guiTexture.color = Color.clear;
				guiTexture.enabled = false;
				FadingToClear = false;
			}
		}
		else if (FadingToBlack)
		{
			//Debug.Log(Time.time.ToString() + " is fading black!");
			FadeBlack();

			if (guiTexture.color.a >= 0.95f)
			{
				FadingToBlack = false;
			}

		}
		else if (FadingToWhite)
		{
			//Debug.Log(Time.time.ToString() + " is fading white!");
			FadeWhite();
			if (guiTexture.color.a >= 0.65f)
			{
				guiTexture.color = Color.clear;
				guiTexture.enabled = false;
				FadingToWhite = false;

			}
		}

	}
	
	
	void FadeClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	
	void FadeBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	void FadeWhite()
	{
		guiTexture.color = Color.Lerp(guiTexture.color, Color.white, fadeSpeed * Time.deltaTime);
	}

	public void FadeToClear()
	{
		FadingToClear = true;
		FadingToBlack = false;
		FadingToWhite = false;

		guiTexture.enabled = true;

	}

	public void FadeToBlack()
	{
		Debug.Log ("Start fading black");
		FadingToClear = false;
		FadingToBlack = true;
		FadingToWhite = false;
		
		guiTexture.color = Color.clear;
		guiTexture.enabled = true;
	}

	public void FadeToWhite()
	{
		FadingToClear = false;
		FadingToBlack = false;
		FadingToWhite = true;
		
		guiTexture.color = Color.clear;
		guiTexture.enabled = true;
	}

}
