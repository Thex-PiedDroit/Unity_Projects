using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class BuildingBehaviour : MonoBehaviour
{
	#region Variables (public)

	[SerializeField]
	private MeshRenderer m_pRoof = null;
	
	#endregion
	
	#region Variables (private)

	private int m_iPlayersInside = 0;
	
	#endregion


	public void SetRoofActive(bool bActive)
	{
		m_pRoof.enabled = bActive;
		m_pRoof.gameObject.GetComponent<BoxCollider>().enabled = bActive;
	}


	void OnTriggerEnter(Collider tTrigger)
	{
		if (tTrigger.gameObject.tag == "PlayerCharacter")
		{
			m_iPlayersInside++;
			SetRoofActive(false);
		}
	}

	void OnTriggerExit(Collider tTrigger)
	{
		if (tTrigger.gameObject.tag == "PlayerCharacter")
		{
			m_iPlayersInside--;
			Assert.IsTrue(m_iPlayersInside >= 0, "Characters count in building smaller than 0. Maybe there was something wrong with the collision detections");
			
			if (m_iPlayersInside == 0)
				SetRoofActive(true);
		}
	}
}
