Shader "Self/iOS Alpha Tint Shader Cull Off 2"
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

		Blend SrcAlpha OneMinusSrcAlpha

		Tags {Queue=Transparent1}
			
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					ConstantColor [_Color]
					combine texture * constant
				}
			}
		}
	}
}