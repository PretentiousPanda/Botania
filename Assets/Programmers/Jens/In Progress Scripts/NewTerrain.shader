﻿Shader "__Lab/Terrain"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		 #pragma surface surf Lambert  addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int MAX_COLOR_COUNT = 8;
		const static float EPSILON = 1E-4;


		sampler2D _MainTex;
		sampler2D _NoiseTextures;

		int layerCount;
		float3 baseColors[MAX_COLOR_COUNT];
		float baseTextureScales[MAX_COLOR_COUNT];
		float baseTextureStrenght[MAX_COLOR_COUNT];
		//float baseStartHeights[MAX_COLOR_COUNT];
		//float baseBlends[MAX_COLOR_COUNT];

		//float minHeight;
		//float maxHeight;


		UNITY_DECLARE_TEX2DARRAY(baseTextures);
		//UNITY_DECLARE_TEX2DARRAY(noiseTextures);

		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float inverseLerp(float a, float b, float value)
		{
			return saturate((value - a) / (b - a));
		}

		//triplanar mapping
		float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex) {
			float3 scaledWorldPos = worldPos / scale;
			float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
			float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
			float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
			return xProjection + yProjection + zProjection;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;

			float mainTexStrenght = 1;
			float noiseStrenght = tex2D(_NoiseTextures, IN.uv_MainTex).x*baseTextureStrenght[0];
			float3 colour = tex2D(_MainTex, IN.uv_MainTex)*(1 - noiseStrenght);
			for (int i = 0; i < layerCount; i++) {
				//float drawStrength = inverseLerp();

				//float3 baseColor = baseColors[i] * baseColorStrength[i];
				float3 textureColor = triplanar(float3(IN.worldPos), baseTextureScales[i], blendAxes, i)*noiseStrenght;

				o.Albedo = textureColor + colour;
			}

			//o.Albedo = UNITY_SAMPLE_TEX2DARRAY(noiseTextures, float3(IN.uv_MainTex,0));
		}
		ENDCG
	}
		FallBack "Diffuse"
}
