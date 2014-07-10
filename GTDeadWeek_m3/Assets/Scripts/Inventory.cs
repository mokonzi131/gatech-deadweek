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
using System.Collections.Generic;

public class Item {
	public float weight;
	public string name;

	public Item(float w, string n){
		weight = w;
		name = n;
	}
}

public class Inventory : MonoBehaviour {

	public enum ItemCategory {FOOD=0, BOOK=1};

	public int maxWeight;

	private InventoryBar inventoryBar;
	private List<Item>[] items;
	// Use this for initialization
	void Start () {
		items = new List<Item>[2];
		items [0] = new List<Item> ();
		items [1] = new List<Item> ();
		inventoryBar = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().inventory;
		inventoryBar.setMaxInventorySize (maxWeight);
	}

	public bool addItem(ItemCategory c, Item i){
		if (getWeight () + i.weight > maxWeight) {
			return false;
		}

		items [(int) c].Add (i);

		dispatchWeight();

		return true;
	}

	public float getWeight(){
		float totalWeight = 0;
		for (int c = 0; c<items.Length; ++c) {
			foreach (Item i in items[c]) {
				totalWeight += i.weight;
			}
		}
		return totalWeight;
	}

	public int getMaxWeight(){
		return maxWeight;
	}

	public bool remove(ItemCategory c){
		if (items [(int)c].Count == 0)
			return false;

		items [(int)c].RemoveAt (0);

		dispatchWeight();

		return true;
	}

	private void dispatchWeight(){
		float newWeight = getWeight ();
		inventoryBar.setInventoryUsage (newWeight);
		InventoryDisplay ind = GameObject.FindWithTag("HeadUpDisplay").GetComponent<HeadUpDisplay>().inventoryContent;
		ind.setBooksNumber (items [(int)ItemCategory.BOOK].Count);
		ind.setDrinksNumber (items [(int)ItemCategory.FOOD].Count);
		GameObject.FindWithTag("Player").GetComponent<Rigidbody>().mass = 2 + newWeight;
	}
}
