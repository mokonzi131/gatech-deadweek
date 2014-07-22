/*
Created by Team "GT Dead Week"
	Chenglong Jiang
	Arnaud Golinvaux	
	Michael Landes
	Josephine Simon
	Chuan Yao
*/

using UnityEngine;
using System.Collections;

public class StaminaBar {
	private int maxStamina_;
	private float currentStamina_;
	private GameObject staminaDisplay_ = new GameObject();
	private GameObject staminaIcon_ = new GameObject();
	private GameObject border_ = new GameObject();
	
	private float t = 0 ;

	public StaminaBar(Texture2D can,Texture2D border){
		maxStamina_ = 0;
		currentStamina_ = 0;

		staminaDisplay_.AddComponent("GUITexture");
		staminaDisplay_.transform.position = Vector3.zero;
		staminaDisplay_.transform.localScale = Vector3.zero;
		staminaDisplay_.guiTexture.texture = new Texture2D (1000, 1000);

		border_.AddComponent("GUITexture");
		border_.transform.position = Vector3.zero;
		border_.transform.localScale = Vector3.zero;
		border_.guiTexture.texture = border;
		border_.guiTexture.border = new RectOffset (25, 25, 25, 25);

		staminaIcon_.AddComponent ("GUITexture");
		staminaIcon_.transform.position = Vector3.zero;
		staminaIcon_.transform.localScale = Vector3.zero;
		staminaIcon_.guiTexture.texture = can;
	}
	
	public void display() {
		int h = Screen.height;
		float percentageOfHeight = 0.8f;

		float ratioStamina = currentStamina_ / maxStamina_;

		if(ratioStamina < .25f){
			t +=  Time.deltaTime;
			t -= 2*Mathf.PI*(Mathf.Floor(t/(2*Mathf.PI))) ;
		} else {
			t = 0 ;
		}

		staminaDisplay_.guiTexture.color = 0.4f *
			Color.Lerp (Color.red, Color.blue, ratioStamina);

		Color c = staminaDisplay_.guiTexture.color;
		staminaDisplay_.guiTexture.color = new Color (c.r, c.g, c.b, 0.5f * (1 + Mathf.Sin (2 * Mathf.PI * 2.0f * t)));

		staminaDisplay_.guiTexture.pixelInset = 
			new Rect (40, 10+(1 - percentageOfHeight) / 2 * h, 20, percentageOfHeight * h * ratioStamina);

		border_.guiTexture.pixelInset = 
			new Rect (40-3, 10+(1 - percentageOfHeight) / 2 * h-3, 20+6, percentageOfHeight * h + 6);

		staminaIcon_.guiTexture.pixelInset = 
			new Rect (30, (1 - percentageOfHeight) / 2 * h - 40, 40, 40);
	}

	public void setStamina(float stamina) {
		currentStamina_ = stamina > maxStamina_ ? maxStamina_ : stamina;
		currentStamina_ = currentStamina_ > 0 ? currentStamina_ : 0;
	}

	public void setMaxStamina(int stamina) {
		maxStamina_ = stamina > 0 ? stamina : 0;
	}

	public float getCurrentStamina(){
		return currentStamina_;
	}

	public int getMaxStamina(){
		return maxStamina_;
	}
}

public class InventoryBar {
	private int maxInventorySize_;
	private float currentInventorySize_;
	private GameObject InventoryUsageDisplay_ = new GameObject();
	private GameObject inventoryIcon_ = new GameObject();
	private GameObject border_ = new GameObject();

	private float t = 0 ;

	public InventoryBar(Texture2D backpack,Texture2D border){
		maxInventorySize_ = 0;
		currentInventorySize_ = 0;

		InventoryUsageDisplay_.AddComponent("GUITexture");
		InventoryUsageDisplay_.transform.position = Vector3.zero;
		InventoryUsageDisplay_.transform.localScale = Vector3.zero;
		InventoryUsageDisplay_.guiTexture.texture = new Texture2D (1000, 1000);

		border_.AddComponent("GUITexture");
		border_.transform.position = Vector3.zero;
		border_.transform.localScale = Vector3.zero;
		border_.guiTexture.texture = border;
		border_.guiTexture.border = new RectOffset (25, 25, 25, 25);

		inventoryIcon_.AddComponent("GUITexture");
		inventoryIcon_.transform.position = Vector3.zero;
		inventoryIcon_.transform.localScale = Vector3.zero;
		inventoryIcon_.guiTexture.texture = backpack;
	}
	
	public void display() {
		int h = Screen.height;
		int w = Screen.width;
		float percentageOfHeight = 0.8f;
		
		float ratioInventory = currentInventorySize_ / maxInventorySize_;

		if(ratioInventory > .75f){
			t +=  Time.deltaTime;
			t -= 2*Mathf.PI*(Mathf.Floor(t/(2*Mathf.PI))) ;
		} else {
			t = 0 ;
		}

		InventoryUsageDisplay_.guiTexture.color = 0.4f *
			Color.Lerp (Color.blue, Color.red, ratioInventory);

		Color c = InventoryUsageDisplay_.guiTexture.color;
		InventoryUsageDisplay_.guiTexture.color = new Color (c.r, c.g, c.b, 0.5f * (1 + Mathf.Sin (2 * Mathf.PI * 2.0f * t)));

		InventoryUsageDisplay_.guiTexture.pixelInset = 
			new Rect (w-40-20, 10+(1 - percentageOfHeight) / 2 * h, 20, percentageOfHeight * h * ratioInventory);

		border_.guiTexture.pixelInset = 
			new Rect (w-40-20-3, 10+(1 - percentageOfHeight) / 2 * h -3, 20+6, percentageOfHeight * h +6);

		inventoryIcon_.guiTexture.pixelInset = 
			new Rect (w-30-40, (1 - percentageOfHeight) / 2 * h - 40, 40, 40);
	}
	
	public void setInventoryUsage(float inventorySize) {
		currentInventorySize_ = inventorySize > maxInventorySize_ ? maxInventorySize_ : inventorySize;
		currentInventorySize_ = currentInventorySize_ > 0 ? currentInventorySize_ : 0;
	}

	public void setMaxInventorySize(int inventorySize) {
		maxInventorySize_ = inventorySize > 0 ? inventorySize : 0;
	}
}

public class InventoryDisplay {

	private GameObject background = new GameObject();
	GameObject nbBooks = new GameObject ();
	GameObject nbDrinks = new GameObject ();
	GameObject bookIcon = new GameObject ();
	GameObject drinkIcon = new GameObject ();
	private int h_;
	private int w_;

	public InventoryDisplay(int h, int w, Texture2D booksIcon, Texture2D canIcon) {
		h_ = h;
		w_ = w;

		background.AddComponent ("GUITexture");
		background.transform.position = new Vector3(0,0,-0.5f);
		background.transform.localScale = Vector3.zero;
		background.guiTexture.texture = new Texture2D (h, w);
		background.guiTexture.color = 0.4f * Color.grey ;
		background.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w, Screen.height * 0.1f - 40,w,h);

		drinkIcon.AddComponent ("GUITexture");
		drinkIcon.transform.parent = background.transform;
		drinkIcon.transform.position = new Vector3 (0,0,1);
		drinkIcon.transform.localScale = Vector3.zero;
		drinkIcon.guiTexture.texture = canIcon;
		drinkIcon.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w + 5, Screen.height * 0.1f - 40 + 3,(h-10)/2,(h-10)/2);

		bookIcon.AddComponent ("GUITexture");
		bookIcon.transform.parent = background.transform;
		bookIcon.transform.position = new Vector3 (0,0,1);
		bookIcon.transform.localScale = Vector3.zero;
		bookIcon.guiTexture.texture = booksIcon;
		bookIcon.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w + 5, Screen.height * 0.1f - 40 + 7 + (h-10)/2,(h-10)/2,(h-10)/2);

		nbBooks.AddComponent ("GUIText");
		nbBooks.transform.parent = background.transform;
		nbBooks.transform.position = new Vector3 (0,0,1);
		nbBooks.guiText.text = "0";
		nbBooks.guiText.fontSize = 30;
		nbBooks.guiText.anchor = TextAnchor.MiddleLeft;
		nbBooks.guiText.pixelOffset =
			new Vector2 (Screen.width - 80 - w + (h-10)/2 + (w - (h-10)/2)/3, Screen.height * 0.1f - 40 + 7 + (h-10)/2 + h/4);
	
		nbDrinks.AddComponent ("GUIText");
		nbDrinks.transform.parent = background.transform;
		nbDrinks.transform.position = new Vector3 (0,0,1);
		nbDrinks.guiText.text = "0";
		nbDrinks.guiText.fontSize = 30;
		nbDrinks.guiText.anchor = TextAnchor.MiddleLeft;
		nbDrinks.guiText.pixelOffset =
			new Vector2 (Screen.width - 80 - w + (h-10)/2 + (w - (h-10)/2)/3, Screen.height * 0.1f - 40 + 7 + (h-10)/2 - h/4);
	}

	public void display(){
		background.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w_, Screen.height * 0.1f - 40,w_,h_);

		drinkIcon.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w_ + 5, Screen.height * 0.1f - 40 + 3,(h_-10)/2,(h_-10)/2);

		bookIcon.guiTexture.pixelInset = 
			new Rect (Screen.width - 80 - w_ + 5, Screen.height * 0.1f - 40 + 7 + (h_-10)/2,(h_-10)/2,(h_-10)/2);

		nbBooks.guiText.pixelOffset =
			new Vector2 (Screen.width - 80 - w_ + (h_-10)/2 + (w_ - (h_-10)/2)/3, Screen.height * 0.1f - 40 + 7 + (h_-10)/2 + h_/4);

		nbDrinks.guiText.pixelOffset =
			new Vector2 (Screen.width - 80 - w_ + (h_-10)/2 + (w_ - (h_-10)/2)/3, Screen.height * 0.1f - 40 + 7 + (h_-10)/2 - h_/4);
	}

	public void setBooksNumber(int number) {
		nbBooks.guiText.text = number.ToString();
		//nbBooks.guiText.text = "∞";
	}
	
	public void setDrinksNumber(int number) {
		nbDrinks.guiText.text = number.ToString();
	}

	public void setEnable(bool enable){
		background.SetActive (enable);
	}
}

public class TimerDisplay {
	private float time = 0;

	private GameObject TimeDisplay_ = new GameObject();

	public TimerDisplay() {
		TimeDisplay_.AddComponent ("GUIText");
		TimeDisplay_.guiText.alignment = TextAlignment.Center;
		TimeDisplay_.guiText.anchor = TextAnchor.MiddleCenter;
		TimeDisplay_.guiText.fontSize = 24;
		TimeDisplay_.guiText.color = Color.red;
		TimeDisplay_.transform.position = new Vector3 (0.5f, 0.9f, 1);
	}

	public void display() {
		int minutes = (int)(time / 60);
		int secondes = (int)(time - 60 * minutes);
		int rest = (int)(100 * time - 6000 * minutes - 100 * secondes);
		TimeDisplay_.guiText.text = minutes.ToString ("00") + ":" +
									secondes.ToString ("00") + ":" +
									rest.ToString ("00") ;
	}

	public void setTime(float t){
		time = Mathf.Max (t, 0.0f) ;
	}

	public float getTime(){
		return time ;
	}
}

public class HeadUpDisplay : MonoBehaviour {
	
	public Texture2D can;
	public Texture2D backpack;
	public Texture2D booksIcon;
	public Texture2D canIcon;
	public Texture2D border;

	public int inventoryWidth ;
	public int inventoryHeight;

	public StaminaBar energy;
	public InventoryBar inventory;
	public TimerDisplay timer;
	public InventoryDisplay inventoryContent;

	private bool inventoryEnable = true ;
	
	// Use this for initialization
	void Awake () {
		energy = new StaminaBar (can,border);
		inventory = new InventoryBar (backpack,border);
		timer = new TimerDisplay ();
		inventoryContent = new InventoryDisplay (inventoryHeight, inventoryWidth, booksIcon, canIcon);
	}


	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.I)) {
			inventoryEnable = !inventoryEnable;
			inventoryContent.setEnable(inventoryEnable);
		}

		energy.display();
		inventory.display ();
		inventoryContent.display ();
		timer.display ();
	}
}
