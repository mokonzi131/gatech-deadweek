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

public class ThrowScript : MonoBehaviour {
	
	public GameObject throwable;
	//private Inventory inventory;	

	public Transform startPoint;

	public float power = 15.0f;
	
	float gravity = 9.8f;
	
	int layerMask;
	
	void Start(){
		//throwable = GameObject.FindWithTag ("Book");
		//inventory = GameObject.FindWithTag ("GameController").GetComponent<Inventory>();
		
		layerMask = 1 << 8;
		layerMask = ~layerMask;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown(KeyCode.T))
		{
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			
			Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
			//Debug.DrawRay(target.transform.position, rayDirection);
			
			RaycastHit hit;
			
			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				Vector3 relativePos = new Vector3();
				relativePos.z = 0;
				relativePos.x = Mathf.Sqrt( (hit.point.x - startPoint.position.x) * (hit.point.x - startPoint.position.x)
				                           + (hit.point.z - startPoint.position.z) * (hit.point.z - startPoint.position.z) );
				relativePos.y = hit.point.y - startPoint.position.y;
				
				
				Vector3 relativeVelocity = ComputeInitialVelocity(power, relativePos, true);
				
				Vector3 localDirection = hit.point - startPoint.position;
				localDirection.y = 0;
				localDirection = localDirection.normalized;
				Vector3 worldVelocity = new Vector3();
				worldVelocity.y = relativeVelocity.y;
				worldVelocity.x = relativeVelocity.z * localDirection.x;
				worldVelocity.z = relativeVelocity.z * localDirection.z;
				
				//Debug.Log(relativeVelocity.ToString());
				
				if (Vector3.Angle(localDirection, transform.forward )<= 90)
				{
					
					GameObject t1 = Instantiate(throwable, startPoint.position, Quaternion.identity) as GameObject;
					t1.gameObject.GetComponent<AxePropertyScript>().isPlayerAxe = true;
					t1.transform.LookAt(hit.point);
					t1.rigidbody.velocity = worldVelocity;

//					if (inventory.remove(Inventory.ItemCategory.BOOK))
//					{
//						GameObject book = Instantiate(throwable, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
//						book.rigidbody.velocity = worldVelocity;
//					}
				}
			}
			
		}
	}
	
	Vector3 ComputeInitialVelocity (float speed, Vector3 target, bool smallerAngle)
	{
		float temp = Mathf.Pow(speed, 4) - gravity*(gravity*target.x*target.x+2*target.y*speed*speed);
		
		// no real solution, return 45 degrees
		if(temp < 0)
		{
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
		
		
		return new Vector3(0,Mathf.Sin(angle)*speed,Mathf.Cos(angle)*speed);
		
	}
	
	
	
}
