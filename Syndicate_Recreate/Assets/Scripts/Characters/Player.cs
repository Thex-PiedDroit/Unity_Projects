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

	private bool m_bSelected = false;

	private float m_fDefaultHealthBarScaleY = 1.0f;
	
	#endregion


	void Start()
	{
		base.BehaviourStart();

		m_fDefaultHealthBarScaleY = tHealthBarGizmo.localScale.y;
	}
	
	void Update ()
	{
		if (m_bAlive)
		{
			if (m_tNavMeshAgent.hasPath)
				Debug.DrawLine(transform.position, m_tNavMeshAgent.destination, Color.magenta);

			if (m_bSelected)
			{
				if (Input.GetButton("Submit"))
				{
					if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
					{
						Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

						RaycastHit Hit;

						if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, Map.AllButObstaclesLayer, QueryTriggerInteraction.Ignore))
						{
							if (Hit.collider.tag != "UI")
							{
								if (Hit.collider.gameObject.GetComponent<LivingBeing>() != null)
								{
									m_pTarget = Hit.collider.gameObject;
									m_tNavMeshAgent.destination = m_pTarget.transform.position;
								}

								else
								{
									m_pTarget = null;
									m_tNavMeshAgent.destination = Hit.point;
								}
							}
						}
					}
				}

				if (Input.GetButton("Cancel"))
				{
					SelectUnit(false);
				}
			}

			base.BehaviourUpdate();
		}

		else
		{
			if (transform.forward != Vector3.down)
			{
				MissionManager.CharacterDead();
				transform.forward = Vector3.down;
				m_tNavMeshAgent.destination = transform.position;
				m_tNavMeshAgent.Stop();
			}
		}

		if (m_bJustTookDamage)
			UpdateHealthBar();
	}


	public void SelectUnit(bool bSelected)
	{
		m_bSelected = bSelected;
		tSelectedGizmo.SetActive(bSelected);
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
