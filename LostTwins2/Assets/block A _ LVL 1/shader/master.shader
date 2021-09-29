// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/master"
{
	Properties
	{
		[Toggle(_ENABLE_WORLDSPACE_TILING_ON)] _Enable_Worldspace_tiling("Enable_Worldspace_tiling", Float) = 0
		_ALB("ALB", 2D) = "white" {}
		_TilingScale("Tiling Scale", Vector) = (1,1,0,0)
		_Matcap("Matcap", 2D) = "white" {}
		_TilingOffset("Tiling Offset", Vector) = (0,0,0,0)
		_matcap_ALPHA("matcap_ALPHA", 2D) = "white" {}
		_matcapintensity("matcap intensity", Float) = 0
		_emissiveBoost("emissive Boost", Float) = 0
		_ALB_Tint("ALB_Tint", Color) = (1,1,1,0)
		_Depth_Tint("Depth_Tint", Color) = (1,1,1,0)
		_dark("dark", Range( 0 , 20)) = 20
		_Dark_min("Dark_min", Float) = -0.3
		_Dark_max("Dark_max", Float) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_local __ _ENABLE_WORLDSPACE_TILING_ON
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
		};

		uniform float4 _Depth_Tint;
		uniform float _Dark_min;
		uniform float _Dark_max;
		uniform float _dark;
		uniform float4 _ALB_Tint;
		uniform sampler2D _ALB;
		uniform float2 _TilingScale;
		uniform float2 _TilingOffset;
		uniform float _emissiveBoost;
		uniform sampler2D _matcap_ALPHA;
		uniform float4 _matcap_ALPHA_ST;
		uniform sampler2D _Matcap;
		uniform float _matcapintensity;

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_worldPos = i.worldPos;
			float clampResult35 = clamp( ( (_Dark_min + (ase_worldPos.z - -1.0) * (_Dark_max - _Dark_min) / (1.0 - -1.0)) + _dark ) , 0.0 , 1.0 );
			float3 ase_parentObjectScale = (1.0/float3( length( unity_WorldToObject[ 0 ].xyz ), length( unity_WorldToObject[ 1 ].xyz ), length( unity_WorldToObject[ 2 ].xyz ) ));
			float3 break42 = ase_parentObjectScale;
			float2 appendResult46 = (float2(break42.z , break42.y));
			float2 ZYScale50 = appendResult46;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 normalizeResult44 = normalize( ase_vertexNormal );
			float3 break48 = normalizeResult44;
			float2 appendResult43 = (float2(break42.x , break42.z));
			float2 XZScale49 = appendResult43;
			float2 appendResult45 = (float2(break42.x , break42.y));
			float2 XYScale47 = appendResult45;
			float2 uv_TexCoord62 = i.uv_texcoord * ( ( ( ZYScale50 * break48.x ) + ( break48.y * XZScale49 ) + ( break48.z * XYScale47 ) ) * _TilingScale ) + _TilingOffset;
			#ifdef _ENABLE_WORLDSPACE_TILING_ON
				float2 staticSwitch63 = uv_TexCoord62;
			#else
				float2 staticSwitch63 = i.uv_texcoord;
			#endif
			float4 tex2DNode1 = tex2D( _ALB, staticSwitch63 );
			float4 temp_output_18_0 = ( _ALB_Tint * tex2DNode1 );
			o.Albedo = ( ( _Depth_Tint * clampResult35 ) + temp_output_18_0 ).rgb;
			float2 uv_matcap_ALPHA = i.uv_texcoord * _matcap_ALPHA_ST.xy + _matcap_ALPHA_ST.zw;
			o.Emission = ( ( tex2DNode1 * _emissiveBoost ) + ( ( tex2D( _matcap_ALPHA, uv_matcap_ALPHA ) * tex2D( _Matcap, ( ( mul( UNITY_MATRIX_V, float4( ase_worldNormal , 0.0 ) ).xyz * 0.5 ) + 0.5 ).xy ) * tex2DNode1 ) * _matcapintensity ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows exclude_path:deferred 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
0;390;1534;629;1189.31;1215.904;1.410767;True;True
Node;AmplifyShaderEditor.CommentaryNode;38;-3334.403,-86.85907;Inherit;False;979.3345;423;;8;50;49;47;46;45;43;42;39;Get Object Scale;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;40;-3340.058,-698.3672;Inherit;False;1265.621;499;;11;59;57;56;55;54;53;52;51;48;44;41;Combine Scale with Normal to generate tiling;1,1,1,1;0;0
Node;AmplifyShaderEditor.ObjectScaleNode;39;-3284.403,7.565722;Inherit;True;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;42;-3065.087,28.1165;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NormalVertexDataNode;41;-3290.058,-600.8457;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;44;-3102.359,-598.6376;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-2742.069,-36.85909;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;45;-2742.069,203.1416;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-2742.069,91.1411;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;48;-2941.459,-592.0457;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-2598.069,91.1411;Inherit;False;XZScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-2598.069,-36.85909;Inherit;False;ZYScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;47;-2598.069,219.1416;Inherit;False;XYScale;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-2611.684,-440.3622;Inherit;False;49;XZScale;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;51;-2654.542,-353.4282;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-2624.438,-280.3672;Inherit;False;47;XYScale;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-2624.438,-648.3672;Inherit;False;50;ZYScale;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-2384.438,-600.3672;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-2384.438,-488.3671;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-2384.438,-376.3672;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;3;-1270.621,94.1701;Inherit;False;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewMatrixNode;2;-1190.433,0.03626633;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;59;-2218.438,-513.3671;Inherit;False;3;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;58;-2003.304,-210.3;Inherit;False;Property;_TilingScale;Tiling Scale;2;0;Create;True;0;0;False;0;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;4;-1042.707,158.197;Float;False;Constant;_Float1;Float 1;-1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1031.89,29.8222;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;60;-1786.304,-257.3;Inherit;False;Property;_TilingOffset;Tiling Offset;4;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1769.304,-415.3;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1812.655,-865.276;Inherit;False;Property;_Dark_min;Dark_min;11;0;Create;True;0;0;False;0;-0.3;-0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;62;-1473.763,-369.058;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-864.6376,66.268;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-1510.277,-610.7414;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-2024.835,-1070.459;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;29;-1822.014,-767.1689;Inherit;False;Property;_Dark_max;Dark_max;12;0;Create;True;0;0;False;0;0.3;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;31;-1548.732,-976.4347;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-689.3282,116.3449;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-1180.732,-784.4347;Inherit;False;Property;_dark;dark;10;0;Create;True;0;0;False;0;20;20;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;63;-1177.941,-549.2173;Inherit;False;Property;_Enable_Worldspace_tiling;Enable_Worldspace_tiling;0;0;Create;True;0;0;False;0;1;0;0;True;;Toggle;2;Key0;Key1;Create;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;12;-976.1353,-192.572;Inherit;True;Property;_matcap_ALPHA;matcap_ALPHA;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-896.1734,-979.4724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;-710.8833,251.1612;Inherit;True;Property;_Matcap;Matcap;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-799.527,-398.393;Inherit;True;Property;_ALB;ALB;1;0;Create;True;0;0;False;0;-1;None;fb1a1abad445d034a8a37c4cef2a3197;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-559.2619,-608.5116;Inherit;False;Property;_ALB_Tint;ALB_Tint;8;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;35;-669.3864,-1018.346;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-288.3197,312;Inherit;False;Property;_matcapintensity;matcap intensity;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-314.2622,-154.8116;Inherit;False;Property;_emissiveBoost;emissive Boost;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-228.4623,-27.786;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;66;-279.3649,-1129.847;Inherit;False;Property;_Depth_Tint;Depth_Tint;9;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-3.965669,115.0363;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-55.26196,-75.81165;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-168.262,-567.8116;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;31.00403,-909.7678;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;195.8031,-801.4572;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;226.2534,-655.0641;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;254.7378,-105.8116;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;503.2352,-222.4557;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;mkULTRA/master;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.02;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;42;0;39;0
WireConnection;44;0;41;0
WireConnection;46;0;42;2
WireConnection;46;1;42;1
WireConnection;45;0;42;0
WireConnection;45;1;42;1
WireConnection;43;0;42;0
WireConnection;43;1;42;2
WireConnection;48;0;44;0
WireConnection;49;0;43;0
WireConnection;50;0;46;0
WireConnection;47;0;45;0
WireConnection;51;0;48;2
WireConnection;56;0;52;0
WireConnection;56;1;48;0
WireConnection;57;0;48;1
WireConnection;57;1;53;0
WireConnection;55;0;51;0
WireConnection;55;1;54;0
WireConnection;59;0;56;0
WireConnection;59;1;57;0
WireConnection;59;2;55;0
WireConnection;5;0;2;0
WireConnection;5;1;3;0
WireConnection;61;0;59;0
WireConnection;61;1;58;0
WireConnection;62;0;61;0
WireConnection;62;1;60;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;31;0;28;3
WireConnection;31;3;30;0
WireConnection;31;4;29;0
WireConnection;7;0;6;0
WireConnection;7;1;4;0
WireConnection;63;1;64;0
WireConnection;63;0;62;0
WireConnection;34;0;31;0
WireConnection;34;1;33;0
WireConnection;8;1;7;0
WireConnection;1;1;63;0
WireConnection;35;0;34;0
WireConnection;9;0;12;0
WireConnection;9;1;8;0
WireConnection;9;2;1;0
WireConnection;10;0;9;0
WireConnection;10;1;11;0
WireConnection;16;0;1;0
WireConnection;16;1;15;0
WireConnection;18;0;17;0
WireConnection;18;1;1;0
WireConnection;67;0;66;0
WireConnection;67;1;35;0
WireConnection;65;0;67;0
WireConnection;65;1;18;0
WireConnection;37;1;18;0
WireConnection;14;0;16;0
WireConnection;14;1;10;0
WireConnection;0;0;65;0
WireConnection;0;2;14;0
ASEEND*/
//CHKSM=7D8E1D91066608803DC135F5BAF2889C2E822FAB