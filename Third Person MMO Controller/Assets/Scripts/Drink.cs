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
		if (Input.GetKeyDown (KeyCode.C) && inventory.remove (Inventory.ItemCategory.FOOD)) {
			stamina.deltaStamina(staminaRestauration);
		}
	}
}
