using UnityEngine;
using System.Collections;

public class TriggerTrap1 : MonoBehaviour {

	public GameObject trap; 

	Vector3 trapPos;

	bool triggered;

	// Use this for initialization
	void Start () {
		trapPos = this.transform.position + new Vector3(0, -4.0f, 0);
		triggered = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter (Collider other)
	{
		if (!triggered && other.gameObject.tag == "Player")
		{
			Instantiate(trap, trapPos, Quaternion.identity);
			triggered = true;
		}

	}
}
