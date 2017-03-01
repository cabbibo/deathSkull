// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Island" {
 Properties {
  

    _NumberSteps( "Number Steps", Int ) = 20
    _MaxTraceDistance( "Max Trace Distance" , Float ) = 10.0
    _IntersectionPrecision( "Intersection Precision" , Float ) = 0.0001
    _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
    _Fade( "Fade", Int ) = 1
    _Color( "Color", COLOR) = (1,1,1,1)



  }
  
  SubShader {
    //Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

    Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
    LOD 200

    Cull Off

    Pass {
      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending


      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag
      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"
      
 
      
      #include "Chunks/noise.cginc"
      #include "Chunks/sdfFunctions.cginc"

      uniform int _NumberSteps;
      uniform int _Fade;
      uniform float  _IntersectionPrecision;
      uniform float _MaxTraceDistance;

      uniform float3 _Hand1;
      uniform float3 _Hand2;
      uniform float4 _Color;
      
      uniform samplerCUBE _CubeMap;
      uniform sampler2D _AudioMap;

      struct Head{

  float4x4 localToWorld;
  float4x4 worldToLocal;
  float3 pos;
  float3 debug;

};

struct Hand{

  float4x4 localToWorld;
  float4x4 worldToLocal;
  float3 pos;
  float3 vel;
  float trigger;
  float3 debug;

};


struct Human{

  Head head;
  Hand hand1;
  Hand hand2;

};

int _NumberHumans;



      StructuredBuffer<Human> humanBuffer;


      struct VertexIn
      {
         float4 position  : POSITION; 
         float3 normal    : NORMAL; 
         float4 texcoord  : TEXCOORD0; 
         float4 tangent   : TANGENT;
      };

      struct VertexOut {
          float4 pos    : POSITION; 
          float3 normal : NORMAL; 
          float4 uv     : TEXCOORD0; 
          float3 ro     : TEXCOORD2;
          float3 closestHand : TEXCOORD3;

          //float3 rd     : TEXCOORD3;
          float3 camPos : TEXCOORD4;
      };
        

   
      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        float3 closestHand = float3( 1000000, 100000,10000);
        float closest = 10000000;
        float3 mP; float3 dis;
        for( int i = 0; i < _NumberHumans; i++ ){

          mP = mul( unity_WorldToObject , float4( humanBuffer[i].hand1.pos  , 1. ) );
          dis = v.position - mP;
          if( length( dis ) < closest){
            closest = length( dis );
            closestHand = mP;
          }

          mP = mul( unity_WorldToObject , float4( humanBuffer[i].hand2.pos  , 1. ) );
          dis = v.position - mP;
          if( length( dis ) < closest){
            closest = length( dis );
            closestHand = mP;
          }

        }

        o.normal = v.normal;

        o.closestHand = closestHand;
        
        o.uv = v.texcoord;
  
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position );
     
        float3 mPos = mul( unity_ObjectToWorld , v.position );

        o.ro = v.position;
        o.camPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos  , 1. )); 

        return o;

      }



      float3 closestHand;

      float2 map( in float3 pos ){
        
        float2 res;
        float2 lineF;
        float2 sphere;

        //res = float2( -sdBox( pos , float3( 1. , 1. , 1. ) * .53 ) , 0. );
        //res = smoothU( res , float2( sdSphere( pos , .04) , 1. ) , 0.1);

        sphere =float2( opRepSphere(  pos , float3( .0025 , .0025, .0025) , .0012 ) , 1);// float r)float2( sdSphere( pos, .1 ) , 0);
        res = sphere;

        //res = float2( 100000000 , 0 );
       

        float n  =  .13 * noise( pos * 3000 + float3( _Time.y , 0 , 0 ) ) * 4;
        n +=  .6 * noise( pos * 10000 + float3( 0, _Time.y , 0 ) );// + .5 * noise( pos * 800000+ float3( 0,0,_Time.y ) );



        res.x  += n * .0006;// * (1- _LightsOn) + _LightsOn * .06;// + .02 * length( aNoise );

        //res = smoothU( res , float2( box * .01 , 2 ) , 0.01 );
        //
        //

        return res;//float2( length( pos ) - .3, 0.1 ); 
     
      }

      float3 calcNormal( in float3 pos ){

        float3 eps = float3( 0.001, 0.0, 0.0 );
        float3 nor = float3(
            map(pos+eps.xyy).x - map(pos-eps.xyy).x,
            map(pos+eps.yxy).x - map(pos-eps.yxy).x,
            map(pos+eps.yyx).x - map(pos-eps.yyx).x );
        return normalize(nor);

      }
              
         

      float2 calcIntersection( in float3 ro , in float3 rd ){     
            
               
        float h =  _IntersectionPrecision * 2;
        float t = 0.0;
        float res = -1.0;
        float id = -1.0;
        
        for( int i=0; i< 20; i++ ){
            
            if( h < _IntersectionPrecision || t > _MaxTraceDistance ) break;
    
            float3 pos = ro + rd*t;
            float2 m = map( pos );
            
            h = m.x;
            t += h;
            id = m.y;
            
        }
    
    
        if( t <  _MaxTraceDistance ){ res = t; }
        if( t >  _MaxTraceDistance ){ id = -1.0; }
        
        return float2( res , id );
          
      
      }
         

     // Fragment Shader
      fixed4 frag(VertexOut v) : COLOR {

        float3 ro = v.ro;
        float3 rd = normalize(v.ro - v.camPos);
        closestHand = v.closestHand;

        float3 d = closestHand - ro;



        float3 col = float3( 0.0 , 0.0 , 0.0 );
        float n = noise( ro * 10000 );

        ///float3 raytraceCol;

        float3 fNorm;
        float fadeVal = 1;
        if( length( d ) < .001 - .0004 * n ){
          
          float2 res = calcIntersection(ro, rd);

          if( res.y > 0 ){
            fNorm  = calcNormal( ro + rd * res.x);

            fadeVal = 1 - ( res.x / .005);
          }else{
            fNorm = float3( 1,0,0);//v.normal;
            fadeVal = 0;
          }

        }else{ 
          fNorm = v.normal;
        }



        




          float3 fRefl = reflect( -rd , fNorm );
          float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

          float4 aCol = tex2D( _AudioMap , float2(v.ro.z *100 , 0.));
          
          col= cubeCol;// * ( v.normal * .5 + .5 );
          //col *= clamp( (v.ro.z-.0000) * 500 , 0 , 1 );

          col *= ( aCol.xyz * aCol.xyz ) * 3. + .2;

          col *= _Color.xyz;
          col *= fadeVal;

      
       


       
        //col /= length( d ) * 10;

          //float3( .4 , .6 , 3 );
        // col  *= (multipler - 1);
    	
            fixed4 color;
            color = fixed4( col , 1. );
            return color;
      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}