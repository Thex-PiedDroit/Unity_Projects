using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponsIcons : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private Text pWeaponsNamesField;
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

	public void PrintWeaponName(string pWeaponName)
	{
		pWeaponsNamesField.text = pWeaponName;
	}
}
