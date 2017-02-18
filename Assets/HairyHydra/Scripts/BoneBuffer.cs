﻿using UnityEngine;
using System.Collections;


// should be on the object that is moving around the scen
public class BoneBuffer : MonoBehaviour {

	public GameObject mesh;
	public Transform[] bones;
	public Matrix4x4[] bindPoses;

	public int boneCount;
	public ComputeBuffer _boneBuffer;	
	public float[] boneValues;

    //print( AnchorStructSize );
    //print( AnchorStructSize * vc ); 
	struct Bone{
    public Matrix4x4 transform;
    public Matrix4x4 bindPose;
	};

	public Matrix4x4 tmpMat;

	private bool updated = false;

	void Awake(){
		SetBindPoses();
	}
	// Use this for initialization
	void Start () {

		bones = mesh.GetComponent<SkinnedMeshRenderer>().bones;
		boneCount = bones.Length;
	
    _boneBuffer = new ComputeBuffer( boneCount , 2 * 16 * sizeof(float) );    

    boneValues = new float[ 2 * 16 * boneCount ]; 

    bindPoses = new Matrix4x4[ boneCount ];

    SetBindPoses();
    UpdateBoneBuffer();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		UpdateBoneBuffer();
	
	}

 	public void SetBindPoses(){

 		//mesh.GetComponent<SkinnedMeshRenderer>().BakeMesh( mesh.GetComponent<SkinnedMeshRenderer>().sharedMesh );
		bindPoses = mesh.GetComponent<SkinnedMeshRenderer>().sharedMesh.bindposes;

 		/*for( int i = 0; i < boneCount; i++ ){
 			bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;

 			//print( "ss");
 			//print( bones[i].position );
 			//print( bones[i].localScale );
 		}*/

 	}



	void OnDisable(){
		ReleaseBuffer();
	}

	public void ReleaseBuffer(){
    _boneBuffer.Release(); 
  }




	void UpdateBoneBuffer(){

		

		for( int i = 0; i < boneCount; i++ ){
 			//bindPoses[i] = bones[i].worldToLocalMatrix * transform.localToWorldMatrix;
 		}

		for( int i = 0; i < boneCount; i++){



			tmpMat = bones[i].localToWorldMatrix; //bones[i].localToWorldMatrix;
			

			boneValues[ i * 32 + 0] = tmpMat[0,0];
			boneValues[ i * 32 + 1] = tmpMat[1,0];
			boneValues[ i * 32 + 2] = tmpMat[2,0];
			boneValues[ i * 32 + 3] = tmpMat[3,0];
			boneValues[ i * 32 + 4] = tmpMat[0,1];
			boneValues[ i * 32 + 5] = tmpMat[1,1];
			boneValues[ i * 32 + 6] = tmpMat[2,1];
			boneValues[ i * 32 + 7] = tmpMat[3,1];
			boneValues[ i * 32 + 8] = tmpMat[0,2];
			boneValues[ i * 32 + 9] = tmpMat[1,2];
			boneValues[ i * 32 +10] = tmpMat[2,2];
			boneValues[ i * 32 +11] = tmpMat[3,2];
			boneValues[ i * 32 +12] = tmpMat[0,3];
			boneValues[ i * 32 +13] = tmpMat[1,3];
			boneValues[ i * 32 +14] = tmpMat[2,3];
			boneValues[ i * 32 +15] = tmpMat[3,3];

			tmpMat = bindPoses[i]; 

			boneValues[ i * 32 +16] = tmpMat[0,0];
			boneValues[ i * 32 +17] = tmpMat[1,0];
			boneValues[ i * 32 +18] = tmpMat[2,0];
			boneValues[ i * 32 +19] = tmpMat[3,0];
			boneValues[ i * 32 +20] = tmpMat[0,1];
			boneValues[ i * 32 +21] = tmpMat[1,1];
			boneValues[ i * 32 +22] = tmpMat[2,1];
			boneValues[ i * 32 +23] = tmpMat[3,1];
			boneValues[ i * 32 +24] = tmpMat[0,2];
			boneValues[ i * 32 +25] = tmpMat[1,2];
			boneValues[ i * 32 +26] = tmpMat[2,2];
			boneValues[ i * 32 +27] = tmpMat[3,2];
			boneValues[ i * 32 +28] = tmpMat[0,3];
			boneValues[ i * 32 +29] = tmpMat[1,3];
			boneValues[ i * 32 +30] = tmpMat[2,3];
			boneValues[ i * 32 +31] = tmpMat[3,3];


		}

		_boneBuffer.SetData(boneValues);

	}





}
