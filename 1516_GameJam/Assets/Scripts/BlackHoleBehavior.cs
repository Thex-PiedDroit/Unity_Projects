
using UnityEngine;
using System.Collections;



public class BlackHoleBehavior : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private float fAttractionSpeed = 5.0f;
	[SerializeField]
	private float fBottomCatchTreshold = 0.5f;

	#endregion

	#region Variables (private)

	Transform tBottomPos;

	private LoadQuitGame tLoadQuitGameScript;

    #endregion
	

	void Start ()
	{
		tLoadQuitGameScript = GameObject.Find("Main Camera").GetComponent<LoadQuitGame>();

		tBottomPos = transform.FindChild("Bottom");
	}
	
	void Update ()
	{
		
	}


	public void OrbitUpdate(GameObject tPlayer)
	{
		float fNewPosX = Mathf.Lerp(tPlayer.transform.position.x, tBottomPos.position.x, (fAttractionSpeed * 2.0f) * Time.deltaTime * tLoadQuitGameScript.TimeScale);
		float fNewPosZ = Mathf.Lerp(tPlayer.transform.position.z, tBottomPos.position.z, fAttractionSpeed * Time.deltaTime * tLoadQuitGameScript.TimeScale);

		tPlayer.transform.position = new Vector3(fNewPosX, tPlayer.transform.position.y, fNewPosZ);

		float fBottomDist = fNewPosZ - tBottomPos.position.z;
		if ((fBottomDist * fBottomDist) <= (fBottomCatchTreshold * fBottomCatchTreshold))
		{
			tPlayer.GetComponent<FollowTargetBehavior>().Respawn();
		}
	}
}
