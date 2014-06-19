using UnityEngine;
using System.Collections;

public class hitSound : MonoBehaviour {
	public AudioClip clip ;

	void OnCollisionEnter(Collision collision) {
		if (collision.relativeVelocity.magnitude > 3) {
			float relativeNormalVelocity = Mathf.Abs( Vector3.Dot (collision.relativeVelocity, collision.contacts[0].normal)) ;
			AudioSource.PlayClipAtPoint (clip, collision.contacts[0].point, relativeNormalVelocity / 10.0f) ;
		}
	}
}
