using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private GameObject pCivilian;
	[SerializeField]
	private GameObject pBadGuy;
	[SerializeField]
	private bool m_bGenerate = true;
	[SerializeField]
	private Vector2 m_tCivAndBadGuysCountMinMax = new Vector2(150, 200);
	[SerializeField]
	private GameObject pCanvas;
	
	#endregion
	
	#region Variables (private)

	private Transform pCiviliansContainer;
	private Transform pBadGuysContainer;

	static protected int s_iGroundLayer;
	static protected int s_iObstaclesLayer;
	
	#endregion


	void Awake()
	{
		s_iGroundLayer = 1 << LayerMask.NameToLayer("Ground");
		s_iObstaclesLayer = 1 << LayerMask.NameToLayer("Obstacle");

		if (m_bGenerate)
		{
			pCiviliansContainer = GameObject.Find("Civilians").transform;
			pBadGuysContainer = GameObject.Find("BadGuys").transform;

			int iCiviliansRandCount = Random.Range((int)m_tCivAndBadGuysCountMinMax.x, (int)m_tCivAndBadGuysCountMinMax.y + 1);

			GameObject[] pCivAndBadGuys = new GameObject[iCiviliansRandCount];

			for (int i = 0; i < iCiviliansRandCount; i++)
			{
				bool bCivilian = Random.Range(0, 2) == 0;

				Vector3 tRandomSpawnPoint = Vector3.zero;

				bool bSpawnPointFound = false;
				int iAttemps = 0;

				do
				{
					tRandomSpawnPoint.x = Random.Range(-transform.localScale.x / 2.0f, transform.localScale.x / 2.0f);
					tRandomSpawnPoint.y = 0.0f;
					tRandomSpawnPoint.z = Random.Range(-transform.localScale.z / 2.0f, transform.localScale.z / 2.0f);

					NavMeshHit Hit;
					if (NavMesh.FindClosestEdge(tRandomSpawnPoint, out Hit, Map.ObstaclesLayer))
					{
						tRandomSpawnPoint = Hit.position;
						bSpawnPointFound = true;
					}

					iAttemps++;

				} while (!bSpawnPointFound && iAttemps < 50);

				pCivAndBadGuys[i] = Instantiate(bCivilian ? pCivilian : pBadGuy, tRandomSpawnPoint, Quaternion.identity) as GameObject;
				pCivAndBadGuys[i].transform.parent = bCivilian ? pCiviliansContainer : pBadGuysContainer;
				pCivAndBadGuys[i].name = bCivilian ? pCivilian.name : pBadGuy.name;
			}
		}

		pCanvas.SetActive(true);
	}


	#region Getters/Setters

	static public int GroundLayer
	{
		get { return s_iGroundLayer; }
	}

	static public int AllButGroundLayer
	{
		get { return ~s_iGroundLayer; }
	}

	static public int ObstaclesLayer
	{
		get { return s_iObstaclesLayer; }
	}

	static public int AllButObstaclesLayer
	{
		get { return ~s_iObstaclesLayer; }
	}

	#endregion Getters/Setters


	void OnApplicationQuit()
	{
		LivingBeing[] pLivingBeings = GameObject.FindObjectsOfType<LivingBeing>();

		foreach(LivingBeing tLivingBeing in pLivingBeings)
		{
			if (tLivingBeing.gameObject.transform.parent.gameObject.tag != "Target" &&
				tLivingBeing.gameObject.transform.parent.gameObject.tag != "PlayerCharacter")
			{
				Destroy(tLivingBeing.gameObject.transform.parent.gameObject);
			}
		}
	}
}
