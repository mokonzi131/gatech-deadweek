using UnityEngine;
using System.Collections;

public class timer : MonoBehaviour {

	public float time = 120 ;

	private TimerDisplay timerDisplay;
	private bool isPaused = false;
	// Use this for initialization
	void Start () {
		timerDisplay = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().timer;
		timerDisplay.setTime (time);
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPaused) {
			time -= Time.deltaTime;
			timerDisplay.setTime (time);
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
