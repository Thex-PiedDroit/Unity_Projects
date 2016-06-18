
using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class ConstructionMenuItem : MonoBehaviour
{
#region Variables (public)

	public Image m_pSprite = null;

	#endregion

#region Variables (private)

	private BuildingData m_pAttachedBuilding = null;

    #endregion
	
	
	public void Init(BuildingData pBuilding)
	{
		m_pSprite.sprite = ToolKit.GetSpriteFromAtlas(pBuilding.m_pAssetsName, "Buildings");
		m_pAttachedBuilding = pBuilding;
	}

	public void Click()
	{
		
	}
}
