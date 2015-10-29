using UnityEngine;
using System.Collections;

public class LivingBeing : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	protected float m_fMaxHealth = 10.0f;
	[SerializeField]
	protected int m_iPersuasionStrenght = 3;

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
	protected int m_iPersuasion = 0;
	protected bool m_bPursuingTarget = false;

	protected NavMeshAgent m_tNavMeshAgent;

	protected GameObject m_pTarget = null;

	static protected int s_iLivingBeingsLayer = 1 << LayerMask.NameToLayer("LivingBeing");
	static protected int s_iDeadsLayers = 1 << LayerMask.NameToLayer("Dead");

	#endregion

	#region Variables (private)

	private float m_fDefaultSpeed;
	private bool m_bOpenedFire = false;
	
	#endregion


	protected virtual void Start()
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

	protected virtual void Update()
	{
		if (!m_pTarget || m_tNavMeshAgent.hasPath)
			m_bOpenedFire = false;


		if (m_pTarget)
		{
			if (m_pTarget.GetComponent<LivingBeing>().IsDead ||
				(m_eBehaviour == Behaviour.Player && m_pTarget.GetComponent<LivingBeing>().m_eBehaviour == Behaviour.Ally))
			{
				m_pTarget = null;
			}

			else
			{
				switch (m_eBehaviour)
				{
				case Behaviour.Agressive:
				case Behaviour.Defensive:
				case Behaviour.Player:

					if (m_pActiveWeapon && m_pActiveWeapon.gameObject.activeSelf)
						FollowTarget();
					else
						Flee();
					break;

				case Behaviour.Ally:

					FollowPlayer();
					break;

				case Behaviour.Coward:
				case Behaviour.Neutral:

					if (m_tNavMeshAgent.hasPath)
						Debug.DrawLine(transform.position, m_tNavMeshAgent.destination, Color.red);

					Flee();
					break;
				}
			}
		}
	}

	protected virtual void OnAttacked(GameObject pAttacker)
	{
		if (((m_eBehaviour == Behaviour.Player && m_tNavMeshAgent.hasPath) ||
			(m_eBehaviour == Behaviour.Player && m_pTarget != null)) == false)
			m_pTarget = pAttacker;
	}

	protected virtual void OnDeath()
	{
		m_fHealth = 0.0f;
		m_bAlive = false;
		m_pTarget = null;
		gameObject.layer = LayerMask.NameToLayer("Dead");
		transform.forward = Vector3.down;
		m_tNavMeshAgent.SetDestination(transform.position);
	}


	#region Methods

	void FollowTarget()
	{
		if (IsTargetVisible())
		{
			if (m_tNavMeshAgent.hasPath)
				m_tNavMeshAgent.SetDestination(transform.position);

			Attack();
		}

		else
		{
			if (m_eBehaviour != Behaviour.Player && !m_bPursuingTarget)
			{
				if (IsTargetInRange(m_fSightRange))
				{
					m_tNavMeshAgent.SetDestination(m_pTarget.transform.position);
					m_bPursuingTarget = false;
				}

				else if (!m_bPursuingTarget)
				{
					m_tNavMeshAgent.SetDestination(transform.position);
					m_pTarget = null;
				}
			}

			else
				m_tNavMeshAgent.SetDestination(m_pTarget.transform.position);
		}
	}

	void FollowPlayer()
	{
		bool bTargetIsPlayer = m_pTarget.GetComponent<LivingBeing>().m_eBehaviour == Behaviour.Player;

		if (m_pActiveWeapon && !bTargetIsPlayer)
			FollowTarget();

		else if (bTargetIsPlayer)
			m_tNavMeshAgent.SetDestination(m_pTarget.transform.position - (m_pTarget.transform.forward * 3.0f));

		else
			Flee();
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
		m_bOpenedFire = true;

		transform.parent.LookAt(m_pTarget.transform.parent);
		m_pActiveWeapon.Shoot(m_pTarget.transform);
	}

	public void ReceiveDamage(float fDamages, GameObject pAttacker)
	{
		if (pAttacker.GetComponent<Player>() != null && m_eBehaviour == Behaviour.Player)
			fDamages = 0.0f;	// So characters can "argue" safely when one shoots another by mistake

		m_fHealth -= fDamages;

		if (m_fHealth <= 0.0f)
			OnDeath();

		OnAttacked(pAttacker);
	}

	bool IsTargetVisible()
	{
		RaycastHit Hit;
		

		bool bIsVisible = false;

		if (!m_bOpenedFire)
		{
			int iObstaclesLayer = Map.ObstaclesLayer | s_iLivingBeingsLayer;	// Check if no living being inbetween before opening fire (not after)
			bIsVisible = Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, iObstaclesLayer, QueryTriggerInteraction.Ignore) && Hit.collider.gameObject == m_pTarget;
		}

		else
			bIsVisible = Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, s_iLivingBeingsLayer, QueryTriggerInteraction.Ignore);

		bool bInWeaponRange = m_pActiveWeapon != null && IsTargetInRange(m_pActiveWeapon.AttackRange);

		return bInWeaponRange && bIsVisible;
	}

	protected bool IsTargetInRange(float fRange)
	{
		return (transform.position - m_pTarget.transform.position).sqrMagnitude <= (fRange * fRange);
	}

	#endregion Methods


	#region Getters/Setters

	public bool IsPlayer
	{
		get { return m_eBehaviour == Behaviour.Player; }
	}

	public bool IsDead
	{
		get { return !m_bAlive; }
	}

	public GameObject Target
	{
		get { return m_pTarget; }
		set
		{
			if (m_pActiveWeapon != null &&
				m_pActiveWeapon.gameObject.activeSelf)
			{
				m_pTarget = value;
			}
		}
	}

	public static int AllButDeadsLayer
	{
		get { return ~s_iDeadsLayers; }
	}

	#endregion Getters/Setters
}
