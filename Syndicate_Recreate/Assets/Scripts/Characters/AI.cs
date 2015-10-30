using UnityEngine;
using System.Collections;

public class AI : LivingBeing
{
	#region Variables (public)

	[SerializeField]
	private WanderType m_eWanderingBehaviour = WanderType.Area;
	[SerializeField]
	private float m_fWanderAreaRadius = 20.0f;
	[SerializeField]
	private Vector2 m_tIdleTimeMinMax = new Vector2(0.0f, 4.0f);
	[SerializeField]
	private float m_fBodyDespawnTime = 2.0f;

	[SerializeField]
	private GameObject m_pTargetProjectorHostile;
	[SerializeField]
	private GameObject m_pTargetProjectorFriendly;
	
	#endregion
	
	#region Variables (private)

	private enum WanderType
	{
		None,
		Area,
		Anywhere
	}


	private Vector3 m_tWanderCenterPoint;

	private float m_fRandomIdleTime = 0.0f;
	private float m_fIdleStartTime = 0.0f;
	private float m_fTimeOfDeath = 0.0f;

	private Renderer m_tRenderer;

	static private GameObject[] s_pPlayerCharacters = null;
	static private Player[] s_pPlayerCharactersScripts = null;
	static private Transform s_pGround;
	
	#endregion

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, m_fWanderAreaRadius);
	}

	protected override void Start()
	{
		m_tWanderCenterPoint = transform.position;
		s_pGround = GameObject.FindGameObjectWithTag("Ground").transform;

		if (s_pPlayerCharacters == null)
		{
			s_pPlayerCharacters = GameObject.FindGameObjectsWithTag("PlayerCharacter");
			s_pPlayerCharactersScripts = CharactersControl.PlayerCharacters;
		}

		if (gameObject.transform.parent.tag == "Target")
		{
			string pProjectorName = m_pTargetProjectorHostile.name;
			m_pTargetProjectorHostile = Instantiate(m_pTargetProjectorHostile, transform.parent.position, m_pTargetProjectorHostile.transform.rotation) as GameObject;
			m_pTargetProjectorHostile.name = pProjectorName;
			m_pTargetProjectorHostile.transform.parent = transform.parent;
		}

		base.Start();
	}
	
	protected override void Update()
	{
		if (m_bAlive)
		{
			switch (m_eBehaviour)
			{
			case Behaviour.Coward:
			case Behaviour.Defensive:
			case Behaviour.Agressive:

				if (!(m_eBehaviour == Behaviour.Coward && m_pTarget))
					SearchTarget();
				break;

			case Behaviour.Ally:

				GetPlayerTarget();
				break;
			}


			if (m_eWanderingBehaviour != WanderType.None && !m_pTarget)
				Wander();

			else if (!m_pTarget)
				m_tNavMeshAgent.destination = transform.position;

			else
			{
				m_fIdleStartTime = 0.0f;
			}

			base.Update();
		}

		if (m_fTimeOfDeath != 0.0f && Time.fixedTime - m_fTimeOfDeath >= m_fBodyDespawnTime)
			Destroy(transform.parent.gameObject);
	}

	protected override void OnDeath()
	{
		base.OnDeath();

		m_fTimeOfDeath = Time.fixedTime;
	}


	#region Methods

	void Wander()
	{
		if (!m_tNavMeshAgent.hasPath)
		{
			if (m_fIdleStartTime == 0.0f)
			{
				m_fIdleStartTime = Time.fixedTime;
				m_fRandomIdleTime = Random.Range(m_tIdleTimeMinMax.x, m_tIdleTimeMinMax.y);
			}

			if (Time.fixedTime - m_fIdleStartTime >= m_fRandomIdleTime)
			{
				Vector3 tRandomDestination = Vector3.zero;

				switch (m_eWanderingBehaviour)
				{
				case WanderType.Area:
					{
						Vector2 tRandomPoint = Random.insideUnitCircle * m_fWanderAreaRadius;
						tRandomDestination.x = tRandomPoint.x;
						tRandomDestination.z = tRandomPoint.y;

						tRandomDestination += m_tWanderCenterPoint;
						break;
					}

				case WanderType.Anywhere:
					{
						tRandomDestination.x = Random.Range(-s_pGround.localScale.x / 2.0f, s_pGround.localScale.x / 2.0f);
						tRandomDestination.z = Random.Range(-s_pGround.localScale.z / 2.0f, s_pGround.localScale.z / 2.0f);
						break;
					}
				}

				m_tNavMeshAgent.SetDestination(tRandomDestination);
				m_fIdleStartTime = 0.0f;
			}
		}
	}

	void SearchTarget()
	{
		GameObject pTarget = null;

		switch (m_eBehaviour)
		{
		case Behaviour.Coward:
		case Behaviour.Agressive:
			{
				float fNearestCharacterSqrdDist = m_fSightRange * m_fSightRange;

				for (int i = 0; i < s_pPlayerCharacters.Length; i++)
				{
					if (!s_pPlayerCharacters[i].GetComponentInChildren<LivingBeing>().IsDead)
					{
						float fSqrdDist = (transform.position - s_pPlayerCharacters[i].transform.position).sqrMagnitude;

						if (fSqrdDist < fNearestCharacterSqrdDist)
						{
							pTarget = s_pPlayerCharacters[i].gameObject;
							fNearestCharacterSqrdDist = fSqrdDist;
						}
					}
				}

				break;
			}

		case Behaviour.Defensive:
			{
				Collider[] pLivingBeingsInSight = Physics.OverlapSphere(transform.position, m_fSightRange, s_iLivingBeingsLayer);

				if (pLivingBeingsInSight.Length <= 1)		// If nobody in sight
					m_bPursuingTarget = false;

				else
				{
					for (int i = 0; i < pLivingBeingsInSight.Length; i++)
					{
						LivingBeing tLivingBeing = pLivingBeingsInSight[i].gameObject.GetComponent<LivingBeing>();

						if (tLivingBeing.Target != null ||
							(tLivingBeing.IsPlayer && tLivingBeing.ActiveWeaponName != "No active weapon"))
						{
							if (!m_bPursuingTarget)
							{
								if (tLivingBeing.gameObject.tag != "PlayerCharacter")
								{
									m_pTarget = tLivingBeing.Target;
									m_bPursuingTarget = true;
								}

								else
									pTarget = tLivingBeing.gameObject;
							}

							else
								pTarget = m_pTarget;
						}

						else if (m_bPursuingTarget || (m_pTarget && IsTargetInRange(m_fSightRange)))
							pTarget = m_pTarget;
					}
				}

				break;
			}
		}

		m_tNavMeshAgent.SetDestination(transform.position);
		m_pTarget = pTarget;
	}

	void GetPlayerTarget()
	{
		float fNearestCharacterSqrdDist = m_fSightRange * m_fSightRange;
		int iNearestPlayer = -1;

		for (int i = 0; i < s_pPlayerCharacters.Length; i++)
		{
			if (!s_pPlayerCharactersScripts[i].IsDead)
			{
				float fSqrDist = (transform.position - s_pPlayerCharacters[i].transform.position).sqrMagnitude;

				if (fSqrDist <= fNearestCharacterSqrdDist)
				{
					iNearestPlayer = i;
					fNearestCharacterSqrdDist = fSqrDist;
				}
			}
		}

		if (iNearestPlayer != -1)
		{
			GameObject pNearestPlayerTarget = s_pPlayerCharactersScripts[iNearestPlayer].Attacker;
			if (!pNearestPlayerTarget)
				pNearestPlayerTarget = s_pPlayerCharactersScripts[iNearestPlayer].Target;

			m_pTarget = pNearestPlayerTarget ? pNearestPlayerTarget : s_pPlayerCharacters[iNearestPlayer];
		}
	}

	public void Persuade(GameObject pAttacker)
	{
		m_iPersuasion++;

		if (m_iPersuasion >= m_iPersuasionStrenght)
		{
			m_eBehaviour = Behaviour.Ally;
			m_eWanderingBehaviour = WanderType.Area;

			if (transform.parent.tag == "Target")
			{
				Destroy(m_pTargetProjectorHostile);

				string pProjectorName = m_pTargetProjectorFriendly.name;
				m_pTargetProjectorFriendly = Instantiate(m_pTargetProjectorFriendly, transform.parent.position, m_pTargetProjectorFriendly.transform.rotation) as GameObject;
				m_pTargetProjectorFriendly.name = pProjectorName;
				m_pTargetProjectorFriendly.transform.parent = transform.parent;
			}
		}

		OnAttacked(pAttacker);
	}


	static public void ClearCharactersArray()
	{
		s_pPlayerCharacters = null;
		s_pPlayerCharactersScripts = null;
	}

	#endregion Methods


	public Vector3 WanderCenterPoint
	{
		set { m_tWanderCenterPoint = value; }
	}


	void OnDestroy()
	{
		if (gameObject.transform.parent.tag == "Target")
			MissionManager.TargetDead();
	}
}
