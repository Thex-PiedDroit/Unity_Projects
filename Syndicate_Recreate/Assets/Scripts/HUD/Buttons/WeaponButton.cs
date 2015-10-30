using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WeaponButton : MonoBehaviour
{
	#region Variables (private)

	private CharactersControl pCharactersControl;
	private WeaponsIcons pInventoryScript;

	private GameObject tIconSelected;
	
	#endregion


	void Awake()
	{
		EventTrigger pEventTrigger = gameObject.GetComponent<EventTrigger>();

		pCharactersControl = GameObject.Find("GoodGuys").GetComponent<CharactersControl>();
		EventTrigger.Entry tSendEquip = new EventTrigger.Entry();
		tSendEquip.eventID = EventTriggerType.PointerUp;
		tSendEquip.callback.AddListener(delegate { pCharactersControl.SendEquip(gameObject.name); });
		pEventTrigger.triggers.Add(tSendEquip);

		tIconSelected = transform.GetChild(0).gameObject;
		tIconSelected.SetActive(false);

		pInventoryScript = GameObject.FindObjectOfType<WeaponsIcons>();
		EventTrigger.Entry tPrintWeaponName = new EventTrigger.Entry();
		tPrintWeaponName.eventID = EventTriggerType.PointerEnter;
		tPrintWeaponName.callback.AddListener(delegate { pInventoryScript.PrintWeaponName(gameObject.name); });
		pEventTrigger.triggers.Add(tPrintWeaponName);

		EventTrigger.Entry tClearText = new EventTrigger.Entry();
		tClearText.eventID = EventTriggerType.PointerExit;
		tClearText.callback.AddListener(delegate { pInventoryScript.PrintWeaponName(""); });
		pEventTrigger.triggers.Add(tClearText);
	}

	public bool Equipped
	{
		set { tIconSelected.SetActive(value); }
		get { return tIconSelected.activeSelf; }
	}
}
