// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/FX_Tail_cutout"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_main("main", 2D) = "white" {}
		_eros("eros", 2D) = "white" {}
		_45("45", Float) = 0
		_panspeed("pan speed", Float) = 1
		_color_A("color_A", Color) = (0.990566,0.8201764,0,0)
		_Color_B("Color_B", Color) = (0.9528302,0.09438412,0.09438412,0)
		_Tail_color("Tail_color", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Tail_color;
		uniform float4 _Color_B;
		uniform float4 _color_A;
		uniform sampler2D _main;
		uniform float _panspeed;
		uniform sampler2D _eros;
		uniform float _45;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime24 = _Time.y * _panspeed;
			float2 panner6 = ( mulTime24 * float2( 1,0 ) + i.uv_texcoord);
			float4 lerpResult29 = lerp( _Color_B , _color_A , tex2D( _main, panner6 ));
			float4 lerpResult45 = lerp( _Tail_color , lerpResult29 , ( 1.0 - i.uv_texcoord.x ));
			o.Emission = lerpResult45.rgb;
			o.Alpha = 1;
			float mulTime51 = _Time.y * _45;
			float2 panner54 = ( mulTime51 * float2( 1,0 ) + i.uv_texcoord);
			float4 tex2DNode7 = tex2D( _eros, panner54 );
			clip( tex2DNode7.a - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
333;339;1534;665;725.3467;484.1318;2.041392;True;True
Node;AmplifyShaderEditor.RangedFloatNode;25;-1575.275,259.0077;Inherit;False;Property;_panspeed;pan speed;5;0;Create;True;0;0;False;0;1;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1087.466,-360.8706;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;24;-1218.746,266.511;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-766.4555,164.9083;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;53;-1647.276,406.8235;Inherit;False;Property;_45;45;4;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-332,-8;Inherit;True;Property;_main;main;1;0;Create;True;0;0;False;0;-1;cc95dd42df9b599449a2841b2abda0e5;e26a29db44e9ccf41b2e0f1fbf5f78f8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;27;-419.1502,-675.6324;Inherit;False;Property;_Color_B;Color_B;7;0;Create;True;0;0;False;0;0.9528302,0.09438412,0.09438412,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-382.3494,-341.3947;Inherit;False;Property;_color_A;color_A;6;0;Create;True;0;0;False;0;0.990566,0.8201764,0,0;0.990566,0.8201764,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;51;-1269.607,422.1797;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;290.0811,-481.3503;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;47;593.3459,-701.2673;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;46;218.3352,-915.3479;Inherit;False;Property;_Tail_color;Tail_color;8;0;Create;True;0;0;False;0;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;54;-812.6296,360.6263;Inherit;False;3;0;FLOAT2;1,1;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1142.447,1046.483;Inherit;False;Property;_D;D;3;0;Create;True;0;0;False;0;0.5314584;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-684.0471,876.4832;Inherit;False;Constant;_one;one;2;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-679.2469,771.1832;Inherit;False;Constant;_zero;zero;2;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-748.647,1001.183;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-965.7852,584.3195;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;12;-334.932,696.4748;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;11;416.3015,663.9122;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-139.7471,980.1831;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;9;72.94922,493.1082;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;10;126.2529,770.1667;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;45;1020.644,-795.6678;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;13;667.5198,669.3588;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;919.8569,203.4736;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;55;1255.94,242.8796;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-1018.748,1137.583;Inherit;False;Constant;_N;N;2;0;Create;True;0;0;False;0;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-333.0512,209.6603;Inherit;True;Property;_eros;eros;2;0;Create;True;0;0;False;0;-1;None;e26a29db44e9ccf41b2e0f1fbf5f78f8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1477.998,-280.18;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;mkULTRA/FX_Tail_cutout;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;25;0
WireConnection;6;0;2;0
WireConnection;6;1;24;0
WireConnection;1;1;6;0
WireConnection;51;0;53;0
WireConnection;29;0;27;0
WireConnection;29;1;26;0
WireConnection;29;2;1;0
WireConnection;47;0;2;1
WireConnection;54;0;2;0
WireConnection;54;1;51;0
WireConnection;18;0;20;0
WireConnection;18;1;19;0
WireConnection;12;0;15;1
WireConnection;12;1;16;0
WireConnection;12;2;17;0
WireConnection;12;3;18;0
WireConnection;12;4;17;0
WireConnection;11;0;9;0
WireConnection;11;1;10;0
WireConnection;21;0;12;0
WireConnection;21;1;20;0
WireConnection;9;0;7;4
WireConnection;9;1;12;0
WireConnection;10;0;21;0
WireConnection;10;1;12;0
WireConnection;45;0;46;0
WireConnection;45;1;29;0
WireConnection;45;2;47;0
WireConnection;13;0;11;0
WireConnection;8;1;13;0
WireConnection;55;0;8;0
WireConnection;7;1;54;0
WireConnection;0;2;45;0
WireConnection;0;10;7;4
ASEEND*/
//CHKSM=6B7B821D50418759050EF5F245170C200D2AF8A1