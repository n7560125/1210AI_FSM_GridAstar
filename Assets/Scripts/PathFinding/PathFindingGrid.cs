using UnityEngine;
using System.Collections;

public class PathFindingGrid : BaseGrid, IAStarTerrainInterface
{
	public enum eNeighborDirection
    {
        NoNeighbor = -1,
        Left,
        Top,
        Right,
        Bottom,
		LeftTop,
		RightTop,
		LeftBottom,
		RightBottom,
		No_Direction
    };
	
	private int m_inNeighbor;
	private float m_fPaddingRatio;
	public PathFindingGrid()
	{
		m_inNeighbor = (int)eNeighborDirection.No_Direction;
		m_fPaddingRatio = 4.0f;
	}

	
	public override void Init(Vector3 origin, int numRows, int numCols, float cellSize) 
	{
		base.Init(origin, numRows, numCols, cellSize);
		
	}
	
	public void FillNodesToPool(MyPool<PathNode> pool)
	{
		pool.Init(m_nCell, true);
		int i, j;
		for (i = 0; i < m_nCol; i++)
		{
			for (j = 0; j < m_nRow; j++)
			{
				PathNode node = new PathNode();
				node.Index = i*m_nRow + j;
				pool.AddElementToPool(node, node.Index.ToString(), node.Index);
			}
		}
	}
	
	public void SyncNodeState(MyPool<PathNode> pool)
	{
		int iState = 0;
		foreach(PathNode node in pool) {
			iState = GetCellState(node.Index);
			if(iState < 0) {
				node.State = PathNode.eNodeState.Block;	
			}
		}
	}
	
	private int GetNeighbor(int index, eNeighborDirection nDirection)
    {
		Vector3 neighborPos = GetCellCenter(index);
		
        switch (nDirection)
        {
            case eNeighborDirection.Left:
				neighborPos.x -= m_fCellSize;
                break;
            case eNeighborDirection.Top:
				neighborPos.z += m_fCellSize;
                break;
            case eNeighborDirection.Right:
				neighborPos.x += m_fCellSize;
                break;
            case eNeighborDirection.Bottom:
				neighborPos.z -= m_fCellSize;
                break;
			case eNeighborDirection.LeftTop:
				neighborPos.x = neighborPos.x - m_fCellSize;
				neighborPos.z = neighborPos.z + m_fCellSize;
                break;
            case eNeighborDirection.LeftBottom:
				neighborPos.x = neighborPos.x - m_fCellSize;
				neighborPos.z = neighborPos.z - m_fCellSize;
                break;
            case eNeighborDirection.RightBottom:
				neighborPos.x = neighborPos.x + m_fCellSize;
             	neighborPos.z = neighborPos.z - m_fCellSize;  
				break;
            case eNeighborDirection.RightTop:
				neighborPos.x = neighborPos.x + m_fCellSize;
            	neighborPos.z = neighborPos.z + m_fCellSize;   
			    break;
            default:
                System.Diagnostics.Debug.Assert(false);
                break;
        };
		
		int neighborIndex = GetCellIndex(neighborPos);
		if ( !BeInBoundary(neighborIndex) )
		{
			neighborIndex = (int)eNeighborDirection.NoNeighbor;
		}

        return neighborIndex;
    }

	public eNeighborDirection GetNeighborDirection(int index, int iNeighborIndex)
	{
		for (int i = 0; i < m_inNeighbor; i++)
        {
            int testNeighborIndex = GetNeighbor(index, (eNeighborDirection)i);
			if ( testNeighborIndex	== iNeighborIndex )
			{
				return (eNeighborDirection)i;
			}
        }
		
		return eNeighborDirection.NoNeighbor;
	}
	
    

    public int GetNeighbors(int index, ref int[] neighbors)
    {
        neighbors = new int[m_inNeighbor];

        for (int i = 0; i < m_inNeighbor; i++)
        {
            neighbors[i] = GetNeighbor(index, (eNeighborDirection)i);
        }

        return m_inNeighbor;
    }

    public int GetNodesNumber()
    {
        return m_nCell;
    }
	
    public float GetGValue(int sIndex, int dIndex)
    {
        Vector3 startPos = GetNodePosition(sIndex);
        Vector3 goalPos = GetNodePosition(dIndex);
        float cost = Vector3.Distance(startPos, goalPos);
        return cost;
    }
	
	public float GetHValue(int sIndex, int dIndex)
    {
        Vector3 startPos = GetNodePosition(sIndex);
        Vector3 goalPos = GetNodePosition(dIndex);
		float heuristicWeight = 1.0f;
		float cost = heuristicWeight * Vector3.Distance(startPos, goalPos);
		// Give extra cost to height difference
		cost = cost + Mathf.Abs(goalPos.y - startPos.y) * 1.0f;
        return cost;
    }
	
    public bool IsNodeBlocked(int index)
    {
        if (BeInBoundary(index) == false) {
            return true;
        }
		
		int iState = GetCellState(index);
		if(iState < 0) {
			return true;
		} else {
			return false;
		}
       
    }

    public Vector3 GetNodePosition(int index)
    {
        Vector3 pos = GetCellPosition(index);
        pos += new Vector3(m_fCellSize/2.0f, 0.0f, m_fCellSize / 2.0f);
		pos.y = GetTerrainHeight(pos);
        return pos;
    }

    public int GetNodeIndex(Vector3 pos)
    {
        int index = GetCellIndex(pos);
        if (!BeInBoundary(index)) {
			index = -1;
		}
		
        return index;
    }
	
	public bool BeBlocked(int index)
    {
      	if(GetCellState(index) < 0) {
			return true;
		} else {
			return false;
		}
    }
	
	
	
	
	public float GetTerrainHeight(Vector3 position)
	{
		return m_vOrigin.y;
	}
	
	public void ResetCellBlockState()
	{
		for(int i = 0; i < m_nCell; i++) {
			SetCellState(i, 0);
		}
	}
	
	public void SetCellStateInRect(Bounds bounds, int iState)
	{
		Vector3 UL = new Vector3( bounds.min.x, m_vOrigin.y, bounds.max.z );
		Vector3 UR = new Vector3( bounds.max.x, m_vOrigin.y, bounds.max.z );
		Vector3 DL = new Vector3( bounds.min.x, m_vOrigin.y, bounds.min.z );
		Vector3 DR = new Vector3( bounds.max.x, m_vOrigin.y, bounds.min.z );
		
		Vector3 hDir = UR - UL;
		hDir.y = 0.0f;
		
		Vector3 vDir = UL - DL;
		vDir.y = 0.0f;
		
		float hLength = bounds.size.x;
		float vLength = bounds.size.z;
		hDir.Normalize();
		vDir.Normalize();
		
		int maxOverlapRow = (int)(vLength/m_fCellSize) + 2;
		int maxOverlapCol = (int)(hLength/m_fCellSize) + 2;
		int i, j;
		int iTempCellIndex = 0;
		float fCurrentVLength = 0.0f;
		float fCurrentHLength = 0.0f;
		Vector3 tempPos;

		for (i = 0; i < maxOverlapRow; i++ ) {
			fCurrentVLength = i*m_fCellSize;
			for (j = 0; j < maxOverlapCol; j++ )
			{
				fCurrentHLength = j*m_fCellSize;
				tempPos = DL + hDir*fCurrentHLength + vDir*fCurrentVLength;
				tempPos.x = Mathf.Clamp(tempPos.x, bounds.min.x, bounds.max.x);
				tempPos.z = Mathf.Clamp(tempPos.z, bounds.min.z, bounds.max.z);
				if (BeInBoundary(tempPos)) {
					iTempCellIndex = GetCellIndex(tempPos);
					if(iTempCellIndex > -1) { 
						SetCellState(iTempCellIndex, iState);

					}
				}
				if (fCurrentHLength > hLength ) {
					break;
				}
			}
			if(fCurrentVLength > vLength ) {
				break;
			}
		}
	}
	
}
