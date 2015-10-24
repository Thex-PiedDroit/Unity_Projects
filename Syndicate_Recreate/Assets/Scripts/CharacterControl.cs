using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject tSphereTest;
	
	#endregion
	
	#region Variables (private)

	private Fight m_tFightScript;

	private NavMeshAgent m_tNavMesh;
	private TargetScript m_tTargetScript;
	
	#endregion



	void Awake ()
	{
		tSphereTest.SetActive(false);
		m_tNavMesh = GetComponent<NavMeshAgent>();
		m_tFightScript = GetComponent<Fight>();
		m_tTargetScript = GetComponent<TargetScript>();
	}
	

	void Update ()
	{
		if (m_tTargetScript.IsDead)
		{
			transform.forward = Vector3.down;
			m_tNavMesh.destination = transform.position;
			m_tNavMesh.Stop();
			tSphereTest.SetActive(false);
		}

		else
		{
			if (tSphereTest.activeInHierarchy && !m_tNavMesh.hasPath)
				tSphereTest.SetActive(false);


			if (Input.GetButtonDown("Click"))
			{
				Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				RaycastHit Hit;

				if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, ~Fight.ObstaclesRaycastLayer, QueryTriggerInteraction.Ignore))
				{
					tSphereTest.SetActive(true);
					tSphereTest.transform.position = Hit.point;


					if (Hit.collider.tag != "Target")
					{
						m_tFightScript.Target = null;
						m_tNavMesh.destination = Hit.point;
					}

					else
					{
						m_tFightScript.Target = Hit.collider.gameObject;
						m_tNavMesh.destination = m_tFightScript.Target.transform.position;
					}
				}
			}
		}
	}
}
