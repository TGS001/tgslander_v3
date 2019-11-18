Shader "Unlit/ScrollingBeamShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_Strength("Strength", float) = 1
			_MainScroll("Main Scroll Speed", float) = 1
		   _NoiseScroll("Noise Scroll Speed", float) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			zwrite off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _NoiseTex;
			float4 _MainTex_ST;
			float4 _NoiseTex_ST;
			float _Strength;
			float _MainScroll;
			float _NoiseScroll;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv + float2(_Time.x * _MainScroll, 0), _MainTex);
				o.uv2 = TRANSFORM_TEX(v.uv + float2(_Time.x * _NoiseScroll, 0), _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 mixcol = tex2D(_NoiseTex, i.uv2);
				fixed4 col = tex2D(_MainTex, i.uv);
				
				col.a = col.a * mixcol.a * _Strength;
				col.rgb = (col.rgb + mixcol.rgb) * col.a;
				return col;
			}
			ENDCG
		}
	}
}
