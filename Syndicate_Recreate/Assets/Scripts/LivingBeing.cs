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
	protected float m_fAttackRange = 20.0f;
	[SerializeField]
	private float m_fAttackSpeed = 1.0f;
	[SerializeField]
	private float m_fDefaultDamages = 2.0f;

	[Header("Display")]
	[SerializeField]
	private Material m_pMaterialReg;
	[SerializeField]
	private Material m_pMaterialTransp;
	[SerializeField]
	private Material m_pForwardCubeMaterialReg;
	[SerializeField]
	private Material m_pForwardCubeMaterialTransp;
	
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

	private MeshRenderer m_tMeshRenderer;
	private MeshRenderer m_tForwardCubeMeshRenderer;
	private CapsuleCollider m_tCapsuleCollider;

	protected NavMeshAgent m_tNavMeshAgent;

	protected GameObject m_pTarget = null;

	static protected int s_iAllButGroundLayer = ~(1 << 9);
	static protected int s_iObstaclesLayer = 1 << 8;
	static protected int s_iLivingBeingsLayer = 1 << 10;

	#endregion

	#region Variables (private)

	private float m_fDefaultSpeed;
	
	private float m_fLastAttackTime = 0.0f;
	private bool m_bOpenedFire = false;
	
	#endregion


	protected void BehaviourStart()
	{
		m_fHealth = m_fMaxHealth;

		m_tMeshRenderer = GetComponent<MeshRenderer>();
		m_tForwardCubeMeshRenderer = transform.FindChild("Forward").gameObject.GetComponent<MeshRenderer>();
		m_tCapsuleCollider = GetComponent<CapsuleCollider>();
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

			case Behaviour.Neutral:
			case Behaviour.Coward:

				Flee();
				break;
			}

			if (m_pTarget && m_pTarget.GetComponent<LivingBeing>().IsDead)
			{
				m_pTarget = null;
				m_bOpenedFire = false;
			}
		}

		CheckCameraVisibility();
	}


	#region Methods

	void FollowTarget()
	{
		if (!m_bPursuingTarget && IsTargetVisible())
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
		m_tNavMeshAgent.speed = m_fDefaultSpeed * 2.0f;

		if (tag != "Target" && !IsTargetInRange(m_fSightRange))
		{
			m_pTarget = null;
			m_tNavMeshAgent.destination = transform.position;
			m_tNavMeshAgent.speed = m_fDefaultSpeed;
		}

		else if (!m_tNavMeshAgent.hasPath)
		{
			Vector3 tFleeDestination;

			if (tag == "Target")
				tFleeDestination = GameObject.Find("EscapePoint").transform.position;

			else
			{
				Vector3 tFleeDirection = (transform.position - m_pTarget.transform.position);
				tFleeDestination = transform.position + tFleeDirection;
			}

			m_tNavMeshAgent.SetDestination(tFleeDestination);
		}
	}

	void Attack()
	{
		transform.parent.LookAt(m_pTarget.transform);

		if (Time.fixedTime - m_fLastAttackTime >= m_fAttackSpeed)
		{
			RaycastHit Hit;

			if (Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, s_iAllButGroundLayer, QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(transform.position, Hit.point, Color.red);

				LivingBeing tShotThing = Hit.collider.gameObject.GetComponent<LivingBeing>();

				if (tShotThing)
					tShotThing.ReceiveDamage(m_fDefaultDamages, gameObject.gameObject);
			}

			m_fLastAttackTime = Time.fixedTime;
		}
	}

	void CheckCameraVisibility()
	{
		Vector3 tCapsulePoint1 = transform.position - (transform.up * (m_tCapsuleCollider.height / 4.0f));
		Vector3 tCapsulePoint2 = transform.position + (transform.up * (m_tCapsuleCollider.height / 4.0f));

		bool bObstacleInSight = Physics.CapsuleCast(tCapsulePoint1, tCapsulePoint2, m_tCapsuleCollider.radius, -Camera.main.transform.forward, Camera.main.farClipPlane, s_iObstaclesLayer, QueryTriggerInteraction.Ignore);

		if (bObstacleInSight && (m_tMeshRenderer.material != m_pMaterialTransp))
		{
			m_tMeshRenderer.material = m_pMaterialTransp;
			m_tForwardCubeMeshRenderer.material = m_pForwardCubeMaterialTransp;
		}

		else if (m_tMeshRenderer.material != m_pMaterialReg)
		{
			m_tMeshRenderer.material = m_pMaterialReg;
			m_tForwardCubeMeshRenderer.material = m_pForwardCubeMaterialReg;
		}
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
		int iObstaclesLayer = s_iObstaclesLayer | (m_bOpenedFire ? 0 : s_iAllButGroundLayer);	// Make sure no one is between target and himself before opening fire (and not after)
		RaycastHit Hit;
		bool bIsVisible = !Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, iObstaclesLayer, QueryTriggerInteraction.Ignore) || Hit.collider.gameObject == m_pTarget;

		return IsTargetInRange(m_fAttackRange) && bIsVisible;
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

	static public int GroundLayer
	{
		get { return ~s_iAllButGroundLayer; }
	}

	static public int ObstaclesLayer
	{
		get { return s_iObstaclesLayer; }
	}

	#endregion Getters/Setters
}
