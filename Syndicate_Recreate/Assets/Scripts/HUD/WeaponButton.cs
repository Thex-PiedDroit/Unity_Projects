using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class WeaponButton : MonoBehaviour
{
	#region Variables (private)

	private CharactersControl pCharactersControl;
	
	#endregion


	void Awake()
	{
		pCharactersControl = GameObject.Find("GoodGuys").GetComponent<CharactersControl>();
		EventTrigger.Entry tEntry = new EventTrigger.Entry();
		tEntry.eventID = EventTriggerType.PointerUp;
		tEntry.callback.AddListener(delegate { pCharactersControl.SendEquip(gameObject.name); });
		gameObject.GetComponent<EventTrigger>().triggers.Add(tEntry);
	}
}
