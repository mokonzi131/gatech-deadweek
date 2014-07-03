using UnityEngine;
using System.Collections;

public class AxePropertyScript : MonoBehaviour {

	public bool isPlayerAxe = false;
	// Use this for initialization
	void Start () {
		Invoke ("DestroyAxe", 5.0f);
	}

	void DestroyAxe()
	{
		Destroy (gameObject);
	}
}
