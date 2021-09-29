// Upgrade NOTE: upgraded instancing buffer 'mkULTRAunlit_advanced' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/unlit_advanced"
{
	Properties
	{
		[HDR]_Tint("Tint", Color) = (1,1,1,0)
		_boost("boost", Range( 0 , 1)) = 0
		_texture("texture", 2D) = "white" {}
		_Scroll_U("Scroll_U", Float) = 1
		_Scroll_V("Scroll_V", Float) = 0
		_texture_tiling("texture_tiling", Vector) = (0,0,0,0)
		_pulsespeed("pulse speed", Float) = 0
		_pulse_min("pulse_min", Float) = 0
		_wave_Horizontal_scroll("wave_Horizontal_scroll", Range( -1 , 1)) = -1
		_wave_color("wave_color", Color) = (0,0,0,0)
		_rotate("rotate", Float) = 1.6
		[Toggle]_Flip("Flip", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Offset  0 , 0
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows nofog 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _pulsespeed;
		uniform float _pulse_min;
		uniform sampler2D _texture;
		uniform float _Flip;
		uniform float _wave_Horizontal_scroll;
		uniform float _rotate;
		uniform float4 _wave_color;

		UNITY_INSTANCING_BUFFER_START(mkULTRAunlit_advanced)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Tint)
#define _Tint_arr mkULTRAunlit_advanced
			UNITY_DEFINE_INSTANCED_PROP(float4, _texture_ST)
#define _texture_ST_arr mkULTRAunlit_advanced
			UNITY_DEFINE_INSTANCED_PROP(float2, _texture_tiling)
#define _texture_tiling_arr mkULTRAunlit_advanced
			UNITY_DEFINE_INSTANCED_PROP(float, _boost)
#define _boost_arr mkULTRAunlit_advanced
			UNITY_DEFINE_INSTANCED_PROP(float, _Scroll_U)
#define _Scroll_U_arr mkULTRAunlit_advanced
			UNITY_DEFINE_INSTANCED_PROP(float, _Scroll_V)
#define _Scroll_V_arr mkULTRAunlit_advanced
		UNITY_INSTANCING_BUFFER_END(mkULTRAunlit_advanced)

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime44 = _Time.y * _pulsespeed;
			float _boost_Instance = UNITY_ACCESS_INSTANCED_PROP(_boost_arr, _boost);
			float2 _texture_tiling_Instance = UNITY_ACCESS_INSTANCED_PROP(_texture_tiling_arr, _texture_tiling);
			float _Scroll_U_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scroll_U_arr, _Scroll_U);
			float mulTime18 = _Time.y * _Scroll_U_Instance;
			float _Scroll_V_Instance = UNITY_ACCESS_INSTANCED_PROP(_Scroll_V_arr, _Scroll_V);
			float mulTime7 = _Time.y * _Scroll_V_Instance;
			float2 appendResult17 = (float2(mulTime18 , mulTime7));
			float2 uv_TexCoord4 = i.uv_texcoord * _texture_tiling_Instance + appendResult17;
			float4 clampResult22 = clamp( ( _boost_Instance + tex2D( _texture, uv_TexCoord4 ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 _Tint_Instance = UNITY_ACCESS_INSTANCED_PROP(_Tint_arr, _Tint);
			float4 _texture_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_texture_ST_arr, _texture_ST);
			float2 uv_texture = i.uv_texcoord * _texture_ST_Instance.xy + _texture_ST_Instance.zw;
			float4 temp_output_23_0 = ( ( (_pulse_min + (sin( mulTime44 ) - -1.0) * (1.0 - _pulse_min) / (1.0 - -1.0)) * ( clampResult22 * _Tint_Instance ) ) * tex2D( _texture, uv_texture ).a );
			float2 temp_cast_0 = (( _wave_Horizontal_scroll + 0.0 )).xx;
			float2 uv_TexCoord108 = i.uv_texcoord + temp_cast_0;
			float cos136 = cos( _rotate );
			float sin136 = sin( _rotate );
			float2 rotator136 = mul( uv_TexCoord108 - float2( 0.5,0.5 ) , float2x2( cos136 , -sin136 , sin136 , cos136 )) + float2( 0.5,0.5 );
			float temp_output_113_0 = (rotator136.x*13.0 + -7.0);
			float4 clampResult134 = clamp( ( temp_output_23_0 * (( _Flip )?( ( 1.0 - temp_output_113_0 ) ):( temp_output_113_0 )) * _wave_color ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 clampResult131 = clamp( ( temp_output_23_0 + clampResult134 ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Emission = clampResult131.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
53;527;1920;1019;-3207.112;-268.6058;1.797441;True;True
Node;AmplifyShaderEditor.RangedFloatNode;15;-1897.267,-79.23337;Inherit;False;InstancedProperty;_Scroll_U;Scroll_U;3;0;Create;True;0;0;False;0;1;0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2242.467,-52.53343;Inherit;False;InstancedProperty;_Scroll_V;Scroll_V;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;-2083.639,-48.17613;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;18;-1692.467,-72.63368;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;1266.883,912.1897;Inherit;False;Constant;_Float1;Float 1;1;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;105;1206.29,793.0513;Inherit;False;Property;_wave_Horizontal_scroll;wave_Horizontal_scroll;8;0;Create;True;0;0;False;0;-1;-1;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;19;-1475.135,-259.0518;Inherit;False;InstancedProperty;_texture_tiling;texture_tiling;5;0;Create;True;0;0;False;0;0,0;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;17;-1444.666,-71.8333;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1008.296,322.3057;Inherit;True;Property;_texture;texture;2;0;Create;True;0;0;False;0;None;456585bd3d12cde47a3ce76390332031;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1061.939,-121.9761;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;107;1650.211,768.6821;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-447.4261,-235.0621;Inherit;False;InstancedProperty;_boost;boost;1;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;108;1877.378,531.6312;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;61;132.2561,-388.7886;Inherit;False;Property;_pulsespeed;pulse speed;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;1943.274,861.5436;Inherit;False;Property;_rotate;rotate;10;0;Create;True;0;0;False;0;1.6;1.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-427.9833,-97.69298;Inherit;True;Property;_Texture;Texture;0;0;Create;True;0;0;False;0;-1;None;31d3b3eb028618d48bc012ed312ed7f5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;44;454.4638,-363.5311;Inherit;False;1;0;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;2565.603,1055.196;Inherit;False;Constant;_wave_scale;wave_scale;11;0;Create;True;0;0;False;0;13;5.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;2570.412,1153.451;Inherit;False;Constant;_wave_offset;wave_offset;12;0;Create;True;0;0;False;0;-7;-1.23;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;9;-17.12603,-163.5621;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;136;2234.837,654.1977;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;139;2537.723,726.5404;Inherit;True;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;122;2859.354,985.385;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;80;885.0021,-223.3314;Inherit;False;Property;_pulse_min;pulse_min;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;123;2856.652,1083.047;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;2;-551,181.5;Inherit;False;InstancedProperty;_Tint;Tint;0;1;[HDR];Create;True;0;0;False;0;1,1,1,0;0,0.6312504,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;22;262.5607,-137.6756;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinOpNode;46;725.0596,-401.0778;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;113;2956.473,863.0052;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;517.2095,-59.2126;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;70;1090.768,-421.9696;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0.25;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;142.3741,433.6997;Inherit;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;1706.807,-265.0911;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;148;3126.212,1169.217;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;150;3344.894,1204.356;Inherit;True;Property;_Flip;Flip;11;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;2261.626,126.6558;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;133;3348.447,1007.71;Inherit;False;Property;_wave_color;wave_color;9;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;118;3606.407,915.0363;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;134;4002.793,889.5103;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;132;4383.376,811.8622;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;131;4838.085,653.7112;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;81;5211.122,295.2066;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;mkULTRA/unlit_advanced;False;False;False;False;False;False;False;False;False;True;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;3;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;1;False;-1;1;False;-1;0;1;False;-1;1;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;16;0
WireConnection;18;0;15;0
WireConnection;17;0;18;0
WireConnection;17;1;7;0
WireConnection;4;0;19;0
WireConnection;4;1;17;0
WireConnection;107;0;105;0
WireConnection;107;1;106;0
WireConnection;108;1;107;0
WireConnection;1;0;12;0
WireConnection;1;1;4;0
WireConnection;44;0;61;0
WireConnection;9;0;10;0
WireConnection;9;1;1;0
WireConnection;136;0;108;0
WireConnection;136;2;137;0
WireConnection;139;0;136;0
WireConnection;122;0;111;0
WireConnection;123;0;112;0
WireConnection;22;0;9;0
WireConnection;46;0;44;0
WireConnection;113;0;139;0
WireConnection;113;1;122;0
WireConnection;113;2;123;0
WireConnection;3;0;22;0
WireConnection;3;1;2;0
WireConnection;70;0;46;0
WireConnection;70;3;80;0
WireConnection;13;0;12;0
WireConnection;58;0;70;0
WireConnection;58;1;3;0
WireConnection;148;0;113;0
WireConnection;150;0;113;0
WireConnection;150;1;148;0
WireConnection;23;0;58;0
WireConnection;23;1;13;4
WireConnection;118;0;23;0
WireConnection;118;1;150;0
WireConnection;118;2;133;0
WireConnection;134;0;118;0
WireConnection;132;0;23;0
WireConnection;132;1;134;0
WireConnection;131;0;132;0
WireConnection;81;2;131;0
ASEEND*/
//CHKSM=8A311FD6B94A2A4C5C04B3732B1F28AE4351CAD2