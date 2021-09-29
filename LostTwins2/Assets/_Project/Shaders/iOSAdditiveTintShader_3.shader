Shader "Self/iOS Additive Tint Shader 3 (for NGUI)"
{
	Properties
	{
		_TintColor ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off

		Cull Back

		Blend One One

		Tags {Queue=Transparent}
			
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					ConstantColor [_TintColor]
					combine texture * constant, texture
				}
			}
		}
	}
}