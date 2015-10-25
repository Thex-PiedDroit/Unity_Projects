using UnityEngine;
using System.Collections;

public class SelectAllMouseOver : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject[] m_pSelectedPlayersGizmos = null;
	
	#endregion
	
	#region Variables (private)

	private bool[] m_pIsSelected = null;
	
	#endregion

	void Awake()
	{
		m_pIsSelected = new bool[m_pSelectedPlayersGizmos.Length];
	}


	public void MouseEnter()
	{
		for (int i = 0; i < m_pSelectedPlayersGizmos.Length; i++)
		{
			m_pIsSelected[i] = m_pSelectedPlayersGizmos[i].activeSelf;
			m_pSelectedPlayersGizmos[i].SetActive(true);
		}
	}

	public void MouseClick()
	{
		for (int i = 0; i < m_pIsSelected.Length; i++)
			m_pIsSelected[i] = true;
	}

	public void MouseExit()
	{
		for (int i = 0; i < m_pSelectedPlayersGizmos.Length; i++)
		{
			if (!m_pIsSelected[i])
				m_pSelectedPlayersGizmos[i].SetActive(false);
		}
	}
}
