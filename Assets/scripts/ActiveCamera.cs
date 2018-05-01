using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class ActiveCamera : MonoBehaviour
{
    Texture NearestPOITexture = null;
    string NearestPOIpath = "";
    bool isShowSetting = false;
    public bool isMneuOpen = false;
    public GameObject ARCamera;
    public GameObject MapCamera;
    bool isNearPOIOpen = false;
    Area area = null;
    public int buttonFontSize = 40;
    public float changeInterval = 1.0f;
    Vector4 fontColor = Color.white;
    public string showStr = "";
    public float guiXScale;
    public float guiYScale;
    public Rect guiRect;
    void DrawText(Vector2 screen, String s)
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = buttonFontSize;
        titleStyle.normal.textColor = fontColor;
        if (isARenabled() == true)
            GUI.Label(new Rect(screen.x, screen.y, 500, 200), s, titleStyle);
    }
    void Start()
    {
        area = GameObject.Find("ARCamera").GetComponent<DynamicDataSetLoader>().area;
        FindCamera();
        guiXScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.width : Screen.height) / 480.0f;
        guiYScale = (Screen.orientation == ScreenOrientation.Landscape ? Screen.height : Screen.width) / 640.0f;
        // setup the gui area
        guiRect = new Rect(4.0f * guiXScale, 64.0f * guiXScale, Screen.width / guiXScale - 32.0f * guiXScale, 32.0f * guiYScale);
        //Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        //GameObject.Find("[Map]").transform.parent = hiddenScenes;

    }

    void FindCamera()
    {
        ARCamera = GameObject.Find("ARCamera");//
        MapCamera = GameObject.Find("Main Camera");
        MapCamera.GetComponent<Camera>().enabled = false;
        //MapCamera.SetActive(true);
        

    }
    Camera GetCamera(string name) {
        GameObject c = GameObject.Find(name);//  
     
        return c.GetComponent<Camera>();
    }
    void Update()
    {
        if(area ==null) area = GameObject.Find("ARCamera").GetComponent<DynamicDataSetLoader>().area;
    }

    void OnGUI()
    {
        
        ChangeCamera();
        if(!SceneTools.isTracking)
        ButtonToToggleView();
        if (isARenabled()==true &&!SceneTools.isTracking){
			ButtonToSeeHistory();
			ButtonToClearContent();
			ButtonToAllContent();
            ButtonToNearestPOI();
            ButtonToReadSetting();
            ButtonToChangeSetting();
        }
        if (isShowSetting) {
            ShowSetting();
        }
        if(isNearPOIOpen)
        showNearestPOI();

    }
    void ShowSetting() {
        String s = "";
        foreach (var a in SceneTools.getSetting())
        {
            s = s+a+"\n";
        }
        DrawText(new Vector2(Screen.width * 0.2f, 100), s);
    }
    void ButtonToReadSetting() {
        if (isMneuOpen == true) return;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        Rect r = new Rect(0, 6f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);

        if (GUI.Button(r, "Log\n Setting", titleStyle))
        {
            SceneTools.printSetting();
            isShowSetting = !isShowSetting;
        }
    }
    void ButtonToChangeSetting()
    {
        if (isMneuOpen == true) return;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        Rect r = new Rect(0, 5f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);

        if (GUI.Button(r, "Change\n Setting", titleStyle))
        {
            //SceneTools.isTestMode = !SceneTools.isTestMode;
            if (SceneTools.CurrentSource == "realdata") {
                SceneTools.ChangeSetting("Test", "34", "false");
                SceneTools.CurrentSource = "Test";
            }
              
            else if (SceneTools.CurrentSource == "Test")
            {
                SceneTools.ChangeSetting("realdata", "34", "false");
                SceneTools.CurrentSource = "realdata";
            }
        }
    }
    void showNearestPOI() {       
        
            GPSlocation userPosition = new GPSlocation(51.5221, -0.131411);
            if (UnityEngine.Input.location.status == LocationServiceStatus.Running) {
                double longitude = Input.location.lastData.longitude;
                double latitude = Input.location.lastData.latitude;
                userPosition = new GPSlocation(latitude, longitude);
            }           
            if (area.POIs !=null)
            {

                POI nearestPoi = area.GetNearestPOI(userPosition);// area.POIs[0];
            string name = nearestPoi.Name;
            if (name.Contains("."))
                name = name.Split('.')[1];
                string s = "The nearest POI is " + name + "\n " + (int)(nearestPoi.GetDistance(userPosition)) + "m away.";
                DrawText(new Vector2(Screen.width *0.2f, Screen.height *0.6f),s);
                string p = Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "/" + "StreamingAssets"+"/" + nearestPoi.ImageTarget);
                if (NearestPOIpath != p)
                {
                    Debug.Log("different");
                    if (System.IO.File.Exists(p))
                    {
                         
                        Texture2D texture = new Texture2D(512, 512);
                        texture.LoadImage(System.IO.File.ReadAllBytes(p));
                        NearestPOITexture = texture;
                        NearestPOIpath = p;
                        //GameObject plane = GameObject.Find("NearPOI");
                        //plane.GetComponent<Renderer>().material.mainTexture = NearestPOITexture;
                        
                    }
                    else
                    {
                        Debug.LogWarning("Texture " + p + " does not exist");
                    }
                }
                GUI.DrawTexture(new Rect((float)0.1 * Screen.width, (float)0.1 * Screen.width, (float)0.8 * Screen.width, (float)0.8 * Screen.width), NearestPOITexture, ScaleMode.ScaleToFit, true, 0);

        }

        
    }
    void ButtonToNearestPOI() {
        if (isMneuOpen == true) return;
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        Rect r = new Rect(0, 4f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);

        if (GUI.Button(r, "Nearest\n POI", titleStyle))
        {
            isNearPOIOpen = !isNearPOIOpen;

        }
    }
    void ChangeCamera()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            SetFalse();           
            enableCamera(ARCamera, true);
            showStr = "";
        }

        else if (Input.GetKey(KeyCode.Alpha3))
        {
            SetFalse();
            enableCamera(MapCamera, true);
            showStr = "";
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
        Rect r = new Rect(0, Screen.height - 0.1f * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
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
        titleStyle.normal.textColor = fontColor;// 7f / 8 * Screen.height
        Rect r = new Rect(0.2f * Screen.width, Screen.height- 0.1f * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "Clear\n content", titleStyle))
        {            
            ARCamera.GetComponent<DynamicDataSetLoader>().seenPOIs = new List<POI>();
        }
    }

    void ButtonToAllContent()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = buttonFontSize;
        titleStyle.normal.textColor = fontColor;
        Rect r = new Rect(0.4f * Screen.width, Screen.height - 0.1f * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
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
        //SwitchCompass(ar);
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
