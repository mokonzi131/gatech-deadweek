using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIScriptNavMeshCover : MonoBehaviour
{
	#region variables
	#region cached variables
	NavMeshAgent _agent;
	Transform _transform;
	Transform player;
	Transform _eyes;
	#endregion
	#region movement variables
	public float patrolSpeed = 2;
	public float alertSpeed = 3;
	public float runSpeed = 4;
	float gravity = 9.8f;
	Vector3 moveDirection;
	float maxRotSpeed = 200.0f;
	float minTime = 0.1f;
	float velocity;
	public float range = 2.5f;
	bool isCorouting;
	#endregion

	#region attack variables
	public float detectRange = 20.0f;
	public float rangeAttackRange = 15.0f;
	public float meleeAttackRange = 10.0f;
	
	public float rangeAttackRate = 2.0f;
	float lastRangeAttackTime = -10.0f;

	#endregion
	
	#region throw variables
	public float throwPower = 10;
	public GameObject throwable;
	#endregion

	#region hide variables
	public float maxHideDistance = 10.0f;
	Transform hideSpot;
	Transform wayOut;
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
	bool isHiding;
	int layerMask = 1 << 8;
	
	void Start()
	{
		_agent = GetComponent<NavMeshAgent> ();
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
		isHiding = false;
		layerMask = ~layerMask;
	}
	
	void Update()
	{
		if (!isHiding)
		{
			if (AIFunction() && isCorouting)
			{
				if (isCorouting)
					StopAllCoroutines();
				del = true;
				//Debug.Log(1);
			}
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
	void Move(Transform target, float speed)
	{
		_agent.speed = speed;
		_agent.SetDestination (target.position);
		
		//Old movement using character controller
		//		//Movements
		//		moveDirection = _transform.forward;
		//		moveDirection *= speed;
		//		moveDirection.y -= gravity * Time.deltaTime;
		//		_controller.Move(moveDirection * Time.deltaTime);
		//				//Rotation
		//				var newRotation = Quaternion.LookRotation(target.position - _transform.position).eulerAngles;
		//				var angles = _transform.rotation.eulerAngles;
		//				_transform.rotation = Quaternion.Euler(angles.x,
		//				                                       Mathf.SmoothDampAngle(angles.y, newRotation.y, ref velocity, minTime, maxRotSpeed), angles.z);
	}
	
	
	void Walk()
	{
		if ((_transform.position - waypoint[index].position).sqrMagnitude > range)
		{
			Move(waypoint[index], patrolSpeed);
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
				break;
			default:
				NextIndex(); break;
			}
		}
	}
	
	
	void NextIndex()
	{
		if (++index == waypoint.Count) index = 0;
	}
	
	void AttackMelee()
	{
		Move(player, runSpeed);
		animation.CrossFade("Run");
		stateText = "Melee Attack";
	}


	void AttackRange()
	{
		Move (_transform, 0);
		animation.CrossFade("Idle");
		_transform.LookAt (player.position);
		if (Time.time > lastRangeAttackTime + rangeAttackRate)
		{
			lastRangeAttackTime = Time.time;
			Throw ();
		}
		stateText = "Range Attack";
	}
	
	void Throw()
	{
		//Try to predict the throw location using the player's current velocity
		float axeFlyTime = 1.0f; //which is apparently not right
		
		Vector3 targetPos = player.position + new Vector3(0, 0.7f, 0) + player.gameObject.GetComponent<Rigidbody>().velocity * axeFlyTime;
		
		
		Vector3 relativePos = new Vector3();
		relativePos.z = 0;
		relativePos.x = Mathf.Sqrt( (targetPos.x - _eyes.position.x) * (targetPos.x - _eyes.position.x)
		                           + (targetPos.z - _eyes.position.z) * (targetPos.z - _eyes.position.z) );
		relativePos.y = targetPos.y - _eyes.position.y;
		
		
		Vector3 relativeVelocity = ComputeInitialVelocity(throwPower, relativePos, true);
		
		//Debug.Log (relativePos.x.ToString() + " " + relativeVelocity.ToString ());
		
		Vector3 localDirection = targetPos - _eyes.position;
		localDirection.y = 0;
		localDirection = localDirection.normalized;
		Vector3 worldVelocity = new Vector3();
		worldVelocity.y = relativeVelocity.y;
		worldVelocity.x = relativeVelocity.z * localDirection.x;
		worldVelocity.z = relativeVelocity.z * localDirection.z;
		
		
		if (Vector3.Angle(localDirection, transform.forward )<= 90)
		{
			GameObject t1 = Instantiate(throwable, _eyes.position, Quaternion.identity) as GameObject;
			t1.transform.LookAt(targetPos);
			t1.rigidbody.velocity = worldVelocity;
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
	
	void AttackWalkCloser()
	{
		Move (player, alertSpeed);
		animation.CrossFade ("Walk");
		stateText = "Alert";
	}

	
	void HideIn()
	{
		if (Vector3.Distance (_transform.position, hideSpot.position) > range)
		{
			Move(hideSpot, runSpeed);
			animation.CrossFade("Run");
			stateText = "Hide In";
		}
		else
		{
//			Debug.Log("Reach Hide Spot!");
//			delFunc = this.HideOut;

			del = false;
			isCorouting = false;
			delEnum = this.HideWait;
		}

	}

	void HideOut()
	{
		if (Vector3.Distance (_transform.position, wayOut.position) > range)
		{
			Move(wayOut, alertSpeed);
			animation.CrossFade("Walk");
			stateText = "Hide Out";
		}
		else
		{
			isHiding = false;
		}
	}


	#endregion
	
	#region animation functions
	
	IEnumerator Wait()
	{
		stateText = "Idle";
		animation.CrossFade("Idle");
		yield return new WaitForSeconds(2.0f);
		NextIndex();
		del = true;
	}

	IEnumerator HideWait()
	{
		stateText = "Hide Idle";
		animation.CrossFade("Idle");
		yield return new WaitForSeconds(Random.Range(1, 2));
		delFunc = this.HideOut;
		del = true;
	}

	#endregion
	
	#region AI function
	bool AIFunction(){
		Vector3 direction = player.position - _transform.position;
		
		float dist = Vector3.Distance (player.position, _transform.position);
		
		if (dist < meleeAttackRange)
		{
			delFunc = this.AttackMelee;
			seenAround = true;
			return true;
		}
		
		if (dist < rangeAttackRange)
		{
			if (seenAround)
			{
				delFunc = this.AttackRange;
				return true;
			}
			else{
				if (Vector3.Dot(direction.normalized, _transform.forward) > 0 &&
				    !Physics.Linecast(_eyes.position, player.position + playerOffset, layerMask)){
					delFunc = this.AttackWalkCloser;
					seenAround = true;
					return true;
				}
				return false;
			}
		}
		
		if (dist < detectRange)
		{
			if (seenAround)
			{
				delFunc = this.AttackWalkCloser;
				return true;
			}
			else{
				if (Vector3.Dot(direction.normalized, _transform.forward) > 0 &&
				    !Physics.Linecast(_eyes.position, player.position + playerOffset, layerMask)){
					delFunc = this.AttackWalkCloser;
					seenAround = true;
					return true;
				}
				return false;
			}
		}
		
		delFunc = this.Walk;
		seenAround = false;
		return false;
	}
	#endregion
	
	
	#region GUIFunction
	void OnGUI()
	{
		Vector2 targetPos;
		targetPos = Camera.main.WorldToScreenPoint (_transform.position + playerOffset);
		
		GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 80, 20), stateText);
	}
	#endregion

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Axe")
		{
			if (other.gameObject.GetComponent<AxePropertyScript>().isPlayerAxe)
			{
				if (delFunc != this.AttackMelee)
				{
					//Debug.Log("Hit By Player Axe!");
					FindClosetCoverSpot();

					if (hideSpot != null)
					{
						//Debug.Log("Find Hide Spot!");
						isHiding = true;
						delFunc = this.HideIn;
						del = true;
						if(isCorouting)
							StopAllCoroutines();

					}
				}
			}
		}
	}

	void FindClosetCoverSpot()
	{
		var hitColliders = Physics.OverlapSphere (_transform.position, maxHideDistance);

		float minDist = 99999.0f;
		GameObject targetObject = null;

		foreach(var hitCollider in hitColliders)
		{
			if (hitCollider.gameObject.tag == "Cover")
			{
				//Debug.Log("There is a cover here!");
				float newDist = Vector3.Distance(hitCollider.transform.position, _transform.position);
				if(newDist < minDist)
				{
					minDist = newDist;
					targetObject = hitCollider.gameObject;
				}
			}
		}

		if (targetObject == null)
			hideSpot = null;
		else
		{
			targetObject.GetComponent<AdjustCoverPositionScript>().AdjustCoverPosition();
			hideSpot = targetObject.GetComponent<Transform>().Find("Cover").Find("HideSpot");

			float minDist2 = 99999.0f;
			foreach(Transform child in targetObject.GetComponent<Transform>().Find("Cover"))
			{
				if (child.gameObject.name == "Wayout")
				{
					//Debug.Log("Find one wayout!");
					float newDist2 = Vector3.Distance(child.position, _transform.position);
					if(newDist2 < minDist2)
					{
						minDist2 = newDist2;
						wayOut = child;
					}
				}

			}
		}
	}
}