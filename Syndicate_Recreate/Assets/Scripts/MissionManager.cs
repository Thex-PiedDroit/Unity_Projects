using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class MissionManager : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private MissionType m_eMissionType = MissionType.Kill;

	[SerializeField]
	private GameObject tMissionSucceededScreen;
	[SerializeField]
	private GameObject tMissionFailedScreen;
	[SerializeField]
	private GameObject tHUD;
	
	#endregion
	
	#region Variables (private)

	private enum MissionType
	{
		Kill,
		Persuade
	}


	static private MissionManager pThis;

	private GameObject[] pTargets;

	private Vector2 m_tEndTextLastFramePos = Vector2.zero;

	private int m_iRemainingTargets = 0;
	private bool m_bMissionSucceeded = false;
	private int m_iCharactersAlive = 0;
	private int m_iCharactersAtBay = 0;
	
	#endregion


	void Start()
	{
		pThis = this;

		pTargets = GameObject.FindGameObjectsWithTag("Target");
		m_iRemainingTargets = pTargets.Length;

		m_iCharactersAlive = GameObject.FindGameObjectsWithTag("PlayerCharacter").Length;
	}

	void Update()
	{
		if (tMissionSucceededScreen.activeInHierarchy ||
			tMissionFailedScreen.activeInHierarchy)
		{
			if (Input.anyKeyDown)
			{
				AI.ClearCharactersArray();

				Application.LoadLevel("Main_Title");
			}
		}

		else if (m_bMissionSucceeded &&
			m_iCharactersAtBay == m_iCharactersAlive)
		{
			if (Input.GetButtonUp("Enter"))
				EndMission(true);
		}
	}

	void OnGUI()
	{
		if (!tMissionSucceededScreen.activeInHierarchy &&
			!tMissionFailedScreen.activeInHierarchy &&
			m_bMissionSucceeded &&
			m_iCharactersAtBay == m_iCharactersAlive)
		{
			Vector2 tTextScreenPos = Camera.main.WorldToScreenPoint(transform.position + (Vector3.up * 10.0f));
			tTextScreenPos.y = Screen.height - tTextScreenPos.y;

			if (m_tEndTextLastFramePos != Vector2.zero)
				m_tEndTextLastFramePos = Vector2.Lerp(m_tEndTextLastFramePos, tTextScreenPos, 8.0f * Time.deltaTime);
			else
				m_tEndTextLastFramePos = tTextScreenPos;

			GUI.TextField(new Rect(m_tEndTextLastFramePos, new Vector2(167.0f, 20.0f)), "End mission? (Press Enter)");
		}
	}
	
	
	static public void TargetDead()
	{
		switch (pThis.m_eMissionType)
		{
		case MissionType.Kill:

			pThis.m_iRemainingTargets--;

			if (pThis.m_iRemainingTargets <= 0)
				pThis.m_bMissionSucceeded = true;
			break;

		case MissionType.Persuade:

			EndMission(false);
			break;
		}
	}

	static public void CharacterDead()
	{
		pThis.m_iCharactersAlive--;
		Assert.IsTrue(pThis.m_iCharactersAlive >= 0);

		if (pThis.m_iCharactersAlive == 0)
			EndMission(false);
	}

	static void EndMission(bool bSuccess)
	{
		pThis.tHUD.SetActive(false);
		pThis.tMissionSucceededScreen.SetActive(bSuccess);
		pThis.tMissionFailedScreen.SetActive(!bSuccess);
	}


	void OnCollisionEnter(Collision tCollider)
	{
		if (tCollider.gameObject.tag == "Target")
		{
			switch (m_eMissionType)
			{
			case MissionType.Kill:

				EndMission(false);
				break;

			case MissionType.Persuade:

				m_iRemainingTargets--;
				Assert.IsTrue(m_iRemainingTargets >= 0);

				if (m_iRemainingTargets == 0)
					m_bMissionSucceeded = true;
				break;
			}
		}

		else if (tCollider.gameObject.tag == "PlayerCharacter")
		{
			m_iCharactersAtBay++;
			Assert.IsTrue(m_iCharactersAtBay <= m_iCharactersAlive);
		}
	}

	void OnCollisionExit(Collision tCollider)
	{
		if (tCollider.gameObject.tag == "PlayerCharacter")
		{
			m_iCharactersAtBay--;
			Assert.IsTrue(m_iCharactersAtBay >= 0);
		}
	}
}
