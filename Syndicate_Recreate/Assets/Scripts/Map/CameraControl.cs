using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fCameraSpeed = 60.0f;

	[Header("Shaker")]
	[SerializeField]
	private float m_fShakeSpeed = 5.0f;
	[SerializeField]
	private Vector3 m_tShakeRange = new Vector3(2.0f, 2.0f, 2.0f);
	[SerializeField]
	private float m_fShakeDuration = 0.2f;
	
	#endregion
	
	#region Variables (private)

	private Vector3 m_tMoveForward;

	private Vector3 m_tStartShakePos = Vector3.zero;
	private float m_fShakeTimer = 0.0f;
	private float m_fCurrentShakeSpeed = 50.0f;
	private bool m_bShaking = false;
	
	#endregion


	void Awake()
	{
		m_tMoveForward = transform.forward;
		m_tMoveForward.y = 0.0f;
		m_tMoveForward.Normalize();

		m_tStartShakePos = transform.position;
		m_fCurrentShakeSpeed = m_fShakeSpeed;
	}
	
	void Update()
	{
		float fVertical = Input.GetAxis("Vertical");
		float fHorizontal = Input.GetAxis("Horizontal");

		if (fVertical != 0.0f || fHorizontal != 0.0f)
		{
			Vector3 tMove = ((m_tMoveForward * fVertical) + (transform.right * fHorizontal)) * m_fCameraSpeed;

			if (fVertical != 0.0f && fHorizontal != 0.0f)
				tMove = tMove.normalized * m_fCameraSpeed;

			tMove *= Time.deltaTime;
			m_tStartShakePos += tMove;
		}

		transform.position = Vector3.Lerp(transform.position, m_tStartShakePos, m_fCameraSpeed * Time.deltaTime);


		if (m_bShaking)
		{
			if ((Time.fixedTime - m_fShakeTimer) >= m_fShakeDuration)
			{
				m_fShakeTimer = 0.0f;
				m_bShaking = false;
			}

			else
			{
				transform.position = m_tStartShakePos + Vector3.Scale(SmoothRandom.GetVector3(m_fCurrentShakeSpeed--), m_tShakeRange);
				m_fCurrentShakeSpeed *= -1.0f;
			}
		}
	}

	public static void StartShaking()
	{
		CameraControl pMainCameraControl = Camera.main.GetComponent<CameraControl>();

		pMainCameraControl.m_bShaking = true;
		pMainCameraControl.m_fShakeTimer = Time.fixedTime;
		pMainCameraControl.m_fCurrentShakeSpeed = pMainCameraControl.m_fShakeSpeed;
	}


	static public Vector3 HorizontalForward
	{
		get { return Camera.main.gameObject.GetComponent<CameraControl>().m_tMoveForward; }
	}
}
