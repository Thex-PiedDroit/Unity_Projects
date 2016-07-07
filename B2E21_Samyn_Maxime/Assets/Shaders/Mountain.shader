Shader "HEAJ/Mountain"
{
	Properties
	{
		_RockTex_RGB_nX ("RockTex_RGB_nX", 2D) = "" {}
		[NoScaleOffset]	_RockTex_MGC_nY ("RockTex_MGC_nY", 2D) = "" {}
		_SnowTex_RGB_nX ("SnowTex_RGB_nX", 2D) = "" {}
		[NoScaleOffset]	_SnowTex_MGC_nY ("SnowTex_MGC_nY", 2D) = "" {}
		_GrassTex_RGB_nX ("GrassTex_RGB_nX", 2D) = "" {}
		[NoScaleOffset]	_GrassTex_MGC_nY ("GrassTex_MGC_nY", 2D) = "" {}
		_DirtTex_RGB_nX ("DirtTex_RGB_nX", 2D) = "" {}
		[NoScaleOffset]	_DirtTex_MGC_nY ("DirtTex_MGC_nY", 2D) = "" {}
		_StratumLUT ("StratumLUT", 2D) = "" {}
		_StratumColor ("StratumColor", Color) = (0.5,0.5,0.5,1)
		_StratumAccentuation ("StratumAccentuation", float) = 1.5

		_RockScale ("RockScale", float) = 1

		_DirtSlope ("EarthSlope", float) = 1
		_DirtAltitudeMax ("DirtAltitudeMax", float) = 2
		_DirtSpread ("DirtSpread", float) = 1

		_GrassSlope ("GrassSlope", float) = 1
		_GrassAltitudeMax ("GrassAltitudeMax", float) = 1.5
		_GrassSpread ("GrassSpread", float) = 1

		_SnowSlope ("SnowSlope", float) = 1
		_SnowAltitudeMin ("SnowAltitudeMin", float) = 1
		_SnowSpread ("SnowSpread", float) = 1

		_FogDistance ("FogDistance", float) = 2.0
		_FogColor ("FogColor", Color) = (0.5,0.5,0.5,1)
		_FogMaxHeight ("FogMaxHeight", float) = 1.0
	}

	SubShader
	{
		Tags {"RenderType" = "Opaque"}
		Pass
		{
			Tags {"LighMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"


			sampler2D _RockTex_RGB_nX, _SnowTex_RGB_nX, _GrassTex_RGB_nX, _DirtTex_RGB_nX, _StratumLUT;
			sampler2D _RockTex_MGC_nY, _SnowTex_MGC_nY, _GrassTex_MGC_nY, _DirtTex_MGC_nY;
			sampler3D _CloudTex;
			half _DirtSlope, _DirtAltitudeMax, _DirtSpread, _GrassSlope, _GrassAltitudeMax, _GrassSpread, _SnowSlope, _SnowAltitudeMin, _SnowSpread, _FogDistance, _FogMaxHeight, _StratumAccentuation;
			int _CloudsLoopIterations;
			half _SegmentsSize, _MinCloudsAlpha;
			half3 _CloudColor;
			half4 _CloudSpherePos_wRadius;
			half4 _StratumColor, _FogColor;
			half4 _RockTex_RGB_nX_ST, _SnowTex_RGB_nX_ST, _GrassTex_RGB_nX_ST, _DirtTex_RGB_nX_ST, _StratumLUT_ST;

			struct appData
			{
				half4 vertex	: POSITION;
				half3 normal	: NORMAL;
				half4 tangent	: TANGENT;
				half2 texCoord	: TEXCOORD0;
			};

			struct v2f
			{
				half4 pos		: SV_POSITION;
				half4 T_wPosX	: ATTR0;
				half4 B_wPosY	: ATTR1;
				half4 N_wPosZ	: ATTR2;
				half3 UV_fog	: ATTR3;

				LIGHTING_COORDS (4, 5)
				UNITY_FOG_COORDS(1)
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
				o.N_wPosZ.xyz = normalize(mul(_Object2World, float4(v.normal,0)).xyz);
				o.T_wPosX.xyz = normalize(mul(_Object2World, half4(v.tangent.xyz,0)).xyz);
				o.B_wPosY.xyz = cross(o.N_wPosZ.xyz, o.T_wPosX.xyz) * v.tangent.w;

				float3 wPos = mul(_Object2World, v.vertex).xyz;
				o.T_wPosX.w = wPos.x;
				o.B_wPosY.w = wPos.y;
				o.N_wPosZ.w = wPos.z;

				o.UV_fog.xy = v.texCoord;
				o.UV_fog.z = saturate(distance(wPos, _WorldSpaceCameraPos) / _FogDistance);
				o.UV_fog.z *= saturate(_FogMaxHeight - wPos.y);
				o.UV_fog.z *= o.UV_fog.z;

				half2 _LutUV = wPos.y;

				UNITY_TRANSFER_FOG(o, o.pos);
				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 o;
				half3 wPos = float3(i.T_wPosX.w,i.B_wPosY.w,i.N_wPosZ.w);

				// Tri-planar texturing for rock
				half3 blending = abs(wPos);
				blending = normalize(max(blending, 0.00001));
				half b = (blending.x + blending.y + blending.z);
				blending /= half3(b, b, b);	// normalizing all values

				half4 rockTexX = tex2D(_RockTex_RGB_nX, (wPos.yz * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockTexY = tex2D(_RockTex_RGB_nX, (wPos.xz * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockTexZ = tex2D(_RockTex_RGB_nX, (wPos.xy * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockTex = (rockTexX * blending.x) + (rockTexY * blending.y) + (rockTexZ * blending.z);

				half4 rockMGCX = tex2D(_RockTex_MGC_nY, (wPos.yz * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockMGCY = tex2D(_RockTex_MGC_nY, (wPos.xz * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockMGCZ = tex2D(_RockTex_MGC_nY, (wPos.xy * _RockTex_RGB_nX_ST.xy) + _RockTex_RGB_nX_ST.wz);
				half4 rockMGC = (rockMGCX * blending.x) + (rockMGCY * blending.y) + (rockMGCZ * blending.z);

				// Other textures
				half4 snowTex = tex2D(_SnowTex_RGB_nX, (i.UV_fog.xy * _SnowTex_RGB_nX_ST.xy) + _SnowTex_RGB_nX_ST.wz);
				half4 snowMGC = tex2D(_SnowTex_MGC_nY, (i.UV_fog.xy * _SnowTex_RGB_nX_ST.xy) + _SnowTex_RGB_nX_ST.wz);
				half4 grassTex = tex2D(_GrassTex_RGB_nX, (i.UV_fog.xy * _GrassTex_RGB_nX_ST) + _GrassTex_RGB_nX_ST.wz);
				half4 grassMGC = tex2D(_GrassTex_MGC_nY, (i.UV_fog.xy * _GrassTex_RGB_nX_ST) + _GrassTex_RGB_nX_ST.wz);
				half4 dirtTex = tex2D(_DirtTex_RGB_nX, (i.UV_fog.xy * _DirtTex_RGB_nX_ST.xy) + _DirtTex_RGB_nX_ST.wz);
				half4 dirtMGC = tex2D(_DirtTex_MGC_nY, (i.UV_fog.xy * _DirtTex_RGB_nX_ST.xy) + _DirtTex_RGB_nX_ST.wz);
				half2 noiseLutUV = wPos.x * (wPos.yx * _StratumLUT_ST.yx) * 0.2;	// * 0.2 to scale down the noise effect
				half4 noiseLUTTex = tex2D(_StratumLUT, noiseLutUV + _StratumLUT_ST.wz);
				half2 stratumLutUV = wPos.y * (wPos.yx * _StratumLUT_ST.yx);
				stratumLutUV.x += (noiseLUTTex.r - 0.5) * 0.2;
				half4 stratumLUTTex = tex2D(_StratumLUT, stratumLutUV + _StratumLUT_ST.wz);

				half2 Nxy = (half2(dirtTex.w, dirtMGC.w) * 2) - 1;
				half3 N = normalize(i.B_wPosY.xyz * Nxy.y + (i.T_wPosX.xyz * Nxy.x + i.N_wPosZ.xyz));
				half3 L = -_WorldSpaceLightPos0;
				half3 V = normalize(_WorldSpaceCameraPos - wPos);
				half3 H = normalize(V + L);
				half3 R = reflect(-V,N);

				half NdotL = saturate(dot(N,L));
				half NdotH = saturate(dot(N,H));
				half LdotH = saturate(dot(L,H));
				half NdotV = saturate(dot(N,V));


				// Helpers
				half currentSlope = N.y;
				half currentAltitude = wPos.y;


				/*		CLOUDS		*/
				half cloudDepth = 0.0f;
				half cloudDepthFactor = 0.0f;
				half cloudFactor = GetCloudFactor(wPos, L, cloudDepth, cloudDepthFactor);


				/*		COLORS		*/
				half stratumColor = _StratumColor * stratumLUTTex.r;
				half3 baseColor = (rockTex + (stratumColor * _StratumAccentuation)) * 0.5;
				half3 MGC = rockMGC;

				// Dirt
				half dirtAltitude = saturate(_DirtAltitudeMax - currentAltitude);
				half dirtSlope = saturate(pow(currentSlope, _DirtSlope) * _DirtSpread);
				half dirtFactor = dirtAltitude * dirtSlope;
				baseColor = lerp(baseColor, dirtTex, dirtFactor);
				MGC = lerp(MGC, dirtMGC, dirtFactor);

				// Grass
				half grassAltitude = saturate(_GrassAltitudeMax - currentAltitude);
				half grassSlope = saturate(pow(currentSlope, _GrassSlope) * _GrassSpread);
				half grassFactor = grassAltitude * grassSlope;

				half grassCurve = currentAltitude / _GrassAltitudeMax;
				grassCurve *= (grassCurve * 2);
				half3 grassYellow = half3(1,1,0) * grassCurve;
				half3 grassColor = (((grassTex + grassYellow) * 0.5) * grassAltitude);
				baseColor = lerp(baseColor, grassColor, grassFactor);
				MGC = lerp(MGC, grassMGC, grassFactor);

				// Snow
				half snowAltitude = saturate(currentAltitude - _SnowAltitudeMin);
				half snowSlope = saturate(pow(currentSlope, _SnowSlope) * _SnowSpread);
				half snowFactor = snowAltitude * snowSlope;
				baseColor = lerp(baseColor, snowTex, snowFactor);
				MGC = lerp(MGC, snowMGC, snowFactor);


				half gloss = MGC.g;
				half metalness = MGC.r;
				half e = exp2(gloss * 12);


				/*		PBR CALCULATIONS	*/

				half3 diffuse = NdotL * baseColor * (1 - metalness);

				half3 specDistrib = NdotL * pow(NdotH, e) * ((e * 0.125f) + 1);

				half3 minSpec = half3(0.05, 0.05, 0.05);
				half3 F0 = (metalness * (baseColor - minSpec)) + minSpec;
				half3 invF0 = 1 - F0;
				half fCurve = 1 - LdotH;
				fCurve *= fCurve;
				fCurve *= fCurve;
				half3 fresnel = fCurve * (invF0) + F0;


				half envCurve = 1-NdotV;
				envCurve*=envCurve;
				envCurve*=envCurve;
				half3 envFresnel = envCurve * (invF0) + F0;

				half geoVis = NdotV * (1 - gloss) + gloss;
				half3 specular = specDistrib * fresnel * geoVis;
				
				half mipOffset = 1 - saturate(gloss + envCurve);
				half4 envData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, R, mipOffset * 7);
				half3 env = DecodeHDR(envData, unity_SpecCube0_HDR);
				

				half3 ambSpec = env * envFresnel * geoVis;
			
				half4 ambData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, N, 7);
				half3 ambient = DecodeHDR ( ambData, unity_SpecCube0_HDR) * baseColor * (1 - metalness);

						
				half3 result = (((diffuse + specular) * _LightColor0) * LIGHT_ATTENUATION(i)) + ambSpec + ambient;
				result = lerp(result, _FogColor, i.UV_fog.z);

				half cloudMult = sqrt(cloudDepth) * cloudDepthFactor;
				half3 cloudColor = (_CloudColor * 0.5f) * max(0.05f, cloudMult);	// Cloud color * 0.5 to darken it a bit (too bright otherwise)
				result = lerp(result, cloudColor, cloudFactor);
				
				o.rgb = result;
				o.a = 1;

				return o;
			}

			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
			Tags {"LightMode" = "ShadowCaster"}
			Cull Back
			Offset 0,-4
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0

			#define UNITY_PASS_SHADOWCASTER
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#pragma multi_compile_shadowcaster

			struct appData
			{
				half4 vertex	: POSITION;
			};

			struct v2f
			{
				V2F_SHADOW_CASTER;
			};

			v2f vert(appData v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				TRANSFER_SHADOW_CASTER(o)
				return o;
			};

			half4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
