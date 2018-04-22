using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    private Transform myTransform;
    public Vector3 Axis = Vector3.back;

    void Start()
    {
        myTransform = this.transform;
    }

    void Update()
    {
        if (GameObject.Find("Main Camera").GetComponent<Camera>()!= null)
        {
        Transform cameraTransform = GameObject.Find("Main Camera").GetComponent<Camera>().transform;
        myTransform.LookAt(myTransform.position + cameraTransform.rotation * Axis,
            cameraTransform.rotation * Vector3.up);

        }

    }
}