using UnityEngine;
using System.Collections;

public class Player : LivingBeing
{
	#region Variables (public)

	[Header("HUD")]
	[SerializeField]
	private GameObject tSphereTest;

	[SerializeField]
	private GameObject tSelectedGizmo;
	[SerializeField]
	private RectTransform tHealthBarGizmo;
	
	#endregion
	
	#region Variables (private)

	private bool m_bSelected = false;

	private float m_fDefaultHealthBarScaleY = 1.0f;
	
	#endregion


	void Awake ()
	{
		tSphereTest.SetActive(false);
	}

	void Start()
	{
		base.BehaviourStart();

		m_fDefaultHealthBarScaleY = tHealthBarGizmo.localScale.y;
	}
	
	void Update ()
	{
		if (m_bAlive)
		{
			if (tSphereTest.activeInHierarchy && !m_tNavMesh.hasPath)
				tSphereTest.SetActive(false);

			if (m_bSelected)
			{
				if (Input.GetButton("Submit"))
				{
					if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
					{
						Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

						RaycastHit Hit;

						if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, ~s_iObstaclesRaycastLayer, QueryTriggerInteraction.Ignore))
						{
							if (Hit.collider.tag != "UI")
							{
								tSphereTest.SetActive(true);
								tSphereTest.transform.position = Hit.point;

								if (Hit.collider.gameObject.GetComponent<LivingBeing>() != null)
								{
									m_pTarget = Hit.collider.gameObject;
									m_tNavMesh.destination = m_pTarget.transform.position;
								}

								else
								{
									m_pTarget = null;
									m_tNavMesh.destination = Hit.point;
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
			transform.forward = Vector3.down;
			m_tNavMesh.destination = transform.position;
			m_tNavMesh.Stop();
			tSphereTest.SetActive(false);
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

	void OnDestroy()
	{
		MissionManager.CharacterDead();
	}
}
