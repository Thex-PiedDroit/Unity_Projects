
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class LayersManager : MonoBehaviour
{
#region Variables (public)

	static public LayersManager Instance = null;
	
	#endregion
	
#region Variables (private)

	static private Dictionary<string, int> m_pLayers = null;
	
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

		InitLayers();
	}
	
	void InitLayers()
	{
		m_pLayers = new Dictionary<string, int>();

		for (int i = 0; i < 32; i++)
		{
			string pLayerName = LayerMask.LayerToName(i);
			if (string.IsNullOrEmpty(pLayerName))
				continue;

			m_pLayers.Add(pLayerName, 1 << i);
		}
	}

	public int GetLayer(string pLayerName)
	{
		return m_pLayers[pLayerName];
	}

	public int GetLayers(params string[] pLayerNames)
	{
		int iMask = 0;

		for (int i = 0; i < pLayerNames.Length; i++)
		{
			iMask |= m_pLayers[pLayerNames[i]];
		}

		return iMask;
	}
}
