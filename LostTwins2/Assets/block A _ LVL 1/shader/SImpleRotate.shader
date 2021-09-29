// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/Lambert_SimpleRotate"
{
	Properties
	{
		_Flagalbedo("Flag albedo", 2D) = "white" {}
		_rotaxis("rot axis", Vector) = (0,1,0,0)
		_Float0("Float 0", Float) = 4
		_emissive_boost("emissive_boost", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float3 _rotaxis;
		uniform float _Float0;
		uniform sampler2D _Flagalbedo;
		uniform float4 _Flagalbedo_ST;
		uniform float _emissive_boost;


		float3 RotateAroundAxis( float3 center, float3 original, float3 u, float angle )
		{
			original -= center;
			float C = cos( angle );
			float S = sin( angle );
			float t = 1 - C;
			float m00 = t * u.x * u.x + C;
			float m01 = t * u.x * u.y - S * u.z;
			float m02 = t * u.x * u.z + S * u.y;
			float m10 = t * u.x * u.y + S * u.z;
			float m11 = t * u.y * u.y + C;
			float m12 = t * u.y * u.z - S * u.x;
			float m20 = t * u.x * u.z - S * u.y;
			float m21 = t * u.y * u.z + S * u.x;
			float m22 = t * u.z * u.z + C;
			float3x3 finalMatrix = float3x3( m00, m01, m02, m10, m11, m12, m20, m21, m22 );
			return mul( finalMatrix, original ) + center;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime326 = _Time.y * 45.0;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue319 = RotateAroundAxis( float3( 0,0,0 ), ase_vertex3Pos, _rotaxis, ( v.color * ( radians( mulTime326 ) * _Float0 ) ).r );
			v.vertex.xyz = rotatedValue319;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Flagalbedo = i.uv_texcoord * _Flagalbedo_ST.xy + _Flagalbedo_ST.zw;
			float4 tex2DNode305 = tex2D( _Flagalbedo, uv_Flagalbedo );
			o.Albedo = tex2DNode305.rgb;
			o.Emission = ( tex2DNode305 * _emissive_boost ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
0;62;1906;957;-569.816;1197.951;1;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;326;71.89158,155.2905;Inherit;False;1;0;FLOAT;45;False;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;331;390.118,193.9469;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;335;281.9272,328.7794;Inherit;False;Property;_Float0;Float 0;2;0;Create;True;0;0;False;0;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;334;526.6512,254.9517;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;340;312.345,25.42731;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;320;492.8914,-124.7095;Inherit;False;Property;_rotaxis;rot axis;1;0;Create;True;0;0;False;0;0,1,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;324;640.8914,346.2905;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;765.9955,91.75603;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;305;1049.387,-985.8431;Inherit;True;Property;_Flagalbedo;Flag albedo;0;0;Create;True;0;0;False;0;-1;None;d1bf80dad2da79e468677fbce9c1913c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;342;1180.816,-642.9506;Inherit;False;Property;_emissive_boost;emissive_boost;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;338;46.26276,-117.6868;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;-1;None;08db1449976aca642a84faa0930a3c9f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotateAboutAxisNode;319;1000.946,50.95536;Inherit;False;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;30;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TimeNode;333;65.91428,257.6861;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;341;1486.816,-734.9506;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1742.518,-706.5498;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;mkULTRA/Lambert_SimpleRotate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;331;0;326;0
WireConnection;334;0;331;0
WireConnection;334;1;335;0
WireConnection;339;0;340;0
WireConnection;339;1;334;0
WireConnection;319;0;320;0
WireConnection;319;1;339;0
WireConnection;319;3;324;0
WireConnection;341;0;305;0
WireConnection;341;1;342;0
WireConnection;0;0;305;0
WireConnection;0;2;341;0
WireConnection;0;11;319;0
ASEEND*/
//CHKSM=425700BAC7DC2349B5E7144E28D1A2DDD0AF64FA