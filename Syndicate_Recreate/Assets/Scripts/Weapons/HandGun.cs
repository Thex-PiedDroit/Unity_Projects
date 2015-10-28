using UnityEngine;
using System.Collections;

public class HandGun : Weapon
{
	public override void Shoot(Transform pTarget)
	{
		if (Time.fixedTime - m_fLastAttackTime >= m_fFireSpeed)
		{
			RaycastHit Hit;

			if (Physics.Linecast(m_pGun.transform.position, pTarget.transform.position, out Hit, (Map.AllButGroundLayer & LivingBeing.AllButDeadsLayer), QueryTriggerInteraction.Ignore))
			{
				Debug.DrawLine(m_pGun.transform.position, Hit.point, Color.red);

				LivingBeing tShotThing = Hit.collider.gameObject.GetComponent<LivingBeing>();

				if (tShotThing)
					tShotThing.ReceiveDamage(m_fDefaultDamages, m_pWielder);
			}

			m_fLastAttackTime = Time.fixedTime;
			m_pShootAnimation.Play();
		}
	}
}
