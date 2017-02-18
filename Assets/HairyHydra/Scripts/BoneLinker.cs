using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneLinker : MonoBehaviour {

	public float BoneWidth;
	public GameObject Connection;
	public GameObject DadConnection;

	public bool locked;

	// Use this for initialization
	void Start() {

		//Debug.Log( "happz");

		//Kiddies = new List<Transform>();
		//Breathren = new List<Transform>();
		//Connections = new List<GameObject>();
		
	}
	
	// Update is called once per frame
	void Update () {

		if( locked == true ){
			Connection.GetComponent<ConfigurableJoint>().yMotion = ConfigurableJointMotion.Locked;
			Connection.GetComponent<ConfigurableJoint>().xMotion = ConfigurableJointMotion.Locked;
			Connection.GetComponent<ConfigurableJoint>().zMotion = ConfigurableJointMotion.Locked;

			Connection.GetComponent<ConfigurableJoint>().angularXMotion = ConfigurableJointMotion.Locked;
			Connection.GetComponent<ConfigurableJoint>().angularYMotion = ConfigurableJointMotion.Locked;
			Connection.GetComponent<ConfigurableJoint>().angularZMotion = ConfigurableJointMotion.Locked;
		}
		
	}
}
