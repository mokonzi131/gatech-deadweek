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

public class Drink : MonoBehaviour {

	public float staminaRestauration;

	private Inventory inventory;
	private Stamina stamina;
	// Use this for initialization
	void Start () {
		inventory = GameObject.FindWithTag ("GameController").GetComponent<Inventory> ();
		stamina = GameObject.FindWithTag ("GameController").GetComponent<Stamina> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetKeyDown (KeyCode.C) || Input.GetButtonDown("360_XButton")) && inventory.remove (Inventory.ItemCategory.FOOD)) {
			stamina.deltaStamina(staminaRestauration);
		}
	}
}
