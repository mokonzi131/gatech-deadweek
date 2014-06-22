using UnityEngine;
using System.Collections;

public class PickupAction : MonoBehaviour {

	
	public Rigidbody target;

	public GUIText warningText;

	public float warningTextTimeout = 1.0f;
	float lastWarningTextTime = -10.0f;

	
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
				if (hit.collider.gameObject.tag == "Pickup")
				{
					//Debug.Log("Hit Pickup!");
					
					Vector3 objectPos = hit.collider.transform.position;
					
					if (Vector3.Distance(target.transform.position, objectPos) < 2)
					{
						if (Vector3.Angle(target.transform.forward, objectPos - target.transform.position) < 60)
						{
							Debug.Log("Pick Up!");
							UpdateWarningText("Pick Up!");
							Destroy(hit.collider.gameObject);
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
