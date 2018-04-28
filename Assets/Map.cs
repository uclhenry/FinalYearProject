using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Map : MonoBehaviour {
    double factor = 100 / 2 / 500;//a circle is 1000m
    public double degree = 0f;
    public float x = 0;
    public float y = 0;
    public double angle = 0f;
    public float distance = 40f;
    float DefaultDistance = 500f;//500m
    public POI p = null;
    // Use this for initialization
    void Start () {

         degree = 0;
        GetComponent<Transform>().localPosition = new Vector3(3, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (p == null) return;
        //degree = 90 - degree;
        if (Input.compass.enabled)
            degree = Input.compass.magneticHeading;
        else {
            degree = 0;
            Input.location.Start();
        }
        // angle = (System.Math.PI * degree / 180.0);
        //double angle2 = System.Math.PI / 2 + angle;
        //double sinAngle = System.Math.Sin( angle2);
        //double cosAngle = System.Math.Cos( angle2);
        ////Debug.Log(sinAngle);
        //x =  (float)( DefaultDistance *factor) ;//* cosAngle
        //y = DefaultDistance * (float)(factor * sinAngle);
        //Debug.Log(cosAngle);
        //GetComponent<Transform>().localPosition = new Vector3(distance * (float)cosAngle, distance * (float)sinAngle, 0);
        //degree = 0;

        double angle = Math.PI * degree / 180.0;
        double sinAngle = Math.Sin(-1 * angle);
        double cosAngle = Math.Cos(-1 * angle);
        //s = os = oa + as = x cos(theta) + y sin(theta)
        //t = ot = ay – ab = y cos(theta) – x sin(theta)
        Debug.Log(cosAngle);
        GPSlocation userPosition = new GPSlocation(51.5221, -0.131411);

        double longitude = Input.location.lastData.longitude;//userPosition.longitude;//
        double latitude = Input.location.lastData.latitude;// userPosition.latitude;//
        float currentX = (float)longitude;
        float currentZ = (float)latitude;
        Debug.Log(currentX);

        float x = (float)p.GetGPSlocation().longitude - currentX;
            float z = (float)p.GetGPSlocation().latitude - currentZ;
        Debug.Log(x);
        float s = (float)(x * cosAngle + z * sinAngle);
            float t = (float)(z * cosAngle - x * sinAngle);
        GetComponent<Transform>().localPosition = new Vector3(s*20000, t * 20000, 0);
        //


    }

}
