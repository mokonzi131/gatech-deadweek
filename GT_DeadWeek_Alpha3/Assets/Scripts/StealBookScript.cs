﻿/*
Created by Team "GT Dead Week"
	Chenglong Jiang
	Arnaud Golinvaux	
	Michael Landes
	Josephine Simon
	Chuan Yao
*/

using UnityEngine;
using System.Collections;

public class StealBookScript : MonoBehaviour {

	public float stealRate = 2.0f;
	float lastStealTime = -10.0f;

	Inventory inventory;

	Transform _transform;
	Transform player;

	// Use this for initialization
	void Start () {
		_transform = GetComponent<Transform>();
		player = GameObject.Find("Player").GetComponent<Transform>();
		if (player == null)
			Debug.LogError("No player on scene");

		inventory = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance(_transform.position, player.position) < 1.0f)
		{
			if (lastStealTime + stealRate < Time.time)
			{
				//Add code here to decrease the number of books in the inventory by 1

				inventory.remove(Inventory.ItemCategory.BOOK);

				lastStealTime = Time.time;
			}
		}
	}
}
