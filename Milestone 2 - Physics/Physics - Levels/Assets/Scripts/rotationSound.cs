using UnityEngine;
using System.Collections;

public class rotationSound : MonoBehaviour {
	
	public float speedThreshold = 0.5f;
	private bool isPlaying = false ;

	void Update () {
		if (transform.rigidbody.angularVelocity.magnitude > speedThreshold && isPlaying == false) {
			audio.Play();
			isPlaying = true ;
		} else if (transform.rigidbody.angularVelocity.magnitude < speedThreshold) {
			audio.Stop();
			isPlaying = false ;
		}
	}
}
