Shader "Custom/WaterShader" {
	Properties{
		_Refraction("Refraction", Float) = 5.0
		//_DepthFactor("DepthFactor", Float) = 1.0
		_Distort("Distortion", 2D) = "white" {}
		_Tint("Tint", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Transparent" "Queue" = "Overlay-1"}
			LOD 200
			GrabPass {"_WaterTex"}

			CGPROGRAM

#include "UnityCG.cginc"
			#pragma surface surf NoLighting
			#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		uniform float _Refraction;
	uniform float _DepthFactor;
		sampler2D _WaterTex;
		uniform float4 _WaterTex_TexelSize;
		sampler2D _Distort;
		sampler2D _Tint;
		sampler2D _CameraDepthTexture;

		struct Input {
			//float3 position;
			float2 distortCoord : TEXCOORD1;
			float2 tintCoord : TEXCOORD2;
			float4 screenPos;
			//float eyed;
		};

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void vert(inout appdata_base v, out Input o)
		{
			//UNITY_INITIALIZE_OUTPUT(Input, o);
			float4 clipPos = UnityObjectToClipPos(float4(v.vertex.xyz, 1));
			//o.position = clipPos.xyz;
			o.distortCoord = (float2)v.vertex * 0.1 + float2(_Time.x, _SinTime.x) * 0.3;
			o.tintCoord = (float2)v.vertex * 0.1 + float2(_SinTime.x, _CosTime.y) * 0.3;
			//o.screenPos = float4(ComputeGrabScreenPos(o.position).xyz, 1);
			o.screenPos = ComputeScreenPos(clipPos);
			//o.eyed = o.screenPos.z;
			//o.eyed = COMPUTE_DEPTH_01;
			//COMPUTE_EYEDEPTH(o.eyed);
			//o.eyed = -UnityObjectToViewPos(v.vertex).z;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			float3 distort = tex2D(_Distort, IN.distortCoord).xyz;
			float3 tint = tex2D(_Tint, IN.tintCoord).xyz * tex2D(_Tint, IN.distortCoord).xyz;
			//half texDepth = LinearEyeDepth(tex2D(_CameraDepthTexture, IN.screenPos.xy)).r;
			//half depth = 1 - abs(texDepth - IN.eyed);
			IN.screenPos.x = _WaterTex_TexelSize.x * (distort.r - 0.5) * _Refraction + IN.screenPos.x;
			IN.screenPos.y = _WaterTex_TexelSize.y * (distort.g - 0.5) * _Refraction + IN.screenPos.y;
			float4 refract = tex2D(_WaterTex, IN.screenPos.xy) * float4(tint,1);
			o.Alpha = refract.a;
			o.Emission = refract.rgb;
			//o.Emission = float3(0, 0, 0);
			//o.Emission.r = IN.eyed;
			//o.Emission.g = texDepth;
			//o.Emission.b = depth;
			
		}
		ENDCG
	}
		FallBack "Diffuse"
}
