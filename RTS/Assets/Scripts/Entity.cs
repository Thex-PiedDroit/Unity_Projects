﻿
using UnityEngine;
using System.Collections;


public abstract class EntityData
{
	public enum EEntityType
	{
		BUILDING,
		UNIT,
		HERO,
		NONE,
	}

	public EEntityType m_eType;

	public string m_pPublicName;
	public string m_pAssetsName;

	public float m_fHealth;

	public EntityData()
		: this(EEntityType.NONE, "Undefined", "Undefined", 0.0f)
	{
		
	}

	public EntityData(EEntityType eType)
		: this(eType, "Undefined", "Undefined", 0.0f)
	{

	}

	public EntityData(EEntityType eType, string pPublicName, string pAssetsName, float fHealth)
	{
		m_eType = eType;

		m_pPublicName = pPublicName;
		m_pAssetsName = pAssetsName;

		m_fHealth = fHealth;
	}
}


public abstract class Entity : MonoBehaviour
{
#region Variables (public)

	

	#endregion

#region Variables (private)
	
	

    #endregion
}
