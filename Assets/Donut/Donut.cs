using UnityEngine;
using System.Collections;


using System.IO;
using System.Text;

public class Donut : MonoBehaviour {


    // How the donut looks
    public Material material;

    // How the donut feels
    public ComputeShader computeShader;

    public HumanBuffer humanBuffer;

    public AudioListenerTexture audioTexture;


    public float tubeRadius = .6f;
    public float shellRadius = .8f;

    private float[] inValues;


    public ComputeBuffer _vertBuffer;
    public ComputeBuffer _ogBuffer;
    public ComputeBuffer _transBuffer;


    private const int threadX = 8;
    private const int threadY = 8;
    private const int threadZ = 8;

    private const int strideX = 8;
    private const int strideY = 8;
    private const int strideZ = 8;

    private int gridX { get { return threadX * strideX; } }
    private int gridY { get { return threadY * strideY; } }
    private int gridZ { get { return threadZ * strideZ; } }

    public int vertexCount { get { return gridX * gridY * gridZ; } }


    public int ribbonWidth = 1024;
    public int ribbonLength { get { return (int)Mathf.Floor( (float)vertexCount / ribbonWidth ); } }
    

    private int _kernel;
    //private Material material;

    private Vector3 p1;
    private Vector3 p2;

    private bool objMade = false;
    private float[] transValues = new float[32];



    // Use this for initialization
    void Start () {



        
     

        createBuffers();
       // createMaterial();

        _kernel = computeShader.FindKernel("CSMain");



    
    }

    void FixedUpdate(){

        Dispatch();

    }

    void OnRenderObject(){
        Render(Camera.current);
    }

    //When this GameObject is disabled we must release the buffers or else Unity complains.
    private void OnDisable(){
        Camera.onPostRender -= Render;
        ReleaseBuffer();
    }

      //For some reason I made this method to create a material from the attached shader.
    /*private void createMaterial(){

      material = new Material( shader );

    }*/

    private int getID( int id  ){

        int b = (int)Mathf.Floor( id / 6 );
        int tri  = id % 6;
        int row = (int)Mathf.Floor( b / ribbonWidth );
        int col = (b) % ribbonWidth;

        int rowU = (row + 1) % ribbonLength;
        int colU = (col + 1) % ribbonWidth;

        int rDoID = row * ribbonWidth;
        int rUpID = rowU * ribbonWidth;

        int cDoID = col;
        int cUpID = colU;

        int fID = 0;

        if( tri == 0 ){
            fID = rDoID + cDoID;
        }else if( tri == 1 ){
            fID = rUpID + cUpID;
        }else if( tri == 2 ){
            fID = rUpID + cDoID;
        }else if( tri == 3 ){
            fID = rDoID + cDoID;
        }else if( tri == 4 ){
            fID = rDoID + cUpID;
        }else if( tri == 5 ){
            
            fID = rUpID + cUpID;
        }else{
            fID = 0;
        }

        return fID;

    }



    //Remember to release buffers and destroy the material when play has been stopped.
    void ReleaseBuffer(){

      _vertBuffer.Release(); 
      _ogBuffer.Release(); 
      _transBuffer.Release(); 
      //DestroyImmediate( material );

    }

        //After all rendering is complete we dispatch the compute shader and then set the material before drawing with DrawProcedural
    //this just draws the "mesh" as a set of points
    public void Render(Camera camera) {


        
        int numVertsTotal = ribbonWidth * 3 * 2 * (ribbonLength-1);

        material.SetPass(0);

        material.SetBuffer("buf_Points", _vertBuffer);
        material.SetBuffer("og_Points", _ogBuffer);

        material.SetInt( "_RibbonWidth"  , ribbonWidth  );
        material.SetInt( "_RibbonLength" , ribbonLength );
        material.SetInt( "_TotalVerts"   , vertexCount  );

        material.SetTexture("_AudioMap" , audioTexture.AudioTexture );

        material.SetMatrix("worldMat", transform.localToWorldMatrix);
        material.SetMatrix("invWorldMat", transform.worldToLocalMatrix);

        Graphics.DrawProcedural(MeshTopology.Triangles, numVertsTotal);


    }

    private Vector3 getVertPosition( float uvX , float uvY  ){

        float u = uvY * 2.0f * Mathf.PI;
        float v = uvX * 2.0f * Mathf.PI;

        float largeMovement = Mathf.Sin( uvY * 20.0f - .4f ) * .2f + .2f;
        float smallMovement = Mathf.Sin( uvY * 100.0f + .01f )  * ( uvY * uvY * .03f);
        float tubeRad = tubeRadius * Mathf.Pow(uvY, .5f)  + (1-uvY) * largeMovement * tubeRadius + smallMovement;// * Mathf.Clamp( ( 1 / Mathf.Pow( uvY * 6 , 2 )) , 1, 2) * Mathf.SmoothStep( 0 , 1 , uvY * 20);// * Mathf.SmoothStep( 0 , 1 , uvY * 2 );// * Mathf.Min( uvY * 10 , 1 );// * Mathf.Pow( uvY - .01f , .3f)  * ( 1.0f + largeMovement + smallMovement ) ;
        float slideRad = shellRadius;// / 2.0f;// + uvY;

        float xV = (slideRad + tubeRad * Mathf.Cos(v)) * Mathf.Cos(u * .5f) ;
        float zV = (slideRad + tubeRad * Mathf.Cos(v)) * Mathf.Sin(u * .5f) ;


        float yV = (tubeRad) * Mathf.Sin(v) * Mathf.Abs( Mathf.Sign(Mathf.Sin( v)));
        if( Mathf.Sign( Mathf.Sin( v) ) < 0 ){

            yV *= .4f;
        }
        yV += tubeRad * .4f;
        //print( xV );
        return new Vector3( xV , yV , zV );

    }

    private void createBuffers() {

      _vertBuffer = new ComputeBuffer( vertexCount ,  AssignStructs.VertC4StructSize * sizeof(float));
      _ogBuffer = new ComputeBuffer( vertexCount ,  3 * sizeof(float));
      _transBuffer = new ComputeBuffer( 32 ,  sizeof(float));
      
      inValues = new float[ AssignStructs.VertC4StructSize * vertexCount];
      float[] ogValues = new float[ 3         * vertexCount];

      // Used for assigning to our buffer;
      int index = 0;
      int indexOG = 0;


      for (int z = 0; z < gridZ; z++) {
        for (int y = 0; y < gridY; y++) {
          for (int x = 0; x < gridX; x++) {

            int id = x + y * gridX + z * gridX * gridY; 
            
            float col = (float)(id % ribbonWidth );
            float row = Mathf.Floor( ((float)id+.01f) / ribbonWidth);


            float uvX = col / ribbonWidth;
            float uvY = row / ribbonLength;

            Vector3 fVec = getVertPosition( uvX , uvY );


            //pos
            ogValues[indexOG++] = fVec.x;
            ogValues[indexOG++] = fVec.y;
            ogValues[indexOG++] = fVec.z;

            AssignStructs.VertC4 vert = new AssignStructs.VertC4();


            vert.pos = fVec * .9f;
            vert.vel = new Vector3( 0 , 0 , 0 );
            vert.nor = new Vector3( 0 , 1 , 0 );
            vert.uv  = new Vector2( uvX , uvY );
            vert.ribbonID = 0;
            vert.life = -1;
            vert.debug = new Vector3( 0 , 1 , 0 );
            vert.row   = row; 
            vert.col   = col; 

            vert.lID = convertToID( col - 1 , row + 0 );
            vert.rID = convertToID( col + 1 , row + 0 );
            vert.uID = convertToID( col + 0 , row + 1 );
            vert.dID = convertToID( col + 0 , row - 1 );

            AssignStructs.AssignVertC4Struct( inValues , index , out index , vert );

          }
        }
      }

      _vertBuffer.SetData(inValues);
      _ogBuffer.SetData(ogValues);

    }

    private float convertToID( float col , float row ){

        float id;

        if( col >= ribbonWidth ){ col -= ribbonWidth; }
        if( col < 0 ){ col += ribbonWidth; }

        if( row >= ribbonLength ){ row -= ribbonLength; }
        if( row < 0 ){ row += ribbonLength; }

        id = row * ribbonWidth + col;

        return id;

    }
    
    private void Dispatch() {

        AssignStructs.AssignTransBuffer( transform , transValues , _transBuffer );

        // Setting up hand buffers
        int index = 0;
     

        computeShader.SetInt( "_NumberHumans", humanBuffer.numberHumans);

        computeShader.SetFloat( "_DeltaTime"    , Time.deltaTime );
        computeShader.SetFloat( "_Time"         , Time.time      );

        computeShader.SetInt( "_RibbonWidth"   , ribbonWidth     );
        computeShader.SetInt( "_RibbonLength"  , ribbonLength    );


        computeShader.SetBuffer( _kernel , "transBuffer"  , _transBuffer    );
        computeShader.SetBuffer( _kernel , "vertBuffer"   , _vertBuffer     );
        computeShader.SetBuffer( _kernel , "ogBuffer"     , _ogBuffer       );
        computeShader.SetBuffer( _kernel , "humanBuffer"  , humanBuffer._buffer);
        computeShader.SetBuffer( _kernel , "audioBuffer"  , audioTexture._buffer);
        computeShader.SetInt( "_AudioLength" , audioTexture.size );

        computeShader.Dispatch(_kernel, strideX , strideY , strideZ );


    }
}
