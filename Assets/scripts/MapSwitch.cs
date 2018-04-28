using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSwitch : Hidden {
    public bool lastOn = true;
    public bool on = false; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject MapCamera = GameObject.Find("Main Camera");
        if(MapCamera.GetComponent<Camera>().enabled == false)
            DisableRender();
        if (lastOn != on)
        {
            if (on)
            {
                enableRender();
            }
            else {
                DisableRender();
            }
            lastOn = on;
        }
        
	}
}
