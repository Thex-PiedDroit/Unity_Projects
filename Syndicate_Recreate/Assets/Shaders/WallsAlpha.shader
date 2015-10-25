Shader "Custom/WallsAlpha"
{
	Properties
	{
		_MatColor ("First color", Color) = (1,1,1,1)
		_Ambient ("Ambient", Color) = (.2, .2, .2, 1)
		_Gloss ("Glossiness", Range(0, 1)) = .5
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 5.0

			float3 _MatColor, _SunDir, _SunColor, _Ambient;
			float _Gloss;
			
			struct TAppData
			{
				float4 tPos		: POSITION;
				float3 tNormal	: NORMAL;
			};

			struct TV2F
			{
				float4 tPos			: SV_POSITION;
				float3 tNormal		: ATTR0;
				float3 tWorldPos	: ATTR1;
			};

			TV2F Vert(TAppData tVertex)
			{
				TV2F tOutput;
				tOutput.tPos = mul(UNITY_MATRIX_MVP, tVertex.tPos);
				tOutput.tNormal = mul(_Object2World, float4(tVertex.tNormal, 0)).xyz;
				tOutput.tWorldPos = mul(_Object2World, tVertex.tPos).xyz;

				return tOutput;
			}

			float4 Frag(TV2F tInfo) : COLOR
			{
				float4 tOutput;
				float3 tNormal = normalize(tInfo.tNormal);
				float3 tLight = -_SunDir;
				float3 tView = normalize(_WorldSpaceCameraPos - tInfo.tWorldPos);
				float3 tSaturation = normalize(tView + tLight);

				float NdotL = saturate(dot(tNormal, tLight));
				float NdotS = saturate(dot(tNormal, tSaturation));
				float e = exp2(_Gloss * 12.0f);

				float3 tDiffuse = NdotL * _MatColor;
				float3 tSpecular = (NdotL * pow(NdotS, e)) * ((e * 0.125f) + 0.75f);
				tSpecular *= float3(0.05f, 0.05f, 0.05f);	// Take only 5% of specular color for smoothness

				tOutput.rgb = ((tDiffuse + tSpecular) * _SunColor) + (_Ambient * _MatColor);
				tOutput.a = 1;

				return tOutput;
			}

			ENDCG
		}

		pass
		{
			ZTest Greater

			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma target 5.0

			float3 _MatColor;
			
			struct TAppData
			{
				float4 tPos		: POSITION;
			};

			struct TV2F
			{
				float4 tPos			: SV_POSITION;
			};

			TV2F Vert(TAppData tVertex)
			{
				TV2F tOutput;
				tOutput.tPos = mul(UNITY_MATRIX_MVP, tVertex.tPos);

				return tOutput;
			}

			float4 Frag(TV2F tInfo) : COLOR
			{
				float4 tOutput;
				tOutput.rgb = _MatColor;
				tOutput.a = 0.5f;

				return tOutput;
			}

			ENDCG
		}
	}
}
