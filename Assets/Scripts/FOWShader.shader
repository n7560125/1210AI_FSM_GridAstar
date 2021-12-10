Shader "FOWShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue" = "Overlay" }
		Pass {
			Lighting Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			//ZWrite Off
			SetTexture [_MainTex] { combine texture * constant constantColor [_Color] }
		}
	}
}

