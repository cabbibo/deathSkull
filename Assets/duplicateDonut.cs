using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class duplicateDonut : MonoBehaviour {

	public Donut donut;

	public Material material;
	int numVertsTotal;
	// Use this for initialization
	void Start () {
		numVertsTotal = donut.ribbonWidth * 3 * 2 * (donut.ribbonLength-1);
	}
	

	void OnRenderObject(){
		
		//print( "hello");
		//int numVertsTotal = donut.ribbonWidth * 3 * 2 * (donut.ribbonLength-1);

        material.SetPass(0);

        material.SetBuffer("buf_Points", donut._vertBuffer);
        material.SetBuffer("og_Points", donut._ogBuffer);

        material.SetInt( "_RibbonWidth"  , donut.ribbonWidth  );
        material.SetInt( "_RibbonLength" , donut.ribbonLength );
        material.SetInt( "_TotalVerts"   , donut.vertexCount  );

        material.SetTexture("_AudioMap" , donut.audioTexture.AudioTexture );

        material.SetMatrix("worldMat", transform.localToWorldMatrix);
        material.SetMatrix("invWorldMat", transform.worldToLocalMatrix);

        Graphics.DrawProcedural(MeshTopology.Triangles, numVertsTotal * 1/10);

	}
	// Update is called once per frame
	void Update () {
		
	}
}
