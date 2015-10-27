using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Blip : MonoBehaviour
{
	#region Variables (public)
	
	[SerializeField]
	private Transform m_pTarget;

	[SerializeField]
	private Texture m_tDefaultBlip;
	[SerializeField]
	private Texture m_tOutOfBordersBlip;

	[SerializeField]
	private ForwardUpdate m_eForwardUpdateFrequency = ForwardUpdate.Once;
	[SerializeField]
	private bool m_bLockScreenRotation = false;
	
	#endregion
	
	#region Variables (private)

	public enum ForwardUpdate
	{
		Once,
		IfOutBorders,
		Always
	}

	static private MiniMap s_pMinimap;

	private RectTransform m_tRectTransform;
	private RawImage m_tRawImage;

	private bool m_bTargetIsLivingBeing = false;
	
	#endregion


	void Start()
	{
		if (s_pMinimap == null)
			s_pMinimap = GameObject.FindObjectOfType<MiniMap>();

		m_tRectTransform = GetComponent<RectTransform>();
		m_tRawImage = GetComponent<RawImage>();
		Vector3 tCurrentScale = m_tRectTransform.localScale;
		m_tRectTransform.localScale = new Vector3(tCurrentScale.x * s_pMinimap.m_fZoom, tCurrentScale.y * s_pMinimap.m_fZoom, 1.0f);

		UpdateForward();
	}
	
	void LateUpdate()
	{
		if (s_pMinimap)
		{
			Vector2 tNewLocalPos = s_pMinimap.GetBlipLocalPosition(m_pTarget.position);

			if (m_eForwardUpdateFrequency != ForwardUpdate.Once)
			{
				Vector2 tScaledSize = m_tRectTransform.rect.size;
				tScaledSize.Scale(m_tRectTransform.localScale);
				Vector2 tClampedPos = s_pMinimap.KeepBlipInBounds(tNewLocalPos, tScaledSize);

				if (tClampedPos != tNewLocalPos)
				{
					m_tRawImage.texture = m_tOutOfBordersBlip;

					Vector2 tTargetDirection = (tNewLocalPos - tClampedPos).normalized;
					m_tRectTransform.up = tTargetDirection;
				}

				else if (m_eForwardUpdateFrequency == ForwardUpdate.Always)
					UpdateForward();

				else
					m_tRawImage.texture = m_tDefaultBlip;

				tNewLocalPos = tClampedPos;
			}

			if (!m_bLockScreenRotation)
				m_tRectTransform.localEulerAngles = s_pMinimap.TransformRotation(m_pTarget.eulerAngles);

			m_tRectTransform.localPosition = tNewLocalPos;
		}

		if (m_bTargetIsLivingBeing && m_pTarget.GetComponentInChildren<LivingBeing>().IsDead)
			Destroy(gameObject);
	}


	#region Methods

	void UpdateForward()
	{
		transform.up = new Vector2(m_pTarget.transform.forward.x, m_pTarget.transform.forward.z);
	}

	static public void ClearMinimapLink()
	{
		s_pMinimap = null;
	}

	#endregion Methods


	#region Getters/Setters

	public Transform Target
	{
		set
		{
			m_pTarget = value;

			if (m_pTarget.GetComponentInChildren<LivingBeing>() != null)
				m_bTargetIsLivingBeing = true;
		}
	}

	public ForwardUpdate ForwardUpdateFrequency
	{
		set { m_eForwardUpdateFrequency = value; }
	}

	#endregion Getters/Setters
}
