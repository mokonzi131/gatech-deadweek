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
		
		if (Input.GetKeyDown(KeyCode.C))
		{
			var hitColliders = Physics.OverlapSphere (transform.position, grabRange);
			
			float minAngle = 180.0f;
			GameObject targetObject = null;
			
			
			foreach(var hitCollider in hitColliders)
			{
				if (hitCollider.gameObject.tag == "Book" || hitCollider.gameObject.tag == "Drink" || 
				    hitCollider.gameObject.tag == "Food" || hitCollider.gameObject.tag == "TheBook")
				{
					Vector3 objectDirection = hitCollider.transform.position - target.transform.position;
					objectDirection.y = 0;
					
					float newAngle = Vector3.Angle(target.transform.forward, objectDirection);
					
					//Debug.Log("There is a cover here!");
					if(newAngle < minAngle)
					{
						minAngle = newAngle;
						targetObject = hitCollider.gameObject;
					}
					
				}
			}
			
			if (targetObject == null)
				return;
			
			if (minAngle < 80)
			{
				
				//				UpdateWarningText("Pick Up!");
				//				if (targetObject.tag != "Drink")
				//					Destroy(targetObject);
				
				Inventory inventory = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
				Inventory.ItemCategory c = Inventory.ItemCategory.FOOD ;
				if (targetObject.tag == "Book")
					c = Inventory.ItemCategory.BOOK;
				float mass = targetObject.rigidbody.mass ;
				if (targetObject.tag == "Drink")
					mass /= 10 ;
				if(targetObject.tag != "TheBook" && inventory.addItem(c, new Item(mass, targetObject.name))){
					Debug.Log("Pick Up!");
					UpdateWarningText("Pick Up!");
					if(targetObject.tag != "Drink")
						Destroy(targetObject);
					//targetObject.SetActive(false);
				} else if (targetObject.tag == "TheBook"){
					inventory.hasRetrieveTheBook = true ;
					UpdateWarningText("You found THE Book!");
					Destroy(targetObject);
				} else {
					Debug.Log("There is no more room for this in your backpack!");
					UpdateWarningText("There is no more room for this in your backpack!");
				}
				
			}
			else
			{
				Debug.Log("Wrong Direction!");
				UpdateWarningText("Wrong Direction!");
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
