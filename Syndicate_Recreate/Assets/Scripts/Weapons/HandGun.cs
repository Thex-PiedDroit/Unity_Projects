using UnityEngine;
using System.Collections;

public class HandGun : Weapon
{
	public override void OnShoot(LivingBeing pShotThing)
	{
		pShotThing.ReceiveDamage(m_fDefaultDamages, m_pWielder);
	}
}
