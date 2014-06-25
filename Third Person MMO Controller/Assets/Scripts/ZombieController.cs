using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour {

	public float radarRange = 15.0f;

	private GameObject player;

	public float faintTime = 10.0f;

	bool canMove = true;
	float lastHitTime = -10.0f;
	
	void Start() {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	void Update() {
		CheckFaintTime ();
		MoveZombie ();
	}
	
	void MoveZombie() {


		CharacterController controller = gameObject.GetComponent<CharacterController> ();

		Vector3 playerPosition = player.transform.position;
		Vector3 zombiePosition = gameObject.transform.position;

		float dist = Vector3.Distance (playerPosition, zombiePosition);

		
		if (dist < 1.8f)
			Application.LoadLevel(Application.loadedLevel);

		if (!canMove)
			return;
		if (dist > radarRange)
		{
			return;
		}


		Vector3 direction = playerPosition - zombiePosition;
		direction.Normalize();
		
		controller.SimpleMove(direction);

	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Book")
		{
			canMove = false;

			lastHitTime = Time.time;

			Debug.Log("HIT!");

		}

		if (collision.gameObject.tag == "Player")
		{
			Application.LoadLevel(Application.loadedLevel);
		}

	}

	void CheckFaintTime()
	{
		if (Time.time > lastHitTime + faintTime)
			canMove = true;

	}


}
