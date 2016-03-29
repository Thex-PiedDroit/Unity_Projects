
using UnityEngine;
using System.Collections;



public class CameraBehavior : MonoBehaviour
{
	#region Variables (public)

	private GameObject[] pPlayers;
	private GameObject[] pPlayersFollowTargets;

	public float fFollowSpeed = 1.0f;
	public float fMinDistance = 280.0f;

	[Range (10.0f, 100.0f)]
	[SerializeField]
	public float fDistanceIn = 50.0f;
	[Range (20.0f, 100.0f)]
	[SerializeField]
	public float fDistanceOut = 80.0f;

	#endregion

	#region Variables (private)

	static private Vector2 tPlayersZoneInTopLeft = Vector2.zero;
	static private Vector2 tPlayersZoneInBottomRight = Vector2.zero;
	static private Vector2 tPlayersZoneOutTopLeft = Vector2.zero;
	static private Vector2 tPlayersZoneOutBottomRight = Vector2.zero;

	private Vector3 tPosBetweenPlayers;

	static private bool bSceneLoaded = false;

	private LoadQuitGame tLoadQuitGameScript;

    #endregion
	

	public void FindPlayers()
	{
		if (pPlayers == null)
			pPlayers = new GameObject[2];
		pPlayers[0] = GameObject.Find("Player1").transform.FindChild("Player").gameObject;
		pPlayers[1] = GameObject.Find("Player2").transform.FindChild("Player").gameObject;
		
		if (pPlayersFollowTargets == null)
			pPlayersFollowTargets = new GameObject[2];
		//pPlayersFollowTargets[0] = pPlayers[0].transform.parent.FindChild("FollowTarget").gameObject;
		//pPlayersFollowTargets[1] = pPlayers[1].transform.parent.FindChild("FollowTarget").gameObject;

		pPlayersFollowTargets[0] = GameObject.Find("FollowTargetP1");
		pPlayersFollowTargets[1] = GameObject.Find("FollowTargetP2");
	}

	void Start ()
	{
		if (tPlayersZoneInTopLeft == Vector2.zero)
		{
			tLoadQuitGameScript = GetComponent<LoadQuitGame>();

			float fDistInPercnt = fDistanceIn / 100.0f;
			tPlayersZoneInTopLeft.x = (Screen.width / 2.0f) - ((Screen.width * fDistInPercnt) / 2.0f);
			tPlayersZoneInBottomRight.y = (Screen.height / 2.0f) - ((Screen.height * fDistInPercnt) / 2.0f);
			tPlayersZoneInBottomRight.x = tPlayersZoneInTopLeft.x + (Screen.width * fDistInPercnt);
			tPlayersZoneInTopLeft.y = tPlayersZoneInBottomRight.y + (Screen.height * fDistInPercnt);

			float fDistOutPercnt = fDistanceOut / 100.0f;
			tPlayersZoneOutTopLeft.x = (Screen.width / 2.0f) - ((Screen.width * fDistOutPercnt) / 2.0f);
			tPlayersZoneOutBottomRight.y = (Screen.height / 2.0f) - ((Screen.height * fDistOutPercnt) / 2.0f);
			tPlayersZoneOutBottomRight.x = tPlayersZoneOutTopLeft.x + (Screen.width * fDistOutPercnt);
			tPlayersZoneOutTopLeft.y = tPlayersZoneOutBottomRight.y + (Screen.height * fDistOutPercnt);
		}
	}
	
	void FixedUpdate ()
	{
		Vector3[] tPlayersWorldPos = new Vector3[2];
		tPlayersWorldPos[0] = pPlayers[0].transform.position;
		tPlayersWorldPos[1] = pPlayers[1].transform.position;
		
		for (int i = 0; i < 2; i++)
		{
			FollowTargetBehavior tFollowScript = pPlayersFollowTargets[i].GetComponent<FollowTargetBehavior>();
		
			if (tFollowScript.OnOrbit)
			{
				tPlayersWorldPos[i] = tFollowScript.OrbitPlanetPos;
			}
		}
		
		tPosBetweenPlayers = (tPlayersWorldPos[0] + tPlayersWorldPos[1]) / 2.0f;

		transform.position = Vector3.Lerp(transform.position, new Vector3(tPosBetweenPlayers.x, tPosBetweenPlayers.y, transform.position.z), (fFollowSpeed * 2.0f) * Time.deltaTime * tLoadQuitGameScript.TimeScale);
		
		float fMoveZ = 0.0f;
		
		for (int i = 0; i < 2; i++)
		{
			float fCurrentPlayerMoveZ = 0.0f;
		
			Vector2 tPlayerPos = GetComponent<Camera>().WorldToScreenPoint(new Vector3(pPlayers[i].transform.position.x, pPlayers[i].transform.position.y, 0.0f));
		
			fCurrentPlayerMoveZ -= CheckOut(tPlayerPos);
			fCurrentPlayerMoveZ += CheckIn(tPlayerPos);
		
			if ((fCurrentPlayerMoveZ * fCurrentPlayerMoveZ) > (fMoveZ * fMoveZ))
				fMoveZ = fCurrentPlayerMoveZ;
		}
		
		float fNewZ = transform.position.z + fMoveZ;
		
		if (fNewZ > (-fMinDistance))
		{
			fNewZ = -fMinDistance;
		}

		transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, fNewZ), fFollowSpeed * Time.deltaTime * tLoadQuitGameScript.TimeScale);
	}

	void Update()
	{
		bSceneLoaded = true;
	}


	float CheckOut(Vector2 tPlayerPos)
	{
		float fMoveZ = 0.0f;

		bool bOutUp = tPlayerPos.y > tPlayersZoneOutTopLeft.y;
		bool bOutLeft = tPlayerPos.x < tPlayersZoneOutTopLeft.x;
		bool bOutRight = tPlayerPos.x > tPlayersZoneOutBottomRight.x;
		bool bOutDown = tPlayerPos.y < tPlayersZoneOutBottomRight.y;

		if (bOutLeft || bOutUp ||
			bOutRight || bOutDown)
		{
			float fOutValueX = 0.0f;
			float fOutValueY = 0.0f;

			if (bOutDown)
				fOutValueY = tPlayersZoneOutBottomRight.y - tPlayerPos.y;
			else if (bOutUp)
				fOutValueY = tPlayerPos.y - tPlayersZoneOutTopLeft.y;

			if (bOutRight)
				fOutValueX = tPlayerPos.x - tPlayersZoneOutBottomRight.x;
			else
				fOutValueX = tPlayersZoneOutTopLeft.x - tPlayerPos.x;


			float fFOVRad = GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad;

			if (fOutValueX > fOutValueY)
			{
				float fHorizontalFOV = 2 * Mathf.Atan(Mathf.Tan(fFOVRad / 2.0f) * GetComponent<Camera>().aspect);
				fMoveZ += (Mathf.Sin(fHorizontalFOV / 2.0f) * (fOutValueX));
			}

			else
			{
				fMoveZ += (Mathf.Sin(fFOVRad / 2.0f) * (fOutValueY));
			}
		}

		return fMoveZ;
	}

	float CheckIn(Vector2 tPlayerPos)
	{
		float fMoveZ = 0.0f;

		bool bTL = (tPlayerPos.x > tPlayersZoneInTopLeft.x) && (tPlayerPos.y < tPlayersZoneInTopLeft.y);
		bool bBR = (tPlayerPos.x < tPlayersZoneInBottomRight.x) && (tPlayerPos.y > tPlayersZoneInBottomRight.y);

		if (bTL && bBR)
		{
			float fInValueX = 0.0f;
			float fInValueY = 0.0f;


			float fDistInPercnt = fDistanceIn / 100.0f;
			Vector2 tCenterToPlayer = tPlayerPos - (new Vector2(Screen.width, Screen.height) / 2.0f);

			tCenterToPlayer.x *= tCenterToPlayer.x > 0.0f ? 1.0f : -1.0f;
			tCenterToPlayer.y *= tCenterToPlayer.y > 0.0f ? 1.0f : -1.0f;

			float fSizeX = (Screen.width * fDistInPercnt) / 2.0f;
			float fSizeY = (Screen.height * fDistInPercnt) / 2.0f;
			float fDistFromBorderX = fSizeX - tCenterToPlayer.x;
			float fDistFromBorderY = fSizeY - tCenterToPlayer.y;


			if (fDistFromBorderX > fDistFromBorderY)
			{
				fInValueX = fDistFromBorderX;
			}

			else
			{
				fInValueY = fDistFromBorderY;
			}


			float fFOVRad = GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad;

			if (fInValueX != 0.0f)
			{
				float fHorizontalFOV = 2 * Mathf.Atan(Mathf.Tan(fFOVRad / 2.0f) * GetComponent<Camera>().aspect);

				fMoveZ += (Mathf.Sin(fHorizontalFOV / 2.0f) * (fInValueX));
			}

			else if (fInValueY != 0.0f)
			{
				fMoveZ += (Mathf.Sin(fFOVRad / 2.0f) * (fInValueY));
			}
		}

		return fMoveZ;
	}


	void OnDestroy()
	{
		bSceneLoaded = false;
	}


	public bool SceneLoaded
	{
		get { return bSceneLoaded; }
	}
}
