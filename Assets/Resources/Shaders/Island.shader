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

      uniform int _NumberSteps;
      uniform int _Fade;
      uniform float  _IntersectionPrecision;
      uniform float _MaxTraceDistance;

      uniform float3 _Hand1;
      uniform float3 _Hand2;
      uniform float4 _Color;
      
      uniform samplerCUBE _CubeMap;
      uniform sampler2D _AudioMap;


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

          //float3 rd     : TEXCOORD3;
          float3 camPos : TEXCOORD4;
      };
        

   
      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = v.normal;
        
        o.uv = v.texcoord;
  
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position );
     
        float3 mPos = mul( unity_ObjectToWorld , v.position );

        o.ro = v.position;
        o.camPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos  , 1. )); 

        return o;

      }


     // Fragment Shader
      fixed4 frag(VertexOut v) : COLOR {

        float3 ro = v.ro;
        float3 rd = normalize(ro - v.camPos);

        float3 col = float3( 0.0 , 0.0 , 0.0 );

    		float3 fRefl = reflect( -rd , v.normal );
          float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

        float4 aCol = tex2D( _AudioMap , float2(v.ro.z *100 , 0.));
    		
    		col= cubeCol;// * ( v.normal * .5 + .5 );
        //col *= clamp( (v.ro.z-.0000) * 500 , 0 , 1 );

        col *= ( aCol.xyz * aCol.xyz ) * 3. + .2;

        col *= _Color.xyz;//float3( .4 , .6 , 3 );

    	
            fixed4 color;
            color = fixed4( col , 1. );
            return color;
      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}