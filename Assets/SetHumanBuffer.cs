using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHumanBuffer : MonoBehaviour {

	public HumanBuffer hb;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void OnRenderObject() {

		GetComponent<MeshRenderer>().material.SetBuffer("humanBuffer" , hb._buffer );
		GetComponent<MeshRenderer>().material.SetInt("_NumberHumans" , hb.numberHumans );
		
	}
}
