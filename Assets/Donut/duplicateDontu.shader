﻿Shader "Custom/duplicateDontu" {



    Properties {

        _CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}




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

            uniform samplerCUBE _CubeMap;
 

            struct VertC4 {

                float3 pos;
                float3 vel;
                float3 nor;
                float2 uv;

                float  ribbonID;
                float  life; 
                
                float3 debug;

                float row;
                float col;

                float lID;
                float rID;
                float uID;
                float dID;

            };
            
            struct Pos {
                float3 pos;
            };

            StructuredBuffer<VertC4> buf_Points;
            StructuredBuffer<Pos> og_Points;

            uniform float4x4 worldMat;

            uniform int _RibbonWidth;
            uniform int _RibbonLength;
            uniform int _TotalVerts;

            uniform sampler2D _AudioMap;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 nor      : TEXCOORD0;
                float3 eye      : TEXCOORD2;
                float3 debug    : TEXCOORD3;
                float2 uv       : TEXCOORD4;
            };

            uint getID( uint id  ){

                uint base = floor( id / 6 );
                uint tri  = id % 6;
                uint row = floor( base / _RibbonWidth );
                uint col = (base) % _RibbonWidth;

                uint rowU = (row + 1) % _RibbonLength;
                uint colU = (col + 1) % _RibbonWidth;

                uint rDoID = row * _RibbonWidth;
                uint rUpID = rowU * _RibbonWidth;

                uint cDoID = col;
                uint cUpID = colU;

                uint fID = 0;

                if( tri == 0 ){
                    fID = rDoID + cDoID;
                }else if( tri == 1 ){
                    fID = rUpID + cDoID;
                }else if( tri == 2 ){
                    fID = rUpID + cUpID;
                }else if( tri == 3 ){
                    fID = rDoID + cDoID;
                }else if( tri == 4 ){
                    fID = rUpID + cUpID;
                }else if( tri == 5 ){
                    fID = rDoID + cUpID;
                }else{
                    fID = 0;
                }

                return fID;

            }
           

            //Our vertex function simply fetches a point from the buffer corresponding to the vertex index
            //which we transform with the view-projection matrix before passing to the pixel program.
            varyings vert (uint id : SV_VertexID){

                varyings o;

                // from getRibbonID 
                uint fID = getID( id );
                VertC4 v = buf_Points[fID];
                Pos og = og_Points[fID];

                float3 pos =  mul( worldMat , float4( v.pos , 1.) ).xyz - v.pos;

                o.worldPos = pos;

                o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));

                float3 fDif = normalize( o.worldPos );//normalize( dif ) * 1.;
                float3 fNor = v.nor * 1.;
                float3 fVel = normalize( v.vel) * 1.;

                o.debug = normalize(fDif + fNor + fVel);

                o.eye = _WorldSpaceCameraPos - o.worldPos;
                o.uv = v.uv;

                o.nor = v.nor; 
                return o;

            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {

                float m = dot( i.eye , i.nor );
                float3 cubeCol = texCUBE(_CubeMap,i.debug).rgb;
                float3 col = cubeCol;// * (i.debug * .5 + .5);
                float4 aCol = tex2D( _AudioMap , float2( m , 0) );
                col = col* .2 + col * 20 * aCol.xyz;

                col *= float3( 2, 1.3 , .4);
                return float4( col , 1.);

            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}
