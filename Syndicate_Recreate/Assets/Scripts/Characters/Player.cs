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

	private float m_fDefaultHealthBarScaleY = 1.0f;
	
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

			base.Update();
		}

		else
		{
			if (transform.forward != Vector3.down)
			{
				
				transform.forward = Vector3.down;
				m_tNavMeshAgent.destination = transform.position;
				
			}
		}

		if (m_bJustTookDamage)
			UpdateHealthBar();
	}

	protected override void OnDeath()
	{
		base.OnDeath();
		transform.parent.parent.gameObject.GetComponent<CharactersControl>().DeselectCharacter(this);
		MissionManager.CharacterDead();
	}


	public void ActivateSelectedGizmo(bool bActivated)
	{
		tSelectedGizmo.SetActive(bActivated);
	}

	void UpdateHealthBar()
	{
		float fHealthPercentage = (m_fMaxHealth / 100.0f) * m_fHealth;
		float fNewScale = m_fDefaultHealthBarScaleY * fHealthPercentage;

		Vector3 tScale = tHealthBarGizmo.localScale;
		tScale.y = fNewScale;
		tHealthBarGizmo.localScale = tScale;

		m_bJustTookDamage = false;
	}


	public Vector3 Destination
	{
		set { m_tNavMeshAgent.SetDestination(value); }
	}

	public Weapon ActiveWeapon
	{
		set
		{
			bool bSameWeapon = false;

			if (m_pActiveWeapon != null && m_pActiveWeapon.name == value.name)
			{
				m_pActiveWeapon.gameObject.SetActive(!m_pActiveWeapon.gameObject.activeSelf);

				bSameWeapon = true;
			}
			
			else if (m_pActiveWeapon)
				Destroy(m_pActiveWeapon.gameObject);

			if (!bSameWeapon)
			{
				m_pActiveWeapon = Instantiate(value, transform.position, transform.rotation) as Weapon;
				m_pActiveWeapon.name = value.name;
				m_pActiveWeapon.transform.parent = transform;
			}
		}
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
