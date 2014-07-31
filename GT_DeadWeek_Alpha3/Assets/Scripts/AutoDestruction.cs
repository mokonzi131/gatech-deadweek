using UnityEngine;
using System.Collections;

public class AutoDestruction : MonoBehaviour {

	private ZombiePopUp zombieScript;
	private Transform player;

	private float distance;
	// Use this for initialization
	void Start () {
		zombieScript = GameObject.FindWithTag ("GameController").GetComponentInChildren<ZombiePopUp> ();
		distance = zombieScript.radius + 5;

		player = GameObject.Find("Player").GetComponent<Transform>();
		if (player == null) {
			Debug.LogError("no player found");
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = player.position;
		if(Vector3.Distance(transform.position, playerPos) > distance){
			zombieScript.destroyedOne();
			GameObject.Destroy(this);
		}
	}
}
