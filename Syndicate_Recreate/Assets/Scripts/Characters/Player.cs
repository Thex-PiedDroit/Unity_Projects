using UnityEngine;
using System.Collections;

public class Player : LivingBeing
{
	#region Variables (public)

	[Header("HUD")]

	[SerializeField]
	private GameObject tSelectedGizmo;
	[SerializeField]
	private RectTransform tHealthBarGizmo;
	
	#endregion
	
	#region Variables (private)

	private GameObject m_pAttacker = null;

	private float m_fDefaultHealthBarScaleY = 1.0f;

	private float m_fKeepAttackerInCacheDuration = 2.0f;
	private float m_fLastAttackedTime = 0.0f;

	static private Player[] s_pCharacters;
	
	#endregion


	protected override void Start()
	{
		base.Start();

		s_pCharacters = CharactersControl.PlayerCharacters;

		m_fDefaultHealthBarScaleY = tHealthBarGizmo.localScale.y;
	}
	
	protected override void Update ()
	{
		if (m_bAlive)
		{
			if (m_tNavMeshAgent.hasPath)
				Debug.DrawLine(transform.position, m_tNavMeshAgent.destination, Color.magenta);

			if (m_pAttacker && (Time.fixedTime - m_fLastAttackedTime) >= m_fKeepAttackerInCacheDuration)
				m_pAttacker = null;

			base.Update();
		}
	}

	protected override void OnAttacked(GameObject pAttacker)
	{
		base.OnAttacked(pAttacker);

		m_pAttacker = pAttacker;
		m_fLastAttackedTime = Time.fixedTime;
		UpdateHealthBar();

		for (int i = 0; i < s_pCharacters.Length; i++)
		{
			if (s_pCharacters[i] != this)
			{
				bool bIsPlayerInRange = (transform.position - s_pCharacters[i].transform.position).sqrMagnitude <= (m_fSightRange * m_fSightRange);

				if (s_pCharacters[i].m_pTarget == null && bIsPlayerInRange &&
					s_pCharacters[i].m_tNavMeshAgent.hasPath == false)
				{
					s_pCharacters[i].m_pTarget = pAttacker;
					s_pCharacters[i].m_bPursuingTarget = true;
				}
			}
		}
	}

	protected override void OnDeath()
	{
		base.OnDeath();
		transform.parent.parent.gameObject.GetComponent<CharactersControl>().DeselectCharacter(this);
		m_pAttacker = null;
		MissionManager.CharacterDead();
	}


	public void ActivateSelectedGizmo(bool bActivated)
	{
		tSelectedGizmo.SetActive(bActivated);
	}

	void UpdateHealthBar()
	{
		float fHealthPercentage = m_fHealth / m_fMaxHealth;
		float fNewScale = m_fDefaultHealthBarScaleY * fHealthPercentage;

		Vector3 tScale = tHealthBarGizmo.localScale;
		tScale.y = fNewScale;
		tHealthBarGizmo.localScale = tScale;
	}


	public Vector3 Destination
	{
		set { m_tNavMeshAgent.SetDestination(value); }
	}

	public GameObject Attacker
	{
		get { return m_pAttacker; }
	}


	void OnTriggerEnter(Collider tTrigger)
	{
		if (tTrigger.gameObject.tag == "Building")
		{
			tTrigger.gameObject.GetComponent<BuildingBehaviour>().ToggleFloorsAboveRenderers();
		}
	}

	void OnTriggerExit(Collider tTrigger)
	{
		if (tTrigger.gameObject.tag == "Building")
		{
			tTrigger.gameObject.GetComponent<BuildingBehaviour>().ToggleFloorsAboveRenderers();
		}
	}
}
