Shader "Custom/SpriteTerrain" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Surface1 (RGB)", 2D) = "white" {}
		_MainTex2("Surface2 (RGB)", 2D) = "white" {}
		_RockTex1("Bedrock1 (RGB)", 2D) = "white" {}
		_RockTex2("Bedrock2 (RGB)", 2D) = "white" {}

		//_DepthTex1 ("Depth1 (RGB)", 2D) = "white" {}
		//_DepthTex2 ("Depth2 (RGB)", 2D) = "white" {}

		//_RampTex("Ramp tex", 2D) = "white" {}

		//[Toggle(VCM_ON)] _VCM("Vertex Color Multiply", int) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Anime fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MainTex2;
		sampler2D _RockTex1;
		sampler2D _RockTex2;

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			half4 color : COLOR;
		};

		fixed4 _Color;

		half4 LightingAnime(SurfaceOutput s, half3 lightDir, half atten) {
			half3 rLightDir = normalize(half3(lightDir.x, lightDir.y, -0.5));
			half ndotr = dot(s.Normal, rLightDir);
			half diff = (ndotr * 0.25 + 0.5);

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * atten * diff;

			c.a = s.Alpha;
			return c;
		}

		float Greyscale(fixed3 col) {
			return col.r * 0.21 + col.g * 0.72 + col.b * 0.07;
		}

		float IntensityBlend(out fixed3 pixcol, fixed3 c1, float i1, fixed3 c2, float i2, float ramp) {
			float t = clamp(((i1 - i2 + ramp + 0.1) * 5), 0, 1);
			pixcol = lerp(c2, c1, t);
			return lerp(i2, i1, t);
		}

		void surf(Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed3 c1 = tex2D(_MainTex, IN.uv_MainTex.xy).rgb;
			fixed3 c2 = tex2D(_MainTex2, IN.uv_MainTex.xy).rgb;
		 fixed3 rock1 = tex2D(_RockTex1, IN.uv_MainTex.xy).rgb;
		 fixed3 rock2 = tex2D(_RockTex2, IN.uv_MainTex.xy).rgb;
			float ramp = (IN.color.g) - 0.5;
			float surfRamp = (IN.color.r) - 0.5;
			float bedRamp = (IN.color.b) - 0.5;
			float ic1 = Greyscale(c1);
			float ic2 = Greyscale(c2);
		 float irock1 = Greyscale(rock1);
		 float irock2 = Greyscale(rock2);
			fixed3 modCol = _Color.rgb * IN.color.a;
		 fixed3 surfCol;
		 float surfi;
		 fixed3 bedCol;
		 float bedi;
			surfi = IntensityBlend(surfCol, c1, ic1, c2, ic2, surfRamp);
		 bedi = IntensityBlend(bedCol, rock1, irock1, rock2, irock2, bedRamp);
		 IntensityBlend(o.Albedo, surfCol, surfi, bedCol, bedi, ramp);

			o.Albedo *= modCol;
			o.Alpha = 1;
		}
		ENDCG
	}

		FallBack "Diffuse"
}
