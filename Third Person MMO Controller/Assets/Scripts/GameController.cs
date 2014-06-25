/*
Created by Team "GT Dead Week"
	Arnaud Golinvaux	
	Chenglong Jiang
	Michael Landes
	Josephine Simon
	Chuan Yao
*/


using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	private GameObject[] zombies;
	private GameObject player;

	void Start() {
		zombies = GameObject.FindGameObjectsWithTag ("Zombie");
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void Update() {
		MoveZombies ();
	}

	void MoveZombies() {
		foreach (GameObject zombie in zombies) {
			// get a direction to the current player
			CharacterController controller = zombie.GetComponent<CharacterController>();

			Vector3 playerPosition = player.transform.position;
			Vector3 zombiePosition = zombie.transform.position;
			Vector3 direction = playerPosition - zombiePosition;
			direction.Normalize();
			
			controller.SimpleMove(direction);
		}
	}
}
