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
	
	#endregion
	
	#region Variables (private)

	private NavMeshAgent m_tNavMesh;
	private int iGroundLayer = ~(1 << 8);

	private GameObject m_pTarget = null;
	private float m_fLastAttackTime = 0.0f;
	
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
			if ((transform.position - m_pTarget.transform.position).sqrMagnitude <= (m_fShootRange * m_fShootRange))
			{
				m_tNavMesh.Stop();
				Attack();

				if (m_pTarget.GetComponent<TargetScript>().IsDead)
					m_pTarget = null;
			}
		}


		if (Input.GetButtonDown("Click"))
		{
			Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit Hit;

			if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, iGroundLayer, QueryTriggerInteraction.Ignore))
			{
				tSphereTest.SetActive(true);
				tSphereTest.transform.position = Hit.point;
				m_tNavMesh.destination = Hit.point;

				if (Hit.collider.tag != "Target")
					m_pTarget = null;

				else
					m_pTarget = Hit.collider.gameObject;

				m_tNavMesh.Resume();
			}
		}
	}


	void Attack()
	{
		if (Time.fixedTime - m_fLastAttackTime >= m_fAttackSpeed)
		{
			m_pTarget.GetComponent<TargetScript>().Damage(5.0f);
			m_fLastAttackTime = Time.fixedTime;
		}
	}
}
