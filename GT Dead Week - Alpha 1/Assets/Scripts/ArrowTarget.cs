using UnityEngine;
using System.Collections;

public class ArrowTarget : MonoBehaviour {

	private GameObject book;
	private GameObject destPoint;
	private GameObject player;

	// Use this for initialization
	void Start () {
		book = GameObject.Find ("TheBook");
		destPoint = GameObject.Find ("DestPoint");
		player = GameObject.Find ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if(GameObject.FindWithTag ("GameController").GetComponent<Inventory>().hasRetrieveTheBook){
			transform.LookAt(destPoint.transform.position);
		}else{
			transform.LookAt (book.transform.position);
		}
		Vector3 playerPos = player.transform.position;
		playerPos += new Vector3(0, 2.5f, 0);
		this.transform.position = playerPos;
	}

}
