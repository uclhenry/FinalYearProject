using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenCompass : Hidden {
    public bool on=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (on)
        {
            enableRender();
        }
        else {
            DisableRender();
        }
	}
}
