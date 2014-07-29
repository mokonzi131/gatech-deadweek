using UnityEngine;
using System.Collections;

public class CheckpointManagerScript : MonoBehaviour {

	public MinimapArrowLookAt minimapArrowManager;

	[HideInInspector]
	public int lastCheckpointIndex;
	
	[HideInInspector]
	public float lastCheckpointTime;

	private ScreenFadingScript fadingManager;
	private bool isReseting;

	void Awake ()
	{
		
		lastCheckpointIndex = 0;
		lastCheckpointTime = GameObject.Find ("Timer").GetComponent<Timer> ().time;

		SetMinimapArrow ();

		fadingManager = GameObject.Find("ScreenFadingManager").GetComponent<ScreenFadingScript> ();

		isReseting = false;

	}

	
	// Update is called once per frame
	void Update () {

		if (isReseting && fadingManager.guiTexture.color.a >= 0.95f)
		{
			fadingManager.FadeToClear();
			SetPlayerAndTimer();
			isReseting = false;
		}

	}

	
	public void ResetToLastCheckpoint()
	{
		fadingManager.FadeToBlack ();
		isReseting = true;
	}


	public void UpdateCheckpoint(int index)
	{
		
		lastCheckpointIndex = index;
		lastCheckpointTime = GameObject.Find("Timer").GetComponent<Timer>().time;

		SetMinimapArrow ();

	}


	void SetPlayerAndTimer()
	{
		
		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
		GameObject targetCP = cps[0];
		foreach (GameObject cp in cps)
		{
			if (cp.GetComponent<CheckpointScript>().index == lastCheckpointIndex)
			{
				targetCP = cp;
				break;
			}
		}
		
		GameObject.FindGameObjectWithTag ("Player").transform.position = targetCP.transform.position;
		GameObject.Find ("Timer").GetComponent<Timer> ().time = lastCheckpointTime;
		
	}
	

	void SetMinimapArrow()
	{
		
		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
		GameObject targetCP = null;
		foreach (GameObject cp in cps)
		{
			if (cp.GetComponent<CheckpointScript>().index == lastCheckpointIndex + 1)
			{
				targetCP = cp;
				break;
			}
		}

		if (targetCP != null)
			minimapArrowManager.target = targetCP.transform;
		else
			minimapArrowManager.target = null;

	}


}
