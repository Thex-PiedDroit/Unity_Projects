
using UnityEngine;
using System.Collections;



public class FinishBehavior : MonoBehaviour
{
	#region Variables (public)

	

	#endregion

	#region Variables (private)

	[SerializeField]
	private Texture tWinP1;
	[SerializeField]
	private Texture tWinP2;

	private Transform tOrbit;

	int iPlayerWin = 0;

    #endregion


	void Start()
	{
		tOrbit = transform.FindChild("Orbit");
	}

	
	void Update ()
	{
		Transform tOrbittedTarget = tOrbit.FindChild("FollowTargetP1");
		if (!tOrbittedTarget)
			tOrbittedTarget = tOrbit.FindChild("FollowTargetP2");

		if (tOrbittedTarget)
		{
			string tPlayerName = tOrbittedTarget.GetComponent<FollowTargetBehavior>().PlayerName;

			if (tPlayerName == "Player1")
			{
				// P1 Win
				iPlayerWin = 1;
			}

			else
			{
				// P2 Win
				iPlayerWin = 2;
			}
		}


		if (GameObject.Find("Main Camera").GetComponent<LoadQuitGame>().TimeScale == 0)
		{
			iPlayerWin = 0;
		}
	}

	void OnGUI()
	{
		if (iPlayerWin == 1)
		{
			Vector2 tPos = new Vector2((Screen.width / 2.0f) - (tWinP1.width / 2.0f), (Screen.height / 2.0f) - (tWinP1.height / 2.0f));
			Rect tWinRect = new Rect(tPos, new Vector2(tWinP1.width, tWinP1.height));
			GUI.DrawTexture(tWinRect, tWinP1);
		}

		else if (iPlayerWin == 2)
		{
			Vector2 tPos = new Vector2((Screen.width / 2.0f) - (tWinP2.width / 2.0f), (Screen.height / 2.0f) - (tWinP2.height / 2.0f));
			Rect tWinRect = new Rect(tPos, new Vector2(tWinP2.width, tWinP2.height));
			GUI.DrawTexture(tWinRect, tWinP2);
		}
	}
}
