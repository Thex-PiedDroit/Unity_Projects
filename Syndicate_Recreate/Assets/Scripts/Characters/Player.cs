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
	
	#endregion


	protected override void Start()
	{
		base.Start();

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

	public string ActiveWeaponName
	{
		get
		{
			if (m_pActiveWeapon && m_pActiveWeapon.gameObject.activeSelf)
				return m_pActiveWeapon.name;

			return "No active weapon";
		}
	}

	public Weapon ActiveWeapon
	{
		set
		{
			bool bEquipNewWeapon = true;

			if (m_pActiveWeapon && m_pActiveWeapon.name != value.name)
				Destroy(m_pActiveWeapon.gameObject);

			else if (m_pActiveWeapon)
			{
				m_pActiveWeapon.gameObject.SetActive(!m_pActiveWeapon.gameObject.activeSelf);
				bEquipNewWeapon = false;
			}

			if (bEquipNewWeapon)
			{
				m_pActiveWeapon = Instantiate(value, transform.position, transform.rotation) as Weapon;
				m_pActiveWeapon.name = value.name;
				m_pActiveWeapon.transform.parent = transform;
			}
		}
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
