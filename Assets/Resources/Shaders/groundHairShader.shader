Shader "Custom/groundHairShader" {
	  Properties {
        _CubeMap( "Cube Map" , Cube ) = "white" {}
    }

    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

            Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
            CGPROGRAM
            #pragma target 5.0
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            #include "Chunks/noise.cginc"
            #include "Chunks/hsv.cginc"

 

            struct Vert{
				float3 pos;
				float3 oPos;
				float3 ogPos;
				float3 vel;
				float3 norm;
				float2 uv;
				float3 debug;
			};
            

            StructuredBuffer<Vert> _vertBuffer;

            //uniform float4x4 worldMat;

            uniform int _VertsPerHair;
            uniform int _TotalVerts;
            uniform float3 _CenterPos;

            uniform samplerCUBE _CubeMap;
            uniform sampler2D _AudioMap;
            uniform float _LevelEnded;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 nor      : TEXCOORD0;
                float3 eye      : TEXCOORD2;
                float3 debug    : TEXCOORD3;
                float3 vel      : TEXCOORD5;
                float2 uv       : TEXCOORD4;
            };

            
           

            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                /*int segID =floor( float(id) / 2);
                int offset = id % 2;
                int idInHair = segID % (_VertsPerHair-1);
                int hairID = int( floor( float(segID) / float(_VertsPerHair) ));*/
               		
                int halfID = id/2;
                int hairID = halfID/(_VertsPerHair-1);

                int offsetID = id % 2;



               	int fID = halfID + offsetID + hairID;//(hairID * ((_VertsPerHair ))) + idInHair + offset;

                Vert v = _vertBuffer[fID];

                float3 nor;
                if( offsetID + hairID == 0 ){
                	nor = mul( unity_ObjectToWorld , float4(v.norm,0)).xyz;
                	nor = normalize( nor );
                }else{
                	Vert vDown = _vertBuffer[fID-1];
                	nor = -normalize( v.pos - vDown.pos );
                }

                float3 dif =   - v.pos;

                o.worldPos = v.pos;//mul( worldMat , float4( v.pos , 1.) ).xyz;

                o.eye = _WorldSpaceCameraPos - o.worldPos;

                o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));


                o.debug = v.debug;//n * .5 + .5;
                o.uv = v.uv;
                o.nor = nor;
                o.vel = v.vel;

            
                return o;

            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings v) : COLOR {

                float3 centerVec = v.worldPos - _CenterPos;
                float n = noise( v.worldPos * 10 ) + noise( v.worldPos * 20) * .5;
                if( length( centerVec.xz ) > 7 - n * 1){ discard; }

                float3 nNor = normalize(v.vel) * .5 + .5;
                float3 refl = reflect( normalize( v.eye) , v.nor );
                float3 cubeCol = texCUBE( _CubeMap , refl ).xyz;
                float3 col = float3(1., 0,0) * nNor.x + float3( 1., .4 , .2 ) * nNor.y  + float3( 1. , .8,.5) * nNor.z;////-dot( normalize(v.eye) , reflect( normalize(v.eye), v.nor));// * lerp( float3(1,.5 , .3), float3( 0, .8,.4),  noise( v.worldPos * 100));// v.nor * .5 + .5;
                
                float4 aCol = tex2D( _AudioMap , float2((v.uv.x + v.uv.y) * .2 , 0.));
                col = (aCol.xyz+ .04) * 10. * cubeCol * hsv(v.uv.x + v.uv.y,.2,1) * 2; //normalize(refl + normalize(v.vel))* .5 + .5;
                col *= v.uv.x;

                col = lerp( col * float3( 3 , 2, .6 ) , col * float3( 2, .4 , 1) , v.uv.x);
                //col = lerp( length( col) * .3, col , _LevelEnded);
               // col = v.debug;
                return float4( col , 1 );

            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}