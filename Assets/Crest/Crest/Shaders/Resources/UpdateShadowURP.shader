// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

Shader "Hidden/Crest/Simulation/Update Shadow URP"
{
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			// #pragma enable_d3d11_debug_symbols

			// maybe this is the equivalent of the SHADOW_COLLECTOR_PASS define? inspired from com.unity.render-pipelines.universal\Shaders\Utils\ScreenSpaceShadows.shader
			#define _MAIN_LIGHT_SHADOWS_CASCADE
			#define MAIN_LIGHT_CALCULATE_SHADOWS

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			#define CREST_SAMPLE_SHADOW_HARD

			#include "../ShaderLibrary/UpdateShadow.hlsl"

			half CrestSampleShadows(const float4 i_positionWS)
			{
				// Includes soft shadows if _SHADOWS_SOFT is defined (requires multi-compile pragma).
				return MainLightRealtimeShadow(TransformWorldToShadowCoord(i_positionWS.xyz));
			}

			half CrestComputeShadowFade(const float4 i_positionWS)
			{
				return GetShadowFade(i_positionWS.xyz);
			}

			Varyings Vert(Attributes input)
			{
				Varyings o;

				o.positionCS = TransformObjectToHClip(input.positionOS);

				// World position from [0,1] quad.
				o.positionWS.xyz = float3(input.positionOS.x - 0.5, 0.0, input.positionOS.y - 0.5) * _Scale * 4.0 + _CenterPos;
				o.positionWS.y = _OceanCenterPosWorld.y;

				return o;
			}

			ENDHLSL
		}
	}
}
