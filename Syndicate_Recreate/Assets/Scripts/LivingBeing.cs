using UnityEngine;
using System.Collections;

public class LivingBeing : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fHealth = 10.0f;

	[Header("Fight")]
	[SerializeField]
	protected Behaviour m_eBehaviour = Behaviour.Coward;
	[SerializeField]
	protected float m_fSightRange = 25.0f;
	[SerializeField]
	private float m_fShootRange = 20.0f;
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
		Coward,
		Agressive,
		Defensive,
		Ally,
		Player
	}


	protected bool m_bAlive = true;

	private MeshRenderer m_tMeshRenderer;
	private MeshRenderer m_tForwardCubeMeshRenderer;
	private CapsuleCollider m_tCapsuleCollider;

	protected NavMeshAgent m_tNavMesh;

	protected GameObject m_pTarget = null;

	static protected int s_iShootRaycastLayer = ~(1 << 9);
	static protected int s_iObstaclesRaycastLayer = 1 << 8;

	#endregion

	#region Variables (private)

	private float m_fDefaultSpeed;
	
	private float m_fLastAttackTime = 0.0f;
	private bool m_bOpenedFire = false;
	
	#endregion


	protected void BehaviourStart()
	{
		m_tMeshRenderer = GetComponent<MeshRenderer>();
		m_tForwardCubeMeshRenderer = transform.FindChild("Forward").gameObject.GetComponent<MeshRenderer>();
		m_tCapsuleCollider = GetComponent<CapsuleCollider>();
		m_tNavMesh = GetComponent<NavMeshAgent>();
		m_fDefaultSpeed = m_tNavMesh.speed;
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

				Flee();

				break;
			}
		}

		else if (m_eBehaviour != Behaviour.Player)
			m_tNavMesh.destination = transform.position;

		CheckCameraVisibility();
	}


	void FollowTarget()
	{
		if (IsTargetVisible())
		{
			if (m_tNavMesh.hasPath)
			{
				m_tNavMesh.destination = transform.position;
				m_bOpenedFire = true;
			}

			Attack();

			if (m_pTarget.GetComponent<LivingBeing>().IsDead)
			{
				m_pTarget = null;
				m_bOpenedFire = false;
			}
		}

		else
		{
			if (IsTargetInRange(m_fSightRange))
				m_tNavMesh.destination = m_pTarget.transform.position;
			else
			{
				m_tNavMesh.destination = transform.position;
				m_pTarget = null;
			}
			
			m_bOpenedFire = false;
		}
	}

	void Flee()
	{
		m_tNavMesh.speed = m_fDefaultSpeed * 2.0f;
		
		Vector3 tFleeDirection = transform.position - m_pTarget.transform.position;

		if (!IsTargetInRange(m_fSightRange))
		{
			m_pTarget = null;
			m_tNavMesh.destination = transform.position;
			m_tNavMesh.speed = m_fDefaultSpeed;
		}

		else if (!m_tNavMesh.hasPath)
			m_tNavMesh.SetDestination(transform.position + tFleeDirection);
	}

	void Attack()
	{
		transform.LookAt(m_pTarget.transform);

		if (Time.fixedTime - m_fLastAttackTime >= m_fAttackSpeed)
		{
			RaycastHit Hit;

			if (Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, s_iShootRaycastLayer, QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(transform.position, Hit.point, Color.red);

				LivingBeing tShotThing = Hit.collider.gameObject.GetComponent<LivingBeing>();

				if (tShotThing)
					tShotThing.ReceiveDamage(m_fDefaultDamages, gameObject);
			}

			m_fLastAttackTime = Time.fixedTime;
		}
	}

	void CheckCameraVisibility()
	{
		Vector3 tCapsulePoint1 = transform.position - (transform.up * (m_tCapsuleCollider.height / 4.0f));
		Vector3 tCapsulePoint2 = transform.position + (transform.up * (m_tCapsuleCollider.height / 4.0f));

		bool bObstacleInSight = Physics.CapsuleCast(tCapsulePoint1, tCapsulePoint2, m_tCapsuleCollider.radius, -Camera.main.transform.forward, Camera.main.farClipPlane, s_iObstaclesRaycastLayer, QueryTriggerInteraction.Ignore);

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

	#region Methods

	public void ReceiveDamage(float fDamages, GameObject pAttacker)
	{
		m_fHealth -= fDamages;

		if (m_fHealth <= 0.0f)
			m_bAlive = false;

		else if (m_eBehaviour == Behaviour.Coward ||
				 m_eBehaviour == Behaviour.Defensive)
			m_pTarget = pAttacker;
	}

	public bool IsDead
	{
		get { return !m_bAlive; }
	}


	bool IsTargetVisible()
	{
		int iObstaclesLayer = s_iObstaclesRaycastLayer | (m_bOpenedFire ? 0 : s_iShootRaycastLayer);	// Make sure no one is between target and himself before opening fire
		RaycastHit Hit;
		bool bIsVisible = !Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, iObstaclesLayer, QueryTriggerInteraction.Ignore) || Hit.collider.gameObject == m_pTarget;

		return IsTargetInRange(m_fShootRange) && bIsVisible;
	}

	bool IsTargetInRange(float fRange)
	{
		return (transform.position - m_pTarget.transform.position).sqrMagnitude <= (fRange * fRange);
	}

	#endregion Methods
}
