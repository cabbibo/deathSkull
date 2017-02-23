using UnityEngine;
using System.Collections;
using Normal.Realtime;

public class Sinew: MonoBehaviour {
	

	public bool render;
	public GameObject endPos;
	public ComputeShader collisionShader;
	public ComputeShader constraintShader;
  	public GameObject HumanBuffer;
  	public AudioListenerTexture audioTexture;

	
	public Material inMat;

	private Material material;

	public int SIZE = 8;

	int threadX;
	int threadY;
	int threadZ;

	public int strideX;
	public int strideY;
	public int strideZ;

	public float xzSimulationRadius;

	private int gridX { get { return threadX * strideX; } }
	private int gridY { get { return threadY * strideY; } }
	private int gridZ { get { return threadZ * strideZ; } }

	public int fullVertCount { get { return gridX * gridY * gridZ; } }

	public int numVertsPerHair = 8;
	public float hairLength = .1f;
	public float distBetweenHairs { get { return hairLength / numVertsPerHair; }}

	public int totalHairs { get { return fullVertCount / numVertsPerHair;}}

	struct Vert{

		public Vector3 pos;
		public Vector3 oPos;
		public Vector3 ogPos;
		public Vector3 vel;
		public Vector3 nor;
		public Vector2 uv;
		public Vector3 debug;

	};

	public int vertStructSize = 3+3+ 3+ 3 + 3 + 2 + 3;

	private int _kernelCollision;
	private int _kernelConstraint;

	public ComputeBuffer _vertBuffer;
	public ComputeBuffer _transformBuffer;
	private float[] vertValues;
	private float[] transformValues;


	// Information About mesh
	private Mesh 			mesh;
	private int[] 		triangles;
  	private Vector4[] tangents;
  	private Vector3[] normals;
  	private Vector2[] uvs;
  	private Vector3[] positions;
  	private Vector3[] transformedPositions;//positions;
  	private Vector3[] transformedNormals;
  	private Color[]   colors;

  	private Vector3 oPos;
	private Vector3 vel;



	// Use this for initialization
	void Start () {
		threadX = SIZE; threadY = SIZE; threadZ = SIZE;
		//strideX = SIZE; strideY = SIZE; strideZ = SIZE;

    	_kernelCollision = collisionShader.FindKernel("CSMain");
    	_kernelConstraint = constraintShader.FindKernel("CSMain");


    	oPos = transform.position;

      HumanBuffer = GameObject.Find("HumanBuffer");

		mesh = GetComponent<MeshFilter>().mesh;
	    triangles = mesh.triangles; 
	    positions = mesh.vertices; 
	    transformedPositions = mesh.vertices; 
	    normals   = mesh.normals; 
	    transformedNormals   = mesh.normals; 
	    tangents  = mesh.tangents; 
	    colors    = mesh.colors; 
	    uvs       = mesh.uv; 



	    Camera.onPostRender += Render;

	    material = new Material( inMat );

	    CreateBuffers();

	    Dispatch( 1 );
    //print( "totalHairs" );
    //print( totalHairs);

	}

   void OnDisable(){
    Camera.onPostRender -= Render;
    _vertBuffer.Release(); 
    _transformBuffer.Release(); 
    GameObject.DestroyImmediate( material );
  }




	Vector3 GetRandomPointInTriangle( int seed, Vector3 v1 , Vector3 v2 , Vector3 v3 ){
    /* Triangle verts called a, b, c */

    Random.InitState(seed* 14145);
    float r1 = Random.value;

    Random.InitState(seed* 19247);
    float r2 = Random.value;
    //float r3 = Random.value;

    return (1 - Mathf.Sqrt(r1)) * v1 + (Mathf.Sqrt(r1) * (1 - r2)) * v2 + (Mathf.Sqrt(r1) * r2) * v3;
     
    ///return (r1 * v1 + r2 * v2 + r3 * v3) / (r1 + r2 + r3);
  }

  float AreaOfTriangle( Vector3 v1 , Vector3 v2 , Vector3 v3 ){
     Vector3 v = Vector3.Cross(v1-v2, v1-v3);
     float area = v.magnitude * 0.5f;
     return area;
  }


  Vector3 ToV3(Vector4 parent)
  {
     return new Vector3(parent.x, parent.y, parent.z);
  }

  float getRandomFloatFromSeed( int seed ){
  	Random.InitState(seed);
		return Random.value;
  }

  void transformPositions(){
  	for( int i =0; i< positions.Length; i++){
  		transformedPositions[i] = transform.TransformPoint( positions[i] );
  		transformedNormals[i] = transform.TransformVector( normals[i] );
  	}
  }



	void CreateBuffers(){


		_transformBuffer = new ComputeBuffer( 1 , 16 * sizeof(float));
		transformValues = new float[ 16 ];

		_vertBuffer = new ComputeBuffer( fullVertCount , vertStructSize * sizeof(float));
		vertValues = new float[ fullVertCount * vertStructSize ];

    // Used for assigning to our buffer;
    int index = 0;

    for (int z = 0; z < gridZ; z++) {
      for (int y = 0; y < gridY; y++) {
        for (int x = 0; x < gridX; x++) {

          int id = x + y * gridX + z * gridX * gridY; 
          //print( index);
          //print( inValues.Length );

          int idInHair = id % numVertsPerHair;
          int hairID = (int)Mathf.Floor( (float)id / (float)numVertsPerHair );

          // Resets using same hairID, so RandomPointInTriangle shoudl work
          float randomVal = getRandomFloatFromSeed( hairID * 20 );

          int tri0 = (int)(randomVal * (float)(triangles.Length/3)) * 3;
          int tri1 = tri0 + 1;
          int tri2 = tri0 + 2;

          if( id < 100 ){
          	//print( tri0 );
          	//print( triangles.Length);
          	//print( positions.Length );
          }

          tri0 = triangles[tri0];
          tri1 = triangles[tri1];
          tri2 = triangles[tri2];

          Vector3 pos = GetRandomPointInTriangle( hairID, positions[ tri0 ] ,positions[ tri1 ]  , positions[ tri2 ]  );
					
				  float a0 = AreaOfTriangle( pos , positions[tri1] , positions[tri2] );
				  float a1 = AreaOfTriangle( pos , positions[tri0] , positions[tri2] );
				  float a2 = AreaOfTriangle( pos , positions[tri0] , positions[tri1] );
				  float aTotal = a0 + a1 + a2;

				  float p0 = a0 / aTotal;
				  float p1 = a1 / aTotal;
				  float p2 = a2 / aTotal;


				  Vector3 nor     = normals[tri0]  * p0 + normals[tri1]  * p1 + normals[tri2]  * p2;
				  nor = nor.normalized;
				  /*float3 tang    = tri0.tang  * p0 + tri1.tang  * p1 + tri2.tang  * p2;
				  float3 color   = tri0.color  * p0 + tri1.color  * p1 + tri2.color  * p2;
				  float2 uv      = tri0.uv * p0 + tri1.uv * p1 + tri2.uv * p2;*/
          

          float idVal = (float)id / (float)fullVertCount;
          float uvX = (float)idInHair / (float)numVertsPerHair;
          float uvY = (float)hairID / (float)totalHairs;

          int xID = hairID % 180;
          int zID = (int)Mathf.Floor( (float)hairID / (float)180 );

          float xPos = (float)xID/180;
          float zPos = (float)zID/180;

          if( id < 100 ){
          	//print( uvX );
          }

          Vector3 fPos = pos + nor * hairLength * uvX;
          // pos
          vertValues[index++] = fPos.x;
          vertValues[index++] = fPos.y;
          vertValues[index++] = fPos.z;

          // oPos
          vertValues[index++] = fPos.x;
          vertValues[index++] = fPos.y;
          vertValues[index++] = fPos.z;

          // ogPos
          vertValues[index++] = fPos.x;
          vertValues[index++] = fPos.y;
          vertValues[index++] = fPos.z;

          //vel
          vertValues[index++] = 0;
          vertValues[index++] = 0;
          vertValues[index++] = 0;

          // nor
          vertValues[index++] = nor.x;
          vertValues[index++] = nor.y;
          vertValues[index++] = nor.z;

          // uv
          vertValues[index++] = uvX;
          vertValues[index++] = uvY;

          // debug
		      vertValues[index++] = 1;
          vertValues[index++] = 0;
          vertValues[index++] = (float)hairID;
          

        }
      }
    }

    _vertBuffer.SetData(vertValues);
    updateTransformBuffer();
      
 

	}
	
	void Render( Camera c ){

		if( render == true ){
		material.SetPass(0);

		material.SetInt( "_VertsPerHair" , numVertsPerHair );
		material.SetVector("_EndPos", endPos.transform.position );
		material.SetBuffer("_vertBuffer", _vertBuffer );
		material.SetTexture("_AudioMap" , audioTexture.AudioTexture);
    	
    	Graphics.DrawProcedural(MeshTopology.Lines, totalHairs * (numVertsPerHair-1) * 2 );
    	}
	}

	void updateTransformBuffer(){
		int index = 0;
		for (int i = 0; i < 16; i++){
      int x = i % 4;
      int y = (int)Mathf.Floor(i / 4);
      transformValues[index++] = transform.localToWorldMatrix[x, y];
    }

		_transformBuffer.SetData(transformValues);
	}


	void Dispatch( int reset ){

		vel = GetComponent<Rigidbody>().velocity;//oPos + (centerPos.transform.position - transform.position);

		updateTransformBuffer();
	
		collisionShader.SetInt( "_Reset" , reset );
		collisionShader.SetFloat( "_DeltaTime" , Time.deltaTime );
		collisionShader.SetFloat( "_Time" , Time.time );
    	collisionShader.SetVector("_EndPos", endPos.transform.position );
		//collisionShader.SetFloat("_PullVal", endPos.GetComponent<Hand>().triggerPosition );
    //print(endPos.GetComponent<Hand>().triggerPosition);
		collisionShader.SetInt( "_VertsPerHair" , numVertsPerHair );

    	collisionShader.SetInt( "_NumberHumans" ,HumanBuffer.GetComponent<HumanBuffer>().numberHumans  );
    	collisionShader.SetBuffer( _kernelCollision , "humanBuffer"     , HumanBuffer.GetComponent<HumanBuffer>()._buffer );

		collisionShader.SetBuffer( _kernelCollision , "transformMatrix" , _transformBuffer );
      	collisionShader.SetBuffer( _kernelCollision , "vertBuffer"     , _vertBuffer );
	    
	    collisionShader.SetInt("_StrideX" , strideX);
	    collisionShader.SetInt("_StrideY" , strideY);
	    collisionShader.SetInt("_StrideZ" , strideZ);
	    
	    collisionShader.SetVector("_Vel" , vel);
	    
	    collisionShader.Dispatch( _kernelCollision, strideX , strideY , strideZ );

	    constraintShader.SetInt( "_PassID" , 0 );
	    constraintShader.SetFloat( "_SpringDistance" , distBetweenHairs );
	    constraintShader.SetInt( "_VertsPerHair" , numVertsPerHair );
	    constraintShader.SetInt("_StrideX" , strideX);
	    constraintShader.SetInt("_StrideY" , strideY);
	    constraintShader.SetInt("_StrideZ" , strideZ);
	    constraintShader.SetBuffer( _kernelConstraint , "vertBuffer"     , _vertBuffer );
	    constraintShader.Dispatch( _kernelConstraint, strideX , strideY , strideZ / 2 );


	    constraintShader.SetInt( "_PassID" , 1 );
	    constraintShader.SetFloat( "_SpringDistance" , distBetweenHairs );
	    constraintShader.SetInt( "_VertsPerHair" , numVertsPerHair );
	    constraintShader.SetInt("_StrideX" , strideX);
	    constraintShader.SetInt("_StrideY" , strideY);
	    constraintShader.SetInt("_StrideZ" , strideZ);
	    constraintShader.SetBuffer( _kernelConstraint , "vertBuffer"     , _vertBuffer );
	    constraintShader.Dispatch( _kernelConstraint, strideX , strideY , strideZ / 2  );

	    ///oPos = -(centerPos.transform.position - transform.position);

	}
	// Update is called once per frame
	void FixedUpdate () {
		Dispatch( 0 );
	}
}