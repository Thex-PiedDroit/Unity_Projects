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
	
	#endregion
	
	#region Variables (private)

	private enum WanderType
	{
		None,
		Area,
		Anywhere
	}


	private Vector3 m_tSpawnPoint;

	private float m_fRandomIdleTime = 0.0f;
	private float m_fIdleStartTime = 0.0f;
	private float m_fTimeOfDeath = 0.0f;

	static private GameObject[] s_pPlayerCharacters = null;
	static private Transform s_pGround;
	
	#endregion

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, m_fWanderAreaRadius);
	}

	void Start()
	{
		m_tSpawnPoint = transform.position;
		s_pGround = GameObject.FindGameObjectWithTag("Ground").transform;

		if (s_pPlayerCharacters == null)
			s_pPlayerCharacters = GameObject.FindGameObjectsWithTag("PlayerCharacter");

		base.BehaviourStart();
	}
	
	void Update()
	{
		if (m_bAlive)
		{
			switch (m_eBehaviour)
			{
			case Behaviour.Coward:
			case Behaviour.Defensive:
			case Behaviour.Agressive:

				m_pTarget = SearchTarget();
				break;

			case Behaviour.Ally:

				m_pTarget = GetPlayerTarget();
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

			base.BehaviourUpdate();
		}

		else if (m_fTimeOfDeath == 0.0f)
		{
			transform.forward = Vector3.down;
			m_tNavMeshAgent.destination = transform.position;
			m_pTarget = null;
			m_fTimeOfDeath = Time.fixedTime;
		}

		else if (Time.fixedTime - m_fTimeOfDeath >= m_fBodyDespawnTime)
			Destroy(transform.parent.gameObject);
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

						tRandomDestination += m_tSpawnPoint;
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

	GameObject SearchTarget()
	{
		GameObject pTarget = null;

		switch (m_eBehaviour)
		{
		case Behaviour.Coward:
		case Behaviour.Agressive:
			{
				float fNearestCharacterSqrdDist = m_eBehaviour == Behaviour.Coward ? m_fAttackRange : m_fSightRange;
				fNearestCharacterSqrdDist *= fNearestCharacterSqrdDist;

				foreach (GameObject tCharacter in s_pPlayerCharacters)
				{
					if (!tCharacter.GetComponentInChildren<LivingBeing>().IsDead)
					{
						float fSqrdDist = (transform.position - tCharacter.transform.position).sqrMagnitude;

						if (fSqrdDist < fNearestCharacterSqrdDist)
						{
							pTarget = tCharacter.gameObject;
							fNearestCharacterSqrdDist = fSqrdDist;
						}
					}
				}

				break;
			}

		case Behaviour.Defensive:
			{
				Collider[] pLivingBeingsInSight = Physics.OverlapSphere(transform.position, m_fSightRange, s_iLivingBeingsLayer);

				if (pLivingBeingsInSight.Length <= 1)
					m_bPursuingTarget = false;

				else
				{
					foreach (Collider tLivingBeingCollider in pLivingBeingsInSight)
					{
						LivingBeing tLivingBeing = tLivingBeingCollider.gameObject.GetComponent<LivingBeing>();

						if (tLivingBeing.Target != null)
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
