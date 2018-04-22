using UnityEngine;
using System.Collections;

public class ControlAudio : MonoBehaviour
{
    GUIStyle mBoxStyle;
	float BoxWidth;
	float BoxHeight;
    float BoxOffsetX;
    float BoxOffsetY;
	float ButtonWidth;
	float ButtonHeight;
    float ButtonOffsetX;
    float ButtonOffsetXstep;
    float ButtonOffsetY;

    Texture2D AudioPlay;
    Texture2D AudioPlaySelected;
    Texture2D AudioPause;
    Texture2D AudioPauseSelected;
    Texture2D AudioStop;
    Texture2D AudioStopSelected;

    int NumButtons;
    AudioStatus Status;

    enum AudioStatus { Pause, Play, Stop };

    //bool visible;
    bool first;

    public ControlAudio()
    {
        first = true;
        //visible = false;
        NumButtons = 3;
        Status = AudioStatus.Stop;
	}
    
	private void initGUI() //initialize once
	{
		mBoxStyle = new GUIStyle(GUI.skin.FindStyle("box"));
        mBoxStyle.normal.background = Resources.Load("Textures/boxblack") as Texture2D;
		mBoxStyle.border = new RectOffset(10, 10, 10, 10);
		
        AudioPlay = Resources.Load("Textures/AudioIconPlay") as Texture2D;
        AudioPlaySelected = Resources.Load("Textures/AudioIconPlaySelected") as Texture2D;
        AudioPause = Resources.Load("Textures/AudioIconPause") as Texture2D;
        AudioPauseSelected = Resources.Load("Textures/AudioIconPauseSelected") as Texture2D;
        AudioStop = Resources.Load("Textures/AudioIconStop") as Texture2D;
        AudioStopSelected = Resources.Load("Textures/AudioIconStopSelected") as Texture2D;
	}
	
	private void SetupGUI() //initialize every frame
	{
		BoxWidth                    = Screen.width  * 0.45f;
		BoxHeight                   = Screen.height * 0.15f;
        BoxOffsetX                  = Screen.width  * 0.165f;
        BoxOffsetY                  = Screen.height * 0.7f;
        ButtonWidth                 = BoxWidth / NumButtons;
        ButtonHeight                = BoxHeight;
        ButtonOffsetX               = 0f;
        ButtonOffsetXstep           = ButtonWidth;
        ButtonOffsetY               = BoxHeight / 80f;
    
    }

    public void OnGUI()
    {
        
        if (first)
        {
            initGUI();
            first = false;
        }

        SetupGUI();

        GUI.backgroundColor = Color.clear;
        GUI.BeginGroup(new Rect(BoxOffsetX, BoxOffsetY, BoxWidth, BoxHeight));//,mBoxStyle);

        if (GUI.Button(new Rect(ButtonOffsetX, ButtonOffsetY, ButtonWidth, ButtonHeight), Status == AudioStatus.Play ? AudioPlaySelected : AudioPlay))
        {
            if (Status != AudioStatus.Play)
            {
               GetComponent<AudioSource>().Play();
                Status = AudioStatus.Play;
            }
        }

        if (GUI.Button(new Rect(ButtonOffsetX + ButtonOffsetXstep, ButtonOffsetY, ButtonWidth, ButtonHeight), Status == AudioStatus.Pause ? AudioPauseSelected : AudioPause))
        {
            switch (Status)
            {
                case AudioStatus.Play:
                    GetComponent<AudioSource>().Pause();
                    Status = AudioStatus.Pause;
                    break;
                case AudioStatus.Pause:
                    GetComponent<AudioSource>().Play();
                    Status = AudioStatus.Play;
                    break;
                case AudioStatus.Stop:
                    //nothing;
                    break;
            }
        }

        if (GUI.Button(new Rect(ButtonOffsetX + ButtonOffsetXstep * 2, ButtonOffsetY, ButtonWidth, ButtonHeight), Status == AudioStatus.Stop ? AudioStopSelected : AudioStop))
        {
            if (Status != AudioStatus.Stop)
            {
                GetComponent<AudioSource>().Stop();
                Status = AudioStatus.Stop;
            }
        }

        GUI.EndGroup();
        
    }

    public void Update()
    {
        if (Status == AudioStatus.Play && !GetComponent<AudioSource>().isPlaying) Status = AudioStatus.Stop;
    }

    public bool isPlaying()
    {
        return Status == AudioStatus.Play || Status == AudioStatus.Pause;
    }
}


