﻿Shader "MTE/Standard/2 Textures/Diffuse"
{
	Properties
	{
		_Control ("Control (RGBA)", 2D) = "red" {}
		_Splat0 ("Layer 1", 2D) = "white" {}
		_Splat1 ("Layer 2", 2D) = "white" {}

		[Gamma] _Metallic0("Metallic 0", Range(0.0, 1.0)) = 0.0
		[Gamma] _Metallic1("Metallic 1", Range(0.0, 1.0)) = 0.0
		_Smoothness0("Smoothness 0", Range(0.0, 1.0)) = 1.0
		_Smoothness1("Smoothness 1", Range(0.0, 1.0)) = 1.0
	}

	CGINCLUDE
		#pragma surface surf Standard vertex:MTE_SplatmapVert finalcolor:MTE_SplatmapFinalColor finalprepass:MTE_SplatmapFinalPrepass finalgbuffer:MTE_SplatmapFinalGBuffer
		#pragma multi_compile_fog

		struct Input
		{
			float2 tc_Control : TEXCOORD0;
			float4 tc_Splat01 : TEXCOORD1;
			UNITY_FOG_COORDS(2)
		};

		sampler2D _Control;
		float4 _Control_ST;
		sampler2D _Splat0,_Splat1;
		float4 _Splat0_ST,_Splat1_ST;

		#define MTE_STANDARD_SHADER
		#include "UnityPBSLighting.cginc"
		#include "../../MTECommon.hlsl"

		half _Metallic0;
		half _Metallic1;
		half _Smoothness0;
		half _Smoothness1;

		void MTE_SplatmapVert(inout appdata_full v, out Input data)
		{
			UNITY_INITIALIZE_OUTPUT(Input, data);
			data.tc_Control.xy = TRANSFORM_TEX(v.texcoord, _Control);
			data.tc_Splat01.xy = TRANSFORM_TEX(v.texcoord, _Splat0);
			data.tc_Splat01.zw = TRANSFORM_TEX(v.texcoord, _Splat1);
			float4 pos = UnityObjectToClipPos(v.vertex);
			UNITY_TRANSFER_FOG(data, pos);
		}

		void MTE_SplatmapMix(Input IN, half4 defaultAlpha, out half4 splat_control, out half weight, out fixed4 mixedDiffuse)
		{
			splat_control = tex2D(_Control, IN.tc_Control.xy);
			weight = dot(splat_control, half4(1, 1, 1, 1));
			splat_control /= (weight + 1e-3f);

			mixedDiffuse = 0.0f;
			mixedDiffuse += splat_control.r * tex2D(_Splat0, IN.tc_Splat01.xy) * half4(1.0, 1.0, 1.0, defaultAlpha.r);
			mixedDiffuse += splat_control.g * tex2D(_Splat1, IN.tc_Splat01.zw) * half4(1.0, 1.0, 1.0, defaultAlpha.g);
		}

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 splat_control;
			fixed4 mixedDiffuse;
			half weight;
			half4 defaultSmoothness = half4(_Smoothness0, _Smoothness1, 0, 0);
			MTE_SplatmapMix(IN, defaultSmoothness, splat_control, weight, mixedDiffuse);
			o.Albedo = mixedDiffuse.rgb;
			o.Alpha = weight;
			o.Smoothness = mixedDiffuse.a;
			o.Metallic = dot(splat_control, half4(_Metallic0, _Metallic1, 0, 0));
		}

	ENDCG
	
	Category
	{
		Tags
		{
			"Queue" = "Geometry-99"
			"RenderType" = "Opaque"
		}
		SubShader//for target 3.0+
		{
			CGPROGRAM
				#pragma target 3.0
			ENDCG
		}
		SubShader//for target 2.0
		{
			CGPROGRAM
			ENDCG
		}
	}

	Fallback "Diffuse"
	CustomEditor "MTE.MTEShaderGUI"
}
