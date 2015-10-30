using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class Main_Title : MonoBehaviour
{
	[SerializeField]
	private GameObject pMainTitle;
	[SerializeField]
	private GameObject pLevelSelect;


	private enum MenuState
	{
		MainTitle,
		LevelSelect
	}

	private MenuState m_eMenuState;


	public void LoadMap(string pMapName)
	{
		Application.LoadLevel(pMapName);
	}

	public void GoToMenu(string pMenuName)
	{
		switch(pMenuName)
		{
		case "MainTitle":

			m_eMenuState = MenuState.MainTitle;
			break;

		case "LevelSelect":

			m_eMenuState = MenuState.LevelSelect;
			break;

		default:

			Assert.IsTrue(false, "Menu name is incorrect");
			break;
		}

		pMainTitle.SetActive(m_eMenuState == MenuState.MainTitle);
		pLevelSelect.SetActive(m_eMenuState == MenuState.LevelSelect);
	}

	public void Back()
	{
		switch(m_eMenuState)
		{
		case MenuState.MainTitle:

			Application.Quit();
			break;

		case MenuState.LevelSelect:

			GoToMenu("MainTitle");
			break;
		}
	}
}
