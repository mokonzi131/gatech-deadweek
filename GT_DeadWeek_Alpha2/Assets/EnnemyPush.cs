using UnityEngine;
using System.Collections;

public class EnnemyPush : MonoBehaviour {
	
	Animator _animator;
	private GameObject target;
	public float pushRange = 2.0f;
	float timeAtLastPush;

	// Use this for initialization
	void Start () {
		_animator = gameObject.GetComponent<Animator> ();
		target = GameObject.FindWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButton ("Push") && Time.time - timeAtLastPush > 1.0f)
		{
			timeAtLastPush = Time.time;
			var hitColliders = Physics.OverlapSphere (transform.position, pushRange);
			
			float minAngle = 180.0f;
			GameObject targetObject = null;

			foreach(var hitCollider in hitColliders)
			{
				if (hitCollider.gameObject.tag == "Enemy")
				{
					Vector3 objectDirection = hitCollider.transform.position - target.transform.position;
					objectDirection.y = 0;
					
					float newAngle = Vector3.Angle(target.transform.forward, objectDirection);
					
					//Debug.Log("There is a cover here!");
					if(newAngle < minAngle)
					{
						minAngle = newAngle;
						targetObject = hitCollider.gameObject;
					}

					if (minAngle < 50)
					{
						_animator.SetTrigger("pushTrigger");

						
						Animator zombieAnimator;
						zombieAnimator= hitCollider.gameObject.GetComponent<Animator>();
						zombieAnimator.SetTrigger("isPushed");

						ZombieScript1 zombieScript = hitCollider.gameObject.GetComponent<ZombieScript1>();
						zombieScript.isPushed = true;

						Debug.Log("pushing Zombie");
						
					}
					
				}
			}


		}
	}
}
