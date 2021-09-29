// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "mkULTRA/fake_fog"
{
	Properties
	{
		_cameraFadedistance("cameraFadedistance", Float) = 15
		_depth_distance("depth_distance", Range( 0 , 30)) = 0
		_Color0("Color 0", Color) = (0,0,0,0)
		_Texture0("Texture 0", 2D) = "white" {}
		_scroll_speed("scroll_speed", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float4 _Color0;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _depth_distance;
		uniform float _cameraFadedistance;
		uniform sampler2D _Texture0;
		uniform float _scroll_speed;
		uniform float4 _Texture0_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color0.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth20 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth20 = abs( ( screenDepth20 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _depth_distance ) );
			float3 ase_worldPos = i.worldPos;
			float mulTime32 = _Time.y * _scroll_speed;
			float2 panner28 = ( mulTime32 * float2( 1,0 ) + i.uv_texcoord);
			float2 uv_Texture0 = i.uv_texcoord * _Texture0_ST.xy + _Texture0_ST.zw;
			o.Alpha = ( ( saturate( distanceDepth20 ) * saturate( ( distance( _WorldSpaceCameraPos , ase_worldPos ) * ( 1.0 / _cameraFadedistance ) ) ) ) * tex2D( _Texture0, panner28 ) * tex2D( _Texture0, uv_Texture0 ).a ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
0;427;1920;592;1107.83;-367.8866;1;True;True
Node;AmplifyShaderEditor.WorldSpaceCameraPos;18;-600.3177,10.18683;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;17;-557.3177,163.2446;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;16;-645.3177,473.1868;Inherit;False;Property;_cameraFadedistance;cameraFadedistance;0;0;Create;True;0;0;False;0;15;300;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;15;-336.3177,403.1868;Inherit;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-649.8298,642.8866;Inherit;False;Property;_scroll_speed;scroll_speed;4;0;Create;True;0;0;False;0;0;0.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-569.3177,-110.8132;Inherit;False;Property;_depth_distance;depth_distance;1;0;Create;True;0;0;False;0;0;20;0;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;19;-252.3177,95.18683;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;20;-286.3177,-106.8132;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;32;-409.8298,631.8866;Inherit;False;1;0;FLOAT;0.04;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;29;-246.402,460.5032;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-99.31775,328.1868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;28;83.59802,439.5032;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;0.04;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;13;99.68225,236.1868;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;12;116.6823,13.18683;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;26;-18.40198,630.5032;Inherit;True;Property;_Texture0;Texture 0;3;0;Create;True;0;0;False;0;None;bb47d07b25df6f5459d747a79aa09e91;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;359.6823,95.18683;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;309.598,573.5032;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;False;0;-1;None;bb47d07b25df6f5459d747a79aa09e91;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;321.598,302.0032;Inherit;True;Property;_smoke;smoke;4;0;Create;True;0;0;False;0;-1;None;bb47d07b25df6f5459d747a79aa09e91;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;702.598,93.0032;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;9;73.68225,-219.8132;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;False;0;0,0,0,0;0.1179241,0.6720488,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;900,-182;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;mkULTRA/fake_fog;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;1;16;0
WireConnection;19;0;18;0
WireConnection;19;1;17;0
WireConnection;20;0;21;0
WireConnection;32;0;33;0
WireConnection;14;0;19;0
WireConnection;14;1;15;0
WireConnection;28;0;29;0
WireConnection;28;1;32;0
WireConnection;13;0;14;0
WireConnection;12;0;20;0
WireConnection;10;0;12;0
WireConnection;10;1;13;0
WireConnection;27;0;26;0
WireConnection;22;0;26;0
WireConnection;22;1;28;0
WireConnection;23;0;10;0
WireConnection;23;1;22;0
WireConnection;23;2;27;4
WireConnection;0;2;9;0
WireConnection;0;9;23;0
ASEEND*/
//CHKSM=86A3F3CE216238BDE399B142BD244CEFE29FCEE6