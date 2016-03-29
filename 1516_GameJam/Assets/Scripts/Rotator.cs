
using UnityEngine;
using System.Collections;



public class Rotator : MonoBehaviour
{
	#region Variables (public)

	public float fRotationSpeed = 1.0f;

	public Vector3 Axis = Vector3.forward;

	#endregion

	#region Variables (private)

	private float fAnglePerSecond;
	private float fRotationDirection = 1.0f;

	private LoadQuitGame tLoadQuitGameScript;

    #endregion
	

	void Start ()
	{
		tLoadQuitGameScript = GameObject.Find("Main Camera").GetComponent<LoadQuitGame>();

		float fPerimeter = (2.0f * Mathf.PI) * (transform.localScale.x * transform.parent.localScale.x);
		fAnglePerSecond = fRotationSpeed / fPerimeter;
		fAnglePerSecond *= 360.0f;
	}
	

	void Update ()
	{
		transform.Rotate(Axis, ((-fAnglePerSecond * Time.deltaTime) * fRotationDirection) * tLoadQuitGameScript.TimeScale);
	}


	public float RotationDirection
	{
		get { return fRotationDirection; }
		set
		{
			//if (value != fRotationDirection)
			//	fAnglePerSecond *= -1.0f;
			
			fRotationDirection = value;
		}
	}


	public void RandomRotation()
	{
		float fRandRotation = Random.Range(-360.0f, 360.0f);

		transform.Rotate(Axis, -fRandRotation);
	}
}
