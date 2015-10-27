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
			if (m_tNavMesh.hasPath)
				Debug.DrawLine(transform.position, m_tNavMesh.destination, Color.magenta);

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


			if (m_eWanderingBehaviour != WanderType.None && !m_pTarget)
				Wander();

			else if (!m_pTarget)
				m_tNavMesh.destination = transform.position;

			else
			{
				m_fIdleStartTime = 0.0f;
			}

			base.BehaviourUpdate();
		}

		else
			Destroy(transform.parent.gameObject);
	}


	#region Methods

	void Wander()
	{
		if (!m_tNavMesh.hasPath)
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
						float fRandomDist = Random.Range(0.0f, m_fWanderAreaRadius);
						Vector3 tUnrotatedVector = Vector3.forward * fRandomDist;
						float fRandomAngle = Random.Range(-180.0f, 180.0f);
						fRandomAngle *= Mathf.Deg2Rad;
						float fCos = Mathf.Cos(fRandomAngle);
						float fSin = Mathf.Sin(fRandomAngle);
						tRandomDestination.x = tUnrotatedVector.x * fCos - tUnrotatedVector.z * fSin;
						tRandomDestination.z = tUnrotatedVector.x * fSin + tUnrotatedVector.z * fCos;

						tRandomDestination += m_tSpawnPoint;

						break;
					}

				case WanderType.Anywhere:
					{
						tRandomDestination.x = Random.Range(-s_pGround.localScale.x, s_pGround.localScale.x);
						tRandomDestination.z = Random.Range(-s_pGround.localScale.z, s_pGround.localScale.z);
					}
					break;
				}

				m_tNavMesh.SetDestination(tRandomDestination);
				m_fIdleStartTime = 0.0f;
			}
		}
	}

	GameObject SearchTarget()
	{
		GameObject pTarget = null;

		float fNearestCharacterSqrdDist = m_eBehaviour == Behaviour.Coward ? m_fAttackRange : m_fSightRange;
		fNearestCharacterSqrdDist *= fNearestCharacterSqrdDist;

		foreach (GameObject tCharacter in s_pPlayerCharacters)
		{
			if (!tCharacter.GetComponentInChildren<LivingBeing>().IsDead)
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
