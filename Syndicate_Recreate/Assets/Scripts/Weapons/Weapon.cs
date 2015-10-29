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

	public virtual void Shoot(Transform pTarget)
	{
		if (Time.fixedTime - m_fLastAttackTime >= m_fFireSpeed)
		{
			RaycastHit Hit;

			if (Physics.Linecast(m_pGun.transform.position, pTarget.transform.position, out Hit, (Map.AllButGroundLayer & LivingBeing.AllButDeadsLayer), QueryTriggerInteraction.Ignore))
			{
				LivingBeing tShotThing = Hit.collider.gameObject.GetComponent<LivingBeing>();

				if (tShotThing)
					OnShoot(tShotThing);
			}

			m_fLastAttackTime = Time.fixedTime;
			m_pShootAnimation.Play();
		}
	}

	public abstract void OnShoot(LivingBeing pShotThing);

	public float AttackRange
	{
		get { return m_fAttackRange; }
	}

	public GameObject Icon
	{
		get { return WeaponsIcons.GetIcon(gameObject.name); }
	}
}
