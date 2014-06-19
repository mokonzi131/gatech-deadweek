using UnityEngine;
using System.Collections;

public class PhysicalWindZone : MonoBehaviour {
	public float wind_force;

	void Start() {
		ParticleEmitter dustEmit = transform.GetComponentInChildren<ParticleEmitter> ();
		dustEmit.localVelocity = wind_force * Vector3.right;
		dustEmit.minEmission = 5*wind_force;
		dustEmit.maxEmission = 7*wind_force;
		dustEmit.minEnergy = transform.localScale.x / wind_force;
		dustEmit.maxEnergy = dustEmit.minEnergy;
	}

	void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody) {
			Vector3 force = transform.right * wind_force ;
			/*if(other.tag == "Player")
				force = Vector3.Dot (force, other.transform.right) * other.transform.right ;*/
			other.attachedRigidbody.AddForce(force);
		}
	}
}
