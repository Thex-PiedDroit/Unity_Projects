
//#define DEBUG_SCREENSHAPE

using UnityEngine;
using System.Collections;



public abstract class SelectableEntity : MonoBehaviour
{
#region Variables (public)

	public CapsuleCollider pCollider = null;

	public Texture2D m_pRectTestTexture = null;
	public Texture2D m_pRectTestTextureCollision = null;
	public Texture2D m_pCircleTestTexture = null;
	public Texture2D m_pCircleTestTextureCollision = null;

	public bool m_bSphereCalculation = true;

	public GameObject m_pSelectionPlane_Selected = null;
	public GameObject m_pSelectionPlane_Unselected = null;

	#endregion

#region Variables (private)

#if DEBUG_SCREENSHAPE
	private Vector3 m_tScreenTopLeft = Vector3.zero;
	private Vector2 m_tExtents = Vector2.zero;

	private bool m_bCollision = false;
#endif

    #endregion



#if DEBUG_SCREENSHAPE

	public virtual void Start()
	{
		ScreenSpaceCollision(Vector3.zero, Vector3.zero);
	}

	void OnGUI()
	{
		Texture2D pTexture = null;
		if (m_bSphereCalculation)
			pTexture = m_bCollision ? m_pCircleTestTextureCollision : m_pCircleTestTexture;
		else
			pTexture = m_bCollision ? m_pRectTestTextureCollision : m_pRectTestTexture;

		GUI.DrawTexture(new Rect(m_tScreenTopLeft, m_tExtents), pTexture, ScaleMode.StretchToFill);
	}
#else

	public virtual void Start()
	{

	}

#endif

	public void SetSelected(bool bSelected)
	{
		m_pSelectionPlane_Selected.SetActive(bSelected);
		m_pSelectionPlane_Unselected.SetActive(!bSelected);
	}

	public bool ScreenSpaceCollision(Vector3 tRectCenter, Vector3 tRectHalfExtents)
	{
		bool bCollision = false;

		if (m_bSphereCalculation)		// SPHERE
		{
			/*		CALCULATE SCREEN CIRCLE POSITION	*/
			Vector3 tFeet = ToolKit.TopLeftWorldToScreenPoint(transform.position);
			Vector3 tHead = ToolKit.TopLeftWorldToScreenPoint(transform.position + (transform.up * pCollider.height));

			Vector3 tCenter = tHead - ((tHead - tFeet) / 2.0f);

			Vector3 tBottomRight = ToolKit.TopLeftWorldToScreenPoint(transform.position + (Vector3.right * pCollider.radius));
			Vector3 tBottomLeft = ToolKit.TopLeftWorldToScreenPoint(transform.position + (Vector3.left * pCollider.radius));

			float fRestrictedSizeX = (tBottomRight.x - tBottomLeft.x) / 2.0f;
			float fRadius = (tHead - tFeet).magnitude / 2.0f;
			if (fRadius < fRestrictedSizeX)
				fRadius = fRestrictedSizeX;

			/*		CALCULATE COLLISION		*/
			Vector3 tRectToCenter = tCenter - tRectCenter;

			float fDotCenterOnRight = Vector3.Dot(tRectToCenter, Vector3.right);
			float fDotCenterOnDown = Vector3.Dot(tRectToCenter, Vector3.down);

			float fMinSizeRight = tRectHalfExtents.x + fRadius;
			float fMinSizeDown = tRectHalfExtents.y + fRadius;

			bool bCloseEnoughX = (fDotCenterOnRight * fDotCenterOnRight) <= (fMinSizeRight * fMinSizeRight);
			bool bCloseEnoughY = (fDotCenterOnDown * fDotCenterOnDown) <= (fMinSizeDown * fMinSizeDown);

			bCollision = bCloseEnoughX && bCloseEnoughY;

		#if DEBUG_SCREENSHAPE

			m_tExtents.x = fRadius * 2.0f;
			m_tExtents.y = fRadius * 2.0f;
			m_tScreenTopLeft = tCenter - ((Vector3)(m_tExtents) / 2.0f);

			m_bCollision = bCollision;

		#endif
		}

		else							// RECTANGLE
		{
			/*		CALCULATE SCREEN RECTANGLE POSITION		*/
			Vector3 tDebugBottomRight = ToolKit.TopLeftWorldToScreenPoint(transform.position + (Vector3.right * pCollider.radius));
			Vector3 tDebugTopLeft = ToolKit.TopLeftWorldToScreenPoint(transform.position + (transform.up * pCollider.height) + (Vector3.left * pCollider.radius));
			Vector3 tDebugTopRight = ToolKit.TopLeftWorldToScreenPoint(transform.position + (transform.up * pCollider.height) + (Vector3.right * pCollider.radius));
			Vector3 tDebugBottomLeft = ToolKit.TopLeftWorldToScreenPoint(transform.position + (Vector3.left * pCollider.radius));

			float fFurtherLeft = tDebugBottomLeft.x < tDebugTopLeft.x ? tDebugBottomLeft.x : tDebugTopLeft.x;
			float fFurtherRight = tDebugBottomRight.x > tDebugTopRight.x ? tDebugBottomRight.x : tDebugTopRight.x;
			float fFurtherUp = tDebugTopLeft.y < tDebugTopRight.y ? tDebugTopLeft.y : tDebugTopRight.y;
			float fFurtherDown = tDebugBottomLeft.y > tDebugBottomRight.y ? tDebugBottomLeft.y : tDebugBottomRight.y;

			Vector3 tScreenBottomRight = new Vector3(fFurtherRight, fFurtherDown, 0.0f);
			Vector3 tScreenTopLeft = new Vector3(fFurtherLeft, fFurtherUp, 0.0f);

			Vector3 tExtents = tScreenBottomRight - tScreenTopLeft;

			float fRestrictiveSizeX = tDebugBottomRight.x - tDebugBottomLeft.x;
			if (tExtents.y < fRestrictiveSizeX)
			{
				float fHalfDifference = (fRestrictiveSizeX - tExtents.y) * 0.5f;
				tScreenTopLeft.y -= fHalfDifference;
				tExtents.y = fRestrictiveSizeX;
			}

			/*		CALCULATE COLLISION		*/
			Vector3 tRectTopLeft = tRectCenter - tRectHalfExtents;
			Vector3 tRectBottomRight = tRectCenter + tRectHalfExtents;

			bool bCollisionX = tRectTopLeft.x <= tScreenBottomRight.x && tRectBottomRight.x > tScreenTopLeft.x;
			bool bCollisionY = tRectTopLeft.y <= tScreenBottomRight.y && tRectBottomRight.y > tScreenTopLeft.y;

			bCollision = bCollisionX && bCollisionY;

		#if	DEBUG_SCREENSHAPE

			m_tScreenTopLeft = tScreenTopLeft;
			m_tExtents = tExtents;

			m_bCollision = bCollision;

		#endif
		}

		return bCollision;
	}

	public abstract void GivePosition(Vector3 tPos);
}
