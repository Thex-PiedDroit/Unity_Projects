using UnityEngine;
using System.Collections;

public class TargetScript : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fHealth = 10.0f;
	
	#endregion
	
	#region Variables (private)

	private bool m_bAlive = true;
	
	#endregion


	void Start ()
	{
		
	}
	
	void Update ()
	{

	}


	public void Damage(float fDamages)
	{
		m_fHealth -= fDamages;

		if (m_fHealth <= 0.0f)
		{
			m_bAlive = false;
			Destroy(gameObject);
		}

	}

	public bool IsDead
	{
		get { return !m_bAlive; }
	}
}
