// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "test"
{
	Properties
	{
		_EdgeLength ( "Edge length", Range( 2, 50 ) ) = 2
		_Vector1("Vector 1", Vector) = (0,0,0,0)
		_Vector2("Vector 1", Vector) = (0,0,0,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 5.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			half filler;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float3 _Vector1;
		uniform float3 _Vector2;
		uniform float _EdgeLength;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float mulTime50 = _Time.y * 12.0;
			float temp_output_49_0 = sin( mulTime50 );
			float2 uv_TextureSample0 = v.texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode42 = tex2Dlod( _TextureSample0, float4( uv_TextureSample0, 0, 0.0) );
			float clampResult67 = clamp( ( ( ( temp_output_49_0 + 1.0 ) * tex2DNode42.r ) + ( temp_output_49_0 * tex2DNode42.b ) ) , 0.0 , 1.0 );
			float t195 = clampResult67;
			float mulTime43 = _Time.y * 360.0;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 rotatedValue35 = RotateAroundAxis( float3( 0,0,0 ), ase_vertex3Pos, float3( 1,0,0 ), radians( mulTime43 ) );
			v.vertex.xyz += ( ( ( ( t195 * ( 1.0 - tex2DNode42.a ) ) * _Vector1 ) + ( tex2DNode42.a * t195 * _Vector2 ) ) + ( tex2DNode42.g * ( rotatedValue35 - ase_vertex3Pos ) ) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 temp_cast_0 = (0.5).xxx;
			o.Albedo = temp_cast_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
0;162;1920;857;1714.218;1138.059;3.746363;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;50;-1499.529,-471.3929;Inherit;False;1;0;FLOAT;12;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;49;-1131.527,-503.3929;Inherit;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-763.5258,-519.3929;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;42;-1336.617,-46.72055;Inherit;True;Property;_TextureSample0;Texture Sample 0;7;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-512.7258,-209.893;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-411.5258,-471.3929;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;65;-153.0258,-344.2929;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;67;78.71902,-439.3101;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;43;-1131.228,592.7809;Inherit;False;1;0;FLOAT;360;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;373.5601,-454.0345;Float;True;t1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;36;-718.4818,974.5368;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RadiansOpNode;38;-832.6725,594.1893;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;100;223.1411,-110.3241;Inherit;False;95;t1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;98;223.3578,-23.3763;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;102;-87.96312,291.7723;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;58;56.83738,66.5536;Inherit;False;Property;_Vector1;Vector 1;5;0;Create;True;0;0;False;0;0,0,0;0,-4.02,4.02;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;560.4647,-133.0742;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;103;185.0392,450.042;Inherit;False;Property;_Vector2;Vector 1;6;0;Create;True;0;0;False;0;0,0,0;0,4.02,4.02;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RotateAboutAxisNode;35;-516.4509,567.0269;Inherit;False;False;4;0;FLOAT3;1,0,0;False;1;FLOAT;30;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;214.5345,356.2688;Inherit;False;95;t1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;754.6238,47.14873;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;475.3814,284.4164;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-316.529,940.4366;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;224.0727,667.2368;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;1079.96,320.0107;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;83;1303.843,-95.41805;Inherit;False;Constant;_Float0;Float 0;3;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;62;1436.131,359.3043;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1711.3,-89.34685;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;test;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;2;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;49;0;50;0
WireConnection;63;0;49;0
WireConnection;66;0;49;0
WireConnection;66;1;42;3
WireConnection;64;0;63;0
WireConnection;64;1;42;1
WireConnection;65;0;64;0
WireConnection;65;1;66;0
WireConnection;67;0;65;0
WireConnection;95;0;67;0
WireConnection;38;0;43;0
WireConnection;98;0;42;4
WireConnection;102;0;42;4
WireConnection;97;0;100;0
WireConnection;97;1;98;0
WireConnection;35;1;38;0
WireConnection;35;3;36;0
WireConnection;44;0;97;0
WireConnection;44;1;58;0
WireConnection;101;0;102;0
WireConnection;101;1;99;0
WireConnection;101;2;103;0
WireConnection;39;0;35;0
WireConnection;39;1;36;0
WireConnection;40;0;42;2
WireConnection;40;1;39;0
WireConnection;104;0;44;0
WireConnection;104;1;101;0
WireConnection;62;0;104;0
WireConnection;62;1;40;0
WireConnection;0;0;83;0
WireConnection;0;11;62;0
ASEEND*/
//CHKSM=F97B2480DD184410095FD51C22333028675E1F2A