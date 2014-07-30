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

public class Timer : MonoBehaviour {

	public float time = 120 ;

	//private TimerDisplay timerDisplay;
	private bool isPaused = false;

	private bool loser;
	private bool alreadyClicked;

	private bool isBlinking;

	// Use this for initialization
	void Start () {
		alreadyClicked = false;
		//timerDisplay = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().timer;
		//timerDisplay.setTime (time);
		loser = false;

		isBlinking = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPaused) {
			time -= Time.deltaTime;
			//timerDisplay.setTime (time);
			setTime(time);
			display();

			if (time <= 45)
			{
				guiText.color = Color.red;
				isBlinking = true;
			}
			else
			{
				guiText.color = Color.white;
				isBlinking = false;
			}

//			if (isBlinking)
//			{
//				if(time - Mathf.FloorToInt(time) < 0.75)
//					guiText.enabled = true;
//				else
//					guiText.enabled = false;
//			}


			if(time <= 0){
				loser = true;
				GameObject.FindWithTag("Player").GetComponent<ThirdPersonCamera>().enabled = false;
				GameObject.FindWithTag("Player").rigidbody.constraints = RigidbodyConstraints.FreezeAll ;
			}
		}
	}

	public void OnGUI() 
	{
		if (loser)
		{
			if(!alreadyClicked){
				GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 90, 200, 40), "Far too slow!");
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 70, 200, 40), "Try again"))
				{
					alreadyClicked = true;
					GameObject.FindGameObjectWithTag("GameController").GetComponent<FadeInOut>().EndScene() ;
					//Application.LoadLevel(0);
				}
			}
		}
		
	}

	public void stop(){
		isPaused = true;
	}

	public void start(){
		isPaused = false;
	}

	public void tooglePause(){
		isPaused = !isPaused;
	}


	public void setTime(float t){
		time = Mathf.Max (t, 0.0f) ;
	}
	
	public float getTime(){
		return time ;
	} 

	public void display() {
		int minutes = (int)(time / 60);
		int secondes = (int)(time - 60 * minutes);
		int rest = (int)(100 * time - 6000 * minutes - 100 * secondes);
		
		guiText.text = minutes.ToString ("00") + " : " + secondes.ToString ("00");
	}

}
