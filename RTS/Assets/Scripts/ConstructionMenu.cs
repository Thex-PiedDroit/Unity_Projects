
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ConstructionMenu : MonoBehaviour
{
#region Variables (public)

	static public ConstructionMenu Instance = null;

	public float m_fSpaceBetweenItems = 40.0f;
	public List<ConstructionMenuItem> m_pItems = null;

	#endregion

#region Variables (private)
	
	

    #endregion

	void Awake()
	{
		if (Instance != null)
		{
			if (Instance != this)
				Destroy(this);
			return;
		}

		Instance = this;
	}

	void Start()
	{
		InitPanel();
	}	

	public void InitPanel()
	{
		Dictionary<EBuildingType, BuildingData> pAllBuildings = BuildingsManager.Instance.BuildingsData;

		int iCurrentItemsCount = m_pItems.Count;
		for (int i = iCurrentItemsCount - 1; i >= pAllBuildings.Count; i--)
		{
			Destroy(m_pItems[i].gameObject);
			m_pItems.RemoveAt(i);
		}

		RectTransform tInitItemRectTransform = m_pItems[0].GetComponent<RectTransform>();
		Vector2 tInitPos = tInitItemRectTransform.anchoredPosition;
		float fSizeX = tInitItemRectTransform.rect.width;

		int iInitializedItems = 0;

		foreach(KeyValuePair<EBuildingType, BuildingData> pData in pAllBuildings)
		{
			if (iInitializedItems >= m_pItems.Count - 1)
			{
				GameObject pNew = GameObject.Instantiate(m_pItems[0].gameObject);
				pNew.transform.SetParent(transform, false);
				Vector2 tPos = tInitPos;
				tPos.x += (fSizeX + m_fSpaceBetweenItems) * iInitializedItems;
				pNew.GetComponent<RectTransform>().anchoredPosition = tPos;

				m_pItems.Add(pNew.GetComponent<ConstructionMenuItem>());
			}

			m_pItems[iInitializedItems].Init(pData.Value);
			iInitializedItems++;
		}
	}
}
