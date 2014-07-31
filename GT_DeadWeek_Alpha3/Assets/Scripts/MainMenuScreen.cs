using UnityEngine;
using System.Collections;



public class MainMenuScreen : MonoBehaviour {

	enum MainMenuState
	{
		IDLE,
		ABOUT,
		INSTRUCTION
	};

	public Texture2D menuBackground;
	private Rect menuBackgroundRect;
	
	public Texture2D windowBackground;
	private Rect windowBackgroundRect;

	
	public Texture2D resume;
	public Texture2D resumeOver;
	private Rect resumeRect;

	public Texture2D checkpoint;
	public Texture2D checkpointOver;
	private Rect checkpointRect;
	
	public Texture2D restart;
	public Texture2D restartOver;
	private Rect restartRect;

	
	public Texture2D about;
	public Texture2D aboutOver;
	private Rect aboutRect;

	public Texture2D instruction;
	public Texture2D instructionOver;
	private Rect instructionRect;

	public GUISkin hudSkin;
	
	private GUIStyle panelLeft;
	private Rect panelLeftRect;
	
	private GUIStyle panelRight;
	private Rect panelRightRect;
	
	private GUIStyle descriptionStyle;
	private GUIStyle titleStyle;
	private GUIStyle customBox;
	
	private Vector2 mousePos;
	private Vector2 screenSize;
	
	private Event evt;
	
	private MainMenuState state;
	private float lastMouseTime;

	private Rect scrollPosition;
	private Rect scrollView;
	private Vector2 scroll;
	
	public Texture2D black;
	private float alpha;
	static public bool goingToGame;
	static public bool showProgress;
	
	private Vector2 aboutScroll;
	private Vector2 graphicsScroll;
	private GUIStyle aboutStyle;
	
	private bool resumeGame;
	public bool visible;
	
	private GUIStyle sliderBackground;
	private GUIStyle sliderButton;
	
	public Texture2D greenBar;
	public Texture2D whiteMarker;
	
	private float margin  = 30;
	
	private Rect questionRect;
	private Rect greenRect;
	private GUIStyle tooltipStyle;
	private GUIStyle questionButtonStyle;
	
	private GUIStyle aquirisLogo;
	private GUIStyle unityLogo;
	
	public AudioClip overSound;
	public float overVolume = 0.4f;
	
	public AudioClip clickSound;
	public float clickVolume = 0.7f;
	
	private bool over;
	private bool hideOptions;
	private bool loadingIndustry;
	
	public GUIStyle textStyle;
	private float angle;
	public Texture2D loadingBackground;
	
	void Start()
	{
		angle = 0.0f;
		over = false;
		loadingIndustry = false;
		showProgress = false;
		hideOptions = Application.loadedLevelName != "demo_industry";
		
		questionButtonStyle = hudSkin.GetStyle("QuestionButton");
		tooltipStyle = hudSkin.GetStyle("TooltipStyle");
		aquirisLogo = hudSkin.GetStyle("AquirisLogo");
		unityLogo = hudSkin.GetStyle("UnityLogo");
		questionRect = new Rect(0, 0, 11, 11);
		
		alpha = 1.0f;
		goingToGame = false;
		resumeGame = false;
		
		state = MainMenuState.IDLE;
		
		descriptionStyle = hudSkin.GetStyle("Description");
		titleStyle = hudSkin.GetStyle("Titles");
		customBox = hudSkin.GetStyle("CustomBox");
		panelLeft = hudSkin.GetStyle("PanelLeft");
		panelRight = hudSkin.GetStyle("PanelRight");
		aboutStyle = hudSkin.GetStyle("About");
		
		menuBackgroundRect = new Rect(0, 0, menuBackground.width, menuBackground.height);
		windowBackgroundRect = new Rect(0, 0, windowBackground.width, windowBackground.height);
		panelLeftRect = new Rect(0, 0, 235, 240);
		panelRightRect = new Rect(0, 0, 235, 240);
		aboutRect = new Rect(0, 0, about.width * 0.75f, about.height * 0.75f);
		resumeRect = new Rect(0, 0, resume.width * 0.75f, resume.height * 0.75f);

		checkpointRect = new Rect (0, 0, checkpoint.width * 0.75f, checkpoint.height * 0.75f);
		restartRect = new Rect (0, 0, restart.width * 0.75f, restart.height * 0.75f);
		instructionRect = new Rect(0, 0, instruction.width * 0.45f, instruction.height * 0.45f);

	}
	
	void Update()
	{
		if(goingToGame)
		{
			alpha += Time.deltaTime;
			
			if(alpha >= 1.0f)
			{
				if(!loadingIndustry)
				{
					loadingIndustry = true;
					Application.LoadLevelAsync("demo_industry");
				}
			}
		}
		else
		{
			if(alpha > 0)
			{
				alpha -= Time.deltaTime * 0.5f;
			}
		}
		
		if(Time.timeScale == 0.0f || GameManager.pause)
		{
			lastMouseTime -= 0.01f;
		}
		
		if(showProgress)
		{
			angle -= Time.deltaTime * 360;
			
			if(angle < -360.0f)
			{
				angle += 360.0f;
			}
		}
	}
	
	void DrawGUI(Event _event)
	{
		evt = _event;
		screenSize = new Vector2(Screen.width, Screen.height);
		mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		
		if(visible)
		{
			if(state != MainMenuState.IDLE)
			{
				windowBackgroundRect.x = menuBackgroundRect.x + menuBackgroundRect.width;
				windowBackgroundRect.y = (screenSize.y - windowBackgroundRect.height) * 0.5f;
				
				GUI.DrawTexture(windowBackgroundRect, windowBackground);
				
				if(state == MainMenuState.ABOUT || state == MainMenuState.INSTRUCTION)
				{
					panelLeftRect.width = 475;
					panelLeftRect.x = windowBackgroundRect.x + 15;
					panelLeftRect.y = windowBackgroundRect.y + 55;
					
					GUI.Box(panelLeftRect, "", panelLeft);
				}
			}

			if(state == MainMenuState.ABOUT)
			{
				DrawAbout();
			}
			else if (state == MainMenuState.INSTRUCTION)
			{
				DrawInstruction();
			}
			
			DrawMenu();
		}

		
		if(alpha > 0.0f)
		{
			Color c = GUI.color;
			Color d = c;
			d.a = alpha;
			GUI.color = d;
			
			GUI.DrawTexture(new Rect(0, 0, screenSize.x, screenSize.y), black);
			
			if(goingToGame)
			{
				GUI.Label(new Rect(Screen.width - 120, Screen.height - 40, 90, 20), "Loading...", textStyle);
			}
			GUI.color = c;
		}
	}

	
	private void DrawSliderOverlay (Rect rect, float val)
	{
		rect.width = Mathf.Clamp(val * 199.0f, 0.0f, 199.0f);
		GUI.DrawTexture (rect, greenBar);
	}

	private void BeginToggleRow ()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal(GUILayout.Width(400));
		questionRect.y += 21;
	}
	
	private void EndToggleRow (int pix)
	{
		GUILayout.Space (pix);		
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	
	void DrawMarker(float y, int steps)
	{
		Rect markerRect = new Rect(margin, y + 2, 1, 5);
		float aux;
		float s = steps;
		
		for(int i = 0; i <= steps; i++)
		{
			aux = i;
			aux 	/= s;
			markerRect.x = margin + 5 + aux * 196;
			
			GUI.DrawTexture(markerRect, whiteMarker);
		}
	}
	
	void DrawAbout()
	{
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "ABOUT", titleStyle);	
		
		Rect abRect = new Rect(panelLeftRect.x + 7, panelLeftRect.y + 30, panelLeftRect.width - 12, panelLeftRect.height - 40);
		
		GUISkin cSkin = GUI.skin;
		GUI.skin = hudSkin;
		
		GUILayout.BeginArea(abRect);
		aboutScroll = GUILayout.BeginScrollView(aboutScroll, GUILayout.Width(abRect.width));
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(17);
		GUILayout.BeginVertical();
		GUILayout.Label("Developed by GT Dead Week Team.", aboutStyle, GUILayout.Width(423));
		GUILayout.Space(5);
		GUILayout.Label("Arnaud Golinvaux", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Chenglong Jiang", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Michael Landes", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Josephine Simon", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Chuan Yao", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Space(20);
		GUILayout.Label("Special thanks to:", aboutStyle, GUILayout.Width(423));
		GUILayout.Space(5);
		GUILayout.Label("", GUILayout.Width(400));
		
		GUILayout.Space(70);

		//GUI.DrawTexture(new Rect(170, 180, 339 * 0.75f, 94 * 0.75f), aquirisLogo.normal.background);
		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
		
		GUI.skin = cSkin;
	}

	void DrawInstruction()
	{
		GUI.Label(new Rect(windowBackgroundRect.x + 20, windowBackgroundRect.y + 15, 200, 20), "INSTRUCTION", titleStyle);	
		
		Rect abRect = new Rect(panelLeftRect.x + 7, panelLeftRect.y + 30, panelLeftRect.width - 12, panelLeftRect.height - 40);
		
		GUISkin cSkin = GUI.skin;
		GUI.skin = hudSkin;
		
		GUILayout.BeginArea(abRect);
		aboutScroll = GUILayout.BeginScrollView(aboutScroll, GUILayout.Width(abRect.width));
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(17);
		GUILayout.BeginVertical();
		GUILayout.Label("Support: Keyboard+Mouse / XBOX Joystick", aboutStyle, GUILayout.Width(423));
		GUILayout.Space(5);
		GUILayout.Label("W+S / Left Axis:  Move Forward and Backward", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Left Shift / Left Button:  Hold and Run", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Mouse Movement / Right Axis:  Camera View Angle (Aim)", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("X / Right Button:  Throw", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("Mouse Right Click / Left Trigger: Aim", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("ESC / Button Start: Pause", GUILayout.Width(400));
		GUILayout.Space(5);
		GUILayout.Label("M / Button Back: Mini Map On/Off", GUILayout.Width(400));
		GUILayout.Space(5);

		GUILayout.Space(20);

		
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.EndScrollView();
		
		GUILayout.EndArea();
		
		GUI.skin = cSkin;
	}


	
	void DrawMenu()
	{
		menuBackgroundRect.x = 0;
		menuBackgroundRect.y = (screenSize.y - menuBackgroundRect.height) * 0.5f - 50;

		resumeRect.x = menuBackgroundRect.x + 110;
		resumeRect.y = menuBackgroundRect.y + 55;

		checkpointRect.x = resumeRect.x;
		checkpointRect.y = resumeRect.y + resumeRect.height + 15;

		restartRect.x = checkpointRect.x;
		restartRect.y = checkpointRect.y + checkpointRect.height + 15;
		
		instructionRect.x = restartRect.x;
		instructionRect.y = restartRect.y + restartRect.height + 15;

		aboutRect.x = instructionRect.x;
		aboutRect.y = instructionRect.y + instructionRect.height + 15;

		


		GUI.DrawTexture(menuBackgroundRect, menuBackground);
		
		var overButton = false;

		if(resumeRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				audio.volume = overVolume;
				audio.PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(resumeRect, resumeOver);
			
			if(alpha <= 0.0 && GameManager.pause)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					audio.volume = clickVolume;
					audio.PlayOneShot(clickSound);
					
					GameManager.pause = false;
					Time.timeScale = 1.0f;
					//Time.timeScale = 1.0;
					visible = false;
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(resumeRect, resume);
		}

		if(checkpointRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				audio.volume = overVolume;
				audio.PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(checkpointRect, checkpointOver);
			
			if(alpha <= 0.0 && GameManager.pause)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					audio.volume = clickVolume;
					audio.PlayOneShot(clickSound);
					
					GameManager.pause = false;
					Time.timeScale = 1.0f;
					visible = false;

					GameObject.Find("CheckpointManager").GetComponent<CheckpointManagerScript>().ResetToLastCheckpoint();
				}
			}
		}
		else
		{
			GUI.DrawTexture(checkpointRect, checkpoint);
		}

		if(restartRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				audio.volume = overVolume;
				audio.PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(restartRect, restartOver);
			
			if(alpha <= 0.0 && GameManager.pause)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					audio.volume = clickVolume;
					audio.PlayOneShot(clickSound);
					
					GameManager.pause = false;
					Time.timeScale = 1.0f;
					//Time.timeScale = 1.0;

					Application.LoadLevel(0);
				}
			}
		}
		else
		{
			GUI.DrawTexture(restartRect, restart);
		}


		if(aboutRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				audio.volume = overVolume;
				audio.PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(aboutRect, aboutOver);
			
			if(alpha <= 0.0 && !goingToGame)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					audio.volume = clickVolume;
					audio.PlayOneShot(clickSound);
					
					if(state != MainMenuState.ABOUT)
					{
						state = MainMenuState.ABOUT;
					}
					else
					{
						state = MainMenuState.IDLE;
					}
					
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(aboutRect, about);
		}



		if(instructionRect.Contains(mousePos))
		{
			overButton = true;
			
			if(!over)
			{
				over = true;
				audio.volume = overVolume;
				audio.PlayOneShot(overSound);
			}
			
			GUI.DrawTexture(instructionRect, instructionOver);
			
			if(alpha <= 0.0 && !goingToGame)
			{
				if(evt.type == EventType.MouseUp && evt.button == 0 && Time.time > lastMouseTime)
				{
					audio.volume = clickVolume;
					audio.PlayOneShot(clickSound);
					
					if(state != MainMenuState.INSTRUCTION)
					{
						state = MainMenuState.INSTRUCTION;
					}
					else
					{
						state = MainMenuState.IDLE;
					}
					
					lastMouseTime = Time.time;
				}
			}
		}
		else
		{
			GUI.DrawTexture(instructionRect, instruction);
		}


		
		if(!overButton)
		{
			over = false;
		}
	}


	void OnGUI()
	{
		Event evt = Event.current;

		DrawGUI (evt);

	}

}
