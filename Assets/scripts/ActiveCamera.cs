using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActiveCamera : MonoBehaviour
{
    public bool isMneuOpen = false;
    public GameObject ARCamera;
    public GameObject MapCamera;
    
	public int buttonFontSize = 40;
    public float changeInterval = 1.0f;
    Vector4 fontColor = Color.white;
    public string showStr = "";
    public float guiXScale;
    public float guiYScale;
    public Rect guiRect;

    void Start()
    {
        FindCamera();
        guiXScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.width : Screen.height) / 480.0f;
        guiYScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.height : Screen.width) / 640.0f;
        // setup the gui area
        guiRect = new Rect(4.0f * guiXScale, 64.0f * guiXScale, Screen.width / guiXScale - 32.0f * guiXScale, 32.0f * guiYScale);
        //Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        //GameObject.Find("[Map]").transform.parent = hiddenScenes;

    }

    //找到摄像机对象  
    void FindCamera()
    {
        ARCamera = GameObject.Find("ARCamera");//第一人称视角  
        MapCamera = GameObject.Find("Main Camera");//第三人称视角  
        MapCamera.GetComponent<Camera>().enabled = false;
        //MapCamera.SetActive(true);
        

    }
    Camera GetCamera(string name) {
        GameObject c = GameObject.Find(name);//第三人称视角  
     
        return c.GetComponent<Camera>();
    }
    void Update()
    {

    }

    void OnGUI()
    {
        ChangeCamera();
        ButtonToToggleView();
		if (isARenabled()==true){
			ButtonToSeeHistory();
			ButtonToClearContent();
			ButtonToAllContent();			
				
		}

    }
    void ChangeCamera()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetFalse();           
            enableCamera(ARCamera, true);
            showStr = "切换至第一人称视角";
        }

        else if (Input.GetKey(KeyCode.Alpha3))
        {
            SetFalse();
            enableCamera(MapCamera, true);
            showStr = "切换至第三人称视角";
        }
        //Debug.Log(showStr);
        GUILayout.Button(showStr);  
    }
    void enableCamera(GameObject go, bool enabled) {
        go.GetComponent<Camera>().enabled = enabled;
    }
    bool isARenabled() {
        return ARCamera.GetComponent<Camera>().enabled;
    }
    void ButtonToToggleView() {
        if (isMneuOpen == true) return;
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        Rect r = new Rect(0, 2f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);

        if (GUI.Button(r, "Change\n Mode", titleStyle)) {
            bool ARenabled = isARenabled();
            toggleCamera(!ARenabled, ARenabled);                

        }
    }
    void ButtonToSeeHistory()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        titleStyle.normal.textColor = fontColor;
        Rect r = new Rect(0, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "Historical\n POIs", titleStyle))
        {
            isMneuOpen = !isMneuOpen;
            ARCamera.GetComponent<DynamicDataSetLoader>().isDisplayContent = !ARCamera.GetComponent<DynamicDataSetLoader>().isDisplayContent;

        }
    }
    void  ButtonToClearContent()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        titleStyle.normal.textColor = fontColor;
        Rect r = new Rect(0.2f * Screen.width, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "Clear\n content", titleStyle))
        {
            
           // ARCamera.GetComponent<DynamicDataSetLoader>().AddedPOIs = new List<POI>();
            ARCamera.GetComponent<DynamicDataSetLoader>().seenPOIs = new List<POI>();
        }
    }

    void ButtonToAllContent()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        titleStyle.normal.textColor = fontColor;
        Rect r = new Rect(0.4f * Screen.width, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "All\n content", titleStyle))
        {

            ARCamera.GetComponent<DynamicDataSetLoader>().seenPOIs =  new List<POI>();
            foreach (POI p in ARCamera.GetComponent<DynamicDataSetLoader>().area.POIs)
                ARCamera.GetComponent<DynamicDataSetLoader>().seenPOIs.Add(p);
            
        }
    }
    void SwitchCompass(bool compassState) {
        GameObject go = GameObject.Find("Compass21");
        go.transform.GetComponentInChildren<Canvas>().enabled = compassState;
    }
    void toggleCamera(bool ar, bool map) {
        enableCamera(ARCamera, ar);
        enableCamera(MapCamera, map);
        SwitchMap(map);
        SwitchCompass(ar);
    }
    void SwitchMap(bool on) {
        Transform map = GameObject.Find("[Map]").transform;
        map.GetComponent<MapSwitch>().on = on;
    }
    void SetFalse()
    {
        toggleCamera(false,false);


    }



}
