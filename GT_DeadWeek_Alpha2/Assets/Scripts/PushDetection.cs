using UnityEngine;
using System.Collections;

public class PushDetection : MonoBehaviour {

	Animator _animator;


	// Use this for initialization
	void Start () {
		_animator = transform.parent.parent.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("DETECTION");
		Debug.Log (other.gameObject.tag);
		//if(other.gameObject.tag == "HandL" || other.gameObject.tag == "HandR")
		//{
			_animator.SetTrigger("isPushed");
			Debug.Log("reaction");
		//}
	}
}
