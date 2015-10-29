using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Light_binding : MonoBehaviour
{
	#region Variables (public)
	
	
	
	#endregion
	
	#region Variables (private)
	
	
	
	#endregion


	void Start ()
	{
		UpdateLight();
	}
	
#if UNITY_EDITOR

	void Update ()
	{
		UpdateLight();
	}

#endif


	void UpdateLight()
	{
		Shader.SetGlobalColor("_SunColor", GetComponent<Light>().color * GetComponent<Light>().intensity);
		Shader.SetGlobalVector("_SunDir", transform.forward);
		Shader.SetGlobalFloat("_Bounce", GetComponent<Light>().bounceIntensity);
	}
}
