// Upgrade NOTE: upgraded instancing buffer 'mkULTRAmaster_opacitymask' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/master_opacitymask"
{
	Properties
	{
		_ALB("ALB", 2D) = "white" {}
		_Matcap("Matcap", 2D) = "white" {}
		_matcap_ALPHA("matcap_ALPHA", 2D) = "white" {}
		_matcapintensity("matcap intensity", Float) = 0
		_emissiveBoost("emissive Boost", Float) = 0
		_ALB_Tint("ALB_Tint", Color) = (1,1,1,0)
		_transparent("transparent", 2D) = "white" {}
		_Opacity("Opacity", Range( 0 , 1)) = 0
		_opacity_scroll_speed("opacity_scroll_speed", Float) = 0
		[HideInInspector] _texcoord4( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float2 uv4_texcoord4;
		};

		uniform float4 _ALB_Tint;
		uniform sampler2D _ALB;
		uniform float _emissiveBoost;
		uniform sampler2D _matcap_ALPHA;
		uniform sampler2D _Matcap;
		uniform float _matcapintensity;
		uniform sampler2D _transparent;
		uniform float _opacity_scroll_speed;

		UNITY_INSTANCING_BUFFER_START(mkULTRAmaster_opacitymask)
			UNITY_DEFINE_INSTANCED_PROP(float4, _ALB_ST)
#define _ALB_ST_arr mkULTRAmaster_opacitymask
			UNITY_DEFINE_INSTANCED_PROP(float4, _matcap_ALPHA_ST)
#define _matcap_ALPHA_ST_arr mkULTRAmaster_opacitymask
			UNITY_DEFINE_INSTANCED_PROP(float, _Opacity)
#define _Opacity_arr mkULTRAmaster_opacitymask
		UNITY_INSTANCING_BUFFER_END(mkULTRAmaster_opacitymask)

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 _ALB_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_ALB_ST_arr, _ALB_ST);
			float2 uv_ALB = i.uv_texcoord * _ALB_ST_Instance.xy + _ALB_ST_Instance.zw;
			float4 tex2DNode1 = tex2D( _ALB, uv_ALB );
			o.Albedo = ( _ALB_Tint * tex2DNode1 ).rgb;
			float4 _matcap_ALPHA_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_matcap_ALPHA_ST_arr, _matcap_ALPHA_ST);
			float2 uv_matcap_ALPHA = i.uv_texcoord * _matcap_ALPHA_ST_Instance.xy + _matcap_ALPHA_ST_Instance.zw;
			float3 ase_worldNormal = i.worldNormal;
			o.Emission = ( ( tex2DNode1 * _emissiveBoost ) + ( ( tex2D( _matcap_ALPHA, uv_matcap_ALPHA ) * tex2D( _Matcap, ( ( mul( UNITY_MATRIX_V, float4( ase_worldNormal , 0.0 ) ).xyz * 0.5 ) + 0.5 ).xy ) * tex2DNode1 ) * _matcapintensity ) ).rgb;
			float mulTime37 = _Time.y * _opacity_scroll_speed;
			float2 appendResult39 = (float2(mulTime37 , 0.0));
			float2 uv4_TexCoord29 = i.uv4_texcoord4 + appendResult39;
			float _Opacity_Instance = UNITY_ACCESS_INSTANCED_PROP(_Opacity_arr, _Opacity);
			float4 clampResult33 = clamp( ( tex2D( _transparent, uv4_TexCoord29 ) + _Opacity_Instance ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Alpha = clampResult33.r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert alpha:premul keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack1.zw = customInputData.uv4_texcoord4;
				o.customPack1.zw = v.texcoord3;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv4_texcoord4 = IN.customPack1.zw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
20;384;1920;1019;-529.0253;298.2532;1;True;True
Node;AmplifyShaderEditor.WorldNormalVector;3;-1270.621,94.1701;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewMatrixNode;2;-1190.433,0.03626633;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1042.707,158.197;Float;False;Constant;_Float1;Float 1;-1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1031.89,29.8222;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;40;397.2997,-58.56808;Inherit;False;Property;_opacity_scroll_speed;opacity_scroll_speed;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-864.6376,66.268;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;35;604.4289,305.9152;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;37;646.0943,-57.66401;Inherit;False;1;0;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-689.3282,116.3449;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;877.3326,-65.52083;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-799.527,-398.393;Inherit;True;Property;_ALB;ALB;0;0;Create;True;0;0;False;0;-1;None;833e050769395e545a9da57f0c446aab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-976.1353,-192.572;Inherit;True;Property;_matcap_ALPHA;matcap_ALPHA;2;0;Create;True;0;0;False;0;-1;None;c3df7f8bf29e4ce40a5f662c34935dc6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;-710.8833,251.1612;Inherit;True;Property;_Matcap;Matcap;1;0;Create;True;0;0;False;0;-1;None;fa283fd12f4d6b64d8fb9f8c5e104b56;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;1068.651,-117.3421;Inherit;False;3;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;1387.739,-120.1376;Inherit;True;Property;_transparent;transparent;6;0;Create;True;0;0;False;0;-1;None;290387f271bc67d4aa558e01b02ab271;True;3;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;1208.004,224.8316;Inherit;False;InstancedProperty;_Opacity;Opacity;7;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;310.6967,-286.1362;Inherit;False;Property;_emissiveBoost;emissive Boost;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-167.0336,259.1819;Inherit;False;Property;_matcapintensity;matcap intensity;3;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-228.4623,-27.786;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;17;-559.2619,-608.5116;Inherit;False;Property;_ALB_Tint;ALB_Tint;5;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;32;1758.716,-136.7086;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;822.2987,-419.8867;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;118.5952,-128.4292;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;33;2005.979,-129.1085;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;1237.735,-347.2794;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-168.262,-567.8116;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2354.778,-436.0095;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;mkULTRA/master_opacitymask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Premultiply;0.5;True;True;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;3;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.02;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;37;0;40;0
WireConnection;7;0;6;0
WireConnection;7;1;4;0
WireConnection;39;0;37;0
WireConnection;39;1;35;0
WireConnection;8;1;7;0
WireConnection;29;1;39;0
WireConnection;19;1;29;0
WireConnection;9;0;12;0
WireConnection;9;1;8;0
WireConnection;9;2;1;0
WireConnection;32;0;19;0
WireConnection;32;1;31;0
WireConnection;16;0;1;0
WireConnection;16;1;15;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;33;0;32;0
WireConnection;14;0;16;0
WireConnection;14;1;10;0
WireConnection;18;0;17;0
WireConnection;18;1;1;0
WireConnection;0;0;18;0
WireConnection;0;2;14;0
WireConnection;0;9;33;0
ASEEND*/
//CHKSM=A7592B18C60E3DC40637F6C40B952C9E915CCF93