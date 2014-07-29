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
using System.Collections.Generic;

public class ZombieScript1 : MonoBehaviour
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
	public float range = 2.0f;
	bool isCorouting;
	#endregion
	
	#region attack variables
	public float detectRange = 20.0f;
	public float meleeAttackRange = 10.0f;

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

	#region attraction variable
	bool isAttracted;
	Transform attractPos;
	float lastAttractedTime = -20.0f;
	public float attractTimeout = 15.0f;

	#endregion

	#endregion
	
	Vector3 playerOffset = new Vector3(0, 1.8f, 0);
	string stateText = "";
	
	bool seenAround;

	int layerMask = 1 << 8;

	Animator _animator;
	private Vector3 lastPosition;
	private int updateAnim;

	void Start()
	{
		_agent = GetComponent<NavMeshAgent> ();
		_transform = GetComponent<Transform>();
		_eyes = transform.Find ("Eyes");
		_animator = GetComponent<Animator> ();
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
		isAttracted = false;
		layerMask = ~layerMask;
	}
	
	void Update()
	{
		CheckAttraction ();
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
		Transform enemyDot = this.transform.FindChild("EnemyMarker");
		Vector3 enemyPos = transform.position;
		enemyPos += Vector3.up * 10.0f;
		enemyDot.transform.position = enemyPos;
	}

	void LateUpdate()
	{
		if(updateAnim == 0)
		{
			_animator.SetFloat ("linearSpeed", (lastPosition - transform.position).magnitude/Time.deltaTime/5.0f);
			//Debug.Log ("velocity = " + (lastPosition - target.transform.position).magnitude/Time.deltaTime/5.0f);
			lastPosition = transform.position;
		}
		updateAnim = (updateAnim + 1) % 5;
	}
	
	#region movement functions
	void Move(Transform target, float speed)
	{
		_agent.speed = speed;
		_agent.SetDestination (target.position-transform.forward*.5f);
//		_animator.SetFloat ("linearSpeed", speed);
//		Debug.Log ("linearSpeed = " + speed);
	}
	
	
	void Walk()
	{
		if (Vector3.Distance(_transform.position, waypoint[index].position) > range)
		{
			Move(waypoint[index], patrolSpeed);
			//animation.CrossFade("Walk");
			stateText = "Walk";
		}
		else
		{
			switch (index)
			{
			case 0:
			case 1:
			case 2:
			case 3:
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
		//animation.CrossFade("Run");
		stateText = "Melee Attack";
		if (Vector3.Distance(_transform.position, player.position) < 1.0f)
			player.gameObject.GetComponent<IsAttackedScript>().isMeleeAttacked();
	}

	void AttackWalkCloser()
	{
		Move (player, alertSpeed);
		//animation.CrossFade ("Walk");
		stateText = "Alert";
	}

	void AttractWalkCloser()
	{
		Vector3 relativeDist = _transform.position - attractPos.position;
		relativeDist.y = 0;
		if (relativeDist.magnitude > 1.0f)
		{
			Move (attractPos, patrolSpeed);
			//animation.CrossFade ("Walk");
			stateText = "Attract Walk";
		}
		else
		{
			delFunc = this.AttractIdle;
			del = true;
		}
	}
	
	void AttractIdle()
	{
		Move (_transform, 0);
		Vector3 aPos = attractPos.position;
		aPos.y = _transform.position.y;
		_transform.LookAt (aPos);
		//animation.CrossFade ("Idle");
		stateText = "Attract Idle";
		//_animator.SetFloat ("linearSpeed", 0.0f);
	}
	

	
	#endregion
	
	#region delenum functions
	
	IEnumerator Wait()
	{
		stateText = "Idle";
		//animation.CrossFade("Idle");
		yield return new WaitForSeconds(2.0f);
		NextIndex();
		del = true;
	}

	
	#endregion
	
	#region AI function
	bool AIFunction(){

		if (isAttracted)
		{
			if (delFunc != this.AttractWalkCloser && delFunc != this.AttractIdle)
			{
				delFunc = this.AttractWalkCloser;
				del = true;
				seenAround = false;
			}
			return false;
		}

		//Debug.Log ((delFunc).ToString ());
		Vector3 direction = (player.position - _transform.position).normalized;
		
		float dist = Vector3.Distance (player.position, _transform.position);
		//Debug.Log ("distance to player = " + dist);
		
		bool isWithinSight = (Vector3.Dot(direction, _transform.forward) > 0);
		bool isBlocked = Physics.Linecast (_eyes.position, player.position + playerOffset, layerMask);
		
		if (dist < meleeAttackRange)
		{
			if (seenAround || !isBlocked)
			{
				delFunc = this.AttackMelee;
				del = true;
				seenAround = true;
				return true;
			}
			//Debug.Log ("Melee Attack!");
			return false;
		}

		
		if (dist < detectRange)
		{
			if (seenAround)
			{
				delFunc = this.AttackWalkCloser;
				del = true;
				return true;
			}
			else{
				if (isWithinSight && !isBlocked){
					delFunc = this.AttackWalkCloser;
					seenAround = true;
					del = true;
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
	
	
//	#region GUIFunction
//	void OnGUI()
//	{
//		Vector2 targetPos;
//		targetPos = Camera.main.WorldToScreenPoint (_transform.position + playerOffset);
//		
//		GUI.Box(new Rect(targetPos.x, Screen.height - targetPos.y, 80, 20), stateText);
//	}
//	#endregion


	#region AttractionFunction

	void CheckAttraction()
	{
		var hitColliders = Physics.OverlapSphere (_transform.position, 5);

		isAttracted = false;
		
		foreach(var hitCollider in hitColliders)
		{
			if (hitCollider.gameObject.tag == "Book")
			{
				if (!Physics.Linecast (_eyes.position, hitCollider.gameObject.transform.position + new Vector3(0, 0.3f, 0), layerMask))
					if (hitCollider.gameObject.GetComponent<BookPropertyScript>().isJustThrowed)
					{
						isAttracted = true;
						attractPos = hitCollider.gameObject.transform;
						break;
					}
			}
		}

	}



	#endregion

}