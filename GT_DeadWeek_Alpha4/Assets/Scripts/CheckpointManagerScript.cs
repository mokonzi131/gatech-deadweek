using UnityEngine;
using System.Collections;

public class CheckpointManagerScript : MonoBehaviour {

	public GUIText victoryText;

	public bool[] curCheckpointStatus;
	public bool[] lastCheckpointStatus;

	public GameObject[] checkPoints;
	public GameObject destPoint;


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

		curCheckpointStatus = new bool[checkPoints.Length];
		curCheckpointStatus [0] = true;
		for (int i = 1; i < curCheckpointStatus.Length; i++)
			curCheckpointStatus[i] = false;
		
		lastCheckpointStatus = new bool[checkPoints.Length];
		for (int i = 0; i < lastCheckpointStatus.Length; i++)
			lastCheckpointStatus[i] = curCheckpointStatus[i];


		SetCheckpoints ();

		SetMinimapArrow ();

		fadingManager = GameObject.Find("ScreenFadingManager").GetComponent<ScreenFadingScript> ();

		isReseting = false;

	}

	
	// Update is called once per frame
	void Update () {

		//Debug.Log (curCheckpointStatus [0].ToString () + " " + curCheckpointStatus [1].ToString () + " " + curCheckpointStatus [2].ToString () + " " + lastCheckpointIndex.ToString ());

		if (isReseting && fadingManager.guiTexture.color.a >= 0.95f)
		{
			fadingManager.FadeToClear();
			SetPlayerAndTimer();
			SetCheckpoints();
			victoryText.text = "";
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

		curCheckpointStatus [index] = true;

		for (int i = 0; i < lastCheckpointStatus.Length; i++)
			lastCheckpointStatus[i] = curCheckpointStatus[i];

		SetCheckpoints ();
		SetMinimapArrow ();

	}


	public void SetCheckpoints()
	{
		bool destActive = true;
		for (int i = 0; i< checkPoints.Length; i++)
		{
			curCheckpointStatus[i] = lastCheckpointStatus[i];
			if (!curCheckpointStatus[i])
				destActive = false;

			if (curCheckpointStatus[i])
			{
				checkPoints[i].SetActive(false);
			}
			else
			{
				checkPoints[i].SetActive(true);
			}
		}

		destPoint.SetActive (destActive);

	}

	void SetPlayerAndTimer()
	{


		
//		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
//		GameObject targetCP = cps[0];
//		foreach (GameObject cp in cps)
//		{
//			if (cp.GetComponent<CheckpointScript>().index == lastCheckpointIndex)
//			{
//				targetCP = cp;
//				break;
//			}
//		}
//		
//		GameObject.FindGameObjectWithTag ("Player").transform.position = targetCP.transform.position;
		GameObject.Find ("Timer").GetComponent<Timer> ().time = lastCheckpointTime;		
		GameObject.FindGameObjectWithTag ("Player").transform.position = checkPoints[lastCheckpointIndex].transform.position;

		
	}
	

	void SetMinimapArrow()
	{
		
		GameObject targetCP = null;
		int i = 0;
		for (i = 0; i< checkPoints.Length; i++)
		{
			if (!curCheckpointStatus[i])
				break;
		}

		if (i < checkPoints.Length)
			targetCP = checkPoints[i];
		else
			targetCP = destPoint;

		minimapArrowManager.target = targetCP.transform;

//		GameObject[] cps = GameObject.FindGameObjectsWithTag("Checkpoint");
//
//		foreach (GameObject cp in cps)
//		{
//			if (cp.GetComponent<CheckpointScript>().index == lastCheckpointIndex + 1)
//			{
//				targetCP = cp;
//				break;
//			}
//		}
//
//		if (targetCP != null)
//			minimapArrowManager.target = targetCP.transform;
//		else
//			minimapArrowManager.target = null;

	}


}
