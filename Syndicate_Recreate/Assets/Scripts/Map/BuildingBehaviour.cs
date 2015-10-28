using UnityEngine;
using System.Collections;

public class BuildingBehaviour : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private MeshRenderer[] m_pFloorsAbove = null;
	
	#endregion
	
	#region Variables (private)
	
	
	
	#endregion


	public void ToggleFloorsAboveRenderers()
	{
		foreach(MeshRenderer tFloorRenderer in m_pFloorsAbove)
		{
			tFloorRenderer.enabled = !tFloorRenderer.enabled;
			tFloorRenderer.gameObject.GetComponent<BoxCollider>().enabled = tFloorRenderer.enabled;
		}
	}
}
