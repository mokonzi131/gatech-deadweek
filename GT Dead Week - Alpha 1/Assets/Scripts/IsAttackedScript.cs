using UnityEngine;
using System.Collections;

public class IsAttackedScript : MonoBehaviour {

	public bool isSlowed;

	public float meleeAttackTimeout = 1.0f;
	public float rangeAttackTimeout = 5.0f;

	float lastMeleeAttackTimeout = -10.0f;
	float lastRangeAttackTimeout = -10.0f;

	// Use this for initialization
	void Start () {
		isSlowed = false;
	}
	
	// Update is called once per frame
	void Update () {
		CheckAttackedStatus ();
	
	}

	public void isMeleeAttacked()
	{
		isSlowed = true;
		lastMeleeAttackTimeout = Time.time;

	}

	public void isRangeAttacked()
	{
		isSlowed = true;
		lastRangeAttackTimeout = Time.time;
	}

	void CheckAttackedStatus()
	{
		if (lastMeleeAttackTimeout + meleeAttackTimeout < Time.time)
		{
			if (lastRangeAttackTimeout + rangeAttackTimeout < Time.time)
				isSlowed = false;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Axe")
			isRangeAttacked();

	}


}
