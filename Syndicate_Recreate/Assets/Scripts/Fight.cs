using UnityEngine;
using System.Collections;

public class Fight : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fSightRange = 30.0f;
	[SerializeField]
	private float m_fShootRange = 20.0f;
	[SerializeField]
	private float m_fAttackSpeed = 1.0f;
	[SerializeField]
	private float m_fDefaultDamages = 2.0f;
	
	#endregion
	
	#region Variables (private)

	private NavMeshAgent m_tNavMesh;

	private GameObject m_pTarget = null;
	private float m_fLastAttackTime = 0.0f;
	private bool m_bOpenedFire = false;

	static private int s_iShootRaycastLayer = ~(1 << 9);
	static private int s_iObstaclesRaycastLayer = 1 << 8;
	
	#endregion


	void Start ()
	{
		m_tNavMesh = GetComponent<NavMeshAgent>();
	}
	
	void Update ()
	{
		if (m_pTarget)
		{
			if (IsTargetVisible())
			{
				if (m_tNavMesh.hasPath)
				{
					m_tNavMesh.destination = transform.position;
					m_bOpenedFire = true;
				}

				Attack();

				if (m_pTarget.GetComponent<TargetScript>().IsDead)
				{
					m_pTarget = null;
					m_bOpenedFire = false;
				}
			}

			else
			{
				m_tNavMesh.destination = m_pTarget.transform.position;
				m_bOpenedFire = false;
			}
		}

		else if (gameObject.tag != "PlayerCharacter")
			m_tNavMesh.destination = transform.position;
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

				TargetScript tShotThing = Hit.collider.gameObject.GetComponent<TargetScript>();

				if (tShotThing)
					tShotThing.Damage(m_fDefaultDamages);
			}

			m_fLastAttackTime = Time.fixedTime;
		}
	}


	#region Methods

	bool IsTargetVisible()
	{
		int iObstaclesLayer = s_iObstaclesRaycastLayer | (m_bOpenedFire ? 0 : s_iShootRaycastLayer);	// Make sure no one is between target and himself before opening fire
		RaycastHit Hit;
		bool bIsVisible = !Physics.Linecast(transform.position, m_pTarget.transform.position, out Hit, iObstaclesLayer, QueryTriggerInteraction.Ignore) || Hit.collider.gameObject == m_pTarget;

		return IsTargetOnRange(m_fShootRange) && bIsVisible;
	}

	bool IsTargetOnRange(float fRange)
	{
		return (transform.position - m_pTarget.transform.position).sqrMagnitude <= (fRange * fRange);
	}

	#endregion Methods


	#region Getters / Setters

	public GameObject Target
	{
		get { return m_pTarget; }
		set { m_pTarget = value; }
	}

	public float SightRange
	{
		get { return m_fSightRange; }
		set { m_fSightRange = value; }
	}

	static public int ObstaclesRaycastLayer
	{
		get { return s_iObstaclesRaycastLayer; }
	}

	#endregion Getters / Setters
}
