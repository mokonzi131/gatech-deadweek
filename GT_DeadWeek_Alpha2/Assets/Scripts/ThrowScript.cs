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

public class ThrowScript : MonoBehaviour {
	
	public GameObject throwable;
	private Inventory inventory;	

	public Transform eyePoint;

	public float power = 15.0f;
	
	float gravity = 9.8f;
	
	public LayerMask ignoreLayer;

	Vector3 startPoint;

	void Start(){
		//throwable = GameObject.FindWithTag ("Book");
		inventory = GameObject.FindWithTag ("GameController").GetComponent<Inventory>();

		ignoreLayer = ~ignoreLayer;

		startPoint = new Vector3 ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown("Throw"))
		{

			startPoint = eyePoint.position + transform.forward * 0.2f;


			if (gameObject.GetComponent<PlayerController>().aim)
			{
				Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

				
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreLayer))
				{
					Vector3 relativePos = new Vector3();
					relativePos.z = 0;
					relativePos.x = Mathf.Sqrt( (hit.point.x - startPoint.x) * (hit.point.x - startPoint.x)
					                           + (hit.point.z - startPoint.z) * (hit.point.z - startPoint.z) );
					relativePos.y = hit.point.y - startPoint.y;
					
					bool reachable = true;
					Vector3 relativeVelocity = ComputeInitialVelocity(power, relativePos, true, ref reachable);
					
					Vector3 localDirection = hit.point - startPoint;
					localDirection.y = 0;
					localDirection = localDirection.normalized;
					Vector3 worldVelocity = new Vector3();
					worldVelocity.y = relativeVelocity.y;
					worldVelocity.x = relativeVelocity.z * localDirection.x;
					worldVelocity.z = relativeVelocity.z * localDirection.z;
					
					//Debug.Log(relativeVelocity.ToString());
					
					if (Vector3.Angle(localDirection, transform.forward )<= 90)
					{
						if (inventory.remove(Inventory.ItemCategory.BOOK))
						{
							GameObject book = Instantiate(throwable, startPoint, Quaternion.identity) as GameObject;
							book.gameObject.GetComponent<BookPropertyScript>().BeingThrowed();
							book.transform.LookAt(hit.point);
							book.rigidbody.velocity = worldVelocity;
						}
					}
				}
			}
			else
			{
				if (inventory.remove(Inventory.ItemCategory.BOOK))
				{
					GameObject book = Instantiate(throwable, startPoint, Quaternion.identity) as GameObject;
					book.gameObject.GetComponent<BookPropertyScript>().BeingThrowed();
					book.transform.LookAt(transform.position);
				}
			}


			
		}
	}
	
	public Vector3 ComputeInitialVelocity (float speed, Vector3 target, bool smallerAngle, ref bool reachable)
	{
		float temp = Mathf.Pow(speed, 4) - gravity*(gravity*target.x*target.x+2*target.y*speed*speed);
		
		// no real solution, return 45 degrees
		if(temp < 0)
		{
			reachable = false;
			return new Vector3(0,Mathf.Sin(45*Mathf.Deg2Rad)*speed,Mathf.Cos(45*Mathf.Deg2Rad)*speed);
		}
		
		temp = Mathf.Sqrt (temp);
		float angle;
		if(smallerAngle)
		{
			angle = Mathf.Atan((speed*speed - temp)/(gravity*target.x));
		}
		else
		{
			angle = Mathf.Atan((speed*speed + temp)/(gravity*target.x));
		}
		
		reachable = true;
		return new Vector3(0,Mathf.Sin(angle)*speed,Mathf.Cos(angle)*speed);
		
	}
}
