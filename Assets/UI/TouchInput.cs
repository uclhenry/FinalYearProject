using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Operation{idle,translate,scalerotate}

public class TouchInput : MonoBehaviour 
{
	public LayerMask touchInputMask;
	
	private int 		touchCount;		
	private Operation 	currentOperation = Operation.idle;
	private Vector2[] 	InitialTouchPosition;
	private Vector2 	centerPoint; //the point from where to cast a ray and select the active object to transform.
	private RaycastHit  lastHit;	

	//private Vector3 	initialPosition;
	private Vector3 	initialScale;
	private Quaternion  initialRotation;
	
	GameObject recipient = null;
	
	void Start()
	{
		InitialTouchPosition = new Vector2[2];
		//initialPosition = new Vector3();
		initialScale 	= new Vector3();
		initialRotation = new Quaternion();
		
		//touchInputMask = LayerMask.NameToLayer(SceneTools.ContentLayerMaskName());
//		Debug.Log (SceneTools.ContentLayerMaskName());
//		Debug.Log (touchInputMask.value);
	}
	
	void Update () 
	{
		touchCount = Input.touchCount; 
		
		switch (touchCount)
		{
		case 0:  //Idle
			currentOperation = Operation.idle;
			recipient = null;
			break;
		case 1: //Translate
			currentOperation = Operation.translate;
			centerPoint = Input.touches[0].position;
			break;
		case 2: //Rotate and Scale
			if( currentOperation == Operation.idle)
			{		
				currentOperation = Operation.scalerotate;
				InitialTouchPosition[0] = Input.touches[0].position;	
				InitialTouchPosition[1] = Input.touches[1].position;	
				centerPoint = (Input.touches[0].position + Input.touches[1].position) / 2;
			}
			break;
		}		
		
		if(currentOperation != Operation.idle)
		{
			Ray ray = GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(centerPoint);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 1000, touchInputMask))
			{ Debug.Log ("Hit");
				if(!recipient) 
				{
					recipient = hit.transform.gameObject;
					//initialPosition = recipient.transform.localPosition;
					initialScale 	= recipient.transform.localScale;
					initialRotation = recipient.transform.localRotation;
				}
				else {
					Debug.Log (recipient.name);
				}
			}
	
			if(recipient)
			{
				switch (touchCount)
				{					
					case 1: //translation
						if(hit.point != Vector3.zero) 
						{
							recipient.transform.localPosition = new Vector3(hit.point.x, hit.point.y, recipient.transform.position.z);
						}
						break;
					    
					case 2: //rotate and scale
						float scaleFactor = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) / Vector2.Distance(InitialTouchPosition[0], InitialTouchPosition[1]);
						recipient.transform.localScale = initialScale * scaleFactor;
						
						Vector2 a = Input.touches[0].position - Input.touches[1].position;
						Vector2 b = InitialTouchPosition[0] - InitialTouchPosition[1];
						float rotationAngle = Vector2.Angle(a, b);
						recipient.transform.localRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationAngle);
						break;
				}				
			}
		}
	}
	
	void SaveChanges()
	{
		
	}

}
