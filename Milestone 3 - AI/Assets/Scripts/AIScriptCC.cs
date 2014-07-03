using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class AIScriptCC : MonoBehaviour
{
	#region variables
	#region cached variables
	CharacterController _controller;
	Transform _transform;
	Transform player;
	Transform _eyes;
	#endregion
	#region movement variables
	public float speed = 2;
	float gravity = 100;
	Vector3 moveDirection;
	float maxRotSpeed = 200.0f;
	float minTime = 0.1f;
	float velocity;
	public float range = 2.5f;
	public float attackRange = 50.0f;
	bool isCorouting;
	#endregion
	#region waypoint variables
	int index;
	public string strTag;
	Dictionary<int, Transform> waypoint = new Dictionary<int, Transform>();
	#endregion
	#region delegate variable
	delegate void DelFunc();
	delegate IEnumerator DelEnum();
	DelFunc delFunc;
	DelEnum delEnum;
	bool del;
	#endregion
	#endregion
	
	Vector3 playerOffset = new Vector3(0, 1.8f, 0);
	string stateText = "";

	bool seenAround;
	int layerMask = 1 << 8;
	
	void Start()
	{
		_controller = GetComponent<CharacterController>();
		_transform = GetComponent<Transform>();
		_eyes = transform.Find ("Eyes");
		player = GameObject.Find("Player").GetComponent<Transform>();
		if (player == null)
			Debug.LogError("No player on scene");
		if (string.IsNullOrEmpty(strTag)) 
			Debug.LogError("No waypoint tag given");
		
		index = 0;
		
		GameObject[] gos = GameObject.FindGameObjectsWithTag(strTag);
		foreach (GameObject go in gos)
		{
			WaypointScript script = go.GetComponent<WaypointScript>();
			waypoint.Add(script.index, go.transform);
		}
		
		delFunc = this.Walk;
		delEnum = null;
		del = true;
		stateText = "Walk";

		isCorouting = false;
		seenAround = false;
		layerMask = ~layerMask;
	}
	
	void Update()
	{
		if (AIFunction() && isCorouting)
		{
			if (isCorouting)
				StopAllCoroutines();
			del = true;
			//Debug.Log(1);
		}
		if (del)
			delFunc();
		else if (!isCorouting)
		{
			isCorouting = true;
			StartCoroutine(delEnum());
		}
	}
	
	#region movement functions
	void Move(Transform target)
	{
		//Movements
		moveDirection = _transform.forward;
		moveDirection *= speed;
		moveDirection.y -= gravity * Time.deltaTime;
		_controller.Move(moveDirection * Time.deltaTime);
		//Rotation
		var newRotation = Quaternion.LookRotation(target.position - _transform.position).eulerAngles;
		var angles = _transform.rotation.eulerAngles;
		_transform.rotation = Quaternion.Euler(angles.x,
		                                       Mathf.SmoothDampAngle(angles.y, newRotation.y, ref velocity, minTime, maxRotSpeed), angles.z);
	}
	
	void NextIndex()
	{
		if (++index == waypoint.Count) index = 0;
	}
	
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
	
	void Attack()
	{
		
		Move(player);
		animation.CrossFade("Run");
		stateText = "Attack";
	}
	
	#endregion
	
	#region animation functions
	
	IEnumerator Wait()
	{
		animation.CrossFade("Idle");
		yield return new WaitForSeconds(2.0f);
		NextIndex();
		del = true;
	}
	#endregion
	
	#region AI function
	bool AIFunction(){
		Vector3 direction = player.position - _transform.position;
		if (direction.sqrMagnitude < attackRange){
			if (seenAround){
				delFunc = this.Attack;
				return true;
			} else{
				if (Vector3.Dot(direction.normalized, _transform.forward) > 0 &&
				    !Physics.Linecast(_eyes.position, player.position + playerOffset, layerMask)){
					delFunc = this.Attack;
					seenAround = true;
					return true;
				}
				return false;
			}
		}else{
			delFunc = this.Walk;
			seenAround = false;
			return false;
		}
	}
	#endregion


	#region GUIFunction
	void OnGUI()
	{
		Vector2 targetPos;
		targetPos = Camera.main.WorldToScreenPoint (_transform.position + playerOffset);
		
		
		GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 60, 20), stateText);
	}
	#endregion
}