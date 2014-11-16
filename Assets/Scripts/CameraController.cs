using UnityEngine;
using System.Collections;

class CameraData
{
   public Vector3 trgt;
   public Vector3 trnsfrm;
}
public class CameraController : MonoBehaviour
{	
	private float xSpeed = 250.0f;
	private float ySpeed = 120.0f;
	private float distance = 25.0f; 
	
	private float x = 0.0f;
	private float y = 0.0f;
	
	private int yMinLimit = -20;
	private int yMaxLimit = 80;
	
	public Transform target;
	
	private CameraData cam = new CameraData();	

	float coordX;
	float coordY;
	
	void Start () 
	{
	   y = ClampAngle(0, yMinLimit, yMaxLimit);


       Quaternion rotation = Quaternion.Euler(y, x, 0);
		Vector3 position = new Vector3(0f,0f,0f);
			position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
     
        transform.rotation = rotation;
        transform.position = position;			
	}	
	void Update () 
	{
		cameraRotator();
	}
	void cameraRotator()
	{			
 		if(Input.touchCount > 0)
		{
  		  Touch theTouch = Input.GetTouch(0);			
  		  coordY = theTouch.deltaPosition.y;
		  coordX = theTouch.deltaPosition.x; 
		}		
		else if(Input.GetMouseButton(0))
		{  				
  		  coordY = Input.GetAxis("Mouse Y");
		  coordX = Input.GetAxis("Mouse X"); 
		}
		GameEvents.InitiateEvent("cameraPositionIsChanged",cam);
		Debug.Log("rot");
		if(Input.GetMouseButton(0))
	    {
	        x += coordX * xSpeed * 0.02f;
	        y -= coordY * ySpeed * 0.02f;
			
		    y = ClampAngle(y, yMinLimit, yMaxLimit);
	 
	        Quaternion rotation = Quaternion.Euler(y, x, 0);
	        Vector3 position = new Vector3(0f,0f,0f);
				position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
	     
	        transform.rotation = rotation;
	        transform.position = position;	

			cam.trnsfrm =  transform.position;
			cam.trgt =  target.position;				
	    }	
	}
	static float ClampAngle (float angle ,float min,float max ) 
	{
	    if (angle < -360)   angle += 360;
	    if (angle > 360)    angle -= 360;
	    return Mathf.Clamp (angle, min, max);
	}
}
