Shader "Self/iOS Selection Shader"
{
	Properties
	{
		_Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	}
	
	Category
	{
		ZWrite Off

		Cull Back

		//Blend OneMinusSrcAlpha SrcAlpha 
		AlphaTest Greater 0.5

		Tags {Queue=Transparent}
			
		SubShader
		{
			Pass
			{
				SetTexture [_MainTex]
				{
					ConstantColor [_Color]
					combine constant, texture
				}
			}
		}
	}
}