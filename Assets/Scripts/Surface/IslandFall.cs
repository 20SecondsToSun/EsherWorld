using UnityEngine;
using System.Collections;

public class IslandFall : MonoBehaviour 
{
	/*public IslandFall(GameObject go) : base(go)
	{
	
	}*/
	private GameObject robot;
	private Vector3 center;
	public bool active = false;
	public string direction;
	
	public void Start()
	{
		robot = GameObject.FindWithTag("Robot");
		center  = this.transform.position;
	}
	public void Update()
	{
		if (!active) return;
		
		if ((epsilon(robot.transform.localPosition.x, 0.0f)&&( direction=="LEFT"    || direction=="RIGHT"))||
			(epsilon(robot.transform.localPosition.z, 0.0f)&&( direction=="FORWARD" || direction=="BACK")))
		{
			Debug.Log("Fall   " + robot.transform.localPosition.x+" "+ robot.transform.localPosition.z);
			active = false;
			GameEvents.InitiateEvent("robotStateChanged_Fall",null);		
		}	
	}
	private bool  epsilon(float a,float b)
	{		
		return Mathf.Abs(a-b)<0.001f ;		
	}
}
