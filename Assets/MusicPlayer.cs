//This script allows you to toggle music to play and stop.
//Assign an AudioSource to a GameObject and attach an Audio Clip in the Audio Source. Attach this script to the GameObject.

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource Sound ;
    int playing = 1;
    int pause = 2;
    int stop = 3;
    public int state = 3;
    public GameObject file;
    void Start()
    {
        GameObject prefeb_Sound = GameObject.Find("Audio Source");
        file = GameObject.Instantiate<GameObject>(prefeb_Sound, new Vector3(0, 0, 0), Quaternion.identity);
        
    }


}