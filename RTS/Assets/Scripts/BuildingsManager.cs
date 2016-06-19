
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;



public class BuildingsManager : MonoBehaviour
{
#region Variables (public)

	static public BuildingsManager Instance = null;
	
	public Dictionary<EBuildingType, BuildingData> BuildingsData
	{
		get { return m_pBuildingsData; }
	}

	public bool DraggingBuildingShade
	{
		get { return m_bDraggingBuildingShade; }
	}
	
	#endregion
	
#region Variables (private)

	private List<Building> m_pBuildingsPrefabs = null;
	private List<BuildingShade> m_pBuildingShadesPrefabs = null;
	private Dictionary<EBuildingType, BuildingData> m_pBuildingsData = null;

	private BuildingShade m_pCurrentBuildingShade = null;
	private bool m_bDraggingBuildingShade = false;
	private bool m_bDraggingCancelled = false;
	
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

		Initialize();
	}
	
	void Initialize()
	{
		Building[] pBuildings = Resources.LoadAll<Building>("Buildings");
		m_pBuildingsPrefabs = new List<Building>(pBuildings.Length);
		BuildingShade[] pBuildingShades = Resources.LoadAll<BuildingShade>("Buildings/ConstructionShades");
		m_pBuildingShadesPrefabs = new List<BuildingShade>(pBuildings.Length);
		for (int i = 0; i < pBuildings.Length; i++)
		{
			m_pBuildingsPrefabs.Add(pBuildings[i]);
			m_pBuildingShadesPrefabs.Add(pBuildingShades[i]);
		}		

		m_pBuildingsData = new Dictionary<EBuildingType, BuildingData>(m_pBuildingsPrefabs.Count);

		JSONObject tJSONData = new JSONObject(File.ReadAllText(Application.dataPath + "/Resources/Data/Buildings.json"));

		for (int i = 0; i < tJSONData.list[0].Count; i++)
		{
			BuildingData pData = new BuildingData();

			pData.m_eBuildingType = ToolKit.ToEnum<EBuildingType>(tJSONData.list[0].keys[i], EBuildingType.NONE);

			for (int j = 0; j < tJSONData.list[0].list[i].keys.Count; j++)
			{
				switch (tJSONData.list[0].list[i].keys[j])
				{
					case "PublicName": pData.m_pPublicName = tJSONData.list[0].list[i].list[j].str; break;
					case "AssetsName": pData.m_pAssetsName = tJSONData.list[0].list[i].list[j].str; break;
					case "Health": pData.m_fHealth = tJSONData.list[0].list[i].list[j].f; break;
				}
			}

			m_pBuildingsData.Add(pData.m_eBuildingType, pData);
		}
	}

	void Update()
	{
		if (m_bDraggingBuildingShade && Input.GetButtonDown("Move"))
		{
			DestroyCurrentShade();

			StopAllCoroutines();
			m_bDraggingCancelled = true;
		}

		if (m_bDraggingCancelled && Input.GetButtonUp("Move"))
		{
			m_bDraggingBuildingShade = false;
			m_bDraggingCancelled = false;
		}
	}


	void DestroyCurrentShade()
	{
		Destroy(m_pCurrentBuildingShade.gameObject);
		m_pCurrentBuildingShade = null;
	}

	public IEnumerator ShowBuildingShade(EBuildingType eBuildingType)
	{
		m_bDraggingBuildingShade = true;

		for (int i = 0; i < m_pBuildingShadesPrefabs.Count; i++)
		{
			if (m_pBuildingShadesPrefabs[i].m_eType == eBuildingType)
			{
				m_pCurrentBuildingShade = Instantiate<BuildingShade>(m_pBuildingShadesPrefabs[i]);
				m_pCurrentBuildingShade.transform.SetParent(transform, false);
				break;
			}
		}

		if (m_pCurrentBuildingShade == null)
		{
			Debug.LogError("BuildingShade not found: " + eBuildingType);
			yield break;
		}

		RaycastHit tHit;

		do
		{
			if (m_pCurrentBuildingShade == null)
				yield break;

			Ray tRay = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(tRay, out tHit, 100.0f, LayersManager.Instance.GetLayer("Ground"), QueryTriggerInteraction.Ignore))
			{
				m_pCurrentBuildingShade.gameObject.SetActive(true);
				m_pCurrentBuildingShade.transform.position = tHit.point;
			}
			else
			{
				m_pCurrentBuildingShade.gameObject.SetActive(false);
			}

			yield return false;
		} while (!Input.GetButtonDown("Submit") || !m_pCurrentBuildingShade.CanBePlaced);


		DestroyCurrentShade();

		for (int i = 0; i < m_pBuildingsPrefabs.Count; i++)
		{
			if (m_pBuildingsPrefabs[i].m_eBuildingType == eBuildingType)
			{
				Building pNewBuilding = Instantiate<Building>(m_pBuildingsPrefabs[i]);
				pNewBuilding.transform.SetParent(transform, false);
				pNewBuilding.transform.position = tHit.point;
				break;
			}
		}

		m_bDraggingBuildingShade = false;
	}
}
