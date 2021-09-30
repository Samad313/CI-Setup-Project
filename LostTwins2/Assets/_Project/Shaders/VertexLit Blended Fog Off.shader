﻿Shader "Werplay/VertexLit Blended Fog Off" {
	Properties {
		_EmisColor ("Emissive Color", Color) = (.2,.2,.2,0)
			_MainTex ("Particle Texture", 2D) = "white" {}
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Tags { "LightMode" = "Vertex" }
		Cull Off
		Lighting On
		Material { Emission [_EmisColor] }
		ColorMaterial AmbientAndDiffuse
		ZWrite Off
		ColorMask RGB
		Blend SrcAlpha OneMinusSrcAlpha
		Fog {Mode Off}
		Pass {
			SetTexture [_MainTex] { combine primary * texture }
		}
	}
}