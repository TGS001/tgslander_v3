Shader "Unlit/ShieldShader"
{
	Properties
	{
		_RampTex ("Ramp Texture", 2D) = "white" {}
		_RippleTex ("Ripple Texture", 2D) = "white" {}
		_DamageTex ("Damage Texture", 2D) = "white" {}
		_ShieldThickness("Shield Thickness", Float) = 0.75
		_Ping("Ping Amount", Float) = 0
		_ShieldUp("Shield Up", Float) = 0
			_Transparency("Transparency", Float) = 0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
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
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 view : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _RampTex;
			sampler2D _RippleTex;
			sampler2D _DamageTex;
			float4 _MainTex_ST;
			float _ShieldThickness;
			float _Ping;
			float _ShieldUp;
			uniform float _Transparency;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv + float2(0,_Time.x);
				o.normal = v.normal;
				o.view = normalize(ObjSpaceViewDir(v.vertex).xyz);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float mag = (_ShieldThickness - dot(i.normal, i.view));
				fixed4 col = fixed4(tex2D(_RippleTex, i.uv).rgb, mag);//tex2D(_RippleTex, i.uv);
				float upp = clamp((length(col.rgb) + mag) * 0.5, 0, 1);
				if (upp < _ShieldUp) {
					col.rgb = tex2D(_RampTex, float2(length(col.rgb), 0)) + tex2D(_DamageTex, i.uv) * _Ping;
				} else {
					col.a = 0;
				}

				return col * _Transparency;
			}
			ENDCG
		}
	}
}
