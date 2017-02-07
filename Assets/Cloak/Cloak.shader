Shader "Custom/Cloak" {
	
    SubShader{
//        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        Cull off
        Pass{

           // Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
 
            CGPROGRAM
            #pragma target 5.0
 
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
            #include "Chunks/uvNormalMap.cginc"
            #include "Chunks/noise.cginc"

            uniform sampler2D _NormalMap;
            uniform samplerCUBE _CubeMap;
 
            uniform float4 _Color;

            struct Vert{
							float3 pos;
							float3 oPos;
							float3 ogPos;
							float3 norm;
							float2 uv;
							float life;
							float ids[8];
							float3 debug;
						};

            struct Hand{
						  float active;
						  float3 pos;
						  float3 vel;
						  float3 aVel;
						  float  triggerVal;
						  float  thumbVal;
						  float  sideVal;
						  float2 thumbPos;
						};

            
            struct Pos {
                float3 pos;
            };



            StructuredBuffer<Vert> buf_Points;
            //StructuredBuffer<Shape> shapeBuffer;
            //StructuredBuffer<Hand > handBuffer;

            uniform float4x4 worldMat;

            uniform int _RibbonWidth;
            uniform int _RibbonLength;
            uniform int _TotalVerts;
 
            //A simple input struct for our pixel shader step containing a position.
            struct varyings {
                float4 pos      : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 nor      : TEXCOORD0;
                float3 eye      : TEXCOORD2;
                float3 debug    : TEXCOORD3;
                float2 uv       : TEXCOORD4;
                float closest   : TEXCOORD6;
                float life 			: TEXCOORD5;
            };

            uint getID( uint id  ){

                uint base = floor( id / 6 );
                uint tri  = id % 6;
                uint row = floor( base / ( _RibbonWidth -1 ) );
                uint col = (base) % ( _RibbonWidth - 1 );

                uint rowU = (row + 1);// % _RibbonLength;
                uint colU = (col + 1);// % _RibbonWidth;

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
                Vert v = buf_Points[fID];

                o.worldPos = v.pos;//mul( worldMat , float4( v.pos , 1.) ).xyz;

                o.eye = _WorldSpaceCameraPos - o.worldPos;

                o.pos = mul (UNITY_MATRIX_VP, float4(o.worldPos,1.0f));



                o.debug = v.debug;//n * .5 + .5;
                //o.debug = v.norm * .5 + .5;
                o.life = v.life;
                o.uv = v.uv;
                o.nor = -v.norm;

            
                return o;

            }
 
            //Pixel function returns a solid color for each point.
            float4 frag (varyings i) : COLOR {


                float3 fNorm = uvNormalMap( _NormalMap , i.worldPos ,  i.uv  * float2( 1 , 1), i.nor , 1 , .3);

                float3 col = fNorm * .5 + .5;//i.debug;




                float3 fRefl = reflect( -normalize(i.eye) , fNorm );
                float3 cubeCol = texCUBE(_CubeMap,fRefl ).rgb;

                col =  cubeCol * 2 * _Color;

                //col = fRefl * .5 + .5;

                float m = dot( normalize( i.eye ) , fNorm );

                //col = float3(1,1,1);
                //col *= 1 - 4. * (max(m*m*m,0));

                //col = float3(1,0,0);

                fixed4 color;

	            	color = fixed4( col , 1. );
	            	return color;
	           

            }
 
            ENDCG
 
        }
    }
 
    Fallback Off
	
}