using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScale : MonoBehaviour {

	public Transform scaleTo;
	public GameObject objectToScale;
	public GameObject selectorObject;
	public Camera camera;

	public GameObject hand1;
	public GameObject hand2;

	public TeleportInterface tInterface;

	private MeshRenderer mr;

	// Use this for initialization
	void Start () {

		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Teleport(){
		objectToScale.transform.position = scaleTo.position;
		objectToScale.transform.rotation = scaleTo.rotation;
		objectToScale.transform.localScale = scaleTo.localScale;


		camera.farClipPlane = scaleTo.localScale.x * 100;
		camera.nearClipPlane = scaleTo.localScale.x * .05f;


	}

	void OnTriggerEnter( Collider c ){

	   if( c.gameObject == selectorObject){
	   	Teleport();
	   	tInterface.Teleport(transform.gameObject);
	   }
	}
}
