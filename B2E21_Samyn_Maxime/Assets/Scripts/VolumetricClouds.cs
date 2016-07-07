
using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class VolumetricClouds : MonoBehaviour
{
#region Variables (public)

	public Texture2D m_pClouds2DTex = null;
	public int m_iLayersSquaredSize = 64;

	public Color m_tCloudColor = Color.white;
	public float m_fSegmentsSize = 0.1f;
	public int m_iLoopIterations = 80;
	public float m_fMinAlpha = 0.1f;

	#endregion

#region Variables (private)

	private Texture3D m_pClouds3D = null;
	private Vector3 m_tPos = Vector3.zero;
	private Color m_tColor = Color.white;
	private float m_fSegmentsSizeShader = 0.0f;
	private float m_fScale = 0.0f;
	private int m_iLoopIterationsShader = 0;
	private float m_fMinAlphaShader = 0.0f;

    #endregion
	

	void OnEnable()
	{
		m_pClouds3D = null;
	}

	void Update()
	{
		if (m_pClouds3D == null)
			Create3DTexture();

		if (transform.position != m_tPos || m_tColor != m_tCloudColor || m_fSegmentsSizeShader != m_fSegmentsSize || m_iLoopIterationsShader != m_iLoopIterations || m_fMinAlpha != m_fMinAlphaShader || m_fScale != transform.localScale.x)
			UpdateProperties();
	}
	
	void Create3DTexture()
	{
		int iTexSizeX = m_pClouds2DTex.width;
		int iTexSizeY = m_pClouds2DTex.height;
		int iTextureDepth = iTexSizeX / m_iLayersSquaredSize;
		int iLayerSize = m_iLayersSquaredSize * m_iLayersSquaredSize;
		m_pClouds3D = new Texture3D(m_iLayersSquaredSize, m_iLayersSquaredSize, iTextureDepth, TextureFormat.ARGB32, true);

		Color[] pTexColor = new Color[iTexSizeX * iTexSizeY];

		for (int x = 0; x < iTexSizeX; x++)
		{
			for (int y = 0; y < iTexSizeY; y++)
			{
				int iLayer = x / m_iLayersSquaredSize;

				int iLayerIndex = (y * m_iLayersSquaredSize) + (x % m_iLayersSquaredSize);
				int iIndex = (iLayer * iLayerSize) + iLayerIndex;

				Color tColor = m_pClouds2DTex.GetPixel(x, y);
				pTexColor[iIndex] = tColor;
			}
		}

		m_pClouds3D.SetPixels(pTexColor);
		m_pClouds3D.Apply(true);

		Shader.SetGlobalTexture("_CloudTex", m_pClouds3D);

		UpdateProperties();
	}

	void UpdateProperties()
	{
		m_tPos = transform.position;
		Vector4 tPos = new Vector4(m_tPos.x, m_tPos.y, m_tPos.z, transform.localScale.x);
		Shader.SetGlobalVector("_CloudSpherePos_wRadius", tPos);

		m_tColor = m_tCloudColor;
		Shader.SetGlobalVector("_CloudColor", m_tColor);

		m_fSegmentsSizeShader = m_fSegmentsSize;
		Shader.SetGlobalFloat("_SegmentsSize", m_fSegmentsSizeShader);
		m_iLoopIterationsShader = m_iLoopIterations;
		Shader.SetGlobalInt("_CloudsLoopIterations", m_iLoopIterationsShader);
		m_fMinAlphaShader = m_fMinAlpha;
		Shader.SetGlobalFloat("_MinCloudsAlpha", m_fMinAlphaShader);

		m_fScale = transform.localScale.x;
	}
}
