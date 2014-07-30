using UnityEngine;
using System.Collections;

public class PlayerTarget : MonoBehaviour {



	public Texture2D target;
	public Texture2D targetOver;
	
	public bool overEnemy;
	private bool _overEnemy;
	
	private GUITexture gui;
	
	public bool aim;
	private bool _aim;
	
	public LayerMask ignoreLayer;

	public Camera playerCam;

	public PlayerController playerController;
	public PlayerCamera playerCamera;
	public ThrowScript throwScript;
	
	void OnEnable()
	{
		ignoreLayer = ~ignoreLayer;
		
		gui = guiTexture;
		
		gui.pixelInset = new Rect(-target.width * 0.5f, -target.height * 0.5f, target.width, target.height);
		gui.texture = target;
		
		gui.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);
	}
	
	void Update()
	{	
		
		aim = playerController.aim;

		if(!playerCam.gameObject.active || !aim) 
		{
			gui.color = new Color(0.5f, 0.5f, 0.5f, 0);
			return;
		}


		//aim = Input.GetButton("Fire2");
		
		Ray ray = playerCam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

		RaycastHit hit;
		
		bool reachable = false;
		if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreLayer))
		{
			Vector3 startPoint = throwScript.eyePoint.position + transform.forward * 0.2f;

			Vector3 relativePos = new Vector3();
			relativePos.z = 0;
			relativePos.x = Mathf.Sqrt( (hit.point.x - startPoint.x) * (hit.point.x - startPoint.x)
			                           + (hit.point.z - startPoint.z) * (hit.point.z - startPoint.z) );
			relativePos.y = hit.point.y - startPoint.y;

			Vector3 relativeVelocity = throwScript.ComputeInitialVelocity(throwScript.power, relativePos, true, ref reachable);

		}
//		var hit1 : RaycastHit;
//		var hit2 : RaycastHit;
//		
//		overEnemy = Physics.Raycast(ray.origin, ray.direction, hit1, enemyDistance, enemyLayer);
//		
//		if(overEnemy)
//		{
//			if(Physics.Raycast(ray.origin, ray.direction, hit2, enemyDistance, otherLayer))
//			{
//				overEnemy = hit1.distance < hit2.distance;
//			}
//		}


		if (aim)
		{
			if (reachable)
			{
				gui.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
				gui.texture = target;
			}
			else
			{
				gui.color = new Color(0.5f, 0.5f, 0.5f, 0.15f);
				gui.texture = targetOver;
			}
		}
		else
		{
			gui.color = new Color(0.5f, 0.5f, 0.5f, 0.0f);
			gui.texture = target;
		}
	}
}
