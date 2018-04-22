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
        if(lastOn != on)
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
