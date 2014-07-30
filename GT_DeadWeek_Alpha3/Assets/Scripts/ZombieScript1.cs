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
	GameManager gameManager;

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

	private float timeBeingAttacked = 0.0f;

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
	
	public AudioClip groan;
	public float groanVolume;
	#endregion
	
	Vector3 playerOffset = new Vector3(0, 1.8f, 0);
	string stateText = "";
	
	bool seenAround;

	public bool isPushed = false;

	public LayerMask layerMask;

	Animator _animator;
	private Vector3 lastPosition;
	private int updateAnim;

	void Start()
	{
		gameManager = GameObject.FindWithTag ("GameController").GetComponent<GameManager> ();
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
		//seenAround = false;
		LosePlayer ();
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
		playSounds ();
		
	}
	
	// Plays a zombie sound based on the input parameter
	void playSounds(){
		if (seenAround)
		{
			if (!audio.isPlaying)
			{
				audio.clip = groan;
				audio.volume = groanVolume;
				audio.Play();
			}
		}
		else
		{
			audio.clip = null;
		}
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
		_animator.SetBool ("attack", false);

		Move(player, runSpeed);
		//animation.CrossFade("Run");
		stateText = "Melee Attack";

	}

	void AttackWalkCloser()
	{
		
		_animator.SetBool ("attack", false);

		Move (player, alertSpeed);
		//animation.CrossFade ("Walk");
		stateText = "Alert";
	}

	void AttractWalkCloser()
	{
		
		_animator.SetBool ("attack", false);

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
		
		_animator.SetBool ("attack", false);

		Move (_transform, 0);
		Vector3 aPos = attractPos.position;
		aPos.y = _transform.position.y;
		_transform.LookAt (aPos);
		//animation.CrossFade ("Idle");
		stateText = "Attract Idle";
		//_animator.SetFloat ("linearSpeed", 0.0f);
	}
	
	void CloseAttack()
	{
		Debug.Log ("close attack");
		_animator.SetBool ("attack", true); 
		stateText = "One on One attach";
		
		if(timeBeingAttacked>1.5f)
		{
			// injuries
			Debug.Log("injuries : " + timeBeingAttacked);
			player.gameObject.GetComponent<PlayerController>().enemiesShootRef.gameObject.GetComponent<PlayerDamageControl>().isMeleeAttacked();
			timeBeingAttacked = 0.0f;
		}
		timeBeingAttacked += Time.deltaTime;
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

	IEnumerator pushWait()
	{
		Debug.Log ("being pushed!");
		_animator.SetBool ("attack", false);
		stateText = "Idle";
		//animation.CrossFade("Idle");
		yield return new WaitForSeconds(4.0f);
		del = true;
		isPushed = false;
	}
	
	#endregion
	
	#region AI function
	bool AIFunction(){

		if(isPushed)
		{	
			delEnum = pushWait;
			del = false;
			isCorouting = false;
			return false;
		}

		if (isAttracted)
		{
			if (delFunc != this.AttractWalkCloser && delFunc != this.AttractIdle)
			{
				delFunc = this.AttractWalkCloser;
				del = true;
				//seenAround = false;
				LosePlayer();
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
				
				if (Vector3.Distance(_transform.position, player.position) > 1.2f)
				{
					delFunc = this.AttackMelee;
					del = true;
					//seenAround = true;
					SeePlayer();
					return true;
				}
				else
				{
					delFunc = this.CloseAttack;
					del = true;
					//seenAround = true;
					SeePlayer();
					return true;
				}
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
					//seenAround = true;
					SeePlayer();
					del = true;
					return true;
				}
				return false;
			}
		}
		
		delFunc = this.Walk;
		//seenAround = false;
		LosePlayer();
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

	
	void SeePlayer()
	{

		if (!seenAround)
		{
			gameManager.countAttackingZombie ++;
		}

		seenAround = true;
		transform.Find ("EnemyMark").Find ("StaticMark").gameObject.SetActive (false);
		transform.Find ("EnemyMark").Find ("DynamicMark").gameObject.SetActive (true);
		
	}
	void LosePlayer()
	{
		if (seenAround)
		{
			gameManager.countAttackingZombie --;
			if (gameManager.countAttackingZombie < 0)
				gameManager.countAttackingZombie = 0;
		}
		seenAround = false;
		transform.Find ("EnemyMark").Find ("StaticMark").gameObject.SetActive (true);
		transform.Find ("EnemyMark").Find ("DynamicMark").gameObject.SetActive (false);
		
	}

}