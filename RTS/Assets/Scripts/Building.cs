
using UnityEngine;
using System.Collections;


public class BuildingData : EntityData
{
	public EBuildingType m_eBuildingType;

	public BuildingData()
		: base(EEntityType.BUILDING)
	{
		m_eBuildingType = EBuildingType.NONE;
	}

	public BuildingData(EBuildingType eBuildingType, string pPublicName, string pAssetsName, float fHealth)
		: base(EEntityType.BUILDING, pPublicName, pAssetsName, fHealth)
	{
		m_eBuildingType = eBuildingType;
	}
}

public enum EBuildingType
{
	HOUSE,
	WORKSHOP,
	CHURCH,
	BARRACKS,
	BANK,
	NONE,
}


public class Building : MonoBehaviour
{
#region Variables (public)

	public EBuildingType m_eBuildingType = EBuildingType.NONE;

	#endregion

#region Variables (private)

	private BuildingData m_pData;

    #endregion
	

	public void Init(BuildingData pData)
	{
		m_eBuildingType = pData.m_eBuildingType;
	}
	
	void Update()
	{
		
	}
}
