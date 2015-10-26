using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMap : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private Transform m_pTarget;

	[SerializeField]
	private GameObject tBlueBlip;
	[SerializeField]
	private GameObject tRedBlip;

	[SerializeField]
	public float m_fZoom = 2.0f;
	[SerializeField]
	private Vector2 m_tBorderOffset = new Vector2(4.0f, 4.0f);
	
	#endregion
	
	#region Variables (private)

	private Rect m_tBoundaries;

	private GameObject[] pLivingBeings;
	private List<GameObject> pBlips = new List<GameObject>();
	
	#endregion


	void Start()
	{
		m_tBoundaries = GetComponent<RectTransform>().rect;

		LivingBeing[] pLivingBeingsComponents = GameObject.FindObjectsOfType<LivingBeing>();
		pLivingBeings = new GameObject[pLivingBeingsComponents.Length];

		for (int i = 0; i < pLivingBeingsComponents.Length; i++)
		{
			pLivingBeings[i] = pLivingBeingsComponents[i].gameObject;

			switch(pLivingBeings[i].transform.parent.name)
			{
			case "GoodGuys":

				pBlips.Add(Instantiate<GameObject>(tBlueBlip));
				pBlips[i].name = "BlueBlip";
				break;

			case "BadGuys":

				pBlips.Add(Instantiate<GameObject>(tRedBlip));
				pBlips[i].name = "RedBlip";
				break;
			}

			pBlips[i].transform.SetParent(transform, false);
			pBlips[i].transform.localPosition = Vector2.zero;
			pBlips[i].GetComponent<Blip>().Target = pLivingBeings[i].transform;
		}
	}


	#region Methods

	public Vector2 GetBlipLocalPosition(Vector3 tWorldPos)
	{
		Vector3 tWorldLocalPos = tWorldPos - m_pTarget.position;
		Vector2 tLocalPos = new Vector2(tWorldLocalPos.x, tWorldLocalPos.z);
		tLocalPos *= m_fZoom;

		return tLocalPos;
	}

	public Vector2 KeepBlipInBounds(Vector2 tBlipPos, Vector2 tScaledSize)
	{
		tBlipPos = Vector2.Max(tBlipPos, m_tBoundaries.min + tScaledSize - m_tBorderOffset);
		tBlipPos = Vector2.Min(tBlipPos, m_tBoundaries.max - tScaledSize + m_tBorderOffset);

		return tBlipPos;
	}

	#endregion Methods
}
