using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderThickGrass : MonoBehaviour {

	public Material material;
	private Sinew grass;

	public int bladeWidth = 6;
	public int bladeResolution = 101;

int numVertsTotal;
	// Use this for initialization
	void Start () {

		grass = GetComponent<Sinew>();
		numVertsTotal = grass.numVertsPerHair  * bladeWidth * 6 * grass.totalHairs;

	}

	void OnRenderObject(){

		//print( "hello");

		numVertsTotal = (bladeResolution-1)  * bladeWidth * 6 * grass.totalHairs;
 		//print( numVertsTotal );

		material.SetPass(0);
		//material.SetInt( "_VertsPerHair" , grass.numVertsPerHair );
		material.SetBuffer("vertBuffer", grass._vertBuffer );
		material.SetTexture("_AudioMap" , grass.audioTexture.AudioTexture);


    	material.SetInt("_TotalVerts" ,grass.fullVertCount);
    	material.SetInt("_TotalBlades" ,grass.totalHairs);
    	material.SetInt("_BladeWidth" , bladeWidth);
    	material.SetInt("_BladeLength" , grass.numVertsPerHair);
    	material.SetInt("_BladeResolution" ,bladeResolution);

		Graphics.DrawProcedural(MeshTopology.Triangles, numVertsTotal);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
