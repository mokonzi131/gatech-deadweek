using UnityEngine;
using System.Collections;

public class PickupAction : MonoBehaviour {

	
	private Rigidbody target;

	public GUIText warningText;

	public float warningTextTimeout = 1.0f;
	float lastWarningTextTime = -10.0f;

	void Start() {
		target = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {

		if (lastWarningTextTime + warningTextTimeout < Time.time)
			ClearWarningText();

		if (Input.GetKeyDown(KeyCode.F))
		{
			//Debug.Log(Input.mousePosition);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
			//Debug.DrawRay(target.transform.position, rayDirection);
			
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.gameObject.tag == "Book" || hit.collider.gameObject.tag == "Drink" || hit.collider.gameObject.tag == "Food")
				{
					//Debug.Log("Hit Pickup!");
					
					Vector3 objectPos = hit.collider.transform.position;
					
					if (Vector3.Distance(target.transform.position, objectPos) < 2)
					{
						if (Vector3.Angle(target.transform.forward, objectPos - target.transform.position) < 80)
						{
							Inventory inventory = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
							Inventory.ItemCategory c = Inventory.ItemCategory.FOOD ;
							if (hit.collider.gameObject.tag == "Book")
								c = Inventory.ItemCategory.BOOK;
							float mass = hit.collider.gameObject.rigidbody.mass ;
							if (hit.collider.gameObject.tag == "Drink")
								mass /= 10 ;
							if(inventory.addItem(c, new Item(mass, hit.collider.gameObject.name))){
								Debug.Log("Pick Up!");
								UpdateWarningText("Pick Up!");
								if(hit.collider.gameObject.tag != "Drink")
									hit.collider.gameObject.SetActive(false);
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
					else
					{
						Debug.Log("Too Far Away!");
						UpdateWarningText("Too Far Away!");
					}
					
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
