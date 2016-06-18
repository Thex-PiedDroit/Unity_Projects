
using UnityEngine;
using System.Collections;



public class UIManager : MonoBehaviour
{
#region Variables (public)

	static public UIManager Instance = null;


	public bool MouseOverMenu
	{
		set { m_bMouseOverMenu = value; }
		get { return m_bMouseOverMenu; }
	}

	#endregion

#region Variables (private)

	private bool m_bMouseOverMenu = false;

    #endregion
	

	void Awake()
	{
		if (Instance != null)
		{
			if (Instance != this)
				Destroy(this);
			return;
		}

		Instance = this;
	}
}
