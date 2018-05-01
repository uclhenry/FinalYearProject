using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenMyMap : Hidden {
    public bool mapOn = true;
	// Use this for initialization
	void Start () {		
	}	
	// Update is called once per frame
	void Update () {
        GameObject parentGo = GameObject.Find("myui");
        Vector2 size = parentGo.GetComponent<RectTransform>().sizeDelta;
        Vector2 scale = parentGo.GetComponent<RectTransform>().localScale;
        if (Application.platform == RuntimePlatform.IPhonePlayer
        || Application.platform == RuntimePlatform.Android)
        {
            int touchCount = UnityEngine.Input.touchCount;
            if (touchCount > 0)
            {
                foreach (Touch touch in UnityEngine.Input.touches)
                    if (touch.phase == TouchPhase.Ended)
                    {
                        if (touch.position.x >= Screen.width - size.x * scale.y && touch.position.y >= Screen.height - size.y * scale.y)
                        {
                            if (mapOn)
                                DisableRender();
                            else {
                                enableRender();
                            }
                            mapOn = !mapOn;
                        }
                    }
            }
        }
        else {
            
            if (Input.mousePosition.x >= Screen.width - size.x * scale.y && Input.mousePosition.y >= Screen.height - size.y * scale.y&&Input.GetMouseButtonUp(0))
            {
                if (mapOn)
                    DisableRender();
                else
                {
                    enableRender();
                }
                mapOn = !mapOn;

            }
        }
    }
}
