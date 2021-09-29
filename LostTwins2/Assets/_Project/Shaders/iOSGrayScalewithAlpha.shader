// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Self/Grayscale with Alpha" {

 

    Properties {
        _Strength ("Strength", float) = 1
        _Strength ("Slider", Range(0, 2)) = 1

        _MainTex ("Texture", 2D) = ""
        _ColorTint ("Tint", float) = 1
        
        
        

    }

    

    SubShader {

        Tags
		{ 
			"Queue"="Transparent" 
		}

        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass {

            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            struct v2f {

                float4 position : SV_POSITION;

                float2 uv_mainTex : TEXCOORD;

            };

            

            float _Strength;

            uniform float4 _MainTex_ST;

            v2f vert (float4 position : POSITION, float2 uv : TEXCOORD0) {

                v2f o;

                o.position = UnityObjectToClipPos (position);

                o.uv_mainTex = uv * _MainTex_ST.xy + _MainTex_ST.zw;

                return o;

            }

            

            uniform sampler2D _MainTex;

            fixed4 frag(float2 uv_mainTex : TEXCOORD) : COLOR {

                fixed4 mainTex = tex2D (_MainTex, uv_mainTex);

                fixed3 bwColor = dot (mainTex.rgb, fixed3 (.222, .707, .071));
                
                return fixed4(bwColor + (mainTex.rgb-bwColor)*_Strength, mainTex.a);

            }

            ENDCG

        }

    }
    FallBack "Diffuse"

}