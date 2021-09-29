// -Vertex Lit + ShadowCaster
// - Premultiplied Alpha Blending (Optional straight alpha input)
// - Double-sided, no depth

Shader "Spine/Spine-Skeleton-Lit AlwaysVisible" {
	Properties{
		_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
		[NoScaleOffset] _MainTex("Main Texture", 2D) = "black" {}
		_Color("always visible color", Color) = (0,0,0,0)
		_OverlayPwr ("OverlayIntensity", Range(0,1)) = 1
	   [Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
	   [Toggle(_DOUBLE_SIDED_LIGHTING)] _DoubleSidedLighting("Double-Sided Lighting", Int) = 0
	   [HideInInspector] _StencilRef("Stencil Reference", Float) = 1.0
	   [HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 8 // Set to Always as default

	   // Outline properties are drawn via custom editor.
	   [HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
	   [HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
	   [HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
	   [HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
	   [HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
	   [HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
	   [HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

		SubShader{
		   Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		   LOD 100

		   Stencil {
			  Ref[_StencilRef]
			  Comp[_StencilComp]
			  Pass Keep
		   }
			  Pass {
			  Name "overlay"

			  Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

			  ZWrite Off
			  Ztest Always
			  Cull Off
			  Blend SrcAlpha OneMinusSrcAlpha

			  CGPROGRAM
			  #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT         
			  #pragma vertex vert
			  #pragma fragment frag
			  #pragma target 2.0
			  #include "UnityCG.cginc"


			  struct appdata
			  {
			  float4 vertex : POSITION;
			  float2 uv : TEXCOORD0;
			  };

			  struct v2f
			  {
			  float2 uv : TEXCOORD0;
			  float4 vertex : SV_POSITION;
			  };

			  float4 _Color;
			  sampler2D _MainTex;
			  float4 _MainTex_ST;
			  fixed _Cutoff;
			  fixed _OverlayPwr;

			  v2f vert(appdata v)
			  {
				 v2f o;
				 o.vertex = UnityObjectToClipPos(v.vertex);
				 o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				 return o;
			  }

			  fixed4 frag(v2f i) : SV_Target
			  {
				 fixed4 texColor = tex2D(_MainTex, i.uv);
				 clip(texColor.a - _Cutoff);
				 return fixed4(_Color.rgb, (texColor.a * _OverlayPwr));
			  }
				  //fixed4 frag(v2f i) : SV_Target
				  //{
				  //return _Color;
				  //}

				  //#include "CGIncludes/Spine-Skeleton-Lit-Common.cginc"
				  ENDCG
					 }

			   Pass {
				  Name "Normal"

				  Tags { "LightMode" = "Vertex" "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }

				  ZWrite Off
				  Cull Off
				  Blend SrcAlpha OneMinusSrcAlpha

				  CGPROGRAM
				  #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
				  #pragma shader_feature _ _DOUBLE_SIDED_LIGHTING
				  #pragma vertex vert
				  #pragma fragment frag
				  #pragma target 2.0

				  #pragma multi_compile __ POINT SPOT
				  #include "CGIncludes/Spine-Skeleton-Lit-Common.cginc"
				  ENDCG

			   }

			   Pass {
				  Name "Caster"
				  Tags { "LightMode" = "ShadowCaster" }
				  Offset 1, 1

				  Fog { Mode Off }
				  ZWrite On
				  ZTest LEqual
				  Cull Off
				  Lighting Off

				  CGPROGRAM
				  #pragma vertex vertShadow
				  #pragma fragment fragShadow
				  #pragma multi_compile_shadowcaster
				  #pragma fragmentoption ARB_precision_hint_fastest

				  #define SHADOW_CUTOFF _Cutoff
				  #include "CGIncludes/Spine-Skeleton-Lit-Common-Shadow.cginc"

				  ENDCG
			   }

	   }
		   CustomEditor "SpineShaderWithOutlineGUI"
}