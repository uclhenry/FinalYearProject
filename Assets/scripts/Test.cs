using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Test : MonoBehaviour {
    //public MusicPlayer music;
    //spublic Button yourButton;
    public GameObject AudioCanvas = null;
    public GameObject ButtonPrefeb;
    public AudioSource Sound;
    private GameObject audioObject;
    private GameObject newButton = null;
    private bool getName = false;
    private bool setUp = false;
    private String AudioName = "";
    int playing = 1;
    int pause = 2;
    int stop = 3;
    public int state = 3;
    public GameObject file;
    public void Setup(string Description, GameObject ob)
    {
        StartCoroutine(wwwDownload(Description));
        file.transform.parent = ob.transform;
    }
    System.Collections.IEnumerator wwwDownload(string Description)
    {
        WWW www = new WWW("file://" + Path.Combine(Application.persistentDataPath, SceneTools.AreaNameDefault() + "//" + Description));
        yield return www;


        AudioClip clip = www.GetAudioClip();
        //clip = www.GetAudioClip(false,false,AudioType.MPEG);
        clip = www.GetAudioClip(false, false, AudioType.MPEG);
        clip.name = Description;

        file.name = "url:" + Description;
        file.GetComponent<AudioSource>().clip = clip;
        Sound = file.GetComponent<AudioSource>();
        //file = ob;
    }
    public void Play()
    {
        Sound.Play();
        state = playing;
    }
    public void PauseAudio()
    {
        Sound.Pause();
        state = pause;
    }
    public void StopAudio()
    {
        Sound.Stop();
        state = stop;
    }
    public void RestartAudio()
    {
        Sound.Stop();
        Sound.Play();
        state = playing;
    }
    public void sendAudioName(String description,GameObject parent) {
        AudioName = description;
        getName = true;
        audioObject = parent;
    }
    void Start()
    {
        GameObject prefeb_Sound = GameObject.Find("Audio Source");
        file = GameObject.Instantiate<GameObject>(prefeb_Sound, new Vector3(0, 0, 0), Quaternion.identity);


        if (this.transform.parent != null) { 
            Debug.Log(this.transform.parent.name);
            AudioCanvas = this.transform.parent.GetChild(this.transform.parent.childCount - 1 ).gameObject;
            newButton = Instantiate(ButtonPrefeb) as GameObject;
            newButton.transform.SetParent(AudioCanvas.transform, false);
            //music = (GetComponent("MusicPlayer") as MusicPlayer);
            if (getName) { 
                Setup(AudioName, audioObject);
                setUp = true;
            }
            Button btn = newButton.GetComponent<Button>();
            //newButton.transform.position = new Vector3(0, 0, 0);
            Debug.Log(newButton.GetComponent<RectTransform>().localPosition);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            btn.onClick.AddListener(TaskOnClick);
            newButton.GetComponentInChildren<Text>().text = this.transform.parent.name + " Play >";
        }
    }
     void Update()
    {
        if (getName && !setUp)
        {
            Setup(AudioName, audioObject);
            setUp = true; 

        }
    }
    void TaskOnClick()
    {

        // Debug.Log("AudioName  " + AudioName);
        // Debug.Log("Play"+music.Sound.clip.name);
        // Debug.Log("isPlaying" + music.Sound.isPlaying+music.Sound.isActiveAndEnabled);
        // GameObject musicOb = GameObject.Find("file:" + AudioName);
        //musicOb.GetComponent<AudioSource>().Play();
        // Debug.Log("musicOb" + musicOb.GetComponent<AudioSource>().clip.name + musicOb.GetComponent<AudioSource>().isActiveAndEnabled);
        //music.Play();
        Text textOnbutton = newButton.transform.GetChild(0).gameObject.GetComponent<Text>();
        if (Sound.isPlaying == false)
        {
            Play();
            if (state == playing)
                textOnbutton.text = this.transform.parent.name + "- ||";

        }

        else
        {
           PauseAudio();
            if (state == pause)
                textOnbutton.text = this.transform.parent.name + "- |>";
        }
    }

    //void OnGUI()
    //{

    //    if (music.state == stop || music.state == pause)
    //    {
    //        if (GUI.Button(new Rect(10, 10, 100, 50), "PLAY"))
    //        {

    //            Debug.Log("Play!");
    //            music.Play();//调用播放器Play方法

    //        }
    //        if (music.state == pause)
    //            if (GUI.Button(new Rect(120, 10, 100, 50), "Stop"))
    //            {

    //                Debug.Log("Stop!");
    //                music.StopAudio();

    //            }

    //    }
    //    else if (music.state == playing) {
    //        if (GUI.Button(new Rect(10, 10, 100, 50), "PAUSE"))
    //        {

    //            Debug.Log("Pause!");
    //            music.PauseAudio();

    //        }
    //        if (GUI.Button(new Rect(120, 10, 100, 50), "Stop"))
    //        {

    //            Debug.Log("Stop!");
    //            music.StopAudio();

    //        }

    //    }


    //}

}
