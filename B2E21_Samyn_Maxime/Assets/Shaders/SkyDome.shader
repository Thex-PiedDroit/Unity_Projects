Shader "HEAJ/SkyDome"
{
	Properties
	{
		
	}

	SubShader
	{
		Tags {"Queue" = "Transparent"}
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Back

			//Tags {"LighMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			sampler3D _CloudTex;
			int _CloudsLoopIterations;
			half _SegmentsSize, _MinCloudsAlpha;
			half3 _CloudColor;
			half4 _CloudSpherePos_wRadius;

			struct appData
			{
				half4 vertex	: POSITION;
			};

			struct v2f
			{
				half4 pos		: SV_POSITION;
				half3 wPos		: ATTR0;
			};

			/*	External resource -> Source = http://www.cse.chalmers.se/~uffe/xjobb/Robert%20Larsson-Interactive%20Real-Time%20Smoke%20Rendering.pdf	*/
			bool RaySphereIntersect(float3 rO, float3 rD, float3 sO, float sR, out float tnear, out float tfar)
			{
			    float3 delta = rO - sO;
			    float A = dot(rD, rD);
			    float B = 2 * dot(delta, rD);
			    float C = dot(delta, delta) - sR*sR;
			    float disc = (B*B - 4.0*A*C);
			    if (disc < 0.01)
			    {
			        return false;
			    }
			    else
			    {
			        float sqrtDisc = sqrt(disc);
			        tnear = (-B - sqrtDisc) / (2 * A);
			        tfar = (-B + sqrtDisc) / (2 * A);
			        return true;
			    }
			}

			float GetCloudFactor(half3 wPos, half3 L, out float cloudDepth, out float cloudDepthFactor)
			{
				half cloudFactor = 0;

				half3 rayPos = _WorldSpaceCameraPos;
				half3 rayDir = normalize(wPos - _WorldSpaceCameraPos);

				cloudDepth = 0.0f;
				cloudDepthFactor = 0.0f;

				half rayEntry = 0;
				half rayExit = 0;
				if (RaySphereIntersect(rayPos, rayDir, _CloudSpherePos_wRadius.xyz, _CloudSpherePos_wRadius.w, rayEntry, rayExit) && rayEntry > 0)
				{
					rayPos = _WorldSpaceCameraPos + (rayDir * rayEntry);
					half3 sphereNormal = normalize(rayPos - _CloudSpherePos_wRadius.xyz);
					half rayDotNormal = dot(rayDir, sphereNormal);

					half3 wPosToL = normalize(wPos - L);
					cloudDepthFactor = saturate(dot(sphereNormal, L) + 1);

					if (rayDotNormal * rayDotNormal > 0.05f)
					{
						[unroll(100)]
						for (int i = 0; i < _CloudsLoopIterations; i++)
						{
							half3 localPos = rayPos - _CloudSpherePos_wRadius.xyz;
							
							half3 cloudUV = saturate((localPos / (_CloudSpherePos_wRadius.w + 2.0f)) + 0.5f);
							half texAlpha = tex3D(_CloudTex, cloudUV).a;
							cloudFactor += texAlpha;

							
							cloudDepth += texAlpha;

							rayPos += rayDir * _SegmentsSize;
						}
					}
				}

				cloudFactor = smoothstep(cloudFactor, 0, _MinCloudsAlpha);

				return saturate(cloudFactor);
			}

			v2f vert(appData v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.wPos = mul(_Object2World, v.vertex).xyz;

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 o;

				half cloudDepth = 0.0f;
				half cloudDepthFactor = 0.0f;
				half cloudFactor = GetCloudFactor(i.wPos, -_WorldSpaceLightPos0, cloudDepth, cloudDepthFactor);
				half cloudMult = sqrt(cloudDepth) * cloudDepthFactor;
				half3 cloudColor = (_CloudColor * 0.5f) * max(0.05f, cloudMult);	// Cloud color * 0.5 to darken it a bit (too bright otherwise)
				//half3 color = cloudFactor * _CloudColor;

				o.rgb = cloudColor;
				o.a = cloudFactor;

				return o;
			}

			ENDCG
		}
	}
}
