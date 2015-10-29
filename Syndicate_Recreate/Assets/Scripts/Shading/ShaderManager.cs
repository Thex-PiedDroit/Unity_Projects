using UnityEngine;
using System.Collections;

public class ShaderManager : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private Material m_pMaterialReg;
	[SerializeField]
	private Material m_pMaterialTransp;
	
	#endregion
	
	#region Variables (private)

	private MeshRenderer m_tMeshRenderer;
	private Collider m_tCollider;
	
	#endregion


	void Start()
	{
		m_tMeshRenderer = GetComponent<MeshRenderer>();
	}
	
	void LateUpdate()
	{
		if (m_tMeshRenderer.isVisible)
		{
			Vector3 tCapsulePoint1 = transform.position - (transform.up * 0.5f);
			Vector3 tCapsulePoint2 = transform.position + (transform.up * 0.5f);

			bool bObstacleInSight = Physics.CapsuleCast(tCapsulePoint1, tCapsulePoint2, 0.5f, -Camera.main.transform.forward, Camera.main.farClipPlane, Map.ObstaclesLayer, QueryTriggerInteraction.Ignore);

			if (bObstacleInSight && (m_tMeshRenderer.material != m_pMaterialTransp))
				m_tMeshRenderer.material = m_pMaterialTransp;

			else if (m_tMeshRenderer.material != m_pMaterialReg)
				m_tMeshRenderer.material = m_pMaterialReg;
		}
	}
}
