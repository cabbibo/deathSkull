using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copyHandPos : MonoBehaviour {

	public GameObject handPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.position = handPos.transform.position;
		
	}
}
