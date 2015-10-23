using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject tSphereTest;

	[SerializeField]
	private float m_fShootRange = 20.0f;
	[SerializeField]
	private float m_fAttackSpeed = 1.0f;
	[SerializeField]
	private float m_fDefaultDamages = 5.0f;
	
	#endregion
	
	#region Variables (private)

	private NavMeshAgent m_tNavMesh;
	static private int s_iObstacleRaycastLayer = ~(1 << 8);
	static private int s_iShootRaycastLayer = ~(1 << 9);

	private GameObject m_pTarget = null;
	private float m_fLastAttackTime = 0.0f;
	private bool m_bTargetInSight = false;
	
	#endregion



	void Awake ()
	{
		tSphereTest.SetActive(false);
		m_tNavMesh = GetComponent<NavMeshAgent>();
	}
	

	void Update ()
	{
		if (tSphereTest.activeInHierarchy && !m_tNavMesh.hasPath)
			tSphereTest.SetActive(false);


		if (m_pTarget)
		{
			if (!m_bTargetInSight)
			{
				m_bTargetInSight = IsTargetInSight();
			}

			if (TargetOnRange && m_bTargetInSight)
			{
				if (m_tNavMesh.hasPath)
				{
					m_tNavMesh.destination = transform.position;
					transform.LookAt(m_pTarget.transform);
				}

				Attack();

				if (m_pTarget.GetComponent<TargetScript>().IsDead)
				{
					m_pTarget = null;
					m_bTargetInSight = false;
				}
			}

			else
				m_tNavMesh.destination = m_pTarget.transform.position;
		}


		if (Input.GetButtonDown("Click"))
		{
			Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit Hit;

			if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, s_iObstacleRaycastLayer, QueryTriggerInteraction.Ignore))
			{
				tSphereTest.SetActive(true);
				tSphereTest.transform.position = Hit.point;


				if (Hit.collider.tag != "Target")
				{
					m_pTarget = null;
					m_tNavMesh.destination = Hit.point;
				}

				else
				{
					m_pTarget = Hit.collider.gameObject;
					m_tNavMesh.destination = m_pTarget.transform.position;
				}
			}
		}
	}


	void Attack()
	{
		if (Time.fixedTime - m_fLastAttackTime >= m_fAttackSpeed)
		{
			RaycastHit Hit;

			if (Physics.Raycast(transform.position, transform.forward, out Hit, m_fShootRange, s_iShootRaycastLayer, QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(transform.position, Hit.point, Color.red);

				TargetScript tShotThing = Hit.collider.gameObject.GetComponent<TargetScript>();

				if (tShotThing)
					tShotThing.Damage(m_fDefaultDamages);
			}
			
			m_fLastAttackTime = Time.fixedTime;
		}
	}


	bool IsTargetInSight()
	{
		RaycastHit Hit;
		return Physics.Raycast(transform.position, transform.forward, out Hit, m_fShootRange, s_iObstacleRaycastLayer, QueryTriggerInteraction.Ignore) && Hit.collider.gameObject == m_pTarget;
	}

	bool TargetOnRange
	{
		get
		{
			return (transform.position - m_pTarget.transform.position).sqrMagnitude <= (m_fShootRange * m_fShootRange);
		}
	}
}
