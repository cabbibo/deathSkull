Shader "Custom/SelectOrbs" {
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

  Properties {
    _NumberSteps( "Number Steps", Int ) = 10
    _MaxTraceDistance( "Max Trace Distance" , Float ) = 6.0
    _IntersectionPrecision( "Intersection Precision" , Float ) = 0.0001
		_CubeMap( "Cube Map" , Cube )  = "defaulttexture" {}
			_Color( "Color", COLOR) = (1,1,1,1)
  }

  
  SubShader {
    //Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

    Cull Off
    Tags { "RenderType"="Opaque" "Queue" = "Geometry" }
    LOD 200
    Pass {
      //Blend SrcAlpha OneMinusSrcAlpha // Alpha blending


      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      // Use shader model 3.0 target, to get nicer looking lighting
      #pragma target 3.0

      #include "UnityCG.cginc"
      #include "chunks/noise.cginc"
     

      uniform int _NumberSteps;
      uniform float  _IntersectionPrecision;
      uniform float _MaxTraceDistance;

  		uniform float4 _Color;
      uniform float3 _Selector;
      uniform float3 _Hand1;
      uniform float3 _Hand2;
      uniform float3 _Center;
      uniform int _PrismInside;
      uniform int _Selected;
      uniform int _CenterObj;
      uniform int _SelectorMoving;


      uniform samplerCUBE _CubeMap;
      

      float3 origin;

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
          float3 origin : TEXCOORD1;

          //float3 rd     : TEXCOORD3;
          float3 camPos : TEXCOORD4;
      };
        

      float sdBox( float3 p, float3 b ){

        float3 d = abs(p) - b;

        return min(max(d.x,max(d.y,d.z)),0.0) +
               length(max(d,0.0));

      }

      float sdSphere( float3 p, float s ){
        return length(p)-s;
      }

      float sdCapsule( float3 p, float3 a, float3 b, float r )
      {
          float3 pa = p - a, ba = b - a;
          float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
          return length( pa - ba*h ) - r;
      }

      float2 smoothU( float2 d1, float2 d2, float k)
      {
          float a = d1.x;
          float b = d2.x;
          float h = clamp(0.5+0.5*(b-a)/k, 0.0, 1.0);
          return float2( lerp(b, a, h) - k*h*(1.0-h), lerp(d2.y, d1.y, pow(h, 2.0)));
      }

      float3 rotatedBox( float3 p, float4x4 m )
			{
			    float3 q = mul( m , float4( p , 1 )).xyz;
			    return sdBox(q,float3(.2,.2,.2));
			}


      float2 map( in float3 pos ){
        
        float2 res;
        float2 lineF;
        float2 sphere;

        //res = float2( -sdBox( pos , float3( 1. , 1. , 1. ) * .53 ) , 0. );
        //res = smoothU( res , float2( sdSphere( pos , .04) , 1. ) , 0.1);

        sphere = float2( -sdSphere( pos, .5  ) , 1. );

       

        float n  =  .13 * noise( pos * 10 + float3( _Time.y , 0 , 0 ) ) * 4 + .6 * noise( pos * 30 + float3( 0, _Time.y , 0 ) ) + .5 * noise( pos * 80+ float3( 0,0,_Time.y ) );

        res = sphere;
        //res = smoothU( res , float2( sdSphere( pos , .04) , 1. ) , 0 );
       // float3 tPos = mul( _Prism , float4( 0,0,0 , 1)).xyz;

       // float box = rotatedBox(pos , _Prism );


       	if( _CenterObj == 1){
        	res = smoothU( res , float2( sdSphere( pos , .2) , 1. ) , 0.1 );
       	}
        res = smoothU( res , float2( sdSphere( pos - _Hand1 , .1) , 2. ) , 0.1 );
        res = smoothU( res , float2( sdSphere( pos - _Hand2 , .1) , 2. ) , 0.1 );
       
        res = smoothU( res , float2( sdSphere( pos - _Selector , .3) , 3. ) , 0.1 );
        res = smoothU( res , float2( sdCapsule( pos, _Selector , _Center , .04) , 4. ) , 0.1 );

        


        //res.x  += n * .06;// * (1- _LightsOn) + _LightsOn * .06;// + .02 * length( aNoise );

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
            
    

      VertexOut vert(VertexIn v) {
        
        VertexOut o;

        o.normal = v.normal;
        
        o.uv = v.texcoord;
  
        // Getting the position for actual position
        o.pos = mul( UNITY_MATRIX_MVP , v.position );
     
        float3 mPos = mul( unity_ObjectToWorld , v.position );

        o.ro = v.position;
        o.origin = mul( unity_ObjectToWorld , float4(0,0,0,1) );
        o.camPos = mul( unity_WorldToObject , float4( _WorldSpaceCameraPos  , 1. )); 

        return o;

      }


     // Fragment Shader
      fixed4 frag(VertexOut i) : COLOR {

        float3 ro = i.ro;
        float3 rd = normalize(ro - i.camPos);
        origin = i.origin;

        float3 col = float3( 0.0 , 0.0 , 0.0 );
    		float2 res = calcIntersection( ro , rd );
    		
    		col= float3( 0. , 0. , 0. );


    		if( res.y > -0.5 ){


    			float3 pos = ro + rd * res.x;
    			float3 norm = calcNormal( pos );
    			col = float3(1,1,1);//norm * .5 + .5;

    			float3 fRefl = reflect( -rd , norm );
          float3 cubeCol = texCUBE(_CubeMap,-fRefl ).rgb;

          col *= cubeCol;

          if( _SelectorMoving == 1 && res.y == 3. ){
          	col *= 4;

          }
          //col = lerp( col , -dot(rd , norm) , _LightsOn);
          //col *= float3(.4,.4,.4) + float3( 2.2 , 1.0 * ( pos - origin + .1 ).y * 10, ( pos - origin + .1 ).y * .4 );
          
    			
    		}else{
    			discard;
    		}

    		col *= _Color.xyz;
    		if( _Selected == 1 ){
    			col *= 2;
    		}

    		//if( length( i.uv ) > 1.5 ){ col = float3( 0,0,0);}
     
    		//col = float3( 1. , 1. , 1. );
         // col *= float3( 2.2 , 1.3 , .4 );
        fixed4 color;
        color = fixed4( col , 1. );
        return color;
      }

      ENDCG
    }
  }
  FallBack "Diffuse"
}