// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/master_Rocks"
{
	Properties
	{
		_ALB("ALB", 2D) = "white" {}
		_emissiveBoost("emissive Boost", Float) = 0
		_ALB_Tint("ALB_Tint", Color) = (1,1,1,0)
		_scale("scale", Float) = 0
		_offset("offset", Float) = 0
		_moss("moss", 2D) = "white" {}
		_AO_Scale("AO_Scale", Float) = 0
		_AO_offset("AO_offset", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _ALB;
		uniform float4 _ALB_ST;
		uniform float4 _ALB_Tint;
		uniform sampler2D _moss;
		uniform float4 _moss_ST;
		uniform float _scale;
		uniform float _offset;
		uniform float _AO_Scale;
		uniform float _AO_offset;
		uniform float _emissiveBoost;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_ALB = i.uv_texcoord * _ALB_ST.xy + _ALB_ST.zw;
			float4 tex2DNode1 = tex2D( _ALB, uv_ALB );
			float4 alb76 = ( tex2DNode1 * _ALB_Tint );
			float2 uv_moss = i.uv_texcoord * _moss_ST.xy + _moss_ST.zw;
			float3 ase_worldNormal = i.worldNormal;
			float smoothstepResult78 = smoothstep( 0.0 , (alb76*_scale + _offset).r , ase_worldNormal.y);
			float clampResult77 = clamp( smoothstepResult78 , 0.0 , 1.0 );
			float clampResult97 = clamp( (tex2DNode1.a*_AO_Scale + _AO_offset) , 0.0 , 1.0 );
			float4 lerpResult72 = lerp( alb76 , tex2D( _moss, uv_moss ) , ( clampResult77 * clampResult97 ));
			o.Albedo = ( lerpResult72 * i.vertexColor ).rgb;
			o.Emission = ( tex2DNode1 * _emissiveBoost ).rgb;
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
				half4 color : COLOR0;
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
				o.color = v.color;
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
				surfIN.worldNormal = IN.worldNormal;
				surfIN.vertexColor = IN.color;
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
0;0;1920;1019;800.2764;1703.913;2.535775;True;True
Node;AmplifyShaderEditor.SamplerNode;1;2.152018,-206.585;Inherit;True;Property;_ALB;ALB;0;0;Create;True;0;0;False;0;-1;None;313d1fa18bbfd7b4db281af4e6551122;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;149.4594,173.7669;Inherit;False;Property;_ALB_Tint;ALB_Tint;2;0;Create;True;0;0;False;0;1,1,1,0;0.9433962,0.8289351,0.6630474,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;445.8766,179.2598;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;76;755.5735,179.5371;Inherit;False;alb;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;87;890.3443,-523.3472;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;101;1192.908,-551.1533;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;79;25.10728,-820.254;Inherit;True;76;alb;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;81;380.5262,-733.8722;Inherit;False;Property;_scale;scale;3;0;Create;True;0;0;False;0;0;1.37;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;392.2208,-590.9274;Inherit;False;Property;_offset;offset;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;85;654.3406,-978.9085;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;96;1324.37,-432.8381;Inherit;False;Property;_AO_offset;AO_offset;7;0;Create;True;0;0;False;0;0;16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;100;1478.294,-561.989;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;1278.651,-513.3293;Inherit;False;Property;_AO_Scale;AO_Scale;6;0;Create;True;0;0;False;0;0;16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;80;648.9002,-822.9541;Inherit;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;1.68;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;78;920.8501,-866.058;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;94;1570.464,-512.092;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;77;1326.16,-866.7294;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;97;1863.943,-516.5372;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;1935.172,-1231.709;Inherit;True;76;alb;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;2093.792,-870.5388;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;1777.224,-1020.68;Inherit;True;Property;_moss;moss;5;0;Create;True;0;0;False;0;-1;None;bfd94922525aa0a4c97ded8c6f0b0ffa;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;467.1149,29.67616;Inherit;False;Property;_emissiveBoost;emissive Boost;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;72;2389.758,-1035.064;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;103;2561.633,-510.6541;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;2902.096,-531.7304;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;782.8593,-199.9679;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3396.496,-281.1795;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;mkULTRA/master_Rocks;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.02;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;18;0;1;0
WireConnection;18;1;17;0
WireConnection;76;0;18;0
WireConnection;87;0;1;0
WireConnection;101;0;87;3
WireConnection;100;0;101;0
WireConnection;80;0;79;0
WireConnection;80;1;81;0
WireConnection;80;2;82;0
WireConnection;78;0;85;2
WireConnection;78;2;80;0
WireConnection;94;0;100;0
WireConnection;94;1;95;0
WireConnection;94;2;96;0
WireConnection;77;0;78;0
WireConnection;97;0;94;0
WireConnection;89;0;77;0
WireConnection;89;1;97;0
WireConnection;72;0;86;0
WireConnection;72;1;83;0
WireConnection;72;2;89;0
WireConnection;102;0;72;0
WireConnection;102;1;103;0
WireConnection;16;0;1;0
WireConnection;16;1;15;0
WireConnection;0;0;102;0
WireConnection;0;2;16;0
ASEEND*/
//CHKSM=1F80A3E239F3EFDDBB95747C24442BC54E51E464