using UnityEngine;
using System.Collections;

public class WeaponsIcons : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject[] pWeaponsIcons;
	
	#endregion
	
	#region Variables (private)
	
	private static WeaponsIcons s_pInstance;
	
	#endregion


	void Awake()
	{
		s_pInstance = this;
	}


	public static GameObject GetIcon(string pWeaponName)
	{
		for (int i = 0; i < s_pInstance.pWeaponsIcons.Length; i++)
		{
			if (s_pInstance.pWeaponsIcons[i].name == pWeaponName)
				return s_pInstance.pWeaponsIcons[i];
		}

		return null;
	}
}
