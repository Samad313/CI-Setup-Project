// -Vertex Lit + ShadowCaster
// - Premultiplied Alpha Blending (Optional straight alpha input)
// - Double-sided, no depth

Shader "Spine/Spine-Skeleton-Lit AlwaysVisible_C" {
	Properties{
	[NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
	_FillColor("FillColor", Color) = (1,1,1,1)
	_FillPhase("FillPhase", Range(0, 1)) = 0
	_Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.1
	_Color("always visible color", Color) = (0,0,0,0)
	_OverlayPwr("OverlayIntensity", Range(0,1)) = 1
	[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
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
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
			Blend One OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			Lighting Off

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
			Blend One OneMinusSrcAlpha

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _DOUBLE_SIDED_LIGHTING
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
		    //Spine cginclude added 
			#if defined(SHADER_API_GLES)
			#define LIGHT_LOOP_LIMIT 8
			#elif defined(SHADER_API_N3DS)
			#define LIGHT_LOOP_LIMIT 4
			#else
			#define LIGHT_LOOP_LIMIT unity_VertexLightParams.x
			#endif 
			#include "UnityCG.cginc"
			float4 _FillColor;
			//float4 _FillPhase;

			#pragma multi_compile __ POINT SPOT
			
			//#include "CGIncludes/Spine-Skeleton-Lit-Common.cginc"
	
			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 vertexColor : COLOR;
			};

#if defined(_ALPHA_CLIP)
			uniform fixed _Cutoff;
#define ALPHA_CLIP(pixel, color) clip((pixel.a * color.a) - _Cutoff);
#else
#define ALPHA_CLIP(pixel, color)
#endif

			half3 computeLighting(int idx, half3 dirToLight, half3 eyeNormal, half4 diffuseColor, half atten) {
				half NdotL = max(dot(eyeNormal, dirToLight), 0.0);
				// diffuse
				half3 color = NdotL * diffuseColor.rgb * unity_LightColor[idx].rgb;
				return color * atten;
			}

			half3 computeOneLight(int idx, float3 eyePosition, half3 eyeNormal, half4 diffuseColor) {
				float3 dirToLight = unity_LightPosition[idx].xyz;
				half att = 1.0;

#if defined(POINT) || defined(SPOT)
				dirToLight -= eyePosition * unity_LightPosition[idx].w;

				// distance attenuation
				float distSqr = dot(dirToLight, dirToLight);
				att /= (1.0 + unity_LightAtten[idx].z * distSqr);
				if (unity_LightPosition[idx].w != 0 && distSqr > unity_LightAtten[idx].w) att = 0.0; // set to 0 if outside of range
				distSqr = max(distSqr, 0.000001); // don't produce NaNs if some vertex position overlaps with the light
				dirToLight *= rsqrt(distSqr);
#if defined(SPOT)

				// spot angle attenuation
				half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
				half spotAtt = (rho - unity_LightAtten[idx].x) * unity_LightAtten[idx].y;
				att *= saturate(spotAtt);
#endif
#endif

				att *= 0.5; // passed in light colors are 2x brighter than what used to be in FFP
				return min(computeLighting(idx, dirToLight, eyeNormal, diffuseColor, att), 1.0);
			}

			int4 unity_VertexLightParams; // x: light count, y: zero, z: one (y/z needed by d3d9 vs loop instruction)

			struct appdata {
				float3 pos : POSITION;
				float3 normal : NORMAL;
				half4 color : COLOR;
				float2 uv0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput {
				fixed4 color : COLOR0;
				float2 uv0 : TEXCOORD0;
				float4 pos : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			VertexOutput vert(appdata v) {
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				half4 color = v.color;
				float3 eyePos = UnityObjectToViewPos(float4(v.pos, 1)).xyz; //mul(UNITY_MATRIX_MV, float4(v.pos,1)).xyz;
				half3 fixedNormal = half3(0, 0, -1);
				half3 eyeNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, fixedNormal));

#ifdef _DOUBLE_SIDED_LIGHTING
				// unfortunately we have to compute the sign here in the vertex shader
				// instead of using VFACE in fragment shader stage.
				half faceSign = sign(eyeNormal.z);
				eyeNormal *= faceSign;
#endif

				// Lights
				half3 lcolor = half4(0, 0, 0, 1).rgb + color.rgb * glstate_lightmodel_ambient.rgb;
				for (int il = 0; il < LIGHT_LOOP_LIMIT; ++il) {
					lcolor += computeOneLight(il, eyePos, eyeNormal, color);
				}

				color.rgb = lcolor.rgb;
				o.color = saturate(color);
				o.uv0 = v.uv0;
				o.pos = UnityObjectToClipPos(v.pos);
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(VertexOutput i) : SV_Target{
				fixed4 tex = tex2D(_MainTex, i.uv0);
				ALPHA_CLIP(tex, i.color);

				fixed4 col;
				#if defined(_STRAIGHT_ALPHA_INPUT)
				col.rgb = tex * i.color * tex.a;
				#else
				col.rgb = tex * i.color;
				#endif

				col *= 2;
				col.a = tex.a * i.color.a;
				//float3 finalColor = lerp((rawColor.rgb * i.vertexColor.rgb), (_FillColor.rgb * finalAlpha), _FillPhase);
				//col.rgb += _FillColor*col.a;	
				
				//using the alpha from the fill color to run a lerp//
				col.rgb = lerp((col.rgb), (_FillColor.rgb*col.a), _FillColor.a);
				
				return col;
			}

		

			ENDCG

		}
		Pass {
				Name "Caster"
				Tags { "LightMode" = "ShadowCaster" }
				Offset 1, 1
				ZWrite On
				ZTest LEqual

				Fog { Mode Off }
				Cull Off
				Lighting Off

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				sampler2D _MainTex;
				fixed _Cutoff;

				struct VertexOutput {
					V2F_SHADOW_CASTER;
					float4 uvAndAlpha : TEXCOORD1;
				};

				VertexOutput vert(appdata_base v, float4 vertexColor : COLOR) {
					VertexOutput o;
					o.uvAndAlpha = v.texcoord;
					o.uvAndAlpha.a = vertexColor.a;
					TRANSFER_SHADOW_CASTER(o)
					return o;
				}

				float4 frag(VertexOutput i) : SV_Target {
					fixed4 texcol = tex2D(_MainTex, i.uvAndAlpha.xy);
					clip(texcol.a * i.uvAndAlpha.a - _Cutoff);
					SHADOW_CASTER_FRAGMENT(i)
				}
				ENDCG
			}
	}
		FallBack "Diffuse"
						CustomEditor "SpineShaderWithOutlineGUI"
}
