using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HairOnVertBuffer : MonoBehaviour {

	public HumanBuffer humanBuffer;
	public VertBuffer vertBuffer;
	public TriangleBuffer triBuffer;

	public ComputeShader collisionShader;
	public ComputeShader constraintShader;

	public Material material;
	public Color hairColor;

	public int threadX;
	public int threadY;
	public int threadZ;

	public int strideX;
	public int strideY;
	public int strideZ;

	private int gridX { get { return threadX * strideX; } }
	private int gridY { get { return threadY * strideY; } }
	private int gridZ { get { return threadZ * strideZ; } }

	public int fullVertCount { get { return gridX * gridY * gridZ; } }

	public int numVertsPerHair = 6;
	public float hairLength = .1f;
	public float distBetweenHairs { get { return hairLength / numVertsPerHair; }}

	public int totalHairs { get { return fullVertCount / numVertsPerHair;}}

	struct Vert{

		public Vector3 pos;
		public Vector3 oPos;
		public Vector3 vel;
		public Vector3 nor;
		public Vector2 uv;
		public Vector3 debug;

		public Vector3 triIDs;
		public Vector3 triWeights;

	};

	public int vertStructSize = 3+ 3+ 3 + 3 + 2 + 3 + 3 + 3;

	private int _kernelCollision;
	private int _kernelConstraint;

	public ComputeBuffer _hairBuffer;
	private float[] hairValues;


  public int desiredNumberHairs;


  public List<int> factorsOfNumber(int number) {

      List<int> factors = new List<int>();
      int max = (int)Mathf.Sqrt(number);  //round down
      for(int factor = 1; factor <= max; ++factor) { //test from 1 to the square root, or the int below it, inclusive.
          if(number % factor == 0) {
              factors.Add(factor);
              if(factor != number/factor) { // Don't add the square root twice!  Thanks Jon
                  factors.Add(number/factor);
              }
          }
      }
      return factors;
  }

	// Use this for initialization
	void Start () {


    //List<int> factors = factorsOfNumber( desiredNumberHairs );

    strideX = desiredNumberHairs/2;
    strideY = numVertsPerHair;
    strideZ = 2;

    print( fullVertCount );
		print( fullVertCount );
    _kernelCollision = collisionShader.FindKernel("CSMain");
    _kernelConstraint = constraintShader.FindKernel("CSMain");


    Camera.onPostRender += Render;

    CreateBuffers();
	
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


  Vector3 ToV3( Vector4 parent)
  {
     return new Vector3(parent.x, parent.y, parent.z);
  }

  float getRandomFloatFromSeed( int seed ){
  	Random.InitState(seed);
		return Random.value;
  }
	
	void CreateBuffers(){

		_hairBuffer = new ComputeBuffer( fullVertCount , vertStructSize * sizeof(float));
		hairValues = new float[ fullVertCount * vertStructSize ];

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

          int tri0 = (int)(randomVal * (float)(triBuffer.triangles.Length/3)) * 3;
          //int tri0 = (int)(randomVal *  1000 + 1000 ) * 3;
          int tri1 = tri0 + 1;
          int tri2 = tri0 + 2;

          if( id < 100 ){
          	//print( tri0 );
          	//print( triangles.Length);
          	//print( positions.Length );
          }

          tri0 = triBuffer.triangles[tri0];
          tri1 = triBuffer.triangles[tri1];
          tri2 = triBuffer.triangles[tri2];

          Vector3 pos = GetRandomPointInTriangle( hairID , vertBuffer.vertices[ tri0 ] , vertBuffer.vertices[ tri1 ]  , vertBuffer.vertices[ tri2 ]  );
					
					float a0 = AreaOfTriangle( pos , vertBuffer.vertices[tri1] , vertBuffer.vertices[tri2] );
				  float a1 = AreaOfTriangle( pos , vertBuffer.vertices[tri0] , vertBuffer.vertices[tri2] );
				  float a2 = AreaOfTriangle( pos , vertBuffer.vertices[tri0] , vertBuffer.vertices[tri1] );
				  float aTotal = a0 + a1 + a2;

				  float p0 = a0 / aTotal;
				  float p1 = a1 / aTotal;
				  float p2 = a2 / aTotal;


				  Vector3 nor     = vertBuffer.normals[tri0]  * p0 + vertBuffer.normals[tri1]  * p1 + vertBuffer.normals[tri2]  * p2;
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

           if( fPos.x == 0 ){
            Debug.Log("n");
          }
          // pos
          hairValues[index++] = fPos.x;
          hairValues[index++] = fPos.y;
          hairValues[index++] = fPos.z;

          // oPos
          hairValues[index++] = fPos.x;
          hairValues[index++] = fPos.y;
          hairValues[index++] = fPos.z;

          //vel
          hairValues[index++] = 0;
          hairValues[index++] = 0;
          hairValues[index++] = 0;

          // nor
          hairValues[index++] = nor.x;
          hairValues[index++] = nor.y;
          hairValues[index++] = nor.z;

          // uv
          hairValues[index++] = uvX;
          hairValues[index++] = uvY;

          // debug
					hairValues[index++] = 1;
          hairValues[index++] = 0;
          hairValues[index++] = 0;


          // triIDs
					hairValues[index++] = tri0;
          hairValues[index++] = tri1;
          hairValues[index++] = tri2;

          // triWeights
					hairValues[index++] = p0;
          hairValues[index++] = p1;
          hairValues[index++] = p2;
          

        }
      }
    }

    _hairBuffer.SetData(hairValues);
    

	}
	
	void Render( Camera c ){

		material.SetPass(0);

		material.SetInt( "_VertsPerHair" , numVertsPerHair );
		material.SetBuffer("_vertBuffer", _hairBuffer );
		material.SetColor("_Color", hairColor );

    Graphics.DrawProcedural(MeshTopology.Lines, totalHairs * (numVertsPerHair-1) * 2 );

	}


	// Update is called once per frame
	void FixedUpdate () {
	
	
		collisionShader.SetInt( "_Reset" , 0 );
		collisionShader.SetFloat( "_DeltaTime" , Time.deltaTime );
		collisionShader.SetFloat( "_Time" , Time.time );


    collisionShader.SetInt( "_StrideX" , strideX);
    collisionShader.SetInt( "_StrideY" , strideY);
    collisionShader.SetInt( "_StrideZ" , strideZ);
    collisionShader.SetFloat( "_SpringDistance" , distBetweenHairs );
    collisionShader.SetInt( "_NumberHumans" , humanBuffer.numberHumans );
    collisionShader.SetBuffer( _kernelCollision , "hairBuffer"     , _hairBuffer );
    collisionShader.SetBuffer( _kernelCollision , "baseBuffer"     , vertBuffer._vertBuffer );
    collisionShader.SetBuffer( _kernelCollision , "humanBuffer"     , humanBuffer._buffer );
    collisionShader.Dispatch( _kernelCollision, strideX , strideY , strideZ );

    constraintShader.SetInt( "_PassID" , 0 );


    constraintShader.SetInt( "_StrideX" , strideX);
    constraintShader.SetInt( "_StrideY" , strideY);
    constraintShader.SetInt( "_StrideZ" , strideZ/2);

    constraintShader.SetFloat( "_SpringDistance" , distBetweenHairs );
    constraintShader.SetInt( "_VertsPerHair" , numVertsPerHair );
    constraintShader.SetBuffer( _kernelConstraint , "vertBuffer"     , _hairBuffer );
    //constraintShader.SetBuffer( _kernelConstraint , "baseBuffer"     , vertBuffer._vertBuffer );
    constraintShader.Dispatch( _kernelConstraint, strideX, strideY, strideZ/2 );


    constraintShader.SetInt( "_PassID" , 1 );

    constraintShader.SetInt( "_StrideX" , strideX);
    constraintShader.SetInt( "_StrideY" , strideY);
    constraintShader.SetInt( "_StrideZ" , strideZ/2);


    constraintShader.SetFloat( "_SpringDistance" , distBetweenHairs );
    constraintShader.SetInt( "_VertsPerHair" , numVertsPerHair );
    constraintShader.SetBuffer( _kernelConstraint , "vertBuffer"     , _hairBuffer );
    //constraintShader.SetBuffer( _kernelConstraint , "baseBuffer"     , vertBuffer._vertBuffer );
    constraintShader.Dispatch( _kernelConstraint, strideX, strideY, strideZ/2 );
    
	}
}
