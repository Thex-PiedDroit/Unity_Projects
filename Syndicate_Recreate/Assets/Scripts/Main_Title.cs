using UnityEngine;
using System.Collections;

public class Main_Title : MonoBehaviour
{
	#region Variables (public)
	
	
	
	#endregion
	
	#region Variables (private)
	
	
	
	#endregion


	public void LoadMap(string pMapName)
	{
		Application.LoadLevel(pMapName);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
