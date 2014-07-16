using UnityEngine;
using System.Collections;

public class VictoryScript : MonoBehaviour {

	bool victory;
	// Use this for initialization
	void Start () {
		victory = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.tag == "Player")
		{
			GameObject.FindWithTag("Player").GetComponent<ThirdPersonCamera>().enabled = false;
			victory = true;
		}

	}

	public void OnGUI() 
	{
		if (victory)
		{
			GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 150, 200, 40), "Victory!");
			if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 130, 200, 40), "Restart Level"))
			{            
				Application.LoadLevel(0);
			}
		}
		
	}
}
