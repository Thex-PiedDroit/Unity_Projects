Shader "Custom/ConstructionShade"
{
	Properties
	{
		_HolloTex ("Hollogram Tex", 2D) = "white" {}

        _RimCol ("Rim Colour" , Color) = (1,0,0,1)
        _RimWidth ("Rim Width", Range(0,1)) = 1.0
		_PanSpeed ("PanSpeed", float) = 1.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 		LOD 200
 		Blend One One

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 5.0

		struct Input
		{
			half2 uv_HolloTex;
		};

		sampler2D _HolloTex;

		half4 _RimCol;
		half _RimWidth;
		half _PanSpeed;

		void surf (Input IN, inout SurfaceOutput o)
		{
			half timePan = _Time.y * _PanSpeed;
			half crossTex = tex2D(_HolloTex, IN.uv_HolloTex + half2(-timePan, timePan)).r;
			half crossFactor = pow(crossTex, _RimWidth);
			o.Emission = saturate((_RimCol.rgb * crossFactor) + ((_RimCol.rgb * 0.15f) * (1.0f - crossFactor)));
			o.Alpha = 0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
