using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour {
	public Color c;
	
	private MeshRenderer mr;
	// Use this for initialization
	void Start () {

		mr = GetComponent<MeshRenderer>();
		
	}
	
	// Update is called once per frame
	void Update () {

		mr.material.SetColor("_Color", c );
		
	}
}
