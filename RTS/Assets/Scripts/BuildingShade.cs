
using UnityEngine;
using System.Collections;



public class BuildingShade : MonoBehaviour
{
#region Variables (public)

	public Material m_pMaterial = null;

	public EBuildingType m_eType = EBuildingType.NONE;

	public bool CanBePlaced
	{
		get { return !m_bOnCollision; }
	}
	
	#endregion
	
#region Variables (private)

	private bool m_bOnCollision = false;
	private bool m_bPreviousState = false;
	
	#endregion
	

	void Start()
	{
		UpdateMatColor();
	}

	void LateUpdate()
	{
		if (m_bOnCollision != m_bPreviousState)
			UpdateMatColor();
	}

	void UpdateMatColor()
	{
		m_pMaterial.SetColor("_RimCol", m_bOnCollision ? Color.red : Color.green);
		m_bPreviousState = m_bOnCollision;
	}
	
	void OnCollisionStay(Collision pCollider)
	{
		if (pCollider.gameObject.tag == "Ground")
			return;

		m_bOnCollision = true;
	}

	void OnCollisionExit(Collision pCollider)
	{
		if (pCollider.gameObject.tag == "Ground")
			return;

		m_bOnCollision = false;
	}
}
