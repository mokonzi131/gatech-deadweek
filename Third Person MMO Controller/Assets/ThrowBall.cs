using UnityEngine;
using System.Collections;

public class ThrowBall : MonoBehaviour {

	public GameObject ball;



	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T))
		{


			Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString());

			Vector3 forwardDirection = gameObject.transform.TransformDirection(Vector3.forward);
			Vector3 upDirection = gameObject.transform.TransformDirection(Vector3.up);

			GameObject tBall = Instantiate(ball, transform.position + forwardDirection * 1, Quaternion.identity) as GameObject;


			tBall.rigidbody.AddForce((forwardDirection + upDirection) * 5, ForceMode.VelocityChange);

		}
	}
}
