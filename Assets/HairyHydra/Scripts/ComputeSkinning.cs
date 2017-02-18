using UnityEngine;
using System.Collections;

public class ComputeSkinning : MonoBehaviour {



	public GameObject skinnedMesh;

	public BoneBuffer boneBuffer;
	public VertBuffer vertBuffer;
	public TriangleBuffer triBuffer;
	public HumanBuffer humanBuffer;

	/*public Texture texture;
	public CubeMap cubemap;
	public Shader shader;*/


	public ComputeShader computeShader;
  	public Material material;



	private int _kernel;
	private int vertCount;
	private int size;

	public int Set = 0;

	//int mesh;

	// Use this for initialization
	void Start () {

		vertCount = vertBuffer.vertCount;
    size = vertBuffer.SIZE;
    _kernel = computeShader.FindKernel("CSMain");

    Camera.onPostRender += Render;

    SetBegin();

    Set = 0;
	
	}


	void Render( Camera c ){

		//print("hello");
		material.SetPass(0);

		material.SetBuffer("_vertBuffer", vertBuffer._vertBuffer);
		material.SetBuffer("_triBuffer", triBuffer._triBuffer);
		//material.SetBuffer("_vertBuffer", vertBuffer._vertBuffer);

    	Graphics.DrawProcedural(MeshTopology.Triangles, triBuffer.triCount );

	}


	void SetBindPoses(){

	}

	void SetBegin(){

		Set = 1;
		Dispatch();		
		//Dispatch();
		//Dispatch();
		

	}

	void Dispatch(){


		computeShader.SetInt( "Set" , Set );

		///print( vertBuffer._vertBuffer );

		if( vertBuffer._vertBuffer != null &&
			boneBuffer._boneBuffer != null ){
	    	computeShader.SetBuffer( _kernel , "vertBuffer"     , vertBuffer._vertBuffer );
	    	computeShader.SetBuffer( _kernel , "boneBuffer"     , boneBuffer._boneBuffer );
	    	computeShader.SetBuffer( _kernel , "humanBuffer"     , humanBuffer._buffer );
	    	computeShader.SetInt( "_NumberHumans", humanBuffer.numberHumans);
	    	computeShader.Dispatch( _kernel, 6 , 6 , 6 );
    	}

 
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		Dispatch();


	}
}
