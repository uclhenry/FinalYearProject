using UnityEngine;
using System.Collections;

public class MapUI : MonoBehaviour 
{
	void Start () 
	{
	
	}
	
	void Update () 
	{
	
	}
	
	public void HidePanels()
	{
		BroadcastMessage("HidePanel");
	}
}
