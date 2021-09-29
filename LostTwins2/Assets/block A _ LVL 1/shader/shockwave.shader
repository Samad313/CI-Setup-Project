// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/Shockwave"
{
	Properties
	{
		_Flagalbedo("Flag albedo", 2D) = "white" {}
		_offset("offset", Float) = 0
		_scale("scale", Range( 0 , 5)) = 0
		_speed("speed", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert alpha:fade keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			half filler;
		};

		uniform sampler2D _Flagalbedo;
		uniform float _speed;
		uniform float _scale;
		uniform float _offset;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float2 appendResult334 = (float2(( _speed * _Time.y ) , 1.0));
			float2 uv_TexCoord331 = v.texcoord.xy + appendResult334;
			float4 appendResult323 = (float4(ase_vertex3Pos.x , ( (tex2Dlod( _Flagalbedo, float4( uv_TexCoord331, 0, 0.0) ).g*_scale + _offset) + ase_vertex3Pos.y ) , ase_vertex3Pos.z , 0.0));
			v.vertex.xyz = appendResult323.xyz;
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
480;295;1449;988;1676.905;1819.896;4.08075;True;True
Node;AmplifyShaderEditor.RangedFloatNode;340;144,-1568;Inherit;False;Property;_speed;speed;6;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;333;96,-1408;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;332;352,-1296;Inherit;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;339;384,-1472;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;334;592,-1344;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;331;800,-1392;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;305;1136,-1392;Inherit;True;Property;_Flagalbedo;Flag albedo;3;0;Create;True;0;0;False;0;-1;None;4fe251998bc273a4cb0db20cf36eca7b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;329;544,-768;Inherit;True;Property;_scale;scale;5;0;Create;True;0;0;False;0;0;4.879645;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;330;1008,-560;Inherit;True;Property;_offset;offset;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;328;1424,-704;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;325;1216,-400;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;327;1616,-576;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;318;-202.0776,-824.5082;Inherit;False;461.3383;368.4299;Fix normals for back side faces;3;315;317;316;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;297;190.8553,-205.7399;Inherit;False;927.4102;507.1851;calculated new normal by derivation;9;223;107;224;108;88;93;96;97;56;new normal;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;202;-390.5985,1204.967;Inherit;False;1552.676;586.3004;move the position in tangent Y direction by the deviation amount;14;311;310;114;313;292;210;288;209;208;206;207;205;203;204;delta Y position;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;77;-406.5985,-171.033;Inherit;False;959.9028;475.1613;simply apply vertex transformation;6;312;15;306;294;290;307;new vertex position;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;295;-1168.051,485.6435;Inherit;False;645.3955;379.0187;Comment;6;130;199;127;291;112;287;Inputs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;296;-1632.024,1065.471;Inherit;False;1078.618;465.5402;object to tangent matrix without tangent sign;5;116;121;125;118;117;Object to tangent matrix;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;177;-406.5985,484.9671;Inherit;False;1562.402;582.1888;move the position in tangent X direction by the deviation amount;14;308;289;314;309;113;197;293;196;195;198;194;192;200;201;delta X position;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;88;512.4894,-95.5396;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;288;361.4014,1572.967;Inherit;False;287;amplitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;204;-342.5985,1412.967;Inherit;False;121;ObjectToTangent;1;0;OBJECT;;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.DynamicAppendNode;206;-54.59845,1332.967;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;223;240.4893,-111.5396;Inherit;False;113;xDeviation;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;201;-310.5985,804.9668;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;130;-1115.008,744.0876;Float;False;Property;_Normalpositiondeviation;Normal position deviation;2;0;Create;True;0;0;False;0;0.1;0.1;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;307;-310.5985,132.9671;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;312;70.08321,-86.87794;Inherit;False;Waving Vertex;-1;;21;872b3757863bb794c96291ceeebfb188;0;3;1;FLOAT3;0,0,0;False;12;FLOAT;0;False;13;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;1171.447,-264.9138;Inherit;False;56;newVertexPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TangentVertexDataNode;118;-1585.446,1223.085;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.MatrixFromVectors;116;-1057.446,1191.085;Inherit;False;FLOAT3x3;True;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.GetLocalVarNode;224;240.4893,80.46055;Inherit;False;114;yDeviation;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1003.008,648.0879;Float;False;Property;_Frequency;Frequency;1;0;Create;True;0;0;False;0;0;12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;207;-54.59845,1460.967;Inherit;False;2;2;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;-349.6035,696.4691;Inherit;False;121;ObjectToTangent;1;0;OBJECT;;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;291;-795.0076,648.0879;Float;False;frequency;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;377.4014,1380.967;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;209;137.4015,1380.967;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;314;729.4014,644.9669;Inherit;False;Waving Vertex;-1;;20;872b3757863bb794c96291ceeebfb188;0;3;1;FLOAT3;0,0,0;False;12;FLOAT;0;False;13;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;208;89.40154,1540.967;Inherit;False;121;ObjectToTangent;1;0;OBJECT;;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.GetLocalVarNode;203;-294.5985,1300.967;Inherit;False;199;deviation;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;317;-186.0775,-609.3469;Float;False;Constant;_Backnormalvector;Back normal vector;4;0;Create;True;0;0;False;0;0,0,-1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;323;1776,-400;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;205;-310.5985,1508.967;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;308;601.4015,820.9668;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;310;569.4015,1540.967;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;289;393.4013,836.9668;Inherit;False;287;amplitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;313;681.4014,1380.967;Inherit;False;Waving Vertex;-1;;19;872b3757863bb794c96291ceeebfb188;0;3;1;FLOAT3;0,0,0;False;12;FLOAT;0;False;13;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CrossProductOpNode;125;-1233.446,1159.085;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;93;512.4894,80.46055;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CrossProductOpNode;96;704.4896,-15.53952;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;316;-186.0775,-776.5082;Float;False;Constant;_Frontnormalvector;Front normal vector;4;0;Create;True;0;0;False;0;0,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;113;921.4014,644.9669;Float;False;xDeviation;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;309;377.4014,916.9669;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SwitchByFaceNode;315;101.9224,-696.5082;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;293;393.4013,756.9668;Inherit;False;291;frequency;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;287;-795.0076,552.0879;Float;False;amplitude;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;311;361.4014,1652.967;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;114;921.4014,1380.967;Float;False;yDeviation;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;117;-1576.024,1366.692;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;329.4014,-91.03304;Float;False;newVertexPos;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-358.5985,-91.03304;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;112;-1115.008,552.0879;Float;False;Property;_Amplitude;Amplitude;0;0;Create;True;0;0;False;0;0;0.06;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;198;-54.59845,596.967;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;89.40154,788.9668;Inherit;False;121;ObjectToTangent;1;0;OBJECT;;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;-310.5985,580.967;Inherit;False;199;deviation;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;292;361.4014,1492.967;Inherit;False;291;frequency;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;195;137.4015,644.9669;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-801.4461,1191.085;Float;False;ObjectToTangent;-1;True;1;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.NormalizeNode;97;928.4896,-15.53952;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;107;240.8553,-39.07324;Inherit;False;56;newVertexPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;425.4013,644.9669;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;240.4893,160.4606;Inherit;False;56;newVertexPos;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;294;-170.8015,-26.57899;Inherit;False;291;frequency;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;290;-294.5985,52.96703;Inherit;False;287;amplitude;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;-795.0076,744.0876;Float;False;deviation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;194;-54.59845,724.9668;Inherit;False;2;2;0;FLOAT3x3;0,0,0,1,1,1,1,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;306;-70.59844,52.96703;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2358.199,-882.1743;Float;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;mkULTRA/Shockwave;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Absolute;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;339;0;340;0
WireConnection;339;1;333;0
WireConnection;334;0;339;0
WireConnection;334;1;332;0
WireConnection;331;1;334;0
WireConnection;305;1;331;0
WireConnection;328;0;305;2
WireConnection;328;1;329;0
WireConnection;328;2;330;0
WireConnection;327;0;328;0
WireConnection;327;1;325;2
WireConnection;88;0;223;0
WireConnection;88;1;107;0
WireConnection;206;1;203;0
WireConnection;312;1;15;0
WireConnection;312;12;294;0
WireConnection;312;13;306;0
WireConnection;116;0;125;0
WireConnection;116;1;118;0
WireConnection;116;2;117;0
WireConnection;207;0;204;0
WireConnection;207;1;205;0
WireConnection;291;0;127;0
WireConnection;210;0;209;0
WireConnection;210;1;208;0
WireConnection;209;0;206;0
WireConnection;209;1;207;0
WireConnection;314;1;197;0
WireConnection;314;12;293;0
WireConnection;314;13;308;0
WireConnection;323;0;325;1
WireConnection;323;1;327;0
WireConnection;323;2;325;3
WireConnection;308;0;289;0
WireConnection;308;1;309;1
WireConnection;310;0;288;0
WireConnection;310;1;311;1
WireConnection;313;1;210;0
WireConnection;313;12;292;0
WireConnection;313;13;310;0
WireConnection;125;0;117;0
WireConnection;125;1;118;0
WireConnection;93;0;224;0
WireConnection;93;1;108;0
WireConnection;96;0;93;0
WireConnection;96;1;88;0
WireConnection;113;0;314;0
WireConnection;315;0;316;0
WireConnection;315;1;317;0
WireConnection;287;0;112;0
WireConnection;114;0;313;0
WireConnection;56;0;312;0
WireConnection;198;0;200;0
WireConnection;195;0;198;0
WireConnection;195;1;194;0
WireConnection;121;0;116;0
WireConnection;97;0;96;0
WireConnection;197;0;195;0
WireConnection;197;1;196;0
WireConnection;199;0;130;0
WireConnection;194;0;192;0
WireConnection;194;1;201;0
WireConnection;306;0;290;0
WireConnection;306;1;307;1
WireConnection;0;11;323;0
ASEEND*/
//CHKSM=64A1802E40ED59850C861EBA1FA9EFB241C8391A