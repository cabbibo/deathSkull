using UnityEngine;
using System.Collections;

public class VertBuffer : MonoBehaviour {

	public GameObject meshObject;

	public int vertCount;
	public ComputeBuffer _vertBuffer;	
	public float[] vertValues;

	public Vector3[] vertices;
	public BoneWeight[] weights;
	public Vector3[] normals;
	public Vector2[] uvs;


	public int SIZE = 8;

  int threadX;
  int threadY;
  int threadZ;

  int strideX;
  int strideY;
  int strideZ;
  
  private int gridX { get { return threadX * strideX; } }
  private int gridY { get { return threadY * strideY; } }
  private int gridZ { get { return threadZ * strideZ; } }

  public int fullVertCount { get { return gridX * gridY * gridZ; } }

	struct Vert{

		public float used;
	    public Vector3 pos;
	    public Vector3 vel;
	    public Vector3 nor;
	    public Vector2 uv;

	    public Vector3 targetPos;

	    public Vector3 bindPos;
	    public Vector3 bindNor;
	    public Vector4 boneWeights;
	    public Vector4 boneIDs;
	    public Vector3 debug;
	};



	private int vertStructSize = 1 + 3 + 3 + 3+2 + 3 +3 +3+4 +4 + 3;

	// Use this for initialization
	void Awake () {

		threadX = 8;
		threadY = 8;
		threadZ = 8;

		strideX = 8;
		strideY = 8;
		strideZ = 8;

		Mesh m = meshObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;

		//meshObject.GetComponent<SkinnedMeshRenderer>().BakeMesh( m );
		//Mesh m = meshObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		vertices = m.vertices;
		uvs = m.uv;
		normals = m.normals;
		weights = m.boneWeights;


		vertCount = vertices.Length;

		print( "VERTCOUNT");
		print( vertCount);
		print( fullVertCount );


		_vertBuffer = new ComputeBuffer( fullVertCount , vertStructSize * sizeof(float) );

		vertValues = new float[ vertStructSize * fullVertCount ];

		int index = 0;
		for( int i = 0; i < vertCount; i++ ){

			// used 
			vertValues[ index++ ] = 1;

			// positions
			vertValues[ index++ ] = vertices[i].x * .9f;
			vertValues[ index++ ] = vertices[i].y * .9f;
			vertValues[ index++ ] = vertices[i].z * .9f;

			// vel
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

			// normals
			vertValues[ index++ ] = normals[i].x;
			vertValues[ index++ ] = normals[i].y;
			vertValues[ index++ ] = normals[i].z;

			// uvs
			vertValues[ index++ ] = uvs[i].x;
			vertValues[ index++ ] = uvs[i].y;


			// target pos
			vertValues[ index++ ] = vertices[i].x;
			vertValues[ index++ ] = vertices[i].y;
			vertValues[ index++ ] = vertices[i].z;



			// bindPositions
			vertValues[ index++ ] = vertices[i].x;
			vertValues[ index++ ] = vertices[i].y;
			vertValues[ index++ ] = vertices[i].z;


			// bindNor
			vertValues[ index++ ] = normals[i].x;
			vertValues[ index++ ] = normals[i].y;
			vertValues[ index++ ] = normals[i].z;

			// bone weights
			vertValues[ index++ ] = weights[i].weight0;
			vertValues[ index++ ] = weights[i].weight1;
			vertValues[ index++ ] = weights[i].weight2;
			vertValues[ index++ ] = weights[i].weight3;

			// bone indices
			vertValues[ index++ ] = weights[i].boneIndex0;
			vertValues[ index++ ] = weights[i].boneIndex1;
			vertValues[ index++ ] = weights[i].boneIndex2;
			vertValues[ index++ ] = weights[i].boneIndex3;

			// Debug
			vertValues[ index++ ] = 1;
			vertValues[ index++ ] = 0;
			vertValues[ index++ ] = 0;

		} 
		
		_vertBuffer.SetData(vertValues);

	
	}

	void OnDisable(){
		ReleaseBuffer();
	}

	public void ReleaseBuffer(){
    _vertBuffer.Release(); 
  }

	
	// Update is called once per frame
	void Update () {
	
	}
}
