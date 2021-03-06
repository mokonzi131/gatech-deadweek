﻿using UnityEngine;
using System.Collections;

public class VictoryScript : MonoBehaviour {

	bool victory;
	private bool alreadyClicked;
	
	public GUIText warningText;

	public float warningTextTimeout = 1.0f;
	float lastWarningTextTime = -10.0f;
	// Use this for initialization
	void Start () {
		victory = false;
		alreadyClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastWarningTextTime + warningTextTimeout < Time.time)
			ClearWarningText();
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.tag == "Player")
		{
			victory = GameObject.FindWithTag("GameController").GetComponent<Inventory>().hasRetrieveTheBook;
			if(victory){
				GameObject.FindWithTag("Player").GetComponent<ThirdPersonCamera>().enabled = false;
			} else {
				UpdateWarningText("You need THE Book to win!");
			}
		}

	}

	public void OnGUI() 
	{
		if (victory)
		{
			if(!alreadyClicked){
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 90, 200, 40), "Victory!");
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 70, 200, 40), "Restart Level"))
				{
					alreadyClicked = true;
					GameObject.FindGameObjectWithTag("GameController").GetComponent<FadeInOut>().EndScene() ;
					//Application.LoadLevel(0);
				}
			}
		}
		
	}

	void UpdateWarningText(string msg)
	{
		warningText.text = msg;
		lastWarningTextTime = Time.time;
	}

	void ClearWarningText ()
	{
		warningText.text = "";
	}
}
