using UnityEngine;
using System.Collections;

public class ZombiePopUp : MonoBehaviour {

	public Transform player;
	public float radius = 30;
	public float angle = 90;
	public float timeBetweenApparition = 2;
	public float variance = 1;
	public float max = 20;
	public GameObject Zombie1_1;
	public GameObject Zombie1_2;
	public GameObject Zombie1_3;
	public GameObject Zombie1_4;
	public GameObject Zombie1_5;
	public GameObject Zombie2;

	private GameObject[] zombies;
	private int nbZombie = 0 ;
	private float lastApparition;
	// Use this for initialization
	void Start () {
		lastApparition = Time.time;
		zombies = new GameObject[6];
		zombies[0] = Zombie1_1;
		zombies[1] = Zombie1_2;
		zombies[2] = Zombie1_3;
		zombies[3] = Zombie1_4;
		zombies[4] = Zombie1_5;
		zombies[5] = Zombie2;
	}
	
	// Update is called once per frame
	void Update () {
		float sinceLastApparition = Time.time - lastApparition;
		float r = Random.Range (-variance, variance);
		if (sinceLastApparition + r > timeBetweenApparition && nbZombie<max) {
			lastApparition = Time.time;
			Vector3 position = player.forward ;
			float a = Random.Range (-angle/2, angle/2);
			position = Quaternion.Euler(0, a, 0) * position;
			position = player.position + radius*position + 2*Vector3.up;
			int zombie = Mathf.Min(Random.Range (0,8), 5);

			GameObject.Instantiate(zombies[zombie], position, Quaternion.identity);
			nbZombie++;
			Debug.Log ("+1");
		}
	}

	public void destroyedOne(){
		nbZombie--;
		Debug.Log ("-1");
	}
}
