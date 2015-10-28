using UnityEngine;
using System.Collections;

public class LivingBeing : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	protected float m_fMaxHealth = 10.0f;

	[Header("Fight")]
	[SerializeField]
	protected Behaviour m_eBehaviour = Behaviour.Neutral;
	[SerializeField]
	protected float m_fSightRange = 25.0f;
	[SerializeField]
	protected Weapon m_pActiveWeapon;
	
	#endregion

	#region Variables (protected)

	protected enum Behaviour
	{
		Neutral,
		Coward,
		Agressive,
		Defensive,
		Ally,
		Player
	}


	protected bool m_bAlive = true;
	protected float m_fHealth = 10.0f;
	protected bool m_bJustTookDamage = false;
	protected bool m_bPursuingTarget = false;

	protected NavMeshAgent m_tNavMeshAgent;

	protected GameObject m_pTarget = null;

	static protected int s_iLivingBeingsLayer = 1 << 10;

	#endregion

	#region Variables (private)

	private float m_fDefaultSpeed;
	
	private bool m_bOpenedFire = false;
	
	#endregion


	protected void BehaviourStart()
	{
		if (m_pActiveWeapon)
		{
			m_pActiveWeapon = Instantiate(m_pActiveWeapon, transform.position, transform.rotation) as Weapon;
			m_pActiveWeapon.gameObject.transform.parent = transform;
		}

		m_fHealth = m_fMaxHealth;
		m_tNavMeshAgent = GetComponentInParent<NavMeshAgent>();
		m_fDefaultSpeed = m_tNavMeshAgent.speed;
	}

	protected void BehaviourUpdate()
	{
		if (m_pTarget)
		{
			switch(m_eBehaviour)
			{
			case Behaviour.Agressive:
			case Behaviour.Defensive:
			case Behaviour.Ally:
			case Behaviour.Player:

				FollowTarget();
				break;

			case Behaviour.Coward:
			case Behaviour.Neutral:

				if (m_tNavMeshAgent.hasPath)
					Debug.DrawLine(transform.position, m_tNavMeshAgent.destination, Color.red);

				Flee();
				break;
			}

			if (m_pTarget && m_pTarget.GetComponent<LivingBeing>().IsDead)
			{
				m_pTarget = null;
				m_bOpenedFire = false;
			}
		}
	}


	#region Methods

	void FollowTarget()
	{
		if (!m_bPursuingTarget || IsTargetVisible())
		{
			if (m_tNavMeshAgent.hasPath)
			{
				m_tNavMeshAgent.destination = transform.position;
				m_bOpenedFire = true;
			}

			Attack();
		}

		else
		{
			if (m_eBehaviour != Behaviour.Player && !m_bPursuingTarget)
			{
				if (IsTargetInRange(m_fSightRange))
				{
					m_tNavMeshAgent.destination = m_pTarget.transform.position;
					m_bPursuingTarget = false;
				}

				else if (!m_bPursuingTarget)
				{
					m_tNavMeshAgent.destination = transform.position;
					m_pTarget = null;
				}
			}

			else
				m_tNavMeshAgent.destination = m_pTarget.transform.position;
			
			m_bOpenedFire = false;
		}
	}

	void Flee()
	{
		if (!m_tNavMeshAgent.hasPath)
		{
			Vector3 tFleeDestination = transform.position;

			if (tag != "Target")
			{
				if (IsTargetInRange(m_fSightRange))
				{
					Vector3 tFleeDirection = (transform.position - m_pTarget.transform.position).normalized * m_fSightRange;
					tFleeDestination = transform.position + tFleeDirection;
				}

				else
				{
					m_pTarget = null;
					m_tNavMeshAgent.speed = m_fDefaultSpeed;
				}
			}

			else
				tFleeDestination = GameObject.Find("EscapePoint").transform.position;

			m_tNavMeshAgent.SetDestination(tFleeDestination);
		}
	}

	void Attack()
	{
		transform.parent.LookAt(m_pTarget.transform);

		m_pActiveWeapon.Shoot(m_pTarget.transform);
	}

	public void ReceiveDamage(float fDamages, GameObject pAttacker)
	{
		m_fHealth -= fDamages;

		if (m_fHealth <= 0.0f)
			m_bAlive = false;

		else if (m_eBehaviour == Behaviour.Neutral ||
				 m_eBehaviour == Behaviour.Coward ||
				 m_eBehaviour == Behaviour.Defensive)
			m_pTarget = pAttacker;

		m_bJustTookDamage = true;
	}

	bool IsTargetVisible()
	{
		int iObstaclesLayer = Map.ObstaclesLayer | (m_bOpenedFire ? 0 : Map.AllButGroundLayer);	// Make sure no one is between target and himself before opening fire (and not after)
		RaycastHit Hit;
		bool bIsVisible = !Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, iObstaclesLayer, QueryTriggerInteraction.Ignore) || Hit.collider.gameObject == m_pTarget;
		bool bInWeaponRange = m_pActiveWeapon != null && IsTargetInRange(m_pActiveWeapon.AttackRange);

		return bInWeaponRange && bIsVisible;
	}

	protected bool IsTargetInRange(float fRange)
	{
		return (transform.position - m_pTarget.transform.position).sqrMagnitude <= (fRange * fRange);
	}

	#endregion Methods


	#region Getters/Setters

	public bool IsDead
	{
		get { return !m_bAlive; }
	}

	public GameObject Target
	{
		get { return m_pTarget; }
	}

	#endregion Getters/Setters
}
