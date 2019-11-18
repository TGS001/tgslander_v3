Shader "Hidden/OutlineShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
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
				float4 vertex : SV_POSITION;
				float2 uv[9] : TEXCOORD0;
			};

			uniform float4 lineColor;
			uniform half depthComponent;
			uniform half normalComponent;
			uniform half size;

			sampler2D _MainTex;
			uniform float4 _MainTex_TexelSize;
			half4 _MainTex_ST;
			sampler2D _CameraDepthNormalsTexture;
			half4 _CameraDepthNormalsTexture_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv[0] = UnityStereoScreenSpaceUVAdjust(v.uv, _MainTex_ST);

				o.uv[1] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(1, 1) * size, _MainTex_ST);
				o.uv[2] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(-1, -1) * size, _MainTex_ST);
				o.uv[3] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(-1, 1) * size, _MainTex_ST);
				o.uv[4] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(1, -1) * size, _MainTex_ST);

				o.uv[5] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(1.42, 0) * size, _MainTex_ST);
				o.uv[6] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(-1.42, 0) * size, _MainTex_ST);
				o.uv[7] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(0, 1.42) * size, _MainTex_ST);
				o.uv[8] = UnityStereoScreenSpaceUVAdjust(v.uv + _MainTex_TexelSize.xy * half2(0, -1.42) * size, _MainTex_ST);
				return o;
			}
			
			inline half sampleDifference(half4 sample1, half4 sample2)
			{
				half2 normalc = abs(sample1.xy - sample2.xy) * normalComponent;
				half depthc = abs(DecodeFloatRG(sample1.zw)-DecodeFloatRG(sample2.zw)) * depthComponent;
				return clamp(normalc.x + normalc.y + depthc, 0, 1) * size;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				half4 sample1 = tex2D(_CameraDepthNormalsTexture, i.uv[1].xy);
				half4 sample2 = tex2D(_CameraDepthNormalsTexture, i.uv[2].xy);
				half4 sample3 = tex2D(_CameraDepthNormalsTexture, i.uv[3].xy);
				half4 sample4 = tex2D(_CameraDepthNormalsTexture, i.uv[4].xy);
				half4 sample5 = tex2D(_CameraDepthNormalsTexture, i.uv[5].xy);
				half4 sample6 = tex2D(_CameraDepthNormalsTexture, i.uv[6].xy);
				half4 sample7 = tex2D(_CameraDepthNormalsTexture, i.uv[7].xy);
				half4 sample8 = tex2D(_CameraDepthNormalsTexture, i.uv[8].xy);

				half edge =

					(sampleDifference(sample1, sample2)
						+ sampleDifference(sample3, sample4)
						+ sampleDifference(sample5, sample6)
						+ sampleDifference(sample7, sample8)) * 0.25;

				return lerp(tex2D(_MainTex, i.uv[0]), lineColor, edge);
			}
			ENDCG
		}
	}
}
