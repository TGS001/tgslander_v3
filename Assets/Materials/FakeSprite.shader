Shader "Custom/FakeSprite" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Toggle(VCM_ON)] _VCM("Vertex Color Multiply", int) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Anime fullforwardshadows
		#pragma shader_feature VCM_ON 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			#if VCM_ON
			half4 color : COLOR;
			#endif
		};

		fixed4 _Color;

		half4 LightingAnime (SurfaceOutput s, half3 lightDir, half atten) {
			half3 rLightDir = normalize(half3(lightDir.x, lightDir.y, -0.5));
			half ndotr = dot(s.Normal, rLightDir);
			half diff = (ndotr * 0.25 + 0.5);

			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * atten * diff;

			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			#if VCM_ON
			o.Albedo = c.rgb * IN.color;
			#else
			o.Albedo = c.rgb;
			#endif
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	FallBack "Diffuse"
}
