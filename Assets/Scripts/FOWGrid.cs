using UnityEngine;
using System.Collections;

/*---------------------------------------------------------------
 Class for Fog Of War using grid system.
 ---------------------------------------------------------------*/
public class FOWGrid : MonoBehaviour {
	
	/*---------------------------------------------------------------
	 Grid structure.
	 ---------------------------------------------------------------*/
	struct sFOWGrid
	{
		public float m_fExploredTime;  // the time use for fade in/out the alpha.
		public int m_iExplored;		   // explored state : 2 for now in sight, 1 for in sight before.
		public int m_iTexelX;		   // texel x.
		public int m_iTexelY;		   // texel y.
		public Vector2 m_Center;	   // grid center.
	};
	
	/*---------------------------------------------------------------
	 Texel data.
	 ---------------------------------------------------------------*/
	struct sTexelData
	{
		public Color m_aCol;	       // texel color.
	}
	
	/*---------------------------------------------------------------
	 Target's data.(Game object)
	 ---------------------------------------------------------------*/
	struct sTargetData
	{
		public GameObject m_Go;		   // Game object for exploring the grid.
		public float m_fSight;		   // The sight of the object.
	};
	
	public GameObject m_MaskPlane = null;	  // Mask plane for get the grid plane's width and height.
	public Color m_FogColor = new Color(0.0f, 0.0f, 0.0f, 0.75f);
	private ArrayList m_aTargets;			  // Target's array.
	private sFOWGrid [,] m_aGrid;			  // Grid array.
	private sTexelData [,] m_aTexel;		  // Texel array.
	private float m_fBeginX = 0.0f;	          // Begin pos to calculate the position in grid.
	private float m_fBeginZ = 0.0f;			  // Begin pos to calculate the position in grid.
	private float m_fUnitLenght = 1.0f;		  // Grid's unit length.
	private float m_fWidth = 0.0f;			  // Mask's width.
	private float m_fHeight = 0.0f;			  // Mask's height.
	private int   m_inGridW = 0;			  // Number grid in width.
	private int   m_inGridH = 0;			  // Number grid in height.
	private int   m_iTexDegree = 0;		      // Texel degree 0, 1, 2, 3....(Now only for 0 : it means 1 grid match 1 texel) 
	private Texture2D m_MaskTexture = null;   // Texture for FOW on this object.
	
	// Instance self.
	static private FOWGrid m_Instance = null;
	static public FOWGrid Instance() { return m_Instance; }
	
	/*---------------------------------------------------------------
	 Initial before start().
	 ---------------------------------------------------------------*/
	void Awake()
	{
		// Setup control side.
		m_Instance = this;
		m_aTargets = new ArrayList();
		m_aGrid = null;

	}
	
	/*---------------------------------------------------------------
	 Use this for initialization
	 ---------------------------------------------------------------*/
	void Start () {
	
	}
	
	/*---------------------------------------------------------------
	 Find the target in target's array.
	 ---------------------------------------------------------------*/
	int FindTarget(GameObject go)
	{
		sTargetData tData;
		int iCount = m_aTargets.Count;
		int i;
		for(i = 0; i < iCount; i++) {
			tData = (sTargetData)m_aTargets[i];
			if(tData.m_Go == go) {
				return i;	
			}
		}

		return -1;

	}
	
	/*---------------------------------------------------------------
	 Add the target to target's array.
	 ---------------------------------------------------------------*/
	public void AddTarget(GameObject go, float fSight)
	{
		if(FindTarget(go) < 0) {
			sTargetData data;
			data.m_Go = go;
			data.m_fSight = fSight;
			m_aTargets.Add(data);
		}
	}
	
	/*---------------------------------------------------------------
	 Remove the target in target's array.
	 ---------------------------------------------------------------*/
	public void RemoveTarget(GameObject go)
	{
		int index = FindTarget(go);
		if(index < 0) {
			return;
		}
		
		m_aTargets.RemoveAt(index);
	}
	
	/*---------------------------------------------------------------
	 Init the data.
	 ---------------------------------------------------------------*/
	public void InitData () 
	{
		m_fBeginX = m_MaskPlane.GetComponent<Collider>().bounds.min.x;
		m_fBeginZ = m_MaskPlane.GetComponent<Collider>().bounds.min.z;
		m_fWidth = m_MaskPlane.GetComponent<Collider>().bounds.size.x;
		m_fHeight = m_MaskPlane.GetComponent<Collider>().bounds.size.z;
		m_MaskPlane.GetComponent<Collider>().enabled = false;
		Vector3 scale = new Vector3(m_fWidth, m_fHeight, 1.0f);
		this.transform.localScale = scale;
		Vector3 pos = m_MaskPlane.transform.position;
		pos.y = 0.0f;
		this.transform.position = pos;
	}
	
	/*---------------------------------------------------------------
	 Create the grid.
	 ---------------------------------------------------------------*/
	public void CreateGrid(float fUnit, int iTexDegree = 0)
	{	
		int inW = (int)(m_fWidth/fUnit) + 1;
		int inH = (int)(m_fHeight/fUnit) + 1;
		m_inGridW = inW;
		m_inGridH = inH;
		
		int i, j;
		int iRatio = 1;
		int iTextureW = inW;
		int iTextureH = inH;
		if(iTexDegree == 1) {
			iRatio = 2;
			iTextureW = inW/2;
			iTextureH = inH/2;
		} else if(iTexDegree == 2) {
			iRatio = 4;
			iTextureW = inW/4;
			iTextureH = inH/4;
		}
		
		float fHalf = fUnit/2.0f;
		m_aGrid = new sFOWGrid[m_inGridW, m_inGridH];
		for(i = 0; i < m_inGridW; i++) {
			for(j = 0; j < m_inGridH; j++) {
				m_aGrid[i, j].m_iExplored = 0;
				m_aGrid[i, j].m_fExploredTime = 0.0f;
				m_aGrid[i, j].m_iTexelX = i/iRatio;
				m_aGrid[i, j].m_iTexelY = j/iRatio;
				m_aGrid[i, j].m_Center = new Vector3(i*fUnit + fHalf, j*fUnit + fHalf);
			}
		}
		
		m_fUnitLenght = fUnit;
		m_iTexDegree = iTexDegree;
		m_aTexel = new sTexelData[iTextureW, iTextureH];
		m_MaskTexture = new Texture2D(iTextureW, iTextureH, TextureFormat.ARGB32, false);
		
		Color col = m_FogColor;
		for(i = 0; i < iTextureW; i++) {
			for(j = 0; j < iTextureH; j++) {
				m_MaskTexture.SetPixel(i, j, col);
				m_aTexel[i, j].m_aCol = col;
			}
		}
		
		m_MaskTexture.Apply();
		this.GetComponent<Renderer>().material.mainTexture = m_MaskTexture;
	}
	
	/*---------------------------------------------------------------
	 Update Fog Of War.
	 ---------------------------------------------------------------*/
	public void UpdateFOW()
	{	
		sTargetData data;
		Vector3 vec;
		int i;
		int iX = 0;
		int iY = 0;
		int iLen = m_aTargets.Count;
		for(i = 0; i < iLen; i++) {
			data = (sTargetData)m_aTargets[i];
			GetGridIndex(data.m_Go.transform.position, out iX, out iY);
			SetTextureAlphaFromGridSight(iX, iY, data.m_fSight);
		}
		
		// Refreash texture.
		RefreashTexture();

	}
	
	/*---------------------------------------------------------------
	 Get explored state of grid.
	 ---------------------------------------------------------------*/
	public int GetGridVisible(Vector3 pos)
	{
		int iX = 0;
		int iY = 0;
		GetGridIndex(pos, out iX, out iY);
		sFOWGrid grid = (sFOWGrid)m_aGrid[iX, iY];
		return grid.m_iExplored;
	}
	
	/*---------------------------------------------------------------
	 Get grid index by 3D position.
	 ---------------------------------------------------------------*/
	private void GetGridIndex(Vector3 pos, out int x, out int y)
	{
		float w = pos.x - m_fBeginX;
		float h = pos.z - m_fBeginZ;
		
		int iIndexW = (int)(w/m_fUnitLenght);
		int iIndexH = (int)(h/m_fUnitLenght);
		
		//Debug.Log("Get index : "+ iIndexW.ToString()+ "_" + iIndexH.ToString() + "_" + m_fUnitLenght.ToString() + ":" + m_inGridW.ToString() + ":" + m_inGridH.ToString());
		if(iIndexW >= m_inGridW) {
			iIndexW = m_inGridW - 1;
			//Debug.Log("Out side grid : " + pos.ToString());
		} else if(iIndexW < 0) {
			iIndexW = 0;	
		}
		if(iIndexH >= m_inGridH) {
			iIndexH = m_inGridH - 1;
			//Debug.Log("Out side grid : " + pos.ToString());
		} else if(iIndexH < 0) {
			iIndexH = 0;	
		}
		x = iIndexW;
		y = iIndexH;
	}
	
	/*---------------------------------------------------------------
	 Refreash the texture.
	 ---------------------------------------------------------------*/
	private void RefreashTexture()
	{
		sFOWGrid datagrid;
		
		Color col = m_FogColor;
		col.a = 0.5f;
		int i, j;
		for(i = 0; i < m_inGridW; i++) {
			for(j = 0; j < m_inGridH; j++) {
				datagrid = (sFOWGrid)m_aGrid[i, j];
				if(datagrid.m_iExplored == 2) {
					
					if(datagrid.m_fExploredTime > 1.0f) {
						datagrid.m_iExplored = 1;
					} else {
						datagrid.m_fExploredTime += Time.deltaTime;	
					}
					
					m_aGrid[i, j] = datagrid;
					continue;
				} else if(datagrid.m_iExplored == -1) {
					continue;	
				} else if(datagrid.m_iExplored == 1) {
					if(datagrid.m_fExploredTime > 1.5f) {
						col.a = 0.5f;
						m_aTexel[datagrid.m_iTexelX, datagrid.m_iTexelY].m_aCol.a += col.a;
						m_MaskTexture.SetPixel(datagrid.m_iTexelX, datagrid.m_iTexelY, col);
					} else {
						datagrid.m_fExploredTime += Time.deltaTime;	
						col.a = datagrid.m_fExploredTime - 1.0f;
						m_aTexel[datagrid.m_iTexelX, datagrid.m_iTexelY].m_aCol.a += col.a;
						m_MaskTexture.SetPixel(datagrid.m_iTexelX, datagrid.m_iTexelY, col);
						m_aGrid[i, j] = datagrid;
					}
					
				}
			}
		}
		
		// Apply the texel value to texture.
		m_MaskTexture.Apply();
	}
	
	/*---------------------------------------------------------------
	 Set the texel alpha by checking the sight.
	 ---------------------------------------------------------------*/
	private void SetTextureAlphaFromGridSight(int iGridX, int iGridY, float fSight)
	{		
		float fRangeSight = fSight + 2.0f;
		int iGridSight = (int)(fRangeSight/m_fUnitLenght);
		int iCurrentSightX = iGridX;
		int iCurrentSightY = iGridY;

		sFOWGrid grid = (sFOWGrid)m_aGrid[iCurrentSightX, iCurrentSightY];
		Vector2 oCenter = grid.m_Center;
		//grid.m_iExplored = 2;

		Color col = m_FogColor;
		col.a = 0.0f;
		//m_aTexel[grid.m_iTexelX, grid.m_iTexelY].m_aCol.a += col.a;
		//m_MaskTexture.SetPixel(grid.m_iTexelX, grid.m_iTexelY, col);
			
		Vector2 vec;
		int i, j;
		int iTempX = 0;
		int iTempY = 0;
		int indexI = -iGridSight;
		int indexJ = -iGridSight;
		int iTotalGridSight = iGridSight*2 + 1;
		float fLen = 0.0f;
		//Debug.Log("Gridsight : " + iGridSight.ToString());
		for(i = 0; i < iTotalGridSight; i++) {
			iTempY = iGridY + indexI;
			indexI++;

			if(iTempY < 0 || iTempY >= m_inGridH) {
				continue;	
			}
			indexJ = -iGridSight;
			for(j = 0; j < iTotalGridSight; j++) {
				iTempX = iGridX + indexJ;
				indexJ++;
				if(iTempX < 0 || iTempX >= m_inGridW) {
					continue;	
				}
				
				grid = (sFOWGrid)m_aGrid[iTempX, iTempY];
				vec = grid.m_Center - oCenter;
				fLen = vec.magnitude;
				col.a = 0.0f;
				if(fLen > fSight) {
					if(fLen < fRangeSight) {
						//col.a = 0.5f*((fLen - fSight)/(fRangeSight - fSight));
						//grid.m_iExplored = 2;
						//grid.m_fExploredTime = 0.0f;
						//m_aGrid[iTempX, iTempY] = grid;
						//m_aTexel[grid.m_iTexelX, grid.m_iTexelY].m_aCol.a += col.a;
						//m_MaskTexture.SetPixel(grid.m_iTexelX, grid.m_iTexelY, col);
						//if(grid.m_iExplored == 2) {
						//	grid.m_iExplored++;
						//	m_aGrid[iTempX, iTempY] = grid;
						//}
					}
					continue;	
				}
				if(grid.m_iExplored < 2) {
					grid.m_fExploredTime = 0.0f;
				}
				grid.m_iExplored = 2;
				if(grid.m_fExploredTime > 0.5f) {
					grid.m_fExploredTime = 0.5f;	
				} else {
					grid.m_fExploredTime += Time.deltaTime;
				}
				col.a = 0.5f - grid.m_fExploredTime;
				m_aGrid[iTempX, iTempY] = grid;
				m_aTexel[grid.m_iTexelX, grid.m_iTexelY].m_aCol.a += col.a;
				m_MaskTexture.SetPixel(grid.m_iTexelX, grid.m_iTexelY, col);
				//Debug.Log("Grid id : " + iTempX.ToString() + ":" + iTempY.ToString());
				//Debug.Log("Draw pixel : " + grid.m_iTexelX.ToString() + ":" + grid.m_iTexelY.ToString());
			}
			
		}
		
		//m_MaskTexture.Apply();
		
	}
}
