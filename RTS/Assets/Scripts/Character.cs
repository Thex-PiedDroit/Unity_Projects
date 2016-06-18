
using UnityEngine;
using System.Collections;



public class Character : SelectableEntity
{
#region Variables (public)

	public NavMeshAgent m_pNavMeshAgent = null;

	#endregion

#region Variables (private)
	
	

    #endregion
	

	//public override void Start()
	//{
	//	base.Start();
	//}
	
	public override void GivePosition(Vector3 tPos)
	{
		m_pNavMeshAgent.SetDestination(tPos);
	}
}
