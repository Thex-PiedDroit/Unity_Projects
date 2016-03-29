
using UnityEngine;
using System.Collections;

public class PlanetBehavior : MonoBehaviour
{
	#region Variables (public)



	#endregion

	#region Variables (private)



	#endregion

	void Start ()
	{
		float fRotationDirection = 1.0f;
		fRotationDirection = Random.Range(0, 2);
		if (fRotationDirection == 0)
			fRotationDirection = -1;

		GetComponent<Rotator>().RotationDirection = fRotationDirection;
		GameObject tOrbit = transform.parent.gameObject.transform.FindChild("Orbit").gameObject;
		tOrbit.GetComponent<Rotator>().RotationDirection = fRotationDirection;
		print(transform.parent.gameObject.name);
		print(fRotationDirection);
		print(GetComponent<Rotator>().RotationDirection);
		print(tOrbit.GetComponent<Rotator>().RotationDirection);
	}


	void Update ()
	{
		
	}


	public float RotationDirection
	{
		get { return GetComponent<Rotator>().RotationDirection; }
	}
}
