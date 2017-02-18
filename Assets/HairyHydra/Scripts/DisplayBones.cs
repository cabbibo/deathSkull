using UnityEngine;
using System.Collections;

public class DisplayBones : MonoBehaviour {

	public GameObject boneBuffer;
	public Material material;

	// Use this for initialization
	void Start () {

		Camera.onPostRender += Render;
	
	}

	void Render( Camera c ){
		material.SetPass(0);
		material.SetBuffer("_boneBuffer", boneBuffer.GetComponent<BoneBuffer>()._boneBuffer);

    	Graphics.DrawProcedural(MeshTopology.Triangles, boneBuffer.GetComponent<BoneBuffer>().boneCount * 3 );

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
