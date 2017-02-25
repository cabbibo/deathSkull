using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBackToCenter : MonoBehaviour {

	public GameObject center;

	private TeleportInterface ti;
	private LineRenderer lr; 
	private Rigidbody rb;

	private Vector3 v1 = new Vector3();

	// Use this for initialization
	void Start () {
		rb= GetComponent<Rigidbody>();
		lr = GetComponent<LineRenderer>();
		ti = center.GetComponent<TeleportInterface>();
		
	}
	
	// Update is called once per frame
	void Update () {

		lr.SetPosition( 0 , transform.position );
		lr.SetPosition( 1 , center.transform.position );
		
	}


	void FixedUpdate(){

		if( ti.activeButton != null ){

			v1 =  transform.position - ti.activeButton.transform.position;
			//v1.Normalize();
			rb.AddForce( v1 );
		}

	}
}
