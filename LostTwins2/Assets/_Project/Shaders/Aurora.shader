// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Aurora"
{
	Properties
	{
		_Mix("Mix", 2D) = "white" {}
		_Vector_Offset_Strength("Vector_Offset_Strength", Float) = 0
		_VO_Speed_U("VO_Speed_U", Float) = 0
		_Indentations_Speed_U("Indentations_Speed_U", Float) = 0
		_Lines_Speed_U("Lines_Speed_U", Float) = 0
		_VO_Speed_V("VO_Speed_V", Float) = 0
		_Indentations_Speed_V("Indentations_Speed_V", Float) = 0
		_Lines_Speed_V("Lines_Speed_V", Float) = 0
		_Indentations_Add_Value("Indentations_Add_Value", Float) = 0
		_Lines_Strength("Lines_Strength", Float) = 0
		_Lines_Tile_U("Lines_Tile_U", Float) = 0
		_Lines_Tile_V("Lines_Tile_V", Float) = 0
		_Indentations_Tile_U("Indentations_Tile_U", Float) = 0
		_Indentations_Tile_V("Indentations_Tile_V", Float) = 0
		_VO_Tile_U("VO_Tile_U", Float) = 0
		_VO_Tile_V("VO_Tile_V", Float) = 0
		_Color_Slider("Color_Slider", Range( 0 , 2)) = 0
		_Color_1("Color_1", Color) = (0,0,0,0)
		_Color_2("Color_2", Color) = (0,0,0,0)
		_Emissive_Strength("Emissive_Strength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend One One , One One
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mix;
		uniform float _VO_Tile_U;
		uniform float _VO_Tile_V;
		uniform float _VO_Speed_U;
		uniform float _VO_Speed_V;
		uniform float _Vector_Offset_Strength;
		uniform float4 _Color_1;
		uniform float4 _Color_2;
		uniform float _Color_Slider;
		uniform float _Lines_Strength;
		uniform float _Lines_Tile_U;
		uniform float _Lines_Tile_V;
		uniform float _Lines_Speed_U;
		uniform float _Lines_Speed_V;
		uniform float _Indentations_Add_Value;
		uniform float _Indentations_Tile_U;
		uniform float _Indentations_Tile_V;
		uniform float _Indentations_Speed_U;
		uniform float _Indentations_Speed_V;
		uniform float4 _Mix_ST;
		uniform float _Emissive_Strength;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 appendResult51 = (float4(_VO_Tile_U , _VO_Tile_V , 0.0 , 0.0));
			float4 appendResult14 = (float4(_VO_Speed_U , _VO_Speed_V , 0.0 , 0.0));
			float4 appendResult7 = (float4(0.0 , ( tex2Dlod( _Mix, float4( ( appendResult51 * ( ( appendResult14 * _Time.y ) + float4( v.texcoord.xy, 0.0 , 0.0 ) ) ).xy, 0, 0.0) ).r * _Vector_Offset_Strength ) , 0.0 , 0.0));
			v.vertex.xyz += appendResult7.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult43 = (float4(_Lines_Tile_U , _Lines_Tile_V , 0.0 , 0.0));
			float4 appendResult33 = (float4(_Lines_Speed_U , _Lines_Speed_V , 0.0 , 0.0));
			float4 appendResult47 = (float4(_Indentations_Tile_U , _Indentations_Tile_V , 0.0 , 0.0));
			float4 appendResult22 = (float4(_Indentations_Speed_U , _Indentations_Speed_V , 0.0 , 0.0));
			float2 uv_Mix = i.uv_texcoord * _Mix_ST.xy + _Mix_ST.zw;
			float temp_output_28_0 = ( ( ( _Lines_Strength * tex2D( _Mix, ( appendResult43 * ( ( appendResult33 * _Time.y ) + float4( i.uv_texcoord, 0.0 , 0.0 ) ) ).xy ).a ) + ( _Indentations_Add_Value + tex2D( _Mix, ( appendResult47 * ( ( appendResult22 * _Time.y ) + float4( i.uv_texcoord, 0.0 , 0.0 ) ) ).xy ).b ) ) * tex2D( _Mix, uv_Mix ).g );
			float4 lerpResult53 = lerp( _Color_1 , _Color_2 , ( _Color_Slider * temp_output_28_0 ));
			o.Emission = ( ( lerpResult53 * temp_output_28_0 ) * _Emissive_Strength ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
-186;-1029;1855;1023;3878.598;2024.464;2.0487;True;True
Node;AmplifyShaderEditor.RangedFloatNode;21;-2775.337,-853.6497;Inherit;False;Property;_Indentations_Speed_U;Indentations_Speed_U;3;0;Create;True;0;0;False;0;0;0.0125;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-2807.734,-1500.53;Inherit;False;Property;_Lines_Speed_U;Lines_Speed_U;4;0;Create;True;0;0;False;0;0;-0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-2824.377,-1338.717;Inherit;False;Property;_Lines_Speed_V;Lines_Speed_V;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-2766.777,-716.4654;Inherit;False;Property;_Indentations_Speed_V;Indentations_Speed_V;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2236.53,-728.5403;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;33;-2340.158,-1312.128;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;16;-2386.976,-404.1405;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;-2086.468,-1308.111;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-2406.626,-1516.544;Inherit;False;Property;_Lines_Tile_V;Lines_Tile_V;11;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-2788.576,-1147.623;Inherit;False;Property;_Indentations_Tile_U;Indentations_Tile_U;12;0;Create;True;0;0;False;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2404.823,-1639.173;Inherit;False;Property;_Lines_Tile_U;Lines_Tile_U;10;0;Create;True;0;0;False;0;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;11;-2379.431,138.3473;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1982.841,-724.5232;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-2779.545,-1012.721;Inherit;False;Property;_Indentations_Tile_V;Indentations_Tile_V;13;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-2417.587,-1101.25;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;35;-1789.5,-1322.264;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-2054.969,-1592.286;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;-1661.938,-727.6301;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1748.482,-1053.22;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;3;-2399.704,-164.6828;Inherit;True;Property;_Mix;Mix;0;0;Create;True;0;0;False;0;None;f06709e1b293a5b429206e4612af600f;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1759.219,-1574.252;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1214.742,-952.9383;Inherit;False;Property;_Lines_Strength;Lines_Strength;9;0;Create;True;0;0;False;0;0;4.63;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-1406.198,-678.5416;Inherit;True;Property;_TextureSample2;Texture Sample 2;9;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;13;-2533.004,700.7394;Inherit;False;Property;_VO_Speed_V;VO_Speed_V;5;0;Create;True;0;0;False;0;0;-1.77;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-2531.684,565.7894;Inherit;False;Property;_VO_Speed_U;VO_Speed_U;2;0;Create;True;0;0;False;0;0;-0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-1144.748,-365.2159;Inherit;False;Property;_Indentations_Add_Value;Indentations_Add_Value;8;0;Create;True;0;0;False;0;0;0.87;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;26;-1442.957,-390.5767;Inherit;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;14;-2212.994,560.8196;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-770.7103,-802.3039;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-889.7155,-260.1058;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-589.1504,-513.1404;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-1435.952,-118.716;Inherit;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;50;-2541.803,1036.906;Inherit;False;Property;_VO_Tile_V;VO_Tile_V;15;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-2005.054,564.9255;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2546.771,916.0034;Inherit;False;Property;_VO_Tile_U;VO_Tile_U;14;0;Create;True;0;0;False;0;0;2.69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-1762.654,567.9553;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;51;-2245.342,916.0034;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-604.0471,-871.0377;Inherit;False;Property;_Color_Slider;Color_Slider;16;0;Create;True;0;0;False;0;0;0.86;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-391.7661,-147.9668;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;56;-239.683,-938.942;Inherit;False;Property;_Color_1;Color_1;17;0;Create;True;0;0;False;0;0,0,0,0;0.121393,0.8301887,0.4115516,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-1779.95,844.7866;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;57;-231.4018,-715.355;Inherit;False;Property;_Color_2;Color_2;18;0;Create;True;0;0;False;0;0,0,0,0;0.2594314,0.6619983,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-244.6516,-501.705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;53;144.5556,-731.9171;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1435.444,171.7583;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;5;-717.7714,360.4137;Inherit;False;Property;_Vector_Offset_Strength;Vector_Offset_Strength;1;0;Create;True;0;0;False;0;0;-2.69;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-120.4366,-14.78177;Inherit;False;Property;_Emissive_Strength;Emissive_Strength;19;0;Create;True;0;0;False;0;0;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-606.3976,25.64984;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;121.3689,-231.7441;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-592.7714,187.4137;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;282.0204,16.68613;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-264.3976,157.6498;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;473.6734,-43.06123;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Aurora;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;22;0;21;0
WireConnection;22;1;20;0
WireConnection;33;0;31;0
WireConnection;33;1;32;0
WireConnection;34;0;33;0
WireConnection;34;1;16;2
WireConnection;24;0;22;0
WireConnection;24;1;16;2
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;35;0;34;0
WireConnection;35;1;11;0
WireConnection;43;0;41;0
WireConnection;43;1;42;0
WireConnection;25;0;24;0
WireConnection;25;1;11;0
WireConnection;48;0;47;0
WireConnection;48;1;25;0
WireConnection;44;0;43;0
WireConnection;44;1;35;0
WireConnection;36;0;3;0
WireConnection;36;1;44;0
WireConnection;26;0;3;0
WireConnection;26;1;48;0
WireConnection;14;0;12;0
WireConnection;14;1;13;0
WireConnection;38;0;39;0
WireConnection;38;1;36;4
WireConnection;29;0;30;0
WireConnection;29;1;26;3
WireConnection;37;0;38;0
WireConnection;37;1;29;0
WireConnection;19;0;3;0
WireConnection;17;0;14;0
WireConnection;17;1;16;2
WireConnection;18;0;17;0
WireConnection;18;1;11;0
WireConnection;51;0;49;0
WireConnection;51;1;50;0
WireConnection;28;0;37;0
WireConnection;28;1;19;2
WireConnection;52;0;51;0
WireConnection;52;1;18;0
WireConnection;55;0;54;0
WireConnection;55;1;28;0
WireConnection;53;0;56;0
WireConnection;53;1;57;0
WireConnection;53;2;55;0
WireConnection;1;0;3;0
WireConnection;1;1;52;0
WireConnection;58;0;53;0
WireConnection;58;1;28;0
WireConnection;4;0;1;1
WireConnection;4;1;5;0
WireConnection;60;0;58;0
WireConnection;60;1;59;0
WireConnection;7;0;8;0
WireConnection;7;1;4;0
WireConnection;0;2;60;0
WireConnection;0;11;7;0
ASEEND*/
//CHKSM=FB00AEBC5A18114749768ACFF7234CFB8E287218