using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
public class ScaleHands : MonoBehaviour {

	// Use this for initialization
	void Start() {
	    if (GetComponent<Player>().hardwarePlayer != null) {
	        // Local
	        print("SHELLOS");
	            transform.parent = GameObject.Find("[CameraRig]").transform;
	    transform.Find("SphereL").tag ="internal";
	    transform.Find("SphereR").tag ="internal";
	    transform.Find("Head").tag ="internal";
	    transform.Find("Head").transform.Find("RamLoRez").transform.Find("default").GetComponent<MeshRenderer>().enabled = false;
	    transform.Find("Head").transform.Find("RamLoRez").transform.Find("default.001").GetComponent<MeshRenderer>().enabled = false;
	    transform.Find("Head").transform.Find("RamLoRez").transform.Find("default.002").GetComponent<MeshRenderer>().enabled = false;
	    	    } else {
	        // Remote
	        print("non onon");
	    }

	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
