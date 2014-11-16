using UnityEngine;
using System.Collections;

public class GameEvents 
{
	public delegate void EventDispatcher(string id, object obj);
	public static event EventDispatcher eventListener;

	public static void InitiateEvent(string gameEvent, object obj)
	{
		eventListener(gameEvent,obj);			
	}


}
