using UnityEngine;
using System.Collections;

public class AI : LivingBeing
{
	#region Variables (public)
	
	
	
	#endregion
	
	#region Variables (private)

	static private GameObject[] s_pPlayerCharacters = null;
	
	#endregion


	void Start ()
	{
		if (s_pPlayerCharacters == null)
			s_pPlayerCharacters = GameObject.FindGameObjectsWithTag("PlayerCharacter");

		base.BehaviourStart();
	}
	
	void Update ()
	{
		if (m_bAlive)
		{
			switch(m_eBehaviour)
			{
			case Behaviour.Coward:

				if (!m_pTarget)
					m_pTarget = SearchTarget();
				break;

			case Behaviour.Agressive:

				m_pTarget = SearchTarget();
				break;

			case Behaviour.Ally:

				m_pTarget = GetPlayerTarget();
				break;
			}

			base.BehaviourUpdate();
		}

		else
			Destroy(gameObject);
	}


	#region Methods

	GameObject SearchTarget()
	{
		GameObject pTarget = null;

		float fNearestCharacterSqrdDist = m_eBehaviour == Behaviour.Coward ? m_fAttackRange : m_fSightRange;
		fNearestCharacterSqrdDist *= fNearestCharacterSqrdDist;

		foreach (GameObject tCharacter in s_pPlayerCharacters)
		{
			if (!tCharacter.GetComponent<LivingBeing>().IsDead)
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

	GameObject GetPlayerTarget()
	{
		// Will do if needed

		return null;
	}


	static public void ClearCharactersArray()
	{
		s_pPlayerCharacters = null;
	}

	#endregion


	void OnDestroy()
	{
		if (gameObject.tag == "Target")
			MissionManager.TargetDead();
	}
}
