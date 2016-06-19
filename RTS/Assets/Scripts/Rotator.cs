
using UnityEngine;
using System.Collections;



public class Rotator : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fRotationSpeed = 180.0f;
	[SerializeField]
	private bool m_bRandomDirection = false;
	[SerializeField]
	private Vector3 Axis = Vector3.forward;

	#endregion

	#region Variables (private)

	private float fAnglePerSecond;
	private float fRotationDirection = 1.0f;

    #endregion
	

	void Start ()
	{
		if (m_bRandomDirection && (Random.Range(0, 2) == 1))
			m_fRotationSpeed *= -1.0f;
	}
	

	void FixedUpdate ()
	{
		transform.Rotate(Axis, -m_fRotationSpeed * Time.deltaTime);
	}


	public float RotationDirection
	{
		get { return fRotationDirection; }
		set
		{
			if (value != fRotationDirection)
				fAnglePerSecond *= -1.0f;
			
			fRotationDirection = value;
		}
	}


	public void RandomRotation()
	{
		float fRandRotation = Random.Range(-360.0f, 360.0f);

		transform.Rotate(Axis, -fRandRotation);
	}
}
