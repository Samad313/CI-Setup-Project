Shader "Projector/Reflective Alpha" { 
   Properties { 
      _ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear } 
      _Color ( "Main Color (RGB)", Color ) = ( 1, 1, 1, 1 )
   }
   
   Subshader { 
   //Tags {Queue=Transparent}
         ZWrite off 
         //Fog { Color (1, 1, 1) } 
         //ColorMask RGB
         Blend SrcAlpha OneMinusSrcAlpha
      Pass { 
       
         SetTexture [_ShadowTex] { 
         	Constantcolor [_Color]
            combine texture * constant
            Matrix [_Projector] 
         } 
         //SetTexture [_FalloffTex] { 
         //   constantColor (0,0,0,0) 
         //   combine previous lerp (texture) constant 
         //   Matrix [_ProjectorClip] 
         //} 
      }
   }
}