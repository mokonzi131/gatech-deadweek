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

public class PickUpActionNew : MonoBehaviour {
	
	
	private GameObject target;
	
	public GUIText warningText;
	
	public float grabRange = 2.0f;
	public float warningTextTimeout = 1.0f;
	float lastWarningTextTime = -10.0f;
	
	int layerMask;
	
	void Start() {
		target = GameObject.FindWithTag("Player");
		layerMask = 1 << 8;
		
		layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (lastWarningTextTime + warningTextTimeout < Time.time)
			ClearWarningText();
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {

		if (hit.gameObject.tag == "Book")
		{
			if (!hit.gameObject.GetComponent<BookPropertyScript>().isJustThrowed)
			{

				Inventory inventory = GameObject.FindWithTag("GameController").GetComponent<Inventory>();

				Inventory.ItemCategory c = Inventory.ItemCategory.BOOK;

				
				float mass = hit.gameObject.rigidbody.mass;

				if (inventory.addItem(c, new Item(mass, hit.gameObject.name)))
				{
					//UpdateWarningText("Pick Up!");
					Destroy(hit.gameObject);
						
				}
				else 
				{
					gameObject.GetComponent<ThrowScript>().throwAction();
					inventory.addItem(c, new Item(mass, hit.gameObject.name));
					Destroy(hit.gameObject);
					//UpdateWarningText("There is no more room for this in your backpack!");

				}
			}
		}
	}
	
	
	void UpdateWarningText(string msg)
	{
		warningText.text = msg;
		lastWarningTextTime = Time.time;
	}
	
	void ClearWarningText ()
	{
		warningText.text = "";
	}
}
