using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneLearn : MonoBehaviour {

	// Use this for initialization
	void Start () {

		SkinnedMeshRenderer skinnedMesh = GetComponent<SkinnedMeshRenderer>();
//		Debug.Log( skinnedMesh.bones.Length );
		for( int i = 0; i < skinnedMesh.bones.Length; i++){
			//Debug.Log( skinnedMesh.bones[i]);
		}


		//Debug.Log( skinnedMesh.bones[0].children );
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
