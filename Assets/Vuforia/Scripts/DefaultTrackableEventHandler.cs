/*==============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Protected under copyright and other laws.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
///     A custom handler that implements the ITrackableEventHandler interface.
/// </summary>
public class DefaultTrackableEventHandler : MonoBehaviour, ITrackableEventHandler
{

    public int currentSceneIndex = 0;
    public List<GameObject> scenelist;
    public GameObject PoiGameObject;
    public bool ButtonOn = false;
    public POI poiForScene;

    #region PRIVATE_MEMBER_VARIABLES

    protected TrackableBehaviour mTrackableBehaviour;

    #endregion // PRIVATE_MEMBER_VARIABLES

    #region UNTIY_MONOBEHAVIOUR_METHODS

    protected virtual void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
            mTrackableBehaviour.RegisterTrackableEventHandler(this);

        scenelist = new List<GameObject>();
    }

    #endregion // UNTIY_MONOBEHAVIOUR_METHODS

    #region PUBLIC_METHODS

    public void addScene(GameObject Scene) {
        scenelist.Add(Scene);
    }
    public void addPoi() {
        GameObject ARCamera = GameObject.Find("ARCamera");
        DynamicDataSetLoader mainScript= ARCamera.transform.GetComponent<DynamicDataSetLoader>();
        if (mainScript.seenPOIs.Contains(poiForScene) == false) {
            mainScript.seenPOIs.Add(poiForScene);
        }
    }
    /// <summary>
    ///     Implementation of the ITrackableEventHandler function called when the
    ///     tracking state changes.
    /// </summary>
    public void OnTrackableStateChanged(
        TrackableBehaviour.Status previousStatus,
        TrackableBehaviour.Status newStatus)
    {
        if (isMapMode()) return;
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
            
            ButtonOn = true;
           
            linkScene();
            OnTrackingFound();
            addPoi();
            SwitchMap(false);

        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED &&
                 newStatus == TrackableBehaviour.Status.NOT_FOUND)
        {
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
            OnTrackingLost();
            ButtonOn = false;
            HideScenes();
           
        }
        else
        {
            // For combo of previousStatus=UNKNOWN + newStatus=UNKNOWN|NOT_FOUND
            // Vuforia is starting, but tracking has not been lost or found yet
            // Call OnTrackingLost() to hide the augmentations
            OnTrackingLost();
            ButtonOn = false;
            HideScenes();
        }
    }

    #endregion // PUBLIC_METHODS

    #region PRIVATE_METHODS

    protected virtual void OnTrackingFound()
    {
        GameObject.Find("[Map]").transform.position = new Vector3(10000, 0, 0);
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);
        //Transform scene = null;

        
        //DynamicDataSetLoader dyn = GameObject.Find("ARCamera").GetComponent<DynamicDataSetLoader>();
        //if (transform.childCount != 0)
        //{
        //    Transform tPOI = transform.GetChild(transform.childCount - 1);//Poi

        //    if (tPOI.gameObject.name.Contains("Poi"))
        //    {scene = tPOI.GetChild(dyn.SceneIndex);
        //    Debug.Log(tPOI.gameObject.name);

        //    }
            
            
        //}
        //Debug.Log(dyn.SceneIndex);
        // Enable rendering:
        foreach (var component in rendererComponents)
            //if (component.gameObject.transform.parent == scene)
                component.enabled = true;

        // Enable colliders:
        foreach (var component in colliderComponents)
            //if (component.gameObject.transform.parent == scene)
                component.enabled = true;

        // Enable canvas':
        
        foreach (var component in canvasComponents)
                //if(component.gameObject.transform.parent ==scene)
                component.enabled = true;
        SceneTools.isTracking = true;
    }


    protected virtual void OnTrackingLost()
    {
        GameObject.Find("[Map]").transform.position = new Vector3(10000, 0, 0);
        var rendererComponents = GetComponentsInChildren<Renderer>(true);
        var colliderComponents = GetComponentsInChildren<Collider>(true);
        var canvasComponents = GetComponentsInChildren<Canvas>(true);

        // Disable rendering:
        foreach (var component in rendererComponents)
            component.enabled = false;

        // Disable colliders:
        foreach (var component in colliderComponents)
            component.enabled = false;

        // Disable canvas':
        foreach (var component in canvasComponents)
            component.enabled = false;
        SceneTools.isTracking = false;
    }

    #endregion // PRIVATE_METHODS
    void SwitchMap(bool on)
    {
        Transform map = GameObject.Find("[Map]").transform;
        map.GetComponent<MapSwitch>().on = on;
    }
    bool isMapMode()
    {
        Transform map = GameObject.Find("[Map]").transform;
        return map.GetComponent<MapSwitch>().on;
    }

    void HideScenes()
    {
        Transform hiddenScenes = GameObject.Find("HiddenObject").transform;

        if (scenelist != null)
            foreach (GameObject container in scenelist)
            {
                //container is a scene
                container.transform.SetParent(hiddenScenes);
            }
    }
    void linkScene()
    {
        Debug.LogError("number of scenes ,current index = " + scenelist.Count + " " + currentSceneIndex);
        if (scenelist.Count - 1 >= currentSceneIndex) {
            Debug.Log(scenelist[currentSceneIndex].name);
            scenelist[currentSceneIndex].transform.parent = PoiGameObject.transform;
        }
            
        
    }
    void OnGUI()
    {
        if (ButtonOn) {
            //GameObject.Find("[Map]").transform.position = new Vector3(10000, 0, 0);
            if(scenelist.Count>1)
            NextSceneButton();
            if(SceneTools.isTestMode)
            ShowInfoForPoiButton();
     
        }
        
    }
    void ShowInfoForPoiButton() {
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = SceneTools.buttonFontSize;
        if (GUI.Button(new Rect(Screen.width * 0.8f, 4f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height), "POI info", titleStyle)) {
            if (!poiForScene.isShowInfo())
            {
                poiForScene.openBoard();
            }
            else {
                poiForScene.closeBoard();
            }
            
        }
    }
    void NextSceneButton()
    {
        GUIStyle titleStyle = new GUIStyle("button");
        titleStyle.fontSize = SceneTools.buttonFontSize;
        if (GUI.Button(new Rect(Screen.width * 0.8f, 2f / 8 * Screen.height, 0.2f * Screen.width, 0.1f * Screen.height), "Next Scene", titleStyle))
        {
            

            currentSceneIndex += 1;
            
            currentSceneIndex = currentSceneIndex % scenelist.Count;
            OnTrackingLost();
            HideScenes();
            linkScene();
            OnTrackingFound();
            SwitchMap(false);
            //GameObject.Find("[Map]").transform.position = new Vector3(0, 0, 0);

        }
    }

}
