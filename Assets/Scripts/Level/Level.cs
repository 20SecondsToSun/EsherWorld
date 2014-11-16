using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IlevelInterface
{	
	private static float EPSILON = 4.1f;
	private static float EPSILON_HOLE = 10.1f;
	
	private GameObject surface;
	
	private GameObject robot;
	Vector3 robotProjectToPlane;
	
	private GameObject[] simpleSurfaceArray ;
	private GameObject[] pillarArray ;
	private GameObject[] fallArray ;
	
	private int activeCubeNumber = 0;
	
	private  List<Island> islandList = new List<Island>();
	private  List<Hole> holeArray = new List<Hole>();
	
	Coords[]  pointsCoordArray;
	Overlap[] overlapArray;
	
	private Vector3 worldTransform;
	private Vector3 planePosition;
	public Dictionary<string, string> oppositeDirections = new Dictionary<string, string>();	
	
	public void levelInitialization()
	{		
		oppositeDirections["LEFT"]    = "RIGHT";
		oppositeDirections["RIGHT"]   = "LEFT";
		oppositeDirections["FORWARD"] = "BACK";
		oppositeDirections["BACK"]    = "FORWARD";
		
		robot = GameObject.FindWithTag("Robot");		
		
		simpleSurfaceArray  = GameObject.FindGameObjectsWithTag("surface");
   		pillarArray         = GameObject.FindGameObjectsWithTag("pillar");
 	    fallArray         = GameObject.FindGameObjectsWithTag("hole");
		
		addSurfacesToList(simpleSurfaceArray);
		addSurfacesToList(pillarArray);
		
		//for(int i = 0; i< fallArray.Length; i++)				
		//	islandList.Add(new IslandFall(fallArray[i]));	
		addSurfacesToList(fallArray);
		
		setIdsToSurfaces();	
		
		pointsCoordArray = new Coords[islandList.Count];		
		
		setStrongLinks();
		
		setInitIsland ();
	}

	public void setInitIsland( )
	{
		GameEvents.InitiateEvent("setRobotIsland",islandList[0]);
	}
	private void setStrongLinks( )
	{
		for(int i = 0; i < islandList.Count; i++)	
		{
			Island currentIsland = islandList[i];
			Vector3 currentIslandCenter    =  currentIsland.islandGameObject.renderer.bounds.center;
			
			for(int k = 0; k < islandList.Count; k++)	
			{
				if ( i == k ) continue;
				Island compareIsland = islandList[k];
				Vector3 compareIslandCenter    =  compareIsland.islandGameObject.renderer.bounds.center;
				
				string direction = "";
			
				if (epsilon(currentIslandCenter.x,compareIslandCenter.x) && 
					epsilon(currentIslandCenter.y,compareIslandCenter.y) &&
					epsilon(currentIslandCenter.z,compareIslandCenter.z+1))				
					direction = "FORWARD";				
				
				else if(epsilon(currentIslandCenter.x,compareIslandCenter.x) && 
					epsilon(currentIslandCenter.y,compareIslandCenter.y) &&
					epsilon(currentIslandCenter.z,compareIslandCenter.z-1))				
					direction = "BACK";
					
				else if(epsilon(currentIslandCenter.x,compareIslandCenter.x-1) && 
					epsilon(currentIslandCenter.y,compareIslandCenter.y) &&
					epsilon(currentIslandCenter.z,compareIslandCenter.z))				
					direction = "RIGHT";
				
				else if(epsilon(currentIslandCenter.x,compareIslandCenter.x+1) && 
					epsilon(currentIslandCenter.y,compareIslandCenter.y) &&
					epsilon(currentIslandCenter.z,compareIslandCenter.z))				
					direction = "LEFT";
				
				if (direction!="")									
					currentIsland.pushContact(direction,compareIsland,true);
				
			}
		}
	}
	private void addSurfacesToList(GameObject[] array)
	{
		for(int i = 0; i< array.Length; i++)				
			islandList.Add(new Island(array[i]));		
	}
	private void setIdsToSurfaces()
	{
		for(int i = 0; i < islandList.Count; i++)				
			islandList[i].id  = i;	
	}	
	public void createWorldCurrentProjection(Vector3 plane, Vector3 world)
	{		
		worldTransform = world;
		planePosition = plane;
		
		resetSurfacePositions();
		clearAllContacts();
		
		calculateSurfaceProjections();	
		calculateOverlapProjections();
		
		//for(int i = 0; i< islandList.Count; i++)
		calculateContacts(activeCubeNumber);
	}
	public void createWorldCurrentProjectionForFall(Vector3 plane, Vector3 world)
	{
		worldTransform = world;
		planePosition = plane;
		
		resetSurfacePositions();
		clearAllContacts();
		
		calculateSurfaceProjections();	
		//calculateOverlapProjections();
		//wideSurfaceProjection();
		
		calculateRobotProjection();
		
		isRobotFallIntoSurface(); 
	}
	
	public void calculateRobotProjection()
	{	
		Vector3 toProject = new Vector3(robot.transform.position.x,
										 robot.transform.position.y - 0.5f,
										 robot.transform.position.z);
		robotProjectToPlane = projectToPlane(toProject);
		
		Debug.DrawRay(robotProjectToPlane, new Vector3(0,300,0));	
	}
	
	public void isRobotFallIntoSurface()
	{
		for(int i = 0; i<pointsCoordArray.Length; i++)	
		{
			if ( i == activeCubeNumber) continue;
			Coords surf =  pointsCoordArray[i];
			
			if (isInsideSquare(surf.p1,
							   surf.p2,
							   surf.p3,
							   surf.p4,
							   robotProjectToPlane) ||
				isInsideSquare(surf.p1,
							   surf.p4,
							   surf.p3,
							   surf.p2,
							   robotProjectToPlane))
				{					
					activeCubeNumber = i;	
					GameEvents.InitiateEvent("setRobotIsland",islandList[i]);
					GameEvents.InitiateEvent("robotStateChanged_Step",null);
					break;					
				}
		}      
	}
	public bool isInsideSquare(Vector3 A,Vector3 B,Vector3 C,Vector3 D,Vector3 P)
	{
       if (triangleArea(A,B,P)>0 || triangleArea(B,C,P)>0 || triangleArea(C,D,P)>0 || triangleArea(D,A,P)>0)
          return false;
		
	  return true;		
    }
	public float triangleArea(Vector3 A,Vector3 B, Vector3 C)
	{
       return (C.x*B.y-B.x*C.y)-(C.x*A.y-A.x*C.y)+(B.x*A.y-A.x*B.y);   
	}		
	public void wideSurfaceProjection()
	{	
		
	}	
	public void resetSurfacePositions()
	{
		for(int i = 0; i < islandList.Count; i++)				
			islandList[i].resetPosition();
	}
	public void clearAllContacts()
	{
		for(int i = 0; i < islandList.Count; i++)				
			islandList[i].clearContacts();
	}
	public void calculateOverlapProjections()
	{		
		overlapArray= new Overlap[pillarArray.Length];
		int index = 0;
		
		for(int i = 0; i<pillarArray.Length; i++)
		{	
			GameObject pillar  = pillarArray[i];
			
			var m = pillar.transform.localToWorldMatrix;
			Mesh theMesh   = pillar.GetComponent<MeshFilter>().mesh as Mesh;
			
			Overlap overlap = drawBalkaProj(  	   m.MultiplyPoint3x4(theMesh.vertices[4]),
												   m.MultiplyPoint3x4(theMesh.vertices[2]),
												   m.MultiplyPoint3x4(theMesh.vertices[3]),
												   m.MultiplyPoint3x4(theMesh.vertices[5]),
				
										   		   m.MultiplyPoint3x4(theMesh.vertices[0]),
												   m.MultiplyPoint3x4(theMesh.vertices[1]),
												   m.MultiplyPoint3x4(theMesh.vertices[18]),
												   m.MultiplyPoint3x4(theMesh.vertices[20])
												   );
			overlapArray[index++] = overlap;
		}	
	}
	Overlap drawBalkaProj(Vector3 a1,Vector3 a2,Vector3 a3,Vector3 a4,Vector3 b1,Vector3 b2,Vector3 b3,Vector3 b4)
	{ 	
	 	a1 = 	projectToPlane(a1);
	  	a2 = 	projectToPlane(a2);
	    a3 = 	projectToPlane(a3);
	    a4 = 	projectToPlane(a4);		
	    
	    b1 = 	projectToPlane(b1);
	  	b2 = 	projectToPlane(b2);
	    b3 = 	projectToPlane(b3);
	    b4 = 	projectToPlane(b4);	
	
		Overlap overlap = new Overlap();
	
	    float param;
	     
	     param                = Mathf.Max(a1.y,a2.y,a3.y,a4.y);
	     overlap.max_y        = findVectorY(a1,a2,a3,a4,param); 
	     
	     param                = Mathf.Max(a1.x,a2.x,a3.x,a4.x);
	     overlap.top_right    = findVectorX(a1,a2,a3,a4,param);    
	     
	  	 param    			  = Mathf.Min(a1.x,a2.x,a3.x,a4.x);
	     overlap.top_left     = findVectorX(a1,a2,a3,a4,param);     
	     
	     param   			  = Mathf.Min(b1.y,b2.y,b3.y,b4.y);
	     overlap.min_y        = findVectorY(b1,b2,b3,b4,param); 
	     
	     param   			  = Mathf.Max(b1.x,b2.x,b3.x,b4.x);
	     overlap.bottom_right = findVectorX(b1,b2,b3,b4,param);  
	     
	     param   			  = Mathf.Min(b1.x,b2.x,b3.x,b4.x);
	     overlap.bottom_left  = findVectorX(b1,b2,b3,b4,param); 
	     
		
	     Debug.DrawLine(overlap.max_y ,overlap.top_left,Color.blue);
	     Debug.DrawLine(overlap.top_left,overlap.bottom_left,Color.white);
	     Debug.DrawLine(overlap.bottom_left,overlap.min_y,Color.blue);
	     Debug.DrawLine(overlap.min_y,overlap.bottom_right,Color.blue);
	     Debug.DrawLine(overlap.bottom_right,overlap.top_right,Color.blue);
	     Debug.DrawLine(overlap.top_right,overlap.max_y,Color.blue);
	     
		 return overlap;        
	}
	Vector3 findVectorY(Vector3 a1,Vector3 a2,Vector3 a3,Vector3 a4,float param)
	{
		if (a1.y==param) return a1;
		if (a2.y==param) return a2;
		if (a3.y==param) return a3;
		if (a4.y==param) return a4;
		return new Vector3(0,0,0);
	}
	Vector3 findVectorX(Vector3 a1,Vector3 a2,Vector3 a3,Vector3 a4,float param)
	{
		if (a1.x==param) return a1;
		if (a2.x==param) return a2;
		if (a3.x==param) return a3;
		if (a4.x==param) return a4;
		return new Vector3(0,0,0);
	}
	
	/// <summary>
	/// Calculates the surface projections.
	/// </summary>//
	public void calculateSurfaceProjections()
	{			
		int index = 0;
		
		for(int i = 0; i< islandList.Count; i++)
		{			
			GameObject surf  = islandList[i].islandGameObject;			
			Matrix4x4 m = surf.transform.localToWorldMatrix;
			Mesh theMesh   = surf.GetComponent<MeshFilter>().mesh as Mesh;
			
			lineRendererUpdate(m.MultiplyPoint3x4(theMesh.vertices[4]),
							   m.MultiplyPoint3x4(theMesh.vertices[2]),
							   m.MultiplyPoint3x4(theMesh.vertices[3]),
							   m.MultiplyPoint3x4(theMesh.vertices[5]),
							   index,
							   islandList[i].id);
			index++;
		}
		
	}
	private void lineRendererUpdate(Vector3 a,Vector3 b,Vector3 c,Vector3 d,int index, int id)
	{  	
	 	a = 	projectToPlane(a);
	  	b = 	projectToPlane(b);
	    c = 	projectToPlane(c);
	    d = 	projectToPlane(d);
		
	    pointsCoordArray[index] = new Coords(a,b,c,d);
		pointsCoordArray[index].id = id;

	    if (index == activeCubeNumber)
	    {			
	    	 Debug.DrawLine(a,b,Color.white);
	    	 Debug.DrawLine(b,c,Color.black);
	    	 Debug.DrawLine(c,d,Color.gray);
	    	 Debug.DrawLine(d,a,Color.blue);
	    }
	    else    
	    {
	    	 Debug.DrawLine(a,b,Color.green);
	    	 Debug.DrawLine(b,c,Color.red);
	    	 Debug.DrawLine(c,d,Color.yellow);
	    	 Debug.DrawLine(d,a,Color.magenta);
	    }    
	}
	private Vector3 projectToPlane(Vector3 a)
	{		
		Vector3 normal = (planePosition-worldTransform);	
			normal.Normalize();
		
		float dist = Mathf.Abs(-worldTransform.z+planePosition.z);
		if (dist<1) dist = 4;
		
		Vector3 p  = worldTransform + normal*dist*0.5f;	
		Vector3 prj = a - Vector3.Dot(a-p,normal)*normal;		
		return Camera.main.WorldToScreenPoint(prj);
	}
	
	public void calculateContacts(int id)
	{		
		// id = activeCubeNumber;
		Island currentIsland = islandList[id];
		currentIsland.clearContacts();
		
		for(int i = 0; i<pointsCoordArray.Length; i++)	
		{
			if (i == activeCubeNumber) continue;
			
			string direction = "";
			
			if (epsilon3(pointsCoordArray[id].p3,pointsCoordArray[i].p2) && 
				epsilon3(pointsCoordArray[id].p4,pointsCoordArray[i].p1))				
				direction = "LEFT";			
			
			else if(epsilon3(pointsCoordArray[id].p2,pointsCoordArray[i].p1) && 
				epsilon3(pointsCoordArray[id].p3,pointsCoordArray[i].p4))				
				direction = "FORWARD";
			
			else if	(epsilon3(pointsCoordArray[id].p1,pointsCoordArray[i].p2) &&
				epsilon3(pointsCoordArray[id].p4,pointsCoordArray[i].p3))				
				direction = "BACK";
			
			else if	(epsilon3(pointsCoordArray[id].p1,pointsCoordArray[i].p4) &&
				epsilon3(pointsCoordArray[id].p2,pointsCoordArray[i].p3))				
				direction = "RIGHT";			
			
			if (direction!="" && compareRelationPosition(id,i,direction))	
			{
				currentIsland.pushContact(direction,islandList[i],false);
				islandList[i].pushContact(oppositeDirections[direction],currentIsland,false);				
				
				//int idContactSurface = pointsCoordArray[i].id;
				//islandList[idContactSurface].setPosition(currentIsland,direction);
				
				
				
				/*Island son =  islandList[idContactSurface].getContactIsland("LEFT");
				if (son!=null)
				{
					son.setPosition(islandList[idContactSurface],"LEFT");
				}
				son =  islandList[idContactSurface].getContactIsland("RIGHT");
				if (son!=null)
				{
					son.setPosition(islandList[idContactSurface],"RIGHT");
				}
				son =  islandList[idContactSurface].getContactIsland("FORWARD");
				if (son!=null)
				{
					son.setPosition(islandList[idContactSurface],"FORWARD");
				}
				son =  islandList[idContactSurface].getContactIsland("BACK");
				if (son!=null)
				{
					son.setPosition(islandList[idContactSurface],"BACK");
				}*/
				
			}
			
		}
		////////////////////////////////
	
		
		Hole hole;
		holeArray.Clear();
		for(int i = 0; i<pointsCoordArray.Length; i++)	
		{
			if (i == id) continue;
			
			if (epsilonHole(pointsCoordArray[id].p3,pointsCoordArray[id].p2,pointsCoordArray[i].p2))
			{
				hole = new Hole();
				hole.index   = i;
				hole.contact   = "LEFT";///!!!!
				
				hole.vertices = new Vector3[4];
				hole.vertices[0]=pointsCoordArray[id].p3;
				hole.vertices[1]=pointsCoordArray[i].p2; 
				hole.vertices[2]=pointsCoordArray[i].p1;
				hole.vertices[3]=pointsCoordArray[id].p4;
				
				holeArray.Add(hole);				
				draw_hole(hole.vertices);		
			}
			else if	(epsilonHole(pointsCoordArray[id].p1,pointsCoordArray[id].p4,pointsCoordArray[i].p4))
			{				
				hole           = new Hole();
				hole.index     = i;		
				hole.contact   = "RIGHT";//!!!!
				
				hole.vertices = new Vector3[4];
				hole.vertices[0]=pointsCoordArray[id].p1;
				hole.vertices[1]=pointsCoordArray[i].p4;
				hole.vertices[2]=pointsCoordArray[i].p3;
				hole.vertices[3]=pointsCoordArray[id].p2;	
				
				holeArray.Add(hole);	
				draw_hole(hole.vertices);				
			}
			else if(epsilonHole(pointsCoordArray[id].p2, pointsCoordArray[id].p1,pointsCoordArray[i].p1))
			{			
				hole = new Hole();
				hole.index   = i;	
				hole.contact   = "BACK";
				
				hole.vertices = new Vector3[4];
				hole.vertices[0]=pointsCoordArray[id].p2;
				hole.vertices[1]=pointsCoordArray[i].p1;
				hole.vertices[2]=pointsCoordArray[i].p4;
				hole.vertices[3]=pointsCoordArray[id].p3;				
				
				holeArray.Add(hole);	
				draw_hole(hole.vertices);				
			}
			else if	(epsilonHole(pointsCoordArray[id].p1,pointsCoordArray[id].p2,pointsCoordArray[i].p2))
			{						
				hole = new Hole();
				hole.index     = i;
				hole.contact   = "FORWARD";
				
				hole.vertices = new Vector3[4];
				hole.vertices[0]=pointsCoordArray[id].p1;
				hole.vertices[1]=pointsCoordArray[i].p2;
				hole.vertices[2]=pointsCoordArray[i].p3;
				hole.vertices[3]=pointsCoordArray[id].p4;	
				
				holeArray.Add(hole);		
				draw_hole(hole.vertices);			
			}		
		}
		
		for(int i=0; i<holeArray.Count; i++)					
			for(int j=0;j < overlapArray.Length; j++)			
				if (pointsIN(holeArray[i],overlapArray[j]))
				{		
					currentIsland.pushContact(holeArray[i].contact,islandList[holeArray[i].index],false);
					islandList[holeArray[i].index].pushContact(oppositeDirections[holeArray[i].contact],currentIsland,false);
				}

	}
	bool compareRelationPosition(int idCurrent,int idConnecting,string direction)
	{
		Vector3 vecCurrent   = islandList[idCurrent].islandGameObject.transform.position;
		Vector3 vecConnectin = islandList[idConnecting].islandGameObject.transform.position;
		bool good = false;
		
		switch(direction)
		{
			case "LEFT":								
				if(vecCurrent.x>vecConnectin.x)	good = true;			
			break;
			case "RIGHT":	
				if(vecCurrent.x<vecConnectin.x)	good = true;		
			break;
			case "FORWARD":			
				if(vecCurrent.z<vecConnectin.z)	good = true;				
			break;
			case "BACK":			
				if(vecCurrent.z>vecConnectin.z)	good = true;				
			break;
		}	
	//	Debug.Log ("GOOD? "+good);
		
		return good;
		
	
	}
	bool pointsIN(Hole hole,Overlap overlap)
	{
		for(int i = 0; i < 4; i++)
		{
			if (!( overlap.max_y.y>hole.vertices[i].y &&overlap.min_y.y<hole.vertices[i].y &&
				 overlap.top_left.x<hole.vertices[i].x &&overlap.top_right.x>hole.vertices[i].x &&
				  overlap.bottom_left.x<hole.vertices[i].x &&overlap.bottom_right.x>hole.vertices[i].x))
			return false;
			 
		}
		return true;
	}
	
	public void setActiveIsland(Island island)
	{
		activeCubeNumber = island.id;		
	}
	public void createMapForCurrentSurface()
	{
		
	}
	private bool  epsilon3(Vector3 a,Vector3 b)
	{
		Vector3 c = (a-b);
		return (Mathf.Abs(c.x)<EPSILON &&Mathf.Abs(c.y)<EPSILON && Mathf.Abs(c.z)<EPSILON);		
	}
	private bool  epsilon(float a,float b)
	{	
		return Mathf.Abs(a-b)<0.01f ;		
	}
	bool epsilonHole(Vector3 a1, Vector3 a2,Vector3 c)
	{
		float A1 = a1.y-a2.y;
		float B1 = a2.x-a1.x;
		
		float A2 = a1.y-c.y;
		float B2 = c.x-a1.x;
		
		float cosAngle = (A1*A2+B1*B2)/(Mathf.Sqrt(A1*A1+B1*B1)*Mathf.Sqrt(A2*A2+B2*B2));
		float angleInDeegrees = Mathf.Acos(cosAngle)* Mathf.Rad2Deg;
		
		return (Mathf.Abs(180-Mathf.Abs(angleInDeegrees))<EPSILON_HOLE);			
	}
	private void draw_hole(Vector3[] a)
	{
		Debug.DrawLine(a[0],a[1],Color.white);
		Debug.DrawLine(a[1],a[2],Color.white);
		Debug.DrawLine(a[2],a[3],Color.white);
		Debug.DrawLine(a[3],a[0],Color.white);    	 
	}
}

class Coords
{
   public Vector3 p1 ;
   public Vector3 p2 ;
   public Vector3 p3 ;
   public Vector3 p4 ;
   public int id ;
	
   public Coords(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
   {
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
		this.p4 = p4;
   }
}

class Overlap
{
   public Vector3 max_y;
   public Vector3 min_y;
   public Vector3 top_left;
   public Vector3 top_right;
   public Vector3 bottom_left;
   public Vector3 bottom_right;  
}
class Hole
{ 
   public string contact;
   public int index;
   public Vector3[] vertices =  null;
}
