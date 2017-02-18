﻿Shader "Custom/bloodThread" {
	Properties {
        _NormalMap( "Normal Map" , 2D ) = "white" {}
        _CubeMap( "Cube Map" , Cube ) = "white" {}
        _TexMap( "Tex Map" , 2D ) = "white" {}
        _SizeMultiplier( "Size Multiplier" , float ) = 1
    }
  SubShader{

  	


    Cull off
    Pass{


      CGPROGRAM
      #pragma target 5.0

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform sampler2D _NormalMap;
      uniform sampler2D _TexMap;
      uniform samplerCUBE _CubeMap;



			struct Vert{

  float  used;
  float3 pos;
  float3 vel;
  float3 nor;
  float2 uv;

  float3 targetPos;

  float3 bindPos;
  float3 bindNor;
  float4 boneWeights;
  float4 boneIDs;
  float3 debug;
  
};



      StructuredBuffer<Vert> _vertBuffer;
      StructuredBuffer<int> _triBuffer;

      //A simple input struct for our pixel shader step containing a position.
      struct varyings {
          float4 pos 			: SV_POSITION;
          float3 nor 			: TEXCOORD0;
          float2 uv  			: TEXCOORD1;
          //float2 suv 			: TEXCOORD2;
          //float3 col 			: TEXCOORD3;
          //float  lamb 		: TEXCOORD4;
          float3 eye      : TEXCOORD5;
          float3 worldPos : TEXCOORD6;
          float3 debug    : TEXCOORD7;

      };


      varyings vert (uint id : SV_VertexID){

        varyings o;


        Vert v = _vertBuffer[_triBuffer[id]];



				o.pos = mul (UNITY_MATRIX_VP, float4(v.pos,1.0f));
				o.worldPos = v.pos;
				o.eye = _WorldSpaceCameraPos - o.worldPos;
	
				o.nor = v.nor;//normalize(v.nor);
				o.uv = v.uv;
        o.debug = v.debug;

        return o;


      }
      //Pixel function returns a solid color for each point.
      float4 frag (varyings v) : COLOR {

      	float3 eyeRefl = reflect( v.eye , v.nor );

      	float3 cubeCol = texCUBE( _CubeMap , eyeRefl ).xyz;

      	float3 col = tex2D(_TexMap, v.uv);// + float3( .6 , 0,0);// v.nor * .5 + .5;

        return float4( col , 1. );


      }

      ENDCG

    }
  }

  Fallback Off
  
}