// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ShockWaveAmplifyShader"
{
	Properties
	{
		_FocalPoint("FocalPoint", Vector) = (0,0,0,0)
		_Size("Size", Float) = 0.12
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Magnification("Magnification", Float) = 0
		_Speed("Speed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float _Speed;
		uniform float _Size;
		uniform float2 _FocalPoint;
		uniform float _Magnification;

		void surf( Input i , inout SurfaceOutput o )
		{
			float temp_output_15_0 = frac( ( _Time.y * _Speed ) );
			float2 temp_output_10_0 = ( i.uv_texcoord - _FocalPoint );
			float smoothstepResult18 = smoothstep( ( temp_output_15_0 - _Size ) , ( temp_output_15_0 + _Size ) , length( temp_output_10_0 ));
			float2 normalizeResult27 = normalize( temp_output_10_0 );
			o.Albedo = tex2D( _TextureSample0, ( i.uv_texcoord + ( ( ( 1.0 - smoothstepResult18 ) * smoothstepResult18 ) * ( normalizeResult27 * _Magnification ) ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
-1920;25;1912;1034;2335.224;207.6572;1.158279;True;True
Node;AmplifyShaderEditor.RangedFloatNode;33;-1824.32,32.21918;Inherit;False;Property;_Speed;Speed;4;0;Create;True;0;0;False;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;39;-1797.285,-262.6618;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;5;-1585.381,462.8292;Inherit;False;Property;_FocalPoint;FocalPoint;0;0;Create;True;0;0;False;0;0,0;0,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexCoordVertexDataNode;22;-1609.335,256.5808;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1588.648,-46.44641;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;15;-1404.371,-35.93759;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1407.465,-211.2979;Inherit;False;Property;_Size;Size;1;0;Create;True;0;0;False;0;0.12;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-1308.381,396.8292;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-1220.306,133.4533;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;11;-1083.942,349.6265;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1212.843,-44.15705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;18;-958.9373,-16.7567;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;27;-1402.818,694.0209;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;23;-729.3627,-105.634;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1207.65,920.1976;Inherit;False;Property;_Magnification;Magnification;3;0;Create;True;0;0;False;0;0;-0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-606.1559,34.21746;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-866.9739,658.451;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;32;-494.0564,-251.7967;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-534.8958,513.4333;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-242.3698,15.97762;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;4;226.541,-484.0659;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;-1;None;5444122e7cf822941a43bf0baf5b3271;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;36;-1970.644,-241.5401;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;462.7496,-143.3399;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;ShockWaveAmplifyShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;39;2
WireConnection;14;1;33;0
WireConnection;15;0;14;0
WireConnection;10;0;22;0
WireConnection;10;1;5;0
WireConnection;17;0;15;0
WireConnection;17;1;7;0
WireConnection;11;0;10;0
WireConnection;16;0;15;0
WireConnection;16;1;7;0
WireConnection;18;0;11;0
WireConnection;18;1;17;0
WireConnection;18;2;16;0
WireConnection;27;0;10;0
WireConnection;23;0;18;0
WireConnection;24;0;23;0
WireConnection;24;1;18;0
WireConnection;28;0;27;0
WireConnection;28;1;8;0
WireConnection;29;0;24;0
WireConnection;29;1;28;0
WireConnection;25;0;32;0
WireConnection;25;1;29;0
WireConnection;4;1;25;0
WireConnection;0;0;4;0
ASEEND*/
//CHKSM=97311323FFCA244192FB65CFBFAB62BBE50B468F