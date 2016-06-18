
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
	
	#endregion
	
#region Variables (private)

	private List<GameObject> m_pBuildingsPrefabs = null;
	private Dictionary<EBuildingType, BuildingData> m_pBuildingsData = null;
	
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
		Building[] pBuildings = Resources.LoadAll<Building>("Buildings");
		m_pBuildingsPrefabs = new List<GameObject>(pBuildings.Length);
		for (int i = 0; i < m_pBuildingsPrefabs.Count; i++)
		{
			m_pBuildingsPrefabs.Add(pBuildings[i].gameObject);
		}

		m_pBuildingsData = new Dictionary<EBuildingType, BuildingData>(m_pBuildingsPrefabs.Count);

		JSONObject tJSONData = new JSONObject(File.ReadAllText(Application.dataPath + "/Resources/Data/Buildings.json"));

		for (int i = 0; i < tJSONData.list[0].Count; i++)
		{
			BuildingData pData = new BuildingData();

			EBuildingType eType = ToolKit.ToEnum<EBuildingType>(tJSONData.list[0].keys[i], EBuildingType.NONE);

			for (int j = 0; j < tJSONData.list[0].list[i].keys.Count; j++)
			{
				switch (tJSONData.list[0].list[i].keys[j])
				{
					case "PublicName": pData.m_pPublicName = tJSONData.list[0].list[i].list[j].str; break;
					case "AssetsName": pData.m_pAssetsName = tJSONData.list[0].list[i].list[j].str; break;
					case "Health": pData.m_fHealth = tJSONData.list[0].list[i].list[j].f; break;
				}
			}

			m_pBuildingsData.Add(eType, pData);
		}
	}
}
