
//#define DEBUG_CONTINUOUSCOLLISIONTESTS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class InputsManager : MonoBehaviour
{
#region Variables (public)

	static public InputsManager Instance = null;

	public Texture2D m_pSelectionSquare = null;

	#endregion

#region Variables (private)

	private List<SelectableEntity> m_pAllSelectables = null;
	private List<SelectableEntity> m_pCurrentSelection = null;

	private Vector3 m_tSelectionTopLeftScreen = Vector3.zero;
	private Vector3 m_tSelectionBottomRightScreen = Vector3.zero;
	bool m_bShouldDrawSelection = false;

    #endregion
	

	void Awake()
	{
		if (Instance != null)
		{
			if (Instance != this)
				Destroy(this);
			return;
		}

		Instance = this;

		m_pAllSelectables = new List<SelectableEntity>();
		foreach(SelectableEntity tEntity in FindObjectsOfType<SelectableEntity>())
			m_pAllSelectables.Add(tEntity);

		m_pCurrentSelection = new List<SelectableEntity>();
	}
	
	void Update()
	{
		CatchInputs();
	}

	void CatchInputs()
	{
		if (UIManager.Instance.MouseOverMenu || BuildingsManager.Instance.DraggingBuildingShade)
			return;

		if (Input.GetButtonDown("Submit"))
		{
			StartCoroutine(DrawSquareSelection());
		}

		if (Input.GetButton("Move"))
		{
			RaycastHit tHit;

			if (GetWorldPos(Input.mousePosition, out tHit))
			{
				for (int i = 0; i < m_pCurrentSelection.Count; i++)
					m_pCurrentSelection[i].GivePosition(tHit.point);
			}
		}
	}

	void OnGUI()
	{
		if (!m_bShouldDrawSelection)
			return;

		Vector3 tExtents = (m_tSelectionBottomRightScreen - m_tSelectionTopLeftScreen) / 2.0f;
		Vector3 tBoxCenter = m_tSelectionTopLeftScreen + tExtents;

		GUI.DrawTexture(new Rect(tBoxCenter - tExtents, tExtents * 2.0f), m_pSelectionSquare, ScaleMode.StretchToFill);
	}

	IEnumerator DrawSquareSelection()
	{
		Vector3 tInitMousePos = ToolKit.DownToTopCoordinate(Input.mousePosition);
		Vector3 tCurrentMousePos = tInitMousePos;
		Vector3 tTopLeft = tInitMousePos;
		Vector3 tBottomRight = tInitMousePos;
		m_tSelectionTopLeftScreen = tInitMousePos;
		m_tSelectionBottomRightScreen = tInitMousePos;

		m_bShouldDrawSelection = true;

		while (Input.GetButton("Submit"))
		{
			tCurrentMousePos = ToolKit.DownToTopCoordinate(Input.mousePosition);
			tBottomRight = tCurrentMousePos;
			m_tSelectionBottomRightScreen = tBottomRight;

		#if DEBUG_CONTINUOUSCOLLISIONTESTS

			GetTopLeftAndBottomRight(tInitMousePos, tCurrentMousePos, ref tTopLeft, ref tBottomRight);

			Vector3 tSelectionHalfExtentsTest = (tBottomRight - tTopLeft) / 2.0f;
			Vector3 tSelectionCenterTest = tBottomRight - tSelectionHalfExtentsTest;

			for (int i = 0; i < m_pAllSelectables.Count; i++)
			{
				m_pAllSelectables[i].ScreenSpaceCollision(tSelectionCenterTest, tSelectionHalfExtentsTest);
			}

		#endif

			yield return false;
		}

		m_bShouldDrawSelection = false;
		ClearSelection();

		GetTopLeftAndBottomRight(tInitMousePos, tCurrentMousePos, ref tTopLeft, ref tBottomRight);

		Vector3 tSelectionHalfExtents = (tBottomRight - tTopLeft) / 2.0f;
		Vector3 tSelectionCenter = tBottomRight - tSelectionHalfExtents;

		for (int i = 0; i < m_pAllSelectables.Count; i++)
		{
			if (m_pAllSelectables[i].ScreenSpaceCollision(tSelectionCenter, tSelectionHalfExtents))
			{
				m_pCurrentSelection.Add(m_pAllSelectables[i]);
				m_pAllSelectables[i].SetSelected(true);
			}
		}
	}

	void ClearSelection()
	{
		for (int i = 0; i < m_pCurrentSelection.Count; i++)
		{
			m_pCurrentSelection[i].SetSelected(false);
		}

		m_pCurrentSelection.Clear();
	}

	void GetTopLeftAndBottomRight(Vector3 tA, Vector3 tB, ref Vector3 tTopLeft, ref Vector3 tBottomRight)
	{
		float fFurtherLeft = tA.x < tB.x ? tA.x : tB.x;
		float fFurtherRight = tA.x > tB.x ? tA.x : tB.x;
		float fFurtherUp = tA.y < tB.y ? tA.y : tB.y;
		float fFurtherDown = tA.y > tB.y ? tA.y : tB.y;

		Vector3 tFinalTopLeft = new Vector3(fFurtherLeft, fFurtherUp, 0.0f);
		Vector3 tFinalBottomRight = new Vector3(fFurtherRight, fFurtherDown, 0.0f);

		tBottomRight = tFinalBottomRight;
		tTopLeft = tFinalTopLeft;
	}


	bool GetWorldPos(Vector3 tScreenPos, out RaycastHit tHit)
	{
		Vector3 tViewPortPos = Camera.main.ScreenToViewportPoint(tScreenPos);
		Ray tRay = Camera.main.ViewportPointToRay(tViewPortPos);

		return Physics.Raycast(tRay, out tHit, 100.0f, LayersManager.Instance.GetLayer("Ground"), QueryTriggerInteraction.Ignore);
	}
}
