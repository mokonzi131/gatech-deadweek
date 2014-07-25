using UnityEngine;
using System.Collections;

public class FootSteps : MonoBehaviour {
	
	public AudioSource footAudioSource;
	
	public AudioClip[] woodSteps;
	public AudioClip[] concreteSteps;
	public AudioClip[] concreteRunSteps;


	private CharacterController cc;
	private Transform t;
	
	public LayerMask hitLayer;
	private string cTag;

	private float volume;


	void Awake() {
		cc = GetComponent<CharacterController>();
		t = transform;
	}

	IEnumerator Start () {
		while(true) {
			float vel = cc.velocity.magnitude;
			RaycastHit hit = new RaycastHit();
			string floortag;

			if(cc.isGrounded == true && vel > 0.2f) {
				if(Physics.Raycast(transform.position, Vector3.down,out hit, 0.5f, hitLayer))
				{
					floortag = hit.collider.gameObject.tag;
					if (floortag == "concrete")
					{
						if (t.gameObject.GetComponent<PlayerController>().walk)
							audio.clip = concreteSteps[Random.Range(0,concreteSteps.Length)];
						else
							audio.clip = concreteRunSteps[Random.Range(0,concreteRunSteps.Length)];

					}
					else
						audio.clip = null;
				}

				
				volume = 1.0f;
				if(cc != null)
				{
					volume = Mathf.Clamp01(cc.velocity.magnitude * 0.1f);
				}
				else
				{
					volume = 1;
				}


				footAudioSource.PlayOneShot(audio.clip, volume);
				float interval;
				if (audio.clip !=null)
					interval = audio.clip.length;
				else
					interval = 0;

				yield return new WaitForSeconds(interval);
			}
			else {
				yield return 0;
			}
		}
	}


	void OnFootStrike () 
	{
		if(Time.time < 0.5f) return;

		volume = 1.0f;
		if(cc != null)
		{
			volume = Mathf.Clamp01(0.1f + cc.velocity.magnitude * 0.3f);
		}
		else
		{
			volume = 1;
		}
		
		footAudioSource.PlayOneShot(GetAudio(), volume);
	}
	
	AudioClip GetAudio()
	{
		RaycastHit hit;
		
		//Debug.DrawRay(t.position + new Vector3(0, 0.5, 0), -Vector3.up * 5.0);
		
		if(Physics.Raycast(t.position + new Vector3(0, 0.5f, 0), -Vector3.up, out hit, Mathf.Infinity, hitLayer))
		{
			cTag = hit.collider.tag.ToLower();
		}
		
		if(cTag == "wood")
		{
			return woodSteps[Random.Range(0, woodSteps.Length)];
		}
		else
		{
			volume = 0.8f;
			return concreteSteps[Random.Range(0, concreteSteps.Length)];
		}
	}
}
