
using UnityEngine;
using System.Collections;



public class CharacController : MonoBehaviour
{
	#region Variables (public)

	public float fRotationSpeed = 0.5f;

	#endregion

	#region Variables (private)

	private GameObject pFollowTarget;
	private bool FreezePos = false;

	private LoadQuitGame tLoadQuitGameScript;

    #endregion
	

	void Awake ()
	{
		pFollowTarget = transform.parent.gameObject.GetComponentInChildren<FollowTargetBehavior>().gameObject;
		transform.LookAt(pFollowTarget.transform);

		tLoadQuitGameScript = GameObject.Find("Main Camera").GetComponent<LoadQuitGame>();
	}

	void Update ()
	{
		if (!FreezePos)
		{
			transform.position += (((pFollowTarget.transform.position - transform.position) * 10.0f) * Time.deltaTime * tLoadQuitGameScript.TimeScale);

			Vector3 tPlayerToTarget = pFollowTarget.transform.position - transform.position;
			if (tPlayerToTarget == Vector3.zero)
				tPlayerToTarget = transform.forward;

			Quaternion tLookat = Quaternion.LookRotation(tPlayerToTarget, transform.up);
			transform.rotation = Quaternion.Slerp(transform.rotation, tLookat, fRotationSpeed);
		}

		if (Input.GetButtonDown("Start"))
		{
			GameObject.Find("Main Camera").GetComponent<LoadQuitGame>().Pause();
		}
	}


	public bool Freeze
	{
		set { FreezePos = value; }
	}
}
