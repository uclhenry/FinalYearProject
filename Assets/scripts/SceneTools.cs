using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

public static class SceneTools 
{
    public static bool isTracking = false;
    public static bool isTestMode = false;
    public static int buttonFontSize = 40;
    public static string CurrentSource = "realdata"; 

    public static string ControlContentContainerName() { return "ContentContainer"; }
    public static string AreaNameDefault() { return "VisAge";}
    public static string VisAgeXml()
    {
        string sPath = "";
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            // sPath = "file://" + Application.streamingAssetsPath + "/VisAge/VisAge.xml";
            sPath = "file://" + Application.persistentDataPath + "//VisAge//VisAge.xml";

        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            //sPath = Application.streamingAssetsPath + "/VisAge/VisAge.xml";  
            sPath = "file://" + Application.persistentDataPath + "//VisAge//VisAge.xml";//for www

        }
        return sPath;
            
    }
	public static string AreaZipLastTimeUrl() 	{return "http://visage.cs.ucl.ac.uk/VisAge/GetLastZipTime";}
	public static string AreaZipLastTimeLocal() { return Path.Combine (Application.persistentDataPath, "LastZip.txt");}
	public static string AreaZipFileUrl() 	{ return "http://visage.cs.ucl.ac.uk/VisAge/FileServer/VisAge.zip";}
	//public static string AreaZipFileUrl() 	{ 
	//		if(Application.loadedLevelName == "ExperimentNavigation")
	//			return "http://www0.cs.ucl.ac.uk/staff/A.RoviraPerez/VisAge/VisAge.zip";
	//		else
				//return "http://visage.cs.ucl.ac.uk/VisAgeDev/FileServer/VisAge.zip";
        //				
   // }
	public static string AreaZipFileLocal() { return Path.Combine (Application.persistentDataPath, "VisAge.zip");}
    public static string testZip = "http://www0.cs.ucl.ac.uk/staff/A.RoviraPerez/VisAge/VisAge.zip";

    public static bool IsNewSceneDesign()
	{
		if (Application.loadedLevelName == "ExperimentNavigation" || Application.loadedLevelName == "VisAge") return true;
		
		return false;
	}
	
	public static string ContentLayerMaskName()	{ return "TouchInput";}
	
	public static float DefaultGPSaccuracy() {return 65f;}
	//66GS
	public static GPSlocation DefaultGPSlocation() {return new GPSlocation(51.5217660,-0.1319100);}
	//MPEB room 6.22 GPS coordinates
	public static double[] DefaultGPSlocationForSlippyMap ()	{return new double[2] {-0.1327506f,51.5231574f};}
	//Brighton
	//public static double[] DefaultGPSlocation ()	{return new double[2] {-0.1457461,50.8296867};}
	
	public static string MapQuestKey() {return "Fmjtd%7Cluu8210720%2C7s%3Do5-94bahr";} //com.UCL.VisAge2014

    public static string AreaTrackingDataFile()
    {
		return Path.Combine(Application.persistentDataPath, "VisAge/StreamingAssets/TrackingData_VisAge.xml");
    }
	
	public enum SceneEnum
	{
		MAIN 	= 0,
		AR	    = 1,
		MAP  	= 2,
		ACHIEVEMENTS = 3,
	};

	public static void DownloadFileFromUrlSync(string url, string filename)
	{
		Debug.Log ("Downloading... " + url);
		Debug.Log ("Saving file... " + filename);

		WebClient wc = new WebClient ();
		wc.DownloadFile (url, filename);
		wc.Dispose();
	}

    public static void DownloadFileFromUrlAsync(string url, string filename)
    {
        WebClient wc = new WebClient();
        wc.DownloadFileAsync(new Uri(url), filename);
        wc.Dispose();
		
    }

    public static bool IsThereInternetConnection()
    {
		return true;
//        WebClient client;
//
//        try
//        {
//            client = new System.Net.WebClient();
//            client.OpenRead("http://www.google.com");
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
    }

    public static void ClearLocalData()
    {
        String[] files = Directory.GetFiles(Application.persistentDataPath);

        foreach (String file in files) 
            File.Delete(file);

        String[] directories = Directory.GetDirectories(Application.persistentDataPath);

        foreach (String directory in directories)
            Directory.Delete(directory, true);

    }

    public static void CreateFile(string path, string name, string info)
    {
      
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if (!t.Exists)
        {
        
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }

        sw.WriteLine(info);
        sw.Close();
        sw.Dispose();
    }


    public static ArrayList LoadFile(string path, string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(path + "//" + name);
        }
        catch (Exception e)
        {
            return null;
        }
        string line;
        ArrayList arrlist = new ArrayList();
        while ((line = sr.ReadLine()) != null)
        {
            arrlist.Add(line);
        }

        sr.Close();

        sr.Dispose();

        return arrlist;
    }


    public static void DeleteFile(string path, string name)
    {
        File.Delete(path + "//" + name);
        
    }
    public static void AddSetting() {
        CreateFile(Application.persistentDataPath, "Setting.txt", "Source:realdata");
        CreateFile(Application.persistentDataPath, "Setting.txt", "fontsize:30");
        CreateFile(Application.persistentDataPath, "Setting.txt", "CleanData:true");
    }
    public static ArrayList getSetting() {
        ArrayList infoall = LoadFile(Application.persistentDataPath, "Setting.txt");
        return infoall;
    }
    public static void printSetting() {
        ArrayList lines = getSetting();
        if (lines == null) return;
        foreach (var a in lines) {
            Debug.LogWarning(a);
        }
        
    }
    public static void ChangeSetting(string source,string fontsize,string CleanData) {
        File.Delete(Application.persistentDataPath + "//" + "Setting.txt");
        CreateFile(Application.persistentDataPath, "Setting.txt", "Source:"+source);
        CreateFile(Application.persistentDataPath, "Setting.txt", "fontsize:"+ fontsize);
        CreateFile(Application.persistentDataPath, "Setting.txt", "CleanData:"+ CleanData);
    }
    public static string GetSettingValue(string key) {
        ArrayList lines = getSetting();
        if (lines == null) return "";
        foreach (var a in getSetting())
        {
            string[] words = a.ToString().Split(':');
            if (words[0].Equals(key)) return words[1];

        }
        return "";
    }
}

