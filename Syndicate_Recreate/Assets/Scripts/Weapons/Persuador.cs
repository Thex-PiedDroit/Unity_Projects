using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Persuador : Weapon
{
	public override void OnShoot(LivingBeing pShotThing)
	{
		AI tAI = pShotThing.GetComponent<AI>();
		Assert.IsNotNull(tAI, "Trying to persuade non-AI target?");

		tAI.Persuade(m_pWielder);
	}

	private Vector2 m_tEndTextLastFramePos = Vector2.zero;

	void OnGUI()
	{
		if (m_pShootAnimation.isPlaying)
		{
			Vector2 tTextScreenPos = Camera.main.WorldToScreenPoint(transform.position - (transform.right * 1.0f) + (Vector3.up * 3.0f));
			tTextScreenPos.y = Screen.height - tTextScreenPos.y;

			if (m_tEndTextLastFramePos != Vector2.zero)
				m_tEndTextLastFramePos = Vector2.Lerp(m_tEndTextLastFramePos, tTextScreenPos, 8.0f * Time.deltaTime);
			else
				m_tEndTextLastFramePos = tTextScreenPos;

			GUI.TextField(new Rect(m_tEndTextLastFramePos, new Vector2(53.0f, 20.0f)), "Wololo");
		}
	}
}
