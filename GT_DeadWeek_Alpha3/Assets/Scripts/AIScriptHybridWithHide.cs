using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]
public class AIScriptHybridWithHide : MonoBehaviour
{
	#region variables
	#region cached variables
	CharacterController _controller;
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
	public float range = 2.0f;
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
	List<Vector3>[] paths ;
	float cellSize;
	int idxInPath = 0 ;
	bool backOnTrack = true ;
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
		_controller = GetComponent<CharacterController>();
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

		// We precompute all the path for the patrol
		paths = new List<Vector3>[waypoint.Count];
		Astar pathPlanningScript = GameObject.FindWithTag("Terrain").GetComponent<Astar>();
		cellSize = pathPlanningScript.cellSize;
		{
			List<Vector3> path;
			pathPlanningScript.getPath (waypoint [waypoint.Count - 1].position, waypoint [0].position, out path);
			paths [0] = path;
		}
		for (int i=1 ; i<waypoint.Count ; ++i){
			List<Vector3> path;
			pathPlanningScript.getPath(waypoint[i-1].position, waypoint[i].position, out path);
			paths[i] = path ;
		}
		
		delFunc = this.Walk;
		delEnum = null;
		del = true;
		stateText = "Walk";
		
		isCorouting = false;
		seenAround = false;
		isHiding = false;
		backOnTrack = true;
		layerMask = ~layerMask;
	}
	
	void Update()
	{
		if (AIFunction() && isCorouting)
		{
			StopAllCoroutines();
			del = true;
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

	}

	void MoveAstar(int idx, float speed)
	{
		Vector3 nextPos = paths [idx] [idxInPath];
		if (idxInPath<paths[idx].Count-1 && Vector3.Distance(_transform.position, nextPos) < cellSize) {
			//Debug.Log (strTag + " ++idxInPath");
			++idxInPath;
			nextPos = paths [idx] [idxInPath];
		}
//		else {
//			Debug.Log (strTag + " " + Vector3.Distance(_transform.position, nextPos));
//		}
		//Movements
		moveDirection = _transform.forward;
		moveDirection *= speed;
		moveDirection.y -= gravity * Time.deltaTime;
		_controller.Move(moveDirection * Time.deltaTime);
		//Rotation
		var newRotation = Quaternion.LookRotation(nextPos - _transform.position).eulerAngles;
		var angles = _transform.rotation.eulerAngles;
		_transform.rotation = Quaternion.Euler(angles.x,
		                                       Mathf.SmoothDampAngle(angles.y, newRotation.y, ref velocity, minTime, maxRotSpeed), angles.z);
	}
	
	void Walk()
	{
		if (!backOnTrack) {
			Move (waypoint[index], patrolSpeed);
			animation.CrossFade("Walk");
			stateText = "Walk";
			if(Vector3.Distance(_transform.position, waypoint[index].position) < range){
				backOnTrack = true;
				NextIndex();
			}
		}
		else if (Vector3.Distance(_transform.position, waypoint[index].position) > range)
		{
			//Move(waypoint[index], patrolSpeed);
			MoveAstar(index, patrolSpeed);
			animation.CrossFade("Walk");
			stateText = "Walk";
		}
		else
		{
			//Debug.Log(strTag + " else");
			if(index % 5 == 0){
				del = false;
				isCorouting = false;
				delEnum = this.Wait;
			} else {
				NextIndex();
			}
//			switch (index)
//			{
//			case 0:
//			case 5:
//				del = false;
//				isCorouting = false;
//				delEnum = this.Wait;
//				break;
//			default:
//				NextIndex(); break;
//			}
		}
	}
	
	
	void NextIndex()
	{
		if (++index == waypoint.Count) index = 0;
		idxInPath = 0;
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
			del = false;
			isCorouting = false;
			delEnum = this.HideWait;
		}
		
	}
	
	void HideOut()
	{
		if (Vector3.Distance (_transform.position, wayOut.position) > range)
		{
			Move(wayOut, runSpeed);
			animation.CrossFade("Run");
			stateText = "Hide Out";
		}
		else
		{
			del = true;
			delFunc = this.AttackWalkCloser;
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
		yield return new WaitForSeconds(Random.Range(1, 3));
		delFunc = this.HideOut;
		del = true;
	}
	
	#endregion
	
	#region AI function
	bool AIFunction(){

		//Debug.Log ((delFunc).ToString ());
		Vector3 direction = (player.position - _transform.position).normalized;
		
		float dist = Vector3.Distance (player.position, _transform.position);

		bool isWithinSight = (Vector3.Dot(direction, _transform.forward) > 0);
		bool isBlocked = Physics.Linecast (_eyes.position, player.position + playerOffset, layerMask);

		if (dist < meleeAttackRange)
		{
			if (!del || delFunc != this.HideIn)
			{
				if (seenAround)
				{
					delFunc = this.AttackMelee;
					backOnTrack = false;
					index = 0;
					del = true;
					return true;
				}
				else if (!isBlocked)
				{
					delFunc = this.AttackMelee;
					backOnTrack = false;
					index = 0;
					del = true;
					seenAround = true;
					return true;
				}
				else
				{
					return false;
				}
			}
			else
				return false;
		}
		
		if (dist < rangeAttackRange)
		{
			if ((delFunc != this.HideIn && delFunc != this.HideOut && del) || (delEnum != this.HideWait && !del))
			{
				if (seenAround)
				{
					delFunc = this.AttackRange;
					backOnTrack = false;
					index = 0;
					del = true;
					return true;
				}
				else{
					if (isWithinSight && !isBlocked){
						delFunc = this.AttackRange;
						backOnTrack = false;
						index = 0;
						del = true;
						seenAround = true;
						return true;
					}
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		
		if (dist < detectRange)
		{
			if ((delFunc != this.HideIn && delFunc != this.HideOut && del) || (delEnum != this.HideWait && !del))
			{
				if (seenAround)
				{
					delFunc = this.AttackWalkCloser;
					backOnTrack = false;
					index = 0;
					del = true;
					return true;
				}
				else{
					if (isWithinSight && !isBlocked){
						delFunc = this.AttackWalkCloser;
						backOnTrack = false;
						index = 0;
						seenAround = true;
						del = true;
						return true;
					}
					return false;
				}
			}
			else
				return false;
		}

		Move (_transform, 0);
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