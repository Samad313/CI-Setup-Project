Shader "Self/iOS Additive Tint Shader 2 Cull Off"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off

		Cull Off

		Blend One One
		
		Fog {Mode Off}

		Tags {Queue=Transparent}
			
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					ConstantColor [_Color]
					combine texture * constant, texture
				}
			}
		}
	}
}