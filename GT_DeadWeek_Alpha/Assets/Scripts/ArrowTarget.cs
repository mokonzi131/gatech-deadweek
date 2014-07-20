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
		Vector3 playerPos = player.transform.position;
		this.transform.position = playerPos + new Vector3(0, 1, 0);

		if(GameObject.FindWithTag ("GameController").GetComponent<Inventory>().hasRetrieveTheBook){
			transform.LookAt(destPoint.transform.position, Vector3.up);
		}else{
			transform.LookAt (book.transform.position, Vector3.up);
		}

		this.transform.position += this.transform.forward;
	}

}
