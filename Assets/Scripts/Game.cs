using UnityEngine;
using System.Collections;

public class Game: MonoBehaviour
{	
	private Level level;
	private RobotController robot;	
	
	private string robotState = "step";
	
	public void initCurrentGame()
	{
		GameEvents.eventListener += gameEventHandler;
		
		robot = new RobotController();
		
		level = new Level();
		level.levelInitialization();		
	}	

	private void gameEventHandler(string eventId, object prms)
	{
		switch(eventId)
		{
			case "cameraPositionIsChanged":				
				if(robotState=="step")
					level.createWorldCurrentProjection((prms as CameraData).trgt, (prms as CameraData).trnsfrm);
				else if (robotState=="fall")
					level.createWorldCurrentProjectionForFall((prms as CameraData).trgt, (prms as CameraData).trnsfrm);
			break;
			
			case "setRobotIsland":				
				robot.setRobotIsland((prms as Island));
			break;
			
			case "activeCubeChanged":				
				level.setActiveIsland((prms as Island));
			break;	

			case "robotStateChanged_Fall":				
				robotState = "fall";
			break;

			case "robotStateChanged_Step":				
				robotState = "step";
			break;

			case "robotFall":
				robotState = "step";
				level.setInitIsland();
			break;
		}		
	}

	public void Start()
	{
		initCurrentGame();	
	}	

	public void Update()
	{
		switch(robotState)
		{
			case "step":
				robot.doStep();
			break;

			case "fall":
				robot.doFall();
			break;
		}		
	}
}