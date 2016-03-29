
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class PlanetsSpawner : MonoBehaviour
{
	#region Variables (public)

	public int iPlanetsMaxCount = 100;

	public GameObject tSquidReference;
	[Range(0.0f, 100.0f)]
	[SerializeField]
	public float fSquidSpawnChances = 3.0f;


	public GameObject BlackHoleReference;

	[Range(0.0f, 100.0f)]
	[SerializeField]
	public float fBlackHoleSpawnChances = 15.0f;
	[Range(1.0f, 3.3f)]
	[SerializeField]
	float fBlackHoleRadiusMin = 2.0f;
	[Range(1.0f, 3.3f)]
	[SerializeField]
	float fBlackHoleRadiusMax = 3.3f;

	public float fMinimalDistanceBetweenBlackHoles = 1.0f;

	public List<GameObject> PlanetsReferences = new List<GameObject>();

	[Range(0.5f, 3.3f)]
	[SerializeField]
	float fPlanetRadiusMin = 0.5f;
	[Range(0.5f, 3.3f)]
	[SerializeField]
	float fPlanetRadiusMax = 3.3f;

	public float fMinimalDistanceBetweenPlanets = -1.5f;

	#endregion

	#region Variables (private)

	private int iMaxPlacementAttemptsCount = 50;
	private string pPlanetsInstancesName = "Planet";
	private string pSquidsInstancesName = "SquidOrbit";

	GameObject tPlanetsHolder;
	GameObject tSquidsHolder;

    #endregion

	public void SpawnPlanets()
	{
		tPlanetsHolder = GameObject.Find("Planets");

		if (!tPlanetsHolder)
		{
			tPlanetsHolder = new GameObject("Planets");
			tPlanetsHolder.transform.position = Vector3.zero;
		}

		tSquidsHolder = GameObject.Find("Squids");

		if (!tSquidsHolder)
		{
			tSquidsHolder = new GameObject("Squids");
			tSquidsHolder.transform.position = Vector3.zero;
		}

		DeletePlanets();

		GameObject pSafetyPlanes = GameObject.Find("Safety planes");
		if (pSafetyPlanes)
			pSafetyPlanes.SetActive(false);
		

		Vector2 tMapTopLeft = new Vector2(0.0f, 0.0f);
		Vector2 tMapBottomRight = new Vector2(0.0f, 0.0f);
		GetScreenBorders(ref tMapTopLeft, ref tMapBottomRight);

		GameObject[] pCreatedPlanets = new GameObject[iPlanetsMaxCount];
		GameObject[] pCreatedSquids = new GameObject[iPlanetsMaxCount];

		int iCreatedPlanetsCount = 0;
		int iCreatedSquidsCount = 0;


		for (int i = 0; i < iPlanetsMaxCount; i++)
		{
			float fDistanceBetweenPlanets = fMinimalDistanceBetweenPlanets;
			float fLocalScale = 0.0f;
			float fRadius = 0.0f;
			float fPosX = 0.0f;
			float fPosY = 0.0f;

			bool bInvalidPos = false;
			int iAttempts = 0;

			GameObject tPlanetRef;

			float fBlackHoleRoll = Random.Range(0.0f, 100.0f);
			bool bBlackHole = fBlackHoleRoll <= fBlackHoleSpawnChances;

			if (bBlackHole)
			{
				tPlanetRef = BlackHoleReference;

				fLocalScale = BlackHoleReference.transform.FindChild("BlackHole_Transparent").localScale.x;
				fRadius = Random.Range(fBlackHoleRadiusMin, fBlackHoleRadiusMax) * fLocalScale;

				fDistanceBetweenPlanets = fMinimalDistanceBetweenBlackHoles;
			}

			else
			{
				int iPlanetType = Random.Range(0, PlanetsReferences.Count);
				tPlanetRef = PlanetsReferences[iPlanetType];

				fLocalScale = PlanetsReferences[iPlanetType].transform.Find("Globe").localScale.x;
				fRadius = Random.Range(fPlanetRadiusMin, fPlanetRadiusMax) * fLocalScale;
			}


			do
			{
				fPosX = Random.Range(tMapTopLeft.x + fRadius + fDistanceBetweenPlanets, tMapBottomRight.x - fRadius - fMinimalDistanceBetweenPlanets);
				fPosY = Random.Range(tMapBottomRight.y + fRadius + fDistanceBetweenPlanets, tMapTopLeft.y - fRadius - fMinimalDistanceBetweenPlanets);
				Vector2 tPos = new Vector2(fPosX, fPosY);

				if (Physics2D.OverlapCircle(tPos, ((fRadius * 2.0f) * 1.5f) + fDistanceBetweenPlanets) != null)
				{
					bInvalidPos = true;
					iAttempts++;
				}

				else
				{
					fRadius /= fLocalScale;

					pCreatedPlanets[iCreatedPlanetsCount] = Instantiate(tPlanetRef, new Vector3(tPos.x, tPos.y, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
					pCreatedPlanets[iCreatedPlanetsCount].transform.localScale = new Vector3(fRadius, fRadius, fRadius);

					float fRandomRotation = Random.Range(-360.0f, 360.0f);
					pCreatedPlanets[iCreatedPlanetsCount].transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), fRandomRotation);

					pCreatedPlanets[iCreatedPlanetsCount].transform.parent = tPlanetsHolder.transform;
					pCreatedPlanets[iCreatedPlanetsCount].name = pPlanetsInstancesName;


					if (!bBlackHole)
					{
						float fSquidSpawnRoll = Random.Range(0.0f, 100.0f);

						if (fSquidSpawnRoll <= fSquidSpawnChances)
						{
							pCreatedSquids[iCreatedSquidsCount] = Instantiate(tSquidReference, new Vector3(tPos.x, tPos.y, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f)) as GameObject;
							pCreatedSquids[iCreatedSquidsCount].transform.FindChild("Squid").gameObject.GetComponent<SquidBehavior>().tAssignedPlanet = pCreatedPlanets[iCreatedPlanetsCount];
							pCreatedSquids[iCreatedSquidsCount].transform.parent = tSquidsHolder.transform;
							pCreatedSquids[iCreatedSquidsCount].name = pSquidsInstancesName;

							iCreatedSquidsCount++;
						}
					}


					iCreatedPlanetsCount++;

					bInvalidPos = false;
				}

			} while (bInvalidPos && iAttempts < iMaxPlacementAttemptsCount);
		}
	}


	public void DeletePlanets()
	{
		bool bPlanetDestroyed = false;

		do
		{
			bPlanetDestroyed = false;

			Transform tCurrentPlanet = GameObject.Find("Planets").transform.FindChild(pPlanetsInstancesName);
			if (tCurrentPlanet == null)
				tCurrentPlanet = GameObject.Find("Squids").transform.FindChild(pSquidsInstancesName);

			if (tCurrentPlanet)
			{
				GameObject tPlanetObject = tCurrentPlanet.gameObject;

				DestroyImmediate(tPlanetObject);

				bPlanetDestroyed = true;
			}

		} while (bPlanetDestroyed);
	}


	void GetScreenBorders(ref Vector2 tMapTopLeft, ref Vector2 tMapBottomRight)
	{
		Vector2[] tWallsPositions = new Vector2[4];

		Vector2[] pCardinalPoints = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

		for (int i = 0; i < 4; i++ )
		{
			RaycastHit2D Hit = Physics2D.Raycast(Vector2.zero, pCardinalPoints[i]);

			if (Hit)
			{
				tWallsPositions[i] = Hit.point;
			}
		}

		tMapTopLeft.x = tWallsPositions[3].x;
		tMapTopLeft.y = tWallsPositions[0].y;
		tMapBottomRight.x = tWallsPositions[1].x;
		tMapBottomRight.y = tWallsPositions[2].y;
	}
}
