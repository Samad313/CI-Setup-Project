// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Werplay/LightmapSurfaceTint"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Texture", 2D) = "white" {}
		_Light_Map ("Light_Map", 2D) = "white" {}
        _intensity ("intensity", Float ) = 1.99
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
				float2 texcoord0 : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			
			uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _Light_Map; uniform float4 _Light_Map_ST;
            uniform float _intensity;
             uniform float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = fixed4(((tex2D(_Light_Map,TRANSFORM_TEX(i.uv1.rg, _Light_Map)).rgb*_intensity)*tex2D(_MainTex,TRANSFORM_TEX(i.uv0.rg, _MainTex)).rgb * _Color),1);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
