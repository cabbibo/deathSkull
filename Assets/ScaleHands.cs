using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
public class ScaleHands : MonoBehaviour {

	//public GameObject ghostPrefab;


	// Use this for initialization
	void Start() {
	    if (GetComponent<Player>().hardwarePlayer != null) {
	        // Local
	        print("SHELLOS");
	            transform.parent = GameObject.Find("[CameraRig]").transform;
	            
	            GameObject.Find("[CameraRig]").transform.Find("Controller (left)").GetComponent<controllerInfo>().interactionTip = transform.Find("SphereL").gameObject;
				GameObject.Find("[CameraRig]").transform.Find("Controller (right)").GetComponent<controllerInfo>().interactionTip = transform.Find("SphereR").gameObject;
	    		
	    		transform.Find("SphereL").tag ="internal";
	    		transform.Find("SphereR").tag ="internal";
	    		transform.Find("Head").tag ="internal";
	    		transform.Find("Head").transform.Find("RamLoRez").transform.Find("default").GetComponent<MeshRenderer>().enabled = false;
	    		transform.Find("Head").transform.Find("RamLoRez").transform.Find("default.001").GetComponent<MeshRenderer>().enabled = false;
	    		transform.Find("Head").transform.Find("RamLoRez").transform.Find("default.002").GetComponent<MeshRenderer>().enabled = false;
	    	//AddGhost();
	    } else {
	        // Remote
	        print("non onon");

	        
	    }

	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void AddGhost(){
		
		print( "made it aki");
		GameObject ghost  = PhotonNetwork.Instantiate( "Ghost" , Vector3.zero, Quaternion.identity,0);

		ghost.transform.parent = GameObject.Find("[CameraRig]").transform;

		print( transform.gameObject );

		ghost.GetComponent<Ghost>().player = transform.gameObject;//GetComponent<Player>();

	}
}
