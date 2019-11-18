Shader "Unlit/ShieldImpact"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Progress("Progress", Float) = 0
		_Magnitude("Magnitude", Float) = 1
		_BandSize("Band Size", Float) = 0.1
		_Coverage("Coverage", Float) = 0.5
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 100

		Pass
		{
			ZWrite Off
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 snmag : TEXCOORD1;
				
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
	
			float _Progress;
			float _Magnitude;
			float _BandSize;
			float _Coverage;

			float nrange(float range) {
				return 1 - clamp(range * range, 0, 1);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float2 u1 = TRANSFORM_TEX(v.uv + float2(_Time.x, _Time.y), _MainTex);
				float2 u2 = TRANSFORM_TEX(v.uv + float2(-_Time.y, _Time.z), _MainTex);
				o.uv = float4(u1.x, u1.y, u2.x, u2.y);
				o.snmag = normalize(mul(UNITY_MATRIX_MV, float4(v.normal, 0)));
				float coverage = _Coverage * 3.1415926535898;
				o.snmag.w = nrange((dot(v.normal.xyz, float3(0, 1, 0)) - cos(clamp(_Progress, 0, 1) * coverage)) / _BandSize);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float mix = (_Magnitude - dot(i.snmag.xyz, float3(0, 0, 1))) * i.snmag.w * (1 - _Progress);
				fixed4 col = (tex2D(_MainTex, i.uv.xy) + tex2D(_MainTex, i.uv.zw)) * mix;
				return col;
			}
			ENDCG
		}
	}
}
