using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Island
{
	public int id;
	public GameObject islandGameObject;	
	public Dictionary<string, Contact> contacts = new Dictionary<string, Contact>();	
	private Vector3 position;
	
	public Island(GameObject go)
	{
		islandGameObject = go;	
		position = islandGameObject.transform.position;		
		
		newEmptyContact("LEFT");
		newEmptyContact("RIGHT");
		newEmptyContact("FORWARD");
		newEmptyContact("BACK");
		
		clearContacts();		
	}
	public bool havecontacts()
	{
		return false;
	}	
	public void newEmptyContact(string key)
	{
		contacts[key] = new Contact(null,false);
	}	
	
	public void clearContacts()
	{
		if (!contacts["LEFT"].isStrong)		 newEmptyContact("LEFT");
		if (!contacts["RIGHT"].isStrong)	 newEmptyContact("RIGHT");
		if (!contacts["FORWARD"].isStrong)	 newEmptyContact("FORWARD");
		if (!contacts["BACK"].isStrong) 	 newEmptyContact("BACK");
	}
	public void pushContact(string key, Island island, bool isStrongContact)
	{
		if (!contacts[key].isStrong)	
		   if (contacts[key].island == null)	
		    contacts[key] = new Contact(island,isStrongContact);
	}
	public Island getContactIsland(string key)
	{
		return contacts[key].island;
	}	
	public void setPosition(Island activeIsland, string direction)
	{
		/*Vector3 center    =  activeIsland.islandGameObject.renderer.bounds.center;
 		Vector3 extents   =  activeIsland.islandGameObject.renderer.bounds.extents;
		Vector3 extentsThis  =  islandGameObject.renderer.bounds.extents;
		
		float x = activeIsland.position.x;
		float y = activeIsland.position.y + (1.0f-islandGameObject.transform.localScale.y)*extents.y 
							 - (1.0f-activeIsland.islandGameObject.transform.localScale.y)*extentsThis.y;
		float z = activeIsland.position.z;	
		
		switch(direction)
		{
			case "LEFT":								
				x = center.x - 2*extents.x;		
			break;
			case "RIGHT":	
				x = center.x + 2*extents.x;				
			break;
			case "FORWARD":			
				z = center.z + 2*extents.z;				
			break;
			case "BACK":			
				z = center.z - 2*extents.z;			
			break;
		}
		
		islandGameObject.transform.position = new Vector3(x,y,z);*/
		
		//Debug.Log("direction "+direction);
		
		//if (contacts["LEFT"].isStrong && direction!="RIGHT")		 contacts["LEFT"].island.setPosition(this,"LEFT");
		//if (contacts["RIGHT"].isStrong && direction!="LEFT")	     contacts["RIGHT"].island.setPosition(this,"RIGHT");
		//if (contacts["FORWARD"].isStrong && direction!="BACK")		 contacts["FORWARD"].island.setPosition(this,"FORWARD");
		//if (contacts["BACK"].isStrong && direction!="FORWARD") 	     contacts["BACK"].island.setPosition(this,"BACK");
		
		
	}
	public void resetPosition()
	{
		
		islandGameObject.transform.position = position;
		
		/*if (contacts["LEFT"].island !=null)		 contacts["LEFT"].island.resetPosition();
		if (contacts["RIGHT"].island!=null)	     contacts["RIGHT"].island.resetPosition();
		if (contacts["FORWARD"].island!=null)	 contacts["FORWARD"].island.resetPosition();
		if (contacts["BACK"].island!=null) 	     contacts["BACK"].island.resetPosition();*/
	}
	//public Candy candy = null;
	//public ChangeReality changeReality = null;

}
public class Contact
{ 
   public Island island;
   public bool isStrong;
	
   public Contact(Island island, bool isStrong)
   {
		this.island = island;
		this.isStrong = isStrong;
   }
}