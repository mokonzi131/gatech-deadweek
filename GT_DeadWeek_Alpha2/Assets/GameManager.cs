using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public GameObject gamePlaySoldier;
	//public var sarge : SargeManager;
	
	static public bool receiveDamage;
	static public bool pause;
	static public bool scores;
	static public float time;
	static public bool running;
	
	public MainMenuScreen menu;
	
	public Camera[] PauseEffectCameras;
	private bool _paused;
	
	void Start()
	{
		Screen.lockCursor = true;
		
		running = false;
		pause = false;
		scores = false;
		_paused = false;
		time = 0.0f;
		
		Transform auxT;
		bool hasCutscene = false;
		for(int i = 0; i < transform.childCount; i++)
		{
			auxT = transform.GetChild(i);
			
			if(auxT.name == "Cutscene")
			{
				if(auxT.gameObject.active)
				{
					hasCutscene = true;
					break;
				}
			}
		}
		
		if(!hasCutscene)
		{
			StartGame();
		}
	}
	
	void CutsceneStart()
	{
		gamePlaySoldier.SetActive (false);
		//gamePlaySoldier.SetActiveRecursively(false);
	}
	
	void Update()
	{
		if(!pause && running) time += Time.deltaTime;
		
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			pause = !pause;
			
			menu.visible = pause;
			
			if(pause)
			{
				Time.timeScale = 0.00001f;
			}
			else
			{
				Time.timeScale = 1.0f;
			}
		}
		
		if(_paused != pause)
		{
			_paused = pause;

			for(int i = 0; i < PauseEffectCameras.Length; i++)
			{
				Camera cam = PauseEffectCameras[i];
				if (cam == null) continue;
				if (cam.name != "radar_camera") continue;
				
				cam.enabled = !pause;
			}           
		}
		
		Screen.lockCursor = !pause && !scores;
	}
	
	void StartGame()
	{
		running = true;
		
		if(gamePlaySoldier != null)
		{
			if(!gamePlaySoldier.active)
			{
				gamePlaySoldier.SetActive(true);
				//gamePlaySoldier.SetActiveRecursively(true);
			}
		}

		
//		if(sarge != null && Application.loadedLevelName == "demo_forest")
//		{
//			sarge.ShowInstruction("instructions");
//			sarge.ShowInstruction("instructions2");
//			sarge.ShowInstruction("instructions3");
//			sarge.ShowInstruction("instructions4");
//			sarge.ShowInstruction("instructions5");
//			sarge.ShowInstruction("additional_instructions");
//		}
	}

}
