using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController
{
	private GameObject robot;	
	private float velocity = 0.01f;
	private string direction = "LEFT";	
	private Island robotIsland = null;
	private bool directionChangedInCenter = false;
	
	public Dictionary<string, string> oppositeDirections = new Dictionary<string, string>();	
	public Dictionary<string, string[]> chooseDirections = new Dictionary<string, string[]>();
	private GameObject[] pillarArray ;
	

	public RobotController()
	{
		robot = GameObject.FindWithTag("Robot");		

		oppositeDirections["LEFT"]    = "RIGHT";
		oppositeDirections["RIGHT"]   = "LEFT";
		oppositeDirections["FORWARD"] = "BACK";
		oppositeDirections["BACK"]    = "FORWARD";	
		
		chooseDirections["LEFT"] 	= new string[2];
		chooseDirections["LEFT"][0] = "BACK";
		chooseDirections["LEFT"][1] = "FORWARD";
		
		chooseDirections["RIGHT"] 	 = new string[2];
		chooseDirections["RIGHT"][0] = "BACK";
		chooseDirections["RIGHT"][1] = "FORWARD";
		
		chooseDirections["FORWARD"]	   = new string[2];
		chooseDirections["FORWARD"][0] = "LEFT";
		chooseDirections["FORWARD"][1] = "RIGHT";
		
		chooseDirections["BACK"]    = new string[2];
		chooseDirections["BACK"][0] = "RIGHT";
		chooseDirections["BACK"][1] = "LEFT";

		pillarArray  = GameObject.FindGameObjectsWithTag("surface");
	
		
	}
	public void setRobotIsland(Island island)
	{
		robot.transform.parent = island.islandGameObject.transform;// робот находится в локальных координатах куба
		robotIsland = island;
		
		changePosition();
		changeRotation();
		
		if (island.islandGameObject.GetComponent<IslandFall>())
		{
			island.islandGameObject.GetComponent<IslandFall>().active = true;
			island.islandGameObject.GetComponent<IslandFall>().direction = direction;
			Debug.Log ("Init Fall");
		};
	}	
	public void doStep()
	{		
		Vector3 center    =  robotIsland.islandGameObject.renderer.bounds.center;
 		Vector3 extents   =  robotIsland.islandGameObject.renderer.bounds.extents;
		Vector3 robotIslandCoords = robot.transform.position;
	
		Vector3 changedVector = new Vector3(0,0,0); 

		switch(direction)
		{
			case "LEFT":
				if (robotIslandCoords.x - velocity > center.x - extents.x)	
				{	
					if (epsilon1(robotIslandCoords.x - velocity,center.x)) 
						if (checkIslandContacts()) break;
				
					changedVector = new Vector3(-velocity,0,0);					
				}
				else
				{
					checkIslandEndContact();					
				}			
			break;
			case "RIGHT":
				if (robotIslandCoords.x + velocity < center.x + extents.x)	
				{								
					if (epsilon1(robotIslandCoords.x + velocity,center.x))
						if (checkIslandContacts()) break;
				
					changedVector = new Vector3(velocity,0,0);					
				}
				else
				{
					checkIslandEndContact();
				}			
			break;
			case "FORWARD":
				if (robotIslandCoords.z + velocity < center.z+extents.z)	
				{
					if (epsilon1(robotIslandCoords.z + velocity,center.z))
						if (checkIslandContacts()) break;
				
					changedVector = new Vector3(0,0,velocity);	
				}
				else 
				{											
					checkIslandEndContact();
				}
			break;
			case "BACK":
				if (robotIslandCoords.z - velocity > center.z-extents.z)	
				{
					if (epsilon1(robotIslandCoords.z - velocity,center.z)) 
						if (checkIslandContacts()) break;
						
					changedVector = new Vector3(0,0,-velocity);		
				}
				else 
				{											
					checkIslandEndContact();
				}
			break;
		}
		
		robot.transform.position += changedVector;		
		
		
	}
	private bool checkIslandContacts()
	{
		if (directionChangedInCenter) return false;
		
		Island island;
		for(int i = 0; i < 2; i++)
		{			
			string dir = chooseDirections[direction][i];
			island = robotIsland.getContactIsland(dir);	
			
			if (island != null)			
			{				
				directionChangedInCenter = true;
				direction = dir;
				changeRotation();
				
				if (island.islandGameObject.transform.position.y >
					robotIsland.islandGameObject.transform.position.y)
				{					
					// Vector3 center  =  island.islandGameObject.renderer.bounds.center; 					
					// robot.transform.position = new Vector3(robot.transform.position.x, center.y, robot.transform.position.z);
					// Debug.Log ("OK============================ pos " + robot.transform.position);
				}			
				return true;
			}		
		}
		island = robotIsland.getContactIsland(direction);
		if (island != null)		
		if (island.islandGameObject.transform.position.y >
					robotIsland.islandGameObject.transform.position.y)
		{	
			
			// Vector3 center  =  island.islandGameObject.renderer.bounds.center; 					
			// robot.transform.position = new Vector3(robot.transform.position.x, center.y, robot.transform.position.z);
			//	Debug.Log ("OK1============================ pos " + robot.transform.position);
		}			
	
		return false;		
	}
	private void checkIslandEndContact()
	{
		directionChangedInCenter = false;
		
		Island island = robotIsland.getContactIsland(direction);
		
		if ( island != null)			
		{
			//island.resetPosition();
			setRobotIsland(island);
			GameEvents.InitiateEvent("activeCubeChanged",island);
		}
		else
		{
			direction = oppositeDirections[direction];
			changeRotation();
		}			
	}
	private void changeRotation()
	{
		Quaternion angleToRotate = new Quaternion(0,0,0,0);
		
		switch(direction)
		{
			case "LEFT":								
				angleToRotate = Quaternion.Euler(0,270,0);					
			break;
			case "RIGHT":	
				angleToRotate = Quaternion.Euler(0,90,0);				
			break;
			case "FORWARD":			
				angleToRotate = Quaternion.Euler(0,0,0);				
			break;
			case "BACK":			
				angleToRotate = Quaternion.Euler(0,180,0);			
			break;
		}	
		
		robot.transform.rotation = angleToRotate;	
	}
	private void changePosition()
	{
		Vector3 centerRobot = new Vector3(0,0,0);
		Vector3 center  =  robotIsland.islandGameObject.renderer.bounds.center;
 		Vector3 extents =  robotIsland.islandGameObject.renderer.bounds.extents;
		
		center += new Vector3(0,extents.y,0);
		robot.transform.position = center;//+ new Vector3(0,extents.y,0);	
		
		switch(direction)
		{
			case "LEFT":	
				centerRobot = new Vector3(center.x+extents.x, center.y, center.z);	
			break;
			case "RIGHT":				
			    centerRobot = new Vector3(center.x-extents.x, center.y, center.z);	
			break;
			case "FORWARD":				
				centerRobot = new Vector3(center.x, center.y, center.z-extents.z);
			break;
			case "BACK":
				centerRobot = new Vector3(center.x, center.y, center.z+extents.z);
			break;
		}
		
		robot.transform.position = centerRobot;
	}
	private bool epsilon1(float a ,float b)
	{
		float delta = 0.01f;
		return ((a>b-delta) && (a<b+delta));
	}
	public void doFall()
	{		
		robot.transform.position -= new Vector3(0.0f,0.03f,0.0f);
		if (robot.transform.position.y < -4) {
			GameEvents.InitiateEvent("robotFall", null);
				}

	}

	
}