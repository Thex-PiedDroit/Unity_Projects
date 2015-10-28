using UnityEngine;
using System.Collections;

public class Main_Title : MonoBehaviour
{
	public void LoadMap(string pMapName)
	{
		Application.LoadLevel(pMapName);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
