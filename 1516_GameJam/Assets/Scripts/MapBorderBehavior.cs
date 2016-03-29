
using UnityEngine;
using System.Collections;



public class MapBorderBehavior : MonoBehaviour
{
	#region Variables (public)

	

	#endregion

	#region Variables (private)
	
	

    #endregion
	

	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}


	void OnCollisionEnter(Collision tCollision)
	{
		if (tCollision.transform.tag == "Planet")
		{
			Destroy(tCollision.gameObject);
			print("Destroyed");
		}
	}
}
