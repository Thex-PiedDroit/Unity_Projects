using UnityEngine;
using System.Collections;

public class CommunistBehavior : MonoBehaviour
{
	#region Variables (public)


	
	#endregion
	
	#region Variables (private)

	static private GameObject[] s_pPlayerCharacters = null;

	private Fight m_tFightScript;
	private TargetScript m_tTargetScript;
	
	#endregion


	void Start()
	{
		if (s_pPlayerCharacters == null)
			s_pPlayerCharacters = GameObject.FindGameObjectsWithTag("PlayerCharacter");

		m_tFightScript = GetComponent<Fight>();
		m_tTargetScript = GetComponent<TargetScript>();
	}
	
	void Update ()
	{
		if (m_tTargetScript.IsDead)
			Destroy(gameObject);

		else
		{
			m_tFightScript.Target = FindTarget();
		}
	}


	#region Methods

	GameObject FindTarget()
	{
		GameObject pTarget = null;

		float fNearestCharacterSqrdDist = m_tFightScript.SightRange;
		fNearestCharacterSqrdDist *= fNearestCharacterSqrdDist;

		foreach (GameObject tCharacter in s_pPlayerCharacters)
		{
			if (!tCharacter.GetComponent<TargetScript>().IsDead)
			{
				float fSqrdDist = (transform.position - tCharacter.transform.position).sqrMagnitude;

				if (fSqrdDist < fNearestCharacterSqrdDist)
				{
					pTarget = tCharacter;
					fNearestCharacterSqrdDist = fSqrdDist;
				}
			}
		}

		return pTarget;
	}

	#endregion
}
