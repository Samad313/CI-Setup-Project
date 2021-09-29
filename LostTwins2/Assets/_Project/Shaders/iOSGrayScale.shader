Shader "Self/Grayscale"
{

    Properties 

    {

        _MainTex ("Texture", 2D) = ""

    }

 

    SubShader 

    {

        Blend SrcAlpha OneMinusSrcAlpha

 

        Pass 

        {

            Color(.389, .1465, .4645, 0.5)

            SetTexture[_MainTex] {Combine one - texture * primary alpha, texture alpha}

            SetTexture[_] {Combine previous Dot3 primary}

        }

    }

}