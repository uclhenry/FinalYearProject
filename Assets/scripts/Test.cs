using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class Test : MonoBehaviour {
    //public MusicPlayer music;
    //spublic Button yourButton;
    UnityEngine.Object[] normal = null;
    UnityEngine.Object[] pressed = null;
    public GameObject AudioCanvas = null;
    public GameObject ButtonPrefeb;
    public AudioSource Sound;
    private GameObject audioObject;
    private GameObject newButton = null;
    private bool getName = false;
    private bool setUp = false;
    private String AudioName = "";

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

    public void sendAudioName(String description,GameObject parent) {
        AudioName = description;
        getName = true;
        audioObject = parent;
    }
    void Start()
    {
        normal = Resources.LoadAll("normal", typeof(Sprite));
        pressed = Resources.LoadAll("pressed", typeof(Sprite));
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
            //newButton.GetComponentInChildren<Text>().text = this.transform.parent.name + " Play >";
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
        
        newButton.GetComponent<Button>().transition = Selectable.Transition.SpriteSwap;
        SpriteState playing = new SpriteState();
        playing.pressedSprite = (Sprite)pressed[0];     
        SpriteState pause = new SpriteState();
        pause.pressedSprite = (Sprite)pressed[1];        
        if ((Sound.isPlaying == false))
        {
            newButton.GetComponent<Image>().sprite = (Sprite)normal[0];
            newButton.GetComponent<Button>().spriteState = playing;
        }
        else {
            newButton.GetComponent<Image>().sprite = (Sprite)normal[1];
            newButton.GetComponent<Button>().spriteState = pause;
        }
        Text textOnbutton = newButton.transform.GetChild(0).gameObject.GetComponent<Text>();
        if (Sound.isPlaying == false)
        {
            Sound.Play();
        }

        else
        {
            Sound.Pause();
        }
    }

}
