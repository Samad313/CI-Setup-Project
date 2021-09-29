// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ShockWaveAmplifyShader"
{
	Properties
	{
		_FocalPoint("FocalPoint", Vector) = (0,0,0,0)
		_Size("Size", Float) = 0.12
		_MainTex("MainTex", 2D) = "white" {}
		_Speed("Speed", Float) = 0
		_Normal("Normal", 2D) = "white" {}
		_SizeRatio("SizeRatio", Float) = 0
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
			float4 screenPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _MainTex;
		uniform float _Speed;
		uniform float _Size;
		uniform float _SizeRatio;
		uniform float2 _FocalPoint;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = tex2D( _Normal, uv_Normal ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 break61 = ase_screenPosNorm;
			float temp_output_15_0 = frac( ( _Time.y * _Speed ) );
			float4 break45 = ase_screenPosNorm;
			float4 appendResult47 = (float4(( _SizeRatio * break45.x ) , break45.z , 0.0 , 0.0));
			float4 break54 = ( appendResult47 - float4( _FocalPoint, 0.0 , 0.0 ) );
			float4 appendResult55 = (float4(break54.r , break54.g , 0.0 , 0.0));
			float smoothstepResult18 = smoothstep( ( temp_output_15_0 - _Size ) , ( temp_output_15_0 + _Size ) , length( appendResult55 ));
			float temp_output_24_0 = ( ( 1.0 - smoothstepResult18 ) * smoothstepResult18 );
			float4 appendResult62 = (float4(break61.x , ( break61.y + temp_output_24_0 ) , break61.z , 0.0));
			o.Albedo = tex2D( _MainTex, appendResult62.rg ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
-1913;25;1912;1034;3027.78;22.85632;1.09617;True;True
Node;AmplifyShaderEditor.ScreenPosInputsNode;59;-2740.342,503.8053;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-2340.044,618.1754;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;56;-2533.371,165.7452;Inherit;False;Property;_SizeRatio;SizeRatio;6;0;Create;True;0;0;False;0;0;1.777;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-2198.342,272.8053;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;5;-1816.75,574.5245;Inherit;False;Property;_FocalPoint;FocalPoint;0;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;47;-1966.896,214.1441;Inherit;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;-1614.746,371.2989;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TimeNode;13;-1796.589,-248.1714;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;33;-1824.32,32.21918;Inherit;False;Property;_Speed;Speed;4;0;Create;True;0;0;False;0;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;54;-1407.899,259.6587;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-1588.648,-46.44641;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;15;-1404.371,-35.93759;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-1033.861,264.0215;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1407.465,-211.2979;Inherit;False;Property;_Size;Size;1;0;Create;True;0;0;False;0;0.12;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-1220.306,133.4533;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-1212.843,-44.15705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;11;-627.9663,362.9955;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;18;-979.729,-18.24183;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;23;-729.3627,-105.634;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;60;-139.874,-475.5555;Float;True;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;61;150.0169,-436.1124;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-606.1559,34.21746;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;294.7114,-153.3006;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;62;506.2722,-441.5933;Inherit;False;COLOR;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1207.65,920.1976;Inherit;False;Property;_Magnification;Magnification;3;0;Create;True;0;0;False;0;0;-0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-260.1477,639.6689;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;34;638.4894,162.4687;Inherit;True;Property;_Normal;Normal;5;0;Create;True;0;0;False;0;-1;None;e9742c575b8f4644fb9379e7347ff62e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;710.3159,-413.4127;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;False;0;-1;None;5444122e7cf822941a43bf0baf5b3271;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-866.9739,658.451;Inherit;True;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1099.077,59.55383;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;ShockWaveAmplifyShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;59;0
WireConnection;57;0;56;0
WireConnection;57;1;45;0
WireConnection;47;0;57;0
WireConnection;47;1;45;2
WireConnection;10;0;47;0
WireConnection;10;1;5;0
WireConnection;54;0;10;0
WireConnection;14;0;13;2
WireConnection;14;1;33;0
WireConnection;15;0;14;0
WireConnection;55;0;54;0
WireConnection;55;1;54;1
WireConnection;17;0;15;0
WireConnection;17;1;7;0
WireConnection;16;0;15;0
WireConnection;16;1;7;0
WireConnection;11;0;55;0
WireConnection;18;0;11;0
WireConnection;18;1;17;0
WireConnection;18;2;16;0
WireConnection;23;0;18;0
WireConnection;61;0;60;0
WireConnection;24;0;23;0
WireConnection;24;1;18;0
WireConnection;63;0;61;1
WireConnection;63;1;24;0
WireConnection;62;0;61;0
WireConnection;62;1;63;0
WireConnection;62;2;61;2
WireConnection;29;0;24;0
WireConnection;29;1;28;0
WireConnection;4;1;62;0
WireConnection;28;1;8;0
WireConnection;0;0;4;0
WireConnection;0;1;34;0
ASEEND*/
//CHKSM=8EC8B72D5FA1E78FB77995FDECE49858BB6BA640