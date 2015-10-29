using UnityEngine;
using System.Collections;

abstract public class Weapon : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	protected float m_fAttackRange = 20.0f;
	[SerializeField]
	protected float m_fFireSpeed = 1.0f;
	[SerializeField]
	protected float m_fDefaultDamages = 2.0f;
	
	#endregion
	
	#region Variables (private)

	protected GameObject m_pWielder;
	protected GameObject m_pHandle;
	protected Animation m_pShootAnimation;
	protected GameObject m_pGun;

	protected float m_fLastAttackTime = 0.0f;
	
	#endregion

	void Start()
	{
		m_pWielder = transform.parent.gameObject;
		m_pHandle = transform.FindChild("Handle").gameObject;
		m_pShootAnimation = m_pHandle.GetComponent<Animation>();
		m_pGun = m_pHandle.transform.FindChild("Gun").gameObject;
	}

	public abstract void Shoot(Transform pTarget);

	public float AttackRange
	{
		get { return m_fAttackRange; }
	}

	public GameObject Icon
	{
		get { return WeaponsIcons.GetIcon(gameObject.name); }
	}
}
