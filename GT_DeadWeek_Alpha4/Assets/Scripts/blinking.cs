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

public class blinking : MonoBehaviour {

	public float frequency = 10.0f;
	public float minIntensity = 0 ;
	public float maxIntensity = 2 ;
	private float lastTime = 0 ;
	
	// Update is called once per frame
	void Update () {
		lastTime += Time.deltaTime;
		if (lastTime > 1.0f / frequency) {
			lastTime = 0;
			bool dark = Random.Range(0,1) > 0.7f;
			if(dark)
				light.intensity = Random.Range(minIntensity, 0.33f*(minIntensity+maxIntensity));
			else
				light.intensity = Random.Range(0.66f*(minIntensity+maxIntensity), maxIntensity);
		}
	}
}
