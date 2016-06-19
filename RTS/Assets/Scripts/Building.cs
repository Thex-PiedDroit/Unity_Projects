
using UnityEngine;
using System.Collections;


public class BuildingData : EntityData
{
	public BuildingData()
		: base()
	{
		
	}

	public BuildingData(string pPublicName, string pAssetsName, float fHealth)
		: base(EEntityType.BUILDING, pPublicName, pAssetsName, fHealth)
	{

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
		
	}
	
	void Update()
	{
		
	}
}
