using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class SelectButton : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private int m_iPlayerNum;
	
	#endregion
	
	#region Variables (private)

	private CharactersControl pGoodGuysContainer;

	private Player pPlayer;
	private GameObject tIconSelected;
	
	#endregion


	void Awake()
	{
		pGoodGuysContainer = GameObject.Find("GoodGuys").GetComponent<CharactersControl>();
		pPlayer = pGoodGuysContainer.gameObject.transform.GetChild(m_iPlayerNum).gameObject.GetComponentInChildren<Player>();

		EventTrigger.Entry tEntry = new EventTrigger.Entry();
		tEntry.eventID = EventTriggerType.PointerUp;
		tEntry.callback.AddListener(delegate { pGoodGuysContainer.SelectCharacter(pPlayer); });
		gameObject.GetComponent<EventTrigger>().triggers.Add(tEntry);
	}
	
	void Update()
	{
		
	}
}
