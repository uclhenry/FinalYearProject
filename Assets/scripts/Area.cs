using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[XmlRoot("Area")]
public class Area
{
	public string Id;
	public string Name;
	public string ModifiedDate;

	[XmlArray("Pois"),XmlArrayItem("Poi")]
	public POI[] POIs;

 	//-------------------------------XML serializer/deserializer--------------------------------------------//
	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(Area));
		using(var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}
	
	public static Area Load(string mod)//Area 
	{
		string LastTime="";
		string LastTimeNew="";
        string Source = SceneTools.GetSettingValue("Source");
        if (Source == "") {
            Source = "realdata";
            SceneTools.ChangeSetting("realdata", "34", "false");
        }
        SceneTools.CurrentSource = SceneTools.GetSettingValue("Source");
        Debug.Log(Source);
        bool isExistDateFile = File.Exists(SceneTools.AreaZipLastTimeLocal());
        if (mod.Equals("download"))//update or download
        {
          
            if (Source == "realdata")
            {
                
                Debug.LogWarning("Downloading readdata");
                if (!File.Exists(SceneTools.AreaZipFileLocal()))//not exist zip
                {
                    SceneTools.ClearLocalData();
                    SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipLastTimeUrl(), SceneTools.AreaZipLastTimeLocal());
                    SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
                    new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                }
                //}
                else //the have zip,then check the update
                {
                    if (isExistDateFile)
                    {
                        LastTime = File.ReadAllText(SceneTools.AreaZipLastTimeLocal());

                        SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipLastTimeUrl(), SceneTools.AreaZipLastTimeLocal());

                        LastTimeNew = File.ReadAllText(SceneTools.AreaZipLastTimeLocal());

                        if (Convert.ToInt64(LastTimeNew) > Convert.ToInt64(LastTime))
                        {
                            //update
                            Debug.Log("Downloading new zip file");
                            SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());
                            new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                        }
                        else
                        {
                            //not gonna update
                            Debug.Log("Zip file is latest version");
                        }
                    }
                    else //unknown version ,so update   ,or last version is test
                    {
                        SceneTools.ClearLocalData();
                        Debug.Log("Zip not File.Exists (SceneTools.AreaZipLastTimeLocal())");
                        SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipLastTimeUrl(), SceneTools.AreaZipLastTimeLocal());
                        SceneTools.DownloadFileFromUrlSync(SceneTools.AreaZipFileUrl(), SceneTools.AreaZipFileLocal());//AreaZipFileUrl()
                        new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                    }
                }
            }
            else if (Source == "Test") {
                if (isExistDateFile)
                {//realdata
                    SceneTools.ClearLocalData();//delete realdata
                    SceneTools.DownloadFileFromUrlSync(SceneTools.testZip, SceneTools.AreaZipFileLocal());
                    new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                }
                else if (!File.Exists(SceneTools.AreaZipFileLocal())) {
                    SceneTools.ClearLocalData();//delete realdata
                    SceneTools.DownloadFileFromUrlSync(SceneTools.testZip, SceneTools.AreaZipFileLocal());
                    new ZipIt(SceneTools.AreaZipFileLocal(), "", Application.persistentDataPath);
                }

                
            }
            //{

        }
        SceneTools.ChangeSetting(SceneTools.CurrentSource, "34", "false");
        //convert the xml into a cs object (after downloading all the file or file is already)
        var serializer = new XmlSerializer(typeof(Area));

        using (var stream = new FileStream(Path.Combine(Application.persistentDataPath + "/" + SceneTools.AreaNameDefault(), SceneTools.AreaNameDefault() + ".xml"), FileMode.Open))
        {
            return serializer.Deserialize(stream) as Area;
        }
    }
    public POI GetNearestPOI(GPSlocation coordinate) {
        double minDistance = 99999999999f;
        POI minPOI = null;
        foreach (POI p in POIs) {
            if (p.GetDistance(coordinate) < minDistance)
            {
                minDistance = p.GetDistance(coordinate);
                minPOI = p;
            }
        }
        return minPOI;
    }
    //-------------------------end---XML serializer/deserializer--------------------------------------------//

    //public POI findpoi(int cosid)
    //{
    //    foreach (POI poi in POI)
    //        if (poi.getcosid() == cosid)
    //            return poi;

    //    return null;
    //}
}

[XmlRoot("Pois")]
public class POI
{
    public bool rendered = false;//new

    public string Id;
    public string Name;
    public string ImageTarget;
    public double Latitude;
    public double Longitude;
    public string TargetHeight;
    public string TargetWidth;
    public float SimilarityThreshold;
    public string userId;

    [XmlArray("ContentContainers"), XmlArrayItem("ContentContainer")]
    public ContentContainer[] ContentContainers;


    //non-serializable properties and attributes
    //on map
    GameObject mapPOI;
    public GameObject GetmapPOI() { return mapPOI; }
    public void SetmapPOI(GameObject obj) { mapPOI = obj; }
    GameObject markerObj;
    public GameObject GetMarkerObj() { return markerObj; }
    public void SetMarkerObj(GameObject obj) { markerObj = obj; }
    //ar name display popup
    GameObject ARgo;
    //public GameObject GetMapObj() { return mapObj; }
    //public void SetMapObj(GameObject obj) { mapObj = obj; }
    //GameObject mapObj;
    public GameObject GetARgo() { return ARgo; }
    public void SetARgo(GameObject obj) { ARgo = obj; }
    //for showing information on Poi

     //canvas
    GameObject infoBoard;
    public GameObject GetinfoBoard() { return infoBoard; }
    public void SetinfoBoard(GameObject obj) { infoBoard = obj; }
    public void closeBoard() {

        infoBoard.GetComponent<Canvas>().enabled = false;
    }
    public void openBoard()
    {
        infoBoard.GetComponent<Canvas>().enabled = true;
    }
    public bool isShowInfo() {
        return infoBoard.GetComponent<Canvas>().enabled ;
    }

    int cosId;
    public int GetCosId() { return cosId; }

    public GPSlocation GetGPSlocation()
    {
        return new GPSlocation(Latitude, Longitude);
    }

    public double GetDistance(GPSlocation g)
    {
        return GPSlocation.Distance(GetGPSlocation(), g);
    }
    public void Instantiate(GameObject areaObject, int newCosId)
    {
        markerObj = new GameObject();
        markerObj.transform.parent = areaObject.transform;
        markerObj.name = "Poi_" + Name;

        cosId = newCosId;
        int index = 0;
        foreach (ContentContainer cc in ContentContainers)
            cc.Instantiate_link(markerObj,index++);
    }

}

public class ContentContainer
{//this is a scene
	public string Id;
	public string Name;
	public string Description;
    
	[XmlArray("Contents"),XmlArrayItem("Content")]
	public Content[] Contents;
    GameObject SceneGameObject;
    public GameObject GetSceneGameObject() { return SceneGameObject; }
    public void SetSceneGameObject(GameObject obj) { SceneGameObject = obj; }

    public void Instantiate_link(GameObject poi,int index)
	{
		GameObject ContCont = new GameObject ();
        SceneGameObject = ContCont;

        ContCont.transform.parent = poi.transform;
		ContCont.name = index.ToString()+"_"+poi.name+"_"+Name;

		if(Contents != null) //arp, skip the empty content containers
		foreach (Content c in Contents) 
		{
			c.LinkTo(ContCont);
		}
        Transform prefab = GameObject.Find("Canvas").transform;
        GameObject ob = GameObject.Instantiate<GameObject>(prefab.gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        ob.transform.parent = ContCont.transform;
        ob.name = ContCont.name;

    }

}

public class Content
{
	public string 			Id;
	public PoiDataType   	Type;
	public string 			Description;
	public PositionVector 	Position;
	public RotationVector 	Rotation;
	public ScaleVector    	Scale;
    GameObject mediaGo;
    public GameObject GetMediaGo() {
        return mediaGo;
    }
	public void LinkTo(GameObject contentContainer)
	{
        GameObject content;
        Debug.LogWarning("contentContainer instantiate ");
        switch (Type)
		{
		case PoiDataType.texture2D:
                

			content 					= (GameObject) GameObject.CreatePrimitive(PrimitiveType.Plane);
			content.transform.parent 	= contentContainer.transform;
			content.name 				= content.transform.parent.name + "_tex2D";
			
			content.transform.position 	= Position.getVector3();
			content.transform.rotation 	= /*Rotation.getQuaternion() +*/ Quaternion.Euler(0f, 180f, 0f);
			//content.transform.localScale= Scale.getVector3();
			content.transform.localScale = new Vector3(100,100,100);
			content.transform.parent.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                string p = Path.Combine(Application.persistentDataPath,SceneTools.AreaNameDefault()+"/"+Description);
                Debug.Log(p);

                content.AddComponent<DragScript>();
			if(System.IO.File.Exists(p))
			{
				Texture2D texture = new Texture2D(512,512);
				texture.LoadImage(System.IO.File.ReadAllBytes(p));
                    content.GetComponent<MeshCollider>().enabled = true;
                content.GetComponent<Renderer>().material.mainTexture = texture;
                }
			else
			{
				Debug.LogWarning("Texture " + p + " does not exist");
			}

            Debug.Log("A picture from" + content.name);
			break;
			
		case PoiDataType.object3D:
            content 					= new GameObject();
            content.transform.parent 	= contentContainer.transform;
            content.name 				= content.transform.parent.name + "_obj3D";
			//content.layer 				= LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
                
            //Model3D model3D 			= GameObject.Find("Object3DManager").GetComponent<Model3D>();
			//GameObject[] models 		= model3D.Load(Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "/"+Description));
            Debug.Log("A object3D from" + content.name);
			break;
		
        case PoiDataType.audio:
            
            if (Description.Contains("mp3"))
            {
                
                //Debug.Log("A audio called" + Description);
                GameObject prefeb_Container =(GameObject) Resources.Load("AudioContainer", typeof(GameObject));
                content = GameObject.Instantiate<GameObject>(prefeb_Container, new Vector3(0, 0, 0), Quaternion.identity);
                content.transform.parent = contentContainer.transform;
                content.name = content.transform.parent.name + "_audio";
                    //content.layer 				= LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
                Test   t = (content.GetComponent("Test") as Test);                  
                 t.sendAudioName(Description,content);                  
                content.transform.position = Position.getVector3();
                Debug.Log("A audio from" + content.name);                    
                    content.AddComponent<MeshCollider>();
                    content.AddComponent<DragScript>();
                    mediaGo = content;
                }
            break;
		
        case PoiDataType.text:

                GameObject TextContent = GameObject.Find("TextContent");
                 content = (GameObject)GameObject.Instantiate(TextContent);
                //augmentation.transform.parent = tb.gameObject.transform;
                content.transform.SetParent(contentContainer.transform);
                content.transform.localPosition = new Vector3(0f, 1f, 0f);
                Quaternion target = Quaternion.Euler(90f, 0, 0);
                content.transform.localRotation = target;// Quaternion.identity;
                content.name = content.transform.parent.name + "_text";
                content.GetComponentInChildren<Text>().text = Description;
                content.GetComponent<Canvas>().enabled = true;
                content.transform.localScale = new Vector3(10f, 10f, 10f);
                content.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 900);
                GameObject textGo = content.transform.GetChild(0).gameObject;

                textGo.AddComponent<MeshCollider>();
                textGo.AddComponent<DragScript>();
                break;

		default:
			Debug.LogWarning("POI type not implemented");
			break;
			
		}

		
	}
}

public enum PoiDataType
{
	texture2D = 0,
	object3D = 1,
	audio	 = 2,
	text	 = 3,
}

public class MyVector3
{
	public float x;
	public float y;
	public float z;
	public Vector3 	  getVector3(){return new Vector3 ( x, y, z);}
	public Quaternion getQuaternion(){return Quaternion.Euler(x,y,z);}
}

[XmlRoot("Position")]
public class PositionVector : MyVector3{}
[XmlRoot("Rotation")]
public class RotationVector : MyVector3{}
[XmlRoot("Scale")]
public class ScaleVector : MyVector3{}


