using UnityEngine;
using System.Collections;

public class Player : LivingBeing
{
	#region Variables (public)

	[SerializeField]
	private GameObject tSphereTest;
	
	#endregion
	
	#region Variables (private)
	
	
	
	#endregion


	void Awake ()
	{
		tSphereTest.SetActive(false);
	}

	void Start()
	{
		base.BehaviourStart();
	}
	
	void Update ()
	{
		if (m_bAlive)
		{
			if (tSphereTest.activeInHierarchy && !m_tNavMesh.hasPath)
				tSphereTest.SetActive(false);


			if (Input.GetButtonDown("Click"))
			{
				Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				RaycastHit Hit;

				if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, ~s_iObstaclesRaycastLayer, QueryTriggerInteraction.Ignore))
				{
					tSphereTest.SetActive(true);
					tSphereTest.transform.position = Hit.point;

					if (Hit.collider.tag == "Target")
					{
						m_pTarget = Hit.collider.gameObject;
						m_tNavMesh.destination = m_pTarget.transform.position;
					}

					else
					{
						m_pTarget = null;
						m_tNavMesh.destination = Hit.point;
					}
				}
			}

			base.BehaviourUpdate();
		}

		else
		{
			transform.forward = Vector3.down;
			m_tNavMesh.destination = transform.position;
			m_tNavMesh.Stop();
			tSphereTest.SetActive(false);
		}
	}
}
