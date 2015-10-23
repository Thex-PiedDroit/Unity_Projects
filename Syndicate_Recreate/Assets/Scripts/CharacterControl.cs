using UnityEngine;
using System.Collections;

public class CharacterControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject tSphereTest;
	
	#endregion
	
	#region Variables (private)

	private NavMeshAgent m_tNavMesh;

	private int iGroundLayer = ~(1 << 8);
	
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


		if (Input.GetButtonDown("Fire1"))
		{
			Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit Hit;

			if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, iGroundLayer, QueryTriggerInteraction.Ignore))
			{
				tSphereTest.SetActive(true);
				tSphereTest.transform.position = Hit.point;
				m_tNavMesh.destination = Hit.point;
			}
		}
	}
}
