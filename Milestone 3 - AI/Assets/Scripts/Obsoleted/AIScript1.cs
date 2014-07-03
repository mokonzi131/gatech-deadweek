using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class AIScript1 : MonoBehaviour
{
	// Cached variables
	CharacterController _controller;
	Transform _transform;
	Transform _player;

	//Movement variables
	public float speed = 2f;
	float gravity = 20f;
	Vector3 moveDirection;
	float maxRotSpeed = 200.0f;
	float minTime = 0.1f;
	float velocity;
	float range;
	
	//Waypoint variables
	public string strTag;
	Dictionary<int, Transform> waypoint = new Dictionary<int, Transform>();
	int index;
	
	//Added variables
	bool isCorouting;
	bool del;
	delegate void DelFunc();
	delegate IEnumerator DelEnum();
	DelFunc delFunc;
	DelEnum delEnum;

	// GUI box over enemy
	public Vector3 GUIoffset = new Vector3(0, 1.5f, 0);
	string stateText = "";
	
	void Start()
	{


		if(string.IsNullOrEmpty(strTag))Debug.LogError("No waypoint tag given");
		index = 0;
		_controller = GetComponent<CharacterController>();
		_transform = GetComponent<Transform>();

		_player = GameObject.Find ("Player").GetComponent<Transform> ();
		if (_player == null) Debug.LogError ("No player on scene");

		range = 4;
		GameObject[] gos = GameObject.FindGameObjectsWithTag(strTag);
		foreach (GameObject go in gos)
		{
			WaypointScript script = go.GetComponent<WaypointScript>();
			waypoint.Add(script.index, go.transform);
		}
		//animation["victory"].wrapMode = WrapMode.Once;
		
		//Added codes
		delFunc = this.Walk;
		stateText = "Walk";
		delEnum = null;
		del = true;
		isCorouting = false;
	}
	
	//Modified update
	void Update()
	{
		if (del)
			delFunc();
		else if (!isCorouting)
		{
			isCorouting = true;
			StartCoroutine(delEnum());
		}
	}
	
	//Modified Walk
	void Walk()
	{
		if ((_transform.position - waypoint[index].position).sqrMagnitude > range)
		{
			Move(waypoint[index]);
			animation.CrossFade("Walk");
			stateText = "Walk";
		}
		else
		{
			switch (index)
			{
			case 0:
			case 5:
				del = false;
				isCorouting = false;
				delEnum = this.Wait;
				stateText = "Idle";
				break;
			default:
				NextIndex(); break;
			}
		}
	}
	
	void Move(Transform target)
	{
		// Movement
		moveDirection = _transform.forward;
		moveDirection *= speed;
		moveDirection.y -= gravity * Time.deltaTime;
		_controller.Move(moveDirection * Time.deltaTime);
		// Rotation
		var newRotation = Quaternion.LookRotation(target.position - _transform.position).eulerAngles;
		var angles = _transform.rotation.eulerAngles;
		_transform.rotation = Quaternion.Euler(angles.x,
		                                       Mathf.SmoothDampAngle(angles.y, newRotation.y, ref velocity, minTime, maxRotSpeed), angles.z);
	}
	
	void NextIndex()
	{
		if (++index == waypoint.Count) index = 0;
	}
	
	//Modified animation functions
//	IEnumerator Victory()
//	{
//		if (!animation.IsPlaying("victory")) animation.CrossFade("victory");
//		yield return new WaitForSeconds(animation["victory"].length);
//		NextIndex();
//		del = true;
//	}
	IEnumerator Wait()
	{
		animation.CrossFade("Idle");
		yield return new WaitForSeconds(2.0f);
		NextIndex();
		del = true;
	}
	
	
	
	void OnGUI()
	{
		Vector2 targetPos;
		targetPos = Camera.main.WorldToScreenPoint (_transform.position + GUIoffset);


		GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), stateText);
	}


}