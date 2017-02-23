using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScale : MonoBehaviour {

	public Transform scaleTo;
	public GameObject objectToScale;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Teleport(){
		objectToScale.transform.position = scaleTo.position;
		objectToScale.transform.rotation = scaleTo.rotation;
		objectToScale.transform.localScale = scaleTo.localScale;
	}

	void OnTriggerEnter( Collider c ){

	   if( c.tag == "internal"){
	   	print( c );
	   	Teleport();


	   }
		

	}
}
