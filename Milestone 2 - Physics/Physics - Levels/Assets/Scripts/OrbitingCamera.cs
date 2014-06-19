/*
 * From Unity Asset Store
 * https://www.assetstore.unity3d.com/en/#!/content/917
*/

using UnityEngine;
using System.Collections;

public class OrbitingCamera : MonoBehaviour {
	
	public Transform playerTransform;
	
	//set camera orbit speeds
	public float xAxisOrbitSpeed;
	public float yAxisOrbitSpeed;
	
	//set maximum rotation around the X axis (up and down)
	public float maxXAxisRotation;
	public float minXAxisRotation;
	
	//set camera position relative to player
	public float cameraXOffset;
	public float cameraYOffset;
	public float cameraDistanceFromPlayer;
	
	//holds the camera position based on chosen offsets above
	private Vector3 cameraFollowPosition;
	
	//gets the camera's new rotation axes based on location in orbit
	internal float xAxisRotation;
	internal float yAxisRotation;
	
	//holds the rotation of the camera based on xAxisRotation and yAxisRotation
	internal Quaternion cameraRotationFull;
	
	//toggles if the orbit is active
	public bool orbitIsActive;
	
	//toggles inversion of both axes
	public bool invertXAxisRotation;
	public bool invertYAxisRotation;
	
	// Update is called once per frame
	void Update ()
	{	
		if (playerTransform)	//if there is a player attached to this variable in the inspector...
		{
			//toggles camera orbit when C key is pressed
			if(Input.GetKeyDown(KeyCode.C))
			{
				orbitIsActive = !orbitIsActive;
			}
			
			//if the orbit is active
			if(orbitIsActive)
			{
				if(!invertXAxisRotation)	//if the X axis IS NOT inverted...
				{
					//set the camera object's X axis rotation based on mouse input and positive orbit speed
					xAxisRotation -= Input.GetAxis("Mouse Y") * xAxisOrbitSpeed * Time.deltaTime;
				}
				else	//if the X axis IS inverted...
				{
					//set the camera object's X axis rotation based on mouse input and negative orbit speed
					xAxisRotation -= Input.GetAxis("Mouse Y") * -xAxisOrbitSpeed * Time.deltaTime;
				}
				
				if (!invertYAxisRotation)	//if the Y axis IS NOT inverted...
				{
					//set the camera object's Y axis rotation based on mouse input and positive orbit speed
					yAxisRotation += Input.GetAxis("Mouse X") * yAxisOrbitSpeed * Time.deltaTime;	
				}
				else	//if the Y axis IS inverted...
				{
					//set the camera object's Y axis rotation based on mouse input and negative orbit speed
					yAxisRotation += Input.GetAxis("Mouse X") * -yAxisOrbitSpeed * Time.deltaTime;
				}
				
				//set the Y axis between -360/360 and clamp the X axis to the Min and Max X Axis Rotation set in the inspector
				xAxisRotation = CorrectAngle(xAxisRotation);
				
				//get the full desired rotation of the camera in Quaternion format and set the object's rotation to it
				cameraRotationFull = Quaternion.Euler(xAxisRotation, yAxisRotation, 0);
				transform.rotation = cameraRotationFull;	
			}
		}
	}
	
	//having camera movement in LateUpdate usually smooths the movement really well
	void LateUpdate()
	{
		if (orbitIsActive)
		{
			//set the camera follow position based on its current rotation, the follow position settings from the inspector, and the player object's current position
			cameraFollowPosition = cameraRotationFull * new Vector3(cameraXOffset, cameraYOffset, -cameraDistanceFromPlayer) + playerTransform.position;
			transform.position = cameraFollowPosition;
		}
	}
	
	//fixes any rotation errors and clamps down on the X axis rotation
	float CorrectAngle(float givenAngle)
	{
		//givenAngle stays between -360/360 allowing a normal range of angles
		if (givenAngle < -360)
		{
			givenAngle += 360;			
		}
		
		if (givenAngle > 360)
		{
			givenAngle -= 360;
		}
		
		//here the X axis rotation is clamped
		return Mathf.Clamp(givenAngle, minXAxisRotation, maxXAxisRotation);
	}
}