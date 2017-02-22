// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/spheres"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _AudioMap;
			float4 _MainTex_ST;
			float _TotalPickedUp;
			float _Explosion;
			float _id;
			
			v2f vert (appdata v)
			{
				v2f o;	

				float lu = dot( v.normal ,normalize(_WorldSpaceCameraPos- mul( unity_ObjectToWorld , v.vertex).xyz));

				float4 aCol = tex2Dlod( _AudioMap , float4( lu * .1 + .2 ,0 , 0 ,1) );

				float s = 1.5 + sin( _id * 200);
				//s*= s * s*s*s;
				float4 fV = float4( v.vertex.xyz + v.normal * length( aCol) * .1 * s  , 1);
				o.vertex = UnityObjectToClipPos(fV);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float4 aCol = tex2Dlod( _AudioMap , float4( i.uv.x * .1 + .2 ,0 , 0 ,1) );

				fixed4 c1 = tex2D (_MainTex, i.uv + .1 * float2( .8 *  _Time.y * _TotalPickedUp , _Time.y * _TotalPickedUp )) ;
			fixed4 c2 = tex2D (_MainTex, i.uv - .1 * float2( -.3 * _Time.y * _TotalPickedUp , .7 * _Time.y * _TotalPickedUp ));
			//fixed4 c3 = tex2D (_MainTex, IN.uv_MainTex - .05 * float2( _Time.y * _TotalPickedUp , _Time.y * _TotalPickedUp )) * _Color;
			col.xyz = c1.rgb + c2.rgb + _Explosion * 10;

			col *= aCol;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
