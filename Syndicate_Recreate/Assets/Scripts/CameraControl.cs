using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	float m_fCameraSpeed = 60.0f;
	
	#endregion
	
	#region Variables (private)

	private Vector3 m_tMoveForward;
	private Vector3 m_tMoveRight;
	
	#endregion


	void Start ()
	{
		m_tMoveForward = transform.forward;
		m_tMoveForward.y = 0.0f;
		m_tMoveForward.Normalize();

		m_tMoveRight = transform.right;
		m_tMoveRight.y = 0.0f;
		m_tMoveRight.Normalize();
	}
	
	void Update ()
	{
		float fVertical = Input.GetAxis("Vertical");
		float fHorizontal = Input.GetAxis("Horizontal");

		if (fVertical != 0.0f || fHorizontal != 0.0f)
		{
			Vector3 tMove = ((m_tMoveForward * fVertical) + (m_tMoveRight * fHorizontal)) * m_fCameraSpeed;

			if (fVertical != 0.0f && fHorizontal != 0.0f)
				tMove = tMove.normalized * m_fCameraSpeed;

			transform.Translate(tMove * Time.deltaTime, Space.World);
		}
	}
}
