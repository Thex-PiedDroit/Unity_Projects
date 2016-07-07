using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Light_binding : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private Color m_tAmbient;
	
	#endregion
	
	#region Variables (private)

	Light tLight = null;
	
	#endregion


	// Use this for initialization
	void Start ()
	{
		tLight = transform.GetComponent<Light>();
		UpdateLight();
	}
	
#if UNITY_EDITOR
	// Update is called once per frame
	void Update ()
	{
		UpdateLight();
	}

#endif


	void UpdateLight()
	{
		Color tSunColor = tLight.color;
		float fSunIntensity = tLight.intensity;

		Color tPremulSunColor;
		Color tModifiedAmbient;

		if (QualitySettings.activeColorSpace == ColorSpace.Linear)
		{
			tPremulSunColor = tSunColor.linear * fSunIntensity;
			tModifiedAmbient = m_tAmbient.linear;
		}

		else
		{
			tPremulSunColor = tSunColor * fSunIntensity;
			tModifiedAmbient = m_tAmbient;
		}

		Shader.SetGlobalColor("_SunColor", tPremulSunColor);
		Shader.SetGlobalVector("_SunDir", transform.forward);
		Shader.SetGlobalColor("_Ambient", tModifiedAmbient);
	}
}
