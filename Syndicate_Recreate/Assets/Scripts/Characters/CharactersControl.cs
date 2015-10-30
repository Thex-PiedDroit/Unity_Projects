using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class CharactersControl : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float m_fDistanceBetweenCharacters = 1.5f;

	[SerializeField]
	private Weapon[] m_pWeaponsInventory;
	[SerializeField]
	private Vector3 m_pInventoryFrameOffset = Vector3.zero;
	
	#endregion
	
	#region Variables (private)

	private Transform pHUDInventory;
	private GameObject[] m_pRenderedWeaponIcons = new GameObject[8];

	static private Player[] s_pCharacters = null;
	private byte m_iSelectedCharacters = 0;				// Bitfield selection
	private int m_iSelectedCharactersCount = 0;

	private int m_iRenderedWeaponIcons = 0;

	#endregion


	void Awake()
	{
		pHUDInventory = GameObject.Find("Inventory").transform;

		s_pCharacters = FindObjectsOfType<Player>();

		for (int i = 0; i < m_pWeaponsInventory.Length; i++)
		{
			RenderWeaponIcon(m_pWeaponsInventory[i].Icon, m_pWeaponsInventory[i].name);
		}
	}
	
	void Update()
	{
		if (Input.GetButtonDown("Submit"))
		{
			if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				Vector3 tMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				RaycastHit Hit;

				if (Physics.Raycast(tMousePos, Camera.main.transform.forward, out Hit, float.MaxValue, Map.AllButObstaclesLayer, QueryTriggerInteraction.Ignore))
				{
					if (Hit.collider.tag != "UI")
					{
						LivingBeing tLivingBeing = Hit.collider.gameObject.GetComponent<LivingBeing>();

						if (tLivingBeing != null && !tLivingBeing.IsDead)
						{
							if (!tLivingBeing.IsPlayer)
								SendTarget(tLivingBeing.gameObject);

							else
								SelectCharacter(Hit.collider.gameObject.GetComponent<Player>());
						}

						else
							SendDestination(Hit.point);
					}
				}
			}
		}

		if (Input.GetButton("Cancel"))
			DeselectAll();
	}

	void OnSelectionChange()
	{
		UpdateWeaponSelected();
	}


	#region Methods

	void SendTarget(GameObject pTarget)
	{
		for (int i = 0; i < s_pCharacters.Length; i++)
		{
			if ((m_iSelectedCharacters & (1 << i)) != 0)
			{
				s_pCharacters[i].Target = pTarget;
			}
		}
	}

	void SendDestination(Vector3 tPoint)
	{
		SendTarget(null);

		/* Find direction from centroid of selected characters */

		Vector3 tCentroid = Vector3.zero;
		int i = 0;

		for (i = 0; i < s_pCharacters.Length; i++)
		{
			if ((m_iSelectedCharacters & (1 << i)) != 0)
			{
				tCentroid += s_pCharacters[i].transform.position;
			}
		}

		tCentroid.y = 0.0f;
		tCentroid /= i;

		Vector3 tDirection = (tPoint - tCentroid).normalized;


		/* Find first point to the left of tPoint which characters will position themselves from */

		Vector3 tDirectionLeft = new Vector3(-tDirection.z, 0.0f, tDirection.x);

		float fFirstToLastDist = m_fDistanceBetweenCharacters * (m_iSelectedCharactersCount - 1);

		Vector3 tFirstPoint = tPoint + (tDirectionLeft * (fFirstToLastDist / 2.0f));


		/* Set character's destination from left to right of tPoint */

		int iCharactersSentCount = 0;
		for (i = 0; i < s_pCharacters.Length; i++)
		{
			if ((m_iSelectedCharacters & (1 << i)) != 0)
			{
				Vector3 tOffset = -tDirectionLeft * (iCharactersSentCount * m_fDistanceBetweenCharacters);
				s_pCharacters[i].Destination = tFirstPoint + tOffset;

				iCharactersSentCount++;
			}
		}
	}

	public void SelectCharacter(int iCharacterNumber)
	{
		int iCharacterBit = 1 << iCharacterNumber;

		if ((m_iSelectedCharacters & iCharacterBit) == 0 &&
			!s_pCharacters[iCharacterNumber].IsDead)
		{
			m_iSelectedCharacters |= (byte)iCharacterBit;
			m_iSelectedCharactersCount++;

			s_pCharacters[iCharacterNumber].ActivateSelectedGizmo(true);
		}

		OnSelectionChange();
	}

	public void SelectCharacter(Player pCharacter)
	{
		for (int i = 0; i < s_pCharacters.Length; i++)
		{
			if (pCharacter == s_pCharacters[i])
				SelectCharacter(i);
		}
	}

	public void DeselectCharacter(int iCharacterNumber)
	{
		int iCharacterBit = 1 << iCharacterNumber;

		if ((m_iSelectedCharacters & iCharacterBit) != 0)
		{
			m_iSelectedCharacters &= (byte)~iCharacterBit;
			m_iSelectedCharactersCount--;

			s_pCharacters[iCharacterNumber].ActivateSelectedGizmo(false);
		}

		OnSelectionChange();
	}

	public void DeselectCharacter(Player pCharacter)
	{
		for (int i = 0; i < s_pCharacters.Length; i++)
		{
			if (pCharacter == s_pCharacters[i])
				DeselectCharacter(i);
		}
	}

	public void DeselectAll()
	{
		m_iSelectedCharacters = 0;
		m_iSelectedCharactersCount = 0;

		for (int i = 0; i < s_pCharacters.Length; i++)
		{
			s_pCharacters[i].ActivateSelectedGizmo(false);
		}

		OnSelectionChange();
	}

	void RenderWeaponIcon(GameObject pIcon, string pWeaponName)
	{
		GameObject tNewIcon = Instantiate(pIcon, pHUDInventory.position, pHUDInventory.rotation) as GameObject;
		tNewIcon.name = pWeaponName;
		tNewIcon.transform.SetParent(pHUDInventory, false);

		int x = m_iRenderedWeaponIcons % 4;
		int y = -(m_iRenderedWeaponIcons / 4);
		RectTransform tIconRect = tNewIcon.GetComponent<RectTransform>();

		Vector3 tLocalPos = m_pInventoryFrameOffset + new Vector3(tIconRect.rect.size.x * x, tIconRect.rect.size.y * y, 0.0f);
		tNewIcon.transform.localPosition = tLocalPos;

		m_pRenderedWeaponIcons[m_iRenderedWeaponIcons] = tNewIcon;
		m_iRenderedWeaponIcons++;
	}

	public void SendEquip(string pWeaponName)
	{
		int iWeaponID = FindWeaponByName(pWeaponName);

		Assert.IsTrue(iWeaponID != -1, "No weapon to equip! Must be a button error");

		byte iHasTheWeapon = 0;
		int iHasTheWeaponCount = 0;

		for (int j = 0; j < s_pCharacters.Length; j++)
		{
			if ((m_iSelectedCharacters & (1 << j)) != 0 &&
				s_pCharacters[j].ActiveWeaponName == pWeaponName)
			{
				iHasTheWeapon |= (byte)(1 << j);
				iHasTheWeaponCount++;
			}
		}

		bool bSendToAll = iHasTheWeapon == 0 || (iHasTheWeaponCount == m_iSelectedCharactersCount);

		for (int j = 0; j < s_pCharacters.Length; j++)
		{
			if ((m_iSelectedCharacters & (1 << j)) != 0)
			{
				if (bSendToAll || ((iHasTheWeapon & (1 << j)) == 0))
					s_pCharacters[j].ActiveWeapon = m_pWeaponsInventory[iWeaponID];
			}
		}

		UpdateWeaponSelected();
	}

	void UpdateWeaponSelected()
	{
		string pActiveWeapon = null;
		bool bCommonWeapon = true;

		if (m_iSelectedCharacters != 0)
		{
			for (int i = 0; i < s_pCharacters.Length; i++)
			{
				if ((m_iSelectedCharacters & (1 << i)) != 0)
				{
					string pCharActiveWeapon = s_pCharacters[i].ActiveWeaponName;

					if (pActiveWeapon == null && pCharActiveWeapon != "No active weapon")
					{
						pActiveWeapon = pCharActiveWeapon;
					}

					else if (pCharActiveWeapon != pActiveWeapon)
					{
						bCommonWeapon = false;
						break;
					}
				}
			}
		}

		else
			bCommonWeapon = false;

		for (int i = 0; i < m_iRenderedWeaponIcons; i++)
			m_pRenderedWeaponIcons[i].GetComponent<WeaponButton>().Equipped = false;

		if (bCommonWeapon)
		{
			int iWeaponID = FindWeaponByName(pActiveWeapon);
			m_pRenderedWeaponIcons[iWeaponID].GetComponent<WeaponButton>().Equipped = true;
		}
	}

	int FindWeaponByName(string pWeaponName)
	{
		int iWeaponID = -1;

		for (int i = 0; i < m_pWeaponsInventory.Length; i++)
		{
			if (m_pWeaponsInventory[i].name == pWeaponName)
			{
				iWeaponID = i;
			}
		}

		return iWeaponID;
	}

	#endregion Methods


	public static Player[] PlayerCharacters
	{
		get { return s_pCharacters; }
	}
}
