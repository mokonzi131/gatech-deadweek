using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	public float time = 120 ;

	private TimerDisplay timerDisplay;
	private bool isPaused = false;

	private bool loser;
	private bool alreadyClicked;

	// Use this for initialization
	void Start () {
		alreadyClicked = false;
		timerDisplay = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().timer;
		timerDisplay.setTime (time);
		loser = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPaused) {
			time -= Time.deltaTime;
			timerDisplay.setTime (time);
			if(time <= 0){
				loser = true;
				GameObject.FindWithTag("Player").GetComponent<ThirdPersonCamera>().enabled = false;
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

	void stop(){
		isPaused = true;
	}

	void start(){
		isPaused = false;
	}

	void tooglePause(){
		isPaused = !isPaused;
	}
}
