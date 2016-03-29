
using UnityEngine;
using System.Collections;



public class SquidBehavior : MonoBehaviour
{
	#region Variables (public)

	public GameObject tAssignedPlanet = null;

	#endregion

	#region Variables (private)

	private float fXMaxOffset = 53.0f;
	private float fYPosOffset = -32.0f;

	private float fTimeAnimStart = 0;

    #endregion
	

	void Start()
	{
		transform.parent.transform.parent = tAssignedPlanet.transform;
		float fOffsetX = (tAssignedPlanet.transform.localScale.x * tAssignedPlanet.transform.FindChild("Globe").transform.localScale.x) * 2.0f;
		Vector3 tOffset = new Vector3(fOffsetX > fXMaxOffset ? fXMaxOffset : fOffsetX, fYPosOffset, 0.0f);
		transform.localPosition = tOffset;

		transform.parent.GetComponent<Rotator>().RandomRotation();
	}

	void Update()
	{
		
	}

	public void Eat(FollowTargetBehavior tPlayer)
	{
		if (Time.time - fTimeAnimStart >= 2.0f)
		{
			tPlayer.Respawn();
			fTimeAnimStart = 0.0f;
		}
	}

	
	GameObject AssignedPlanet
	{
		set { tAssignedPlanet = value; }
	}


	public void Eating()
	{
		GetComponent<Animator>().SetTrigger("bEating");
		fTimeAnimStart = Time.time;
	}
}
