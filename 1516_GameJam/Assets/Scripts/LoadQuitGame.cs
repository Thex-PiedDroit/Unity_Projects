
using UnityEngine;
using System.Collections;



public class LoadQuitGame : MonoBehaviour
{
	public int TimeScale = 1;

	static private bool bLevelLoaded = false;
	private int iButtonSelected = 0;
	private int iButtonsCount = 2;

	private float fVerticalTimeEntered = 0.0f;
	[SerializeField]
	private float fContinuousInputDelay = 0.5f;

	[SerializeField]
	private GameObject tSelectImageNewGame;
	[SerializeField]
	private GameObject tSelectImageQuit;

	[SerializeField]
	private AudioSource tSound_Button;
	[SerializeField]
	private AudioSource tMusic;

	[SerializeField]
	private GameObject tScenePrefab;

	[SerializeField]
	private CameraBehavior tCameraScript;
	[SerializeField]
	private GameObject tSpaceDust;
	[SerializeField]
	private Canvas tCanvas;

	private GameObject tScene;

	void OnActive()
	{
		TimeScale = 0;

		tSound_Button.enabled = true;
		tMusic.enabled = false;
	}

	public void Load()
	{
		tSound_Button.enabled = false;
		tMusic.enabled = true;

		if (bLevelLoaded)
			DestroyImmediate(tScene);

		tScene = GameObject.Instantiate(tScenePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		tScene.name = "Scene";

		GetComponent<PlanetsSpawner>().SpawnPlanets();

		tCameraScript.FindPlayers();
		Unpause(bLevelLoaded);
		bLevelLoaded = true;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void Pause()
	{
		tSpaceDust.SetActive(false);
		tSound_Button.enabled = true;
		tMusic.Pause();

		tCameraScript.enabled = false;
		tCanvas.enabled = true;

		this.enabled = true;

		TimeScale = 0;
	}

	void Unpause(bool bNewGame)
	{
		tSpaceDust.SetActive(true);
		tSound_Button.enabled = false;

		if (bNewGame)
		{
			tMusic.Stop();
			tMusic.Play();
		}
		else
			tMusic.UnPause();

		tCameraScript.enabled = true;
		tCanvas.enabled = false;

		this.enabled = false;

		tCameraScript.FindPlayers();

		TimeScale = 1;
	}


	void Update()
	{
		if (Input.GetAxis("Vertical") >= 0.2f ||
			Input.GetAxis("Vertical") <= -0.2f)
		{
			if (fVerticalTimeEntered == 0)
			{
				ChangeSelection();
				fVerticalTimeEntered = Time.fixedTime;
			}

			else if (Time.fixedTime - fVerticalTimeEntered > fContinuousInputDelay)
				ChangeSelection();
		}

		else
			fVerticalTimeEntered = 0.0f;

		if (Input.GetButtonDown("Submit"))
		{
			if (iButtonSelected == 0)
				Load();
			else
				Application.Quit();
		}

		if (Input.GetButtonDown("Cancel") ||
			Input.GetButtonDown("Start"))
		{
			if (bLevelLoaded)
				Unpause(false);

			else
				Quit();
		}
	}


	void ChangeSelection()
	{
		iButtonSelected = (iButtonSelected + 1) % iButtonsCount;

		tSelectImageNewGame.SetActive(iButtonSelected == 0);
		tSelectImageQuit.SetActive(iButtonSelected == 1);

		tSound_Button.Play();
	}
}
