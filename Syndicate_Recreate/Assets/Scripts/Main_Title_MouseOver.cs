using UnityEngine;
using System.Collections;

public class Main_Title_MouseOver : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject m_pButtonMouseOverOverlay;
	[SerializeField]
	private GameObject m_pButtonClickedOverlay;
	
	#endregion
	
	#region Variables (private)
	
	
	
	#endregion

	
	public void MouseEnter()
	{
		m_pButtonMouseOverOverlay.SetActive(true);
	}

	public void MouseExit()
	{
		m_pButtonMouseOverOverlay.SetActive(false);
		m_pButtonClickedOverlay.SetActive(false);
	}

	public void MouseDown()
	{
		m_pButtonClickedOverlay.SetActive(true);
	}

	public void MouseUp()
	{
		m_pButtonClickedOverlay.SetActive(false);
	}
}
