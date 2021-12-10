using UnityEngine;
using System.Collections;

public class BaseGrid
{
	protected int m_nRow;
	protected int m_nCol;
	protected int m_nCell;
	protected float m_fWidth;
	protected float m_fHeight;
	protected float m_fCellSize;
	protected Vector3 m_vOrigin;
	protected Vector3 m_vCenter;
	protected Bounds m_Bounds;
	protected int[,] m_CellState;  // -1, 0, 1...  ; 0 for normal , -1 for block
	private Vector3 m_vUnitX;
	private Vector3 m_vUnitZ;
	
	
	public float Width
	{
		get { return m_fWidth; }
	}
		
	public float Height
	{
		get { return m_fHeight; }
	}
	
	public Vector3 Origin
	{
		get { return m_vOrigin; }
	}
	
	public int nCell
	{
		get { return m_nCell; }
	}
	
	public float Left
	{
		get { return m_vOrigin.x; }
	}
	
	public float Right
	{
		get { return m_vOrigin.x + Width; }
	}
	
	public float Top
	{
		get { return m_vOrigin.z + Height; }
	}
	
	public float Bottom
	{
		get { return m_vOrigin.z; }
	}
	
	public float CellSize
	{
		get { return m_fCellSize; }
	}
	
	public Vector3 Center
	{
		get { return m_vCenter; }	
	}
	
	public Bounds Bound
	{
		get { return m_Bounds; }
	}
	
	public BaseGrid()
	{
		m_CellState = null;
	}
	~BaseGrid()
	{
		m_CellState = null;
	}
	
	public virtual void Init(Vector3 origin, int numRows, int numCols, float cellSize) 
	{
		m_vOrigin = origin;
		m_vUnitX = new Vector3(1.0f, 0.0f, 0.0f);
		m_vUnitZ = new Vector3(0.0f, 0.0f, 1.0f);
		m_nRow = numRows;
		m_nCol = numCols;
		m_fCellSize = cellSize;
		m_fWidth = m_nCol*m_fCellSize;
		m_fHeight = m_nRow*m_fCellSize;
		m_nCell = m_nRow*m_nCol;
		m_vCenter = m_vOrigin + (m_fWidth/2.0f)*m_vUnitX + (m_fHeight/2.0f)*m_vUnitZ;
		m_Bounds = new Bounds(m_vCenter, new Vector3(m_fWidth, 1.0f, m_fHeight));
		m_CellState = new int[m_nCol,m_nRow];
			
		// Initialize all columns to false.
		int i, j;
		for (i = 0; i < numCols; i++)
		{
			for (j = 0; j < numRows; j++)
			{
				m_CellState[i, j] = 0;
			}
		}
	}
	
	
	public bool BeInBoundary(int col, int row)
    {
        if (col < 0 || col >= m_nCol) {
            return false;
        } else if (row < 0 || row >= m_nRow) {
            return false;
        } else {
            return true;
        }
    }
	
    public bool BeInBoundary(int index)
    {
		return ( index >= 0 && index < nCell );
    }
	
	// pass in world space coords
	public bool BeInBoundary(Vector3 pos)
	{
		bool bBound = true;
		if(pos.x < Left || pos.x  > Right || pos.z > Top || pos.z < Bottom) {
			bBound = false;
		}
        return bBound;
	}
	
	public int GetRow(int index)
    {
        int row = index/m_nCol;
        return row;
    }

    public int GetColumn(int index)
    {
        int col = index%m_nCol;
        return col;
    }
	
	public int GetCellIndex(Vector3 pos)
    {
		if(BeInBoundary(pos) == false) {
			return -1;	
		}
		
		Vector3 npos = pos - m_vOrigin;
		
        int col = (int)(npos.x/m_fCellSize);
        int row = (int)(npos.z/m_fCellSize);
		int index = row*m_nCol + col;
        return index;
    }
		
	
	public Vector3 GetCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
		float w = col*m_fCellSize;
        float h = row*m_fCellSize;
        Vector3 pos = m_vOrigin + new Vector3(w, 0.0f, h);
		return pos;
    }
	
	public Vector3 GetCellCenter(int index)
	{
		Vector3 cellPosition = GetCellPosition(index);	
		cellPosition.x += (m_fCellSize/2.0f);
		cellPosition.z += (m_fCellSize/2.0f);
		return cellPosition;
	}
	
	public void SetCellState(int index, int iState)
	{
		int col = GetColumn(index);
		int row = GetRow(index);
		if(BeInBoundary(col, row) == false) {
			return;
		}
		m_CellState[col, row] = iState;
	}

	public int GetCellState(int index)
	{
		int col = GetColumn(index);
		int row = GetRow(index);
		if(BeInBoundary(col, row) == false) {
			return -1;
		}
		return m_CellState[col, row];
	}
	
	
	public static void DebugDraw(Vector3 origin, int numRows, int numCols, float cellSize, Color color) 
	{		
		float width = ( numCols * cellSize );
		float height = ( numRows * cellSize );
		Vector3 xVec = new Vector3(1.0f, 0.0f, 0.0f);
		Vector3 zVec = new Vector3(0.0f, 0.0f, 1.0f);
		// Draw the horizontal grid lines
		for (int i = 0; i < numRows + 1; i++)
		{
			Vector3 startPos = origin + i*cellSize*zVec;
			Vector3 endPos = startPos + width*xVec;
			Debug.DrawLine(startPos, endPos, color);
		}
		
		for (int i = 0; i < numCols + 1; i++)
		{
			Vector3 startPos = origin + i*cellSize*xVec;
			Vector3 endPos = startPos + height*zVec;
			Debug.DrawLine(startPos, endPos, color);
		}
	}
	
	
}
