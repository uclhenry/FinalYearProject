using UnityEngine;
using System.Collections;
using System.Linq;
using Vuforia;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System;
using System.IO;

public class DynamicDataSetLoader : MonoBehaviour
{
    public bool isTracking = false;
    string LastPath = "";
    Vector3 lastHitPosition = Vector3.zero;
    Vector3 displacement = Vector3.zero;
    float moveX = 0.0f;
    float moveY = 0.0f;
    public bool isMneuOpen = false; 
    int poiMenu = 1;
    int sceneMenu = 2;
    int contentMenu = 3;
    int currentMenu = 1;
    int imgContent = 1;
    int audioContent = 2;
    int textContent = 3;
    int currentContentType = 0;
    POI currentPOI = null;
    ContentContainer currentContentContainer = null;
    Content currentContent = null;
    float deg;
    private string _result;
 
    // specify these in Unity Inspector
    public GameObject canvasForPoi = null;  // you can use teapot or other object
    public GameObject secondObject = null;
    public string dataSetName = "";  //  Assets/StreamingAssets/QCAR/DataSetName
    public bool once = false;
    public int i = 0;
    public GameObject arrow = null;
    public Area area;
    public Dictionary<String, int> SceneToRendered = new Dictionary<String, int>();
    public int SceneIndex = 0;
    public GameObject ex;
    public List<Content> contentList;
    public List<POI> seenPOIs;
    public bool isDisplayContent = false;
    public Texture aTexture;
    // Use this for initialization
    void Start()
    {
        string Source = SceneTools.GetSettingValue("Source");
        if (Source == "") Source = "Test";
        if (Source == "Test") dataSetName = "UCL";
        else dataSetName = "Realdata";

        contentList = new List<Content>();
        seenPOIs = new List<POI>();
        //AddedPOIs = new List<POI>();
        Input.compass.enabled = true;
        //convert xml into class
        area = Area.Load("download");
        CreateTrackerPois();

        //StartCoroutine(LoadXML());
        // Vuforia 6.2+
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(LoadDataSet);
        linkToVuforiaTarget();

    }
    void updateSeenContent() {
        foreach (POI p in seenPOIs) {

                foreach (ContentContainer container in p.ContentContainers)
                {
                    foreach (Content content in container.Contents)
                    {
                        contentList.Add(content);
                    }
                }
        }
    }

    void updatePoi() {
        float degree = Input.compass.magneticHeading;
        Debug.Log(degree);
        //deg = 180f;
        double angle = Math.PI * degree / 180.0;
        double sinAngle = Math.Sin(-1 * angle);
        double cosAngle = Math.Cos(-1 * angle);
        //s = os = oa + as = x cos(theta) + y sin(theta)
        //t = ot = ay ¨C ab = y cos(theta) ¨C x sin(theta)
        Area a = area;
        float currentX = (float)a.POIs[0].GetGPSlocation().longitude;
        float currentZ = (float)a.POIs[0].GetGPSlocation().latitude;
        foreach (POI p in a.POIs)
        {
            Debug.Log("updating!");

            float x = (float)p.GetGPSlocation().longitude - currentX;
            float z = (float)p.GetGPSlocation().latitude - currentZ;
            float s = (float)(x * cosAngle + z * sinAngle);
            float t = (float)(z * cosAngle - x * sinAngle);
            p.GetARgo().transform.position = new Vector3(500000f * s, 0f, 500000f * t);
        }
    }
    void CreateTrackerPois()
    {
        GameObject areaObject = new GameObject();
        areaObject.name = "[Trackers]";
        int i = 0;
        foreach (POI poi in area.POIs)
        {
            poi.Instantiate(areaObject, ++i);
            SceneToRendered.Add(poi.Name, 0);
            Debug.Log(poi.Id);
        }
        foreach (var p in SceneToRendered) {
            Debug.Log(p);
        }

        //CreatePoi();
    }

    void linkToVuforiaTarget() {
        foreach (POI p in area.POIs)
        {
            String vuforiaTarget = "DynamicImageTarget-" + p.Id;
            GameObject go = GameObject.Find(vuforiaTarget);
            if (go == null) continue;
            Transform vuforiaPoi = go.transform;
            Transform display = GameObject.Find("Poi_" + p.Name).transform;
            display.SetParent(vuforiaPoi);
        }
    }
    void HideScenes() {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        foreach (POI p in area.POIs) {
            foreach (ContentContainer container in p.ContentContainers)
            {
                container.GetSceneGameObject().transform.SetParent(hiddenScenes);
            }

        }
    }
    void AddScene2handler() {
        Debug.Log("========= add to handler ===========");
        foreach (POI p in area.POIs) {
            Debug.Log("========= p ===========");
            String vuforiaTarget = "DynamicImageTarget-" + p.Id;

            GameObject poi = GameObject.Find(vuforiaTarget);
            if (poi == null)
            {
                Debug.Log("========= no poi for " + p.Id + " ===========");
                continue;

            }
            DefaultTrackableEventHandler handler = poi.transform.GetComponent<DefaultTrackableEventHandler>();
            handler.PoiGameObject = poi;
            if (handler != null)
                foreach (ContentContainer scene in p.ContentContainers)
                {
                    handler.scenelist.Add(scene.GetSceneGameObject());//Debug.LogWarning("adding "+scene.Name);
                }

        }
    }


    void LoadDataSet()
    {

        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataSetName))
        {

            objectTracker.Stop();  // stop tracker so that we can add new dataset

            if (!objectTracker.ActivateDataSet(dataSet))
            {
                // Note: ImageTracker cannot have more than 100 total targets activated
                Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
            }

            if (!objectTracker.Start())
            {
                Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
            }

            int counter = 0;

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {

                    // change generic name to include trackable name
                    tb.gameObject.name = "DynamicImageTarget-" + tb.TrackableName; ++counter;

                    // add additional script components for trackable
                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    tb.gameObject.AddComponent<TurnOffBehaviour>();

                    if (canvasForPoi != null)
                    {
                        // instantiate augmentation object and parent to trackable
                        GameObject augmentation = (GameObject)GameObject.Instantiate(canvasForPoi);
                        //augmentation.transform.parent = tb.gameObject.transform;
                        augmentation.transform.SetParent(tb.gameObject.transform);
                        augmentation.transform.localPosition = new Vector3(0f, 0f, 0f);
                        Quaternion target = Quaternion.Euler(90f, 0, 0);
                        augmentation.transform.localRotation = target;// Quaternion.identity;

                        augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                        augmentation.gameObject.SetActive(true);
                        augmentation.name = tb.TrackableName;

                        foreach (POI p in area.POIs)
                        {//if id from xml from server == image name from vuforia
                            if (p.Id == tb.TrackableName)
                            {
                                p.SetinfoBoard(augmentation);
                                p.openBoard();
                                Debug.Log("Info is seted up");
                                DefaultTrackableEventHandler h = tb.gameObject.GetComponent<DefaultTrackableEventHandler>();
                                h.poiForScene = p;

                            }
                        }
                        //GameObject second = (GameObject)GameObject.Instantiate(secondObject);
                        //second.transform.parent = tb.gameObject.transform;
                        //second.transform.localPosition = new Vector3(0f, 0f, 0f);
                        //second.gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("<color=yellow>Warning: No augmentation object specified for: " + tb.TrackableName + "</color>");
                    }

                }
            }
        }
        else
        {
            Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
        }
    }
    void RenderText(Transform board, POI p) {
        Text Name = board.GetChild(0).gameObject.GetComponent<Text>();
        Name.text = p.Name;// board.name;
        //Debug.Log("point" + p.Name);
        Text location = board.GetChild(1).gameObject.GetComponent<Text>();
        Text Id = board.GetChild(2).gameObject.GetComponent<Text>();
        Text UserId = board.GetChild(3).gameObject.GetComponent<Text>();
        Text TargetHeight = board.GetChild(4).gameObject.GetComponent<Text>();
        Text Latitude = board.GetChild(5).gameObject.GetComponent<Text>();
        Text Longitude = board.GetChild(6).gameObject.GetComponent<Text>();
        //find the poi with name = board.name
        //get the location and other infomation
        location.text = "locate at " + p.Latitude.ToString("0.00") + " ," + p.Longitude.ToString("0.00");
        Id.text = "id : " + p.Id;
        UserId.text = "user id : " + p.userId;
        TargetHeight.text = "Target Height :" + p.TargetHeight;
        Latitude.text = "latitude : " + p.Latitude.ToString("0.00000");
        Longitude.text = "longitude :" + p.Longitude.ToString("0.00000");
        p.rendered = true;
    }
    void Update()
    {

            if (once == false)
        {
            AddScene2handler();

            HideScenes();
            once = true;
        }
        if (area != null)
            if (area.POIs != null)
            {

                //updatePoi();
                updateSeenContent();
            }

        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();
        foreach (TrackableBehaviour tb in tbs)
        {
            //board from tb
            //find the p with same name 
            // tb.TrackableName == point Name
            Transform board = tb.gameObject.transform.GetChild(0);
            //POI p = null;
            if (area.POIs != null) {
                //int i = 0;
                foreach (POI p in area.POIs)
                {//if id from xml from server == image name from vuforia
                    if (p.Id == tb.TrackableName)
                    {
                        if (p.rendered == false) {
                            RenderText(board, p);

                            p.rendered = true;
                        }
                    }

                }
            }


        }


    }
    void DisplayLoadState() {
        bool ready = false;
        if (area != null) ready = true;
        if (ready)
        {

            DrawText(new Vector2(Screen.width * 0.80f, Screen.height * 0.95f), "Area ready");
        }
        else {
            DrawText(new Vector2(Screen.width * 0.80f, Screen.height * 0.95f), "Area  not ready");
        }
    }
    void DrawText(Vector2 screen, String s)
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 30;
        titleStyle.normal.textColor = new Color(46f / 256f, 163f / 256f, 256f / 256f, 256f / 256f);
        if (isMapOn() == false)
            GUI.Label(new Rect(screen.x, screen.y, 500, 200), s, titleStyle);
    }
    bool isMapOn()
    {
        Transform map = GameObject.Find("[Map]").transform;
        return map.GetComponent<MapSwitch>().on;
    }
    //IEnumerator LoadXML()
    //{

    //    WWW www = new WWW(SceneTools.VisAgeXml());
    //    yield return www;
    //    _result = www.text;
    //    readExampleXml();

    //}

    void listenTouch()
    {

        bool panning = false;
        bool panningStopped = false;
        Vector3 screenPosition = Vector3.zero;

        if (Application.platform == RuntimePlatform.IPhonePlayer
    || Application.platform == RuntimePlatform.Android)
        {

            int touchCount = UnityEngine.Input.touchCount;
            if (touchCount > 0)
            {
                // movements
                panning = true;
                panningStopped = true;

                int validTouchCount = touchCount;
                foreach (Touch touch in UnityEngine.Input.touches)
                {

                    if (touch.phase == TouchPhase.Began) {
                        screenPosition = new Vector3(touch.position.x, touch.position.y);
                        lastHitPosition = screenPosition;
                        displacement += Vector3.zero;
                    }
                        if (touch.phase == TouchPhase.Moved)
                    {
                        Vector3 tempPosition1 = new Vector3(touch.position.x, touch.position.y);
                        //Vector3 tempPosition2 = Input.GetTouch(1).position;
                        screenPosition = new Vector3(touch.position.x, touch.position.y);
                        
                        displacement += screenPosition - lastHitPosition;

                        lastHitPosition = screenPosition;
                        panningStopped = false;
                       
                    } break;
   

 
                }
                if (panningStopped)
                    panning = false;
            }
        }
    }
    void OnGUI() {
        //NextSceneButton();
        DisplayLoadState();
        if (isMapOn() == false)
        {
            //ButtonAngleDown();
            //ButtonAngleUp();
            if(!SceneTools.isTracking)
            ButtonViewContent();

        }
        //Debug.Log(isDisplayContent);
        if (isDisplayContent)
        {
            currentContentType = 0;
            HistoryMenu();
            if(currentContent!=null)
            switch (currentContent.Type) {
                case PoiDataType.texture2D:
                        currentContentType = imgContent;
                        listenTouch();
                    SetImageContent(currentContent);

                    GUI.DrawTexture(new Rect((float)0.1*Screen.width +displacement.x, (float)0.1 * Screen.width-displacement.y, (float)0.8 * Screen.width, (float)0.8 * Screen.width), aTexture, ScaleMode.ScaleToFit, true, 0);
                    break;
                case PoiDataType.text:
                        currentContentType = textContent;
                    ShowTextContent(currentContent.Description);
                    break;
                case PoiDataType.audio:
                        currentContentType = audioContent;
                    AudioSource AudioFile = currentContent.GetMediaGo().transform.GetComponent<Test>().Sound;
                    if (AudioFile != null)
                    {
                        //draw a button
                        ButtonMusicPlayer(AudioFile);
                    }
                    break;

                default:
                    break;
            }


        }
        else {
            currentMenu = 1;

        }
    }
    void ButtonMusicPlayer(AudioSource file)
    {
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = 30;
        Rect r = new Rect(0.8f * Screen.width, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        bool state = file.isPlaying;

        string text = "play";
        if (file.isPlaying == true) text = "pause";
        if (GUI.Button(r, text, titleStyle))
        {
            if (file.isPlaying)
            {
                file.Stop();
            }
            else {
                file.Play();
            }

        }
    }
    void ButtonAngleUp()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 30;
        Rect r = new Rect(0, 6f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "+", titleStyle))
        {
            deg += 10;

        }
        if (GUI.Button(r, "Historical\n POIs", titleStyle))
        {
            isDisplayContent = !isDisplayContent;
            Debug.Log("click");
        }

    }
    void ButtonAngleDown()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 30;
        Rect r = new Rect(0, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);
        if (GUI.Button(r, "-", titleStyle))
        {
            deg -= 10;

        }
    }
    void ButtonViewContent()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = 30;
        Rect r = new Rect(0, 7f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height);

    }
    void ContentGUI() {
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = 30;
        GUI.Label(new Rect(0, 0, 200, 20), "Recent Contents");
        Debug.Log("click se contents");
    }
    string ParagraghFormat(string str, int maxLength)
    {
        string[] words = str.Split(' ');
        List<int> breakPoints = new List<int>();
        int lineLength = 0;
        int index = 0;
        foreach (string s in words)
        {
            lineLength += s.Length;
            if (lineLength > maxLength)
            {
                lineLength = s.Length;
                breakPoints.Add(index - 1);
            }
            index += 1;
        }
        index = 0;
        string newstring = "";
        foreach (string s in words)
        {
            newstring += " ";
            newstring += s;
            if (breakPoints.Contains(index))
                newstring += "\n";
            index += 1;
        }
        return newstring;
    }
    void ShowTextContent(String content) {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;
        bb.normal.textColor = new Color(1, 0, 0);
        bb.fontSize = 50;
        GUI.Label(new Rect(50, 100, 200, 600), ParagraghFormat(content,30), bb);
    }
    Content FindLatestContent(PoiDataType type){


        Content example = null;
        foreach (Content c in contentList)
        {
            if (c.Type.Equals(type))
            {
                example = c;
            }
        }
        return example;
    }
    void SetImageContent(Content example) {

        string p = Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "/" + example.Description);
        if (LastPath == p) return;
        if (System.IO.File.Exists(p))
        {
            Texture2D texture = new Texture2D(512, 512);
            texture.LoadImage(System.IO.File.ReadAllBytes(p));
            aTexture = texture;
            LastPath = p;
            //GameObject.Find("RawImage").transform.GetComponent<RawImage>().texture  = texture;
        }
        else
        {
            Debug.LogWarning("Texture " + p + " does not exist");
        }
       
    }
    void HistoryMenu()
    {
        GUIStyle titleStyle = new GUIStyle("Button");
        titleStyle.fontSize = 35;
        int h = 100;
        float y = (currentMenu == 4) ? (float)0.8 * Screen.height:(float)0.6 * Screen.height;
        GUILayout.BeginArea(new Rect(200,y , 800, 1000));

        if (currentMenu == poiMenu)
        {
            if (seenPOIs == null || seenPOIs.Count == 0)
                GUILayout.Button("you have not been to any POIs", titleStyle, GUILayout.Height(h));
            foreach (POI poi in seenPOIs)//
            {//area.POIs
                if (GUILayout.Button(poi.Name,titleStyle, GUILayout.Height(h)))
                {
                    //Debug.Log("Clicked the " + poi.Name);
                    currentMenu += 1;
                    currentPOI = poi;
                }

            }

        }

        else if (currentMenu == sceneMenu)
        {

            foreach (ContentContainer cc in currentPOI.ContentContainers)
            {
                if (GUILayout.Button(cc.Name, titleStyle, GUILayout.Height(h)))
                {
                    Debug.Log("Clicked the " + cc.Name); currentMenu += 1;
                    currentContentContainer = cc;
                }

            }
            if (GUILayout.Button("Back", titleStyle, GUILayout.Height(h)))
            {
                currentMenu -= 1;
            }
        }
        else if (currentMenu == contentMenu)
        {
            foreach (Content c in currentContentContainer.Contents)
            {
                if (GUILayout.Button(c.Description, titleStyle, GUILayout.Height(h)))
                {
                    Debug.Log("Clicked the " + c.ToString()); currentMenu += 1;
                    currentContent = c;
                }

            }
            if (GUILayout.Button("Back", titleStyle, GUILayout.Height(h)))
            {
                currentMenu -= 1;
            }
        }
        else if(currentMenu == 4)
        {
            if (GUILayout.Button("Back", titleStyle, GUILayout.Height(h)))
            {
                currentMenu -= 1;
                currentContent = null;
                currentContentType = 0;

            }
        }
        GUILayout.EndArea();
    }
	    //public void readExampleXml()
    //{
    //    XElement result = XElement.Parse(_result);//

    //    IEnumerable<XElement> elements = from ele in result.Descendants("Pois").Elements("Poi")
    //                                     select ele;
    //    SetUpPois(elements);

    //}
    //public static void SetUpPois(IEnumerable<XElement> elements)
    //{
    //    Pois = new List<POI>();

    //    //Debug.Log("inside ShowInfoByElements");

    //    foreach (var ele in elements)
    //    {
    //        //Debug.Log("a loop");
    //        //Debug.Log(ele.Element("Name").Value);
    //        POI point = new POI();
    //        point.Name = ele.Element("Name").Value;
    //        point.Name = point.Name.Replace(".","");
    //        Debug.Log(point.Name.ToString());
    //        //point.Name = mod;
    //        point.Id = ele.Element("Id").Value;
    //        point.ImageTarget = ele.Element("ImageTarget").Value;
    //        point.TargetHeight = ele.Element("TargetHeight").Value;
    //        point.TargetWidth = ele.Element("TargetWidth").Value;
    //        //point.userId = ele.Element("userId").Value;
    //        String s = ele.Element("Latitude").Value;
    //        decimal d = 0;
    //        d = decimal.Round(decimal.Parse(s), 6);//.ToString();
    //        point.Latitude = (double)d;
    //        s = ele.Element("Longitude").Value;
    //        d = decimal.Round(decimal.Parse(s), 6);//.ToString();
    //        point.Longitude = (double)d;
    //        point.SimilarityThreshold = Convert.ToSingle(ele.Element("SimilarityThreshold").Value);
    //        Pois.Add(point);
    //    }

    //}
	    void CreatePoi()
    {
        foreach (POI p in area.POIs)
        {
            Debug.Log("creating!");
            GameObject go = Instantiate(ex);
            p.SetARgo(go);
            go.gameObject.SetActive(true);
            go.name = p.Name;
            go.transform.GetChild(0).GetComponent<Text>().text = p.Name;

        }
    }
}
