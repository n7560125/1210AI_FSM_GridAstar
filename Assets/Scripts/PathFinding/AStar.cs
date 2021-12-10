using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class AStar
{
	public enum eFindingStatus
	{
		Waiting = -1,
		Finding = 0,
		Succeed = 1,
		Failed = 2
	};
	protected MyBinaryHeap<PathNode>     m_OpenHeap;
    protected MyPool<PathNode>           m_GridNodes;
    protected PathNode                 m_StartNode;
    protected PathNode                 m_GoalNode;
    protected LinkedList<PathNode>     m_PathNodes;          
	private	  IAStarTerrainInterface   m_TerrainInterface;
	private   eFindingStatus           m_FindingStatus;
	public MyPool<PathNode> NodePool
    {
        get { return m_GridNodes; }
    }
	public LinkedList<PathNode> SolutionPath
    {
        get { return m_PathNodes; }
    }
	public eFindingStatus FindingStatus
	{
		get { return m_FindingStatus; }	
	}
	
	public AStar()
    {
        
    }

    public void Init(IAStarTerrainInterface TerrainRepresentation)
    {
		m_TerrainInterface = TerrainRepresentation;
		
		m_OpenHeap = new MyBinaryHeap<PathNode>();
		m_PathNodes = new LinkedList<PathNode>();
		m_GridNodes = new MyPool<PathNode>();
		TerrainRepresentation.FillNodesToPool(m_GridNodes);
		m_FindingStatus = eFindingStatus.Waiting;
    }
	
	private void UncheckAllNode(bool bCheckBlock = true)
	{
		foreach(PathNode node in m_GridNodes) {
			if(bCheckBlock && node.State == PathNode.eNodeState.Block) {
				continue;
			}
			node.State = PathNode.eNodeState.UnCheck;
		}
	}
	
	private bool BeGoal(PathNode node)
	{
		bool bGoal = (node.Index == m_GoalNode.Index ? true : false);
		return bGoal;
	}
	
	private void OpenNode(PathNode node)
    {
        node.State = PathNode.eNodeState.Open;
        m_OpenHeap.Add(node);
    }

    private void CloseNode(PathNode node)
    {
        node.State = PathNode.eNodeState.Close;
    }
	
	private void UpdateNode(PathNode recordNode, PathNode parentNode, float fGValue)
	{
		recordNode.GValue = fGValue;
        recordNode.HValue = m_TerrainInterface.GetHValue(recordNode.Index, m_GoalNode.Index);
        recordNode.FValue = recordNode.GValue + recordNode.HValue;
        recordNode.Parent = parentNode;
	}
	
	private void RecordNode(PathNode recordNode, PathNode parentNode)
	{
		recordNode.GValue = m_TerrainInterface.GetGValue(parentNode.Index, recordNode.Index);
	    recordNode.GValue += parentNode.GValue;
        recordNode.HValue = m_TerrainInterface.GetHValue(recordNode.Index, m_GoalNode.Index);
        recordNode.FValue = recordNode.GValue + recordNode.HValue;
        recordNode.Parent = parentNode;
	}
	
	private PathNode GetNode(int index)
	{
		return m_GridNodes.GetItemFromPool(index);
	}
	
	private void BuildPath()
	{
		PathNode parent = m_GoalNode;
		while(parent != null) {
			m_PathNodes.AddFirst(parent);
			parent = parent.Parent;
		}
	}
	
	private eFindingStatus PerformAStarCycle()
    {
		// Check open list.
		if (m_OpenHeap.Count == 0) {
            return eFindingStatus.Failed;
        }
		
        // The current least costing pathnode is considered the "current node", which gets removed from the open list and added to the closed list.
        PathNode currentNode = m_OpenHeap.PopRoot();
        CloseNode(currentNode);

        if(BeGoal(currentNode)) {
            BuildPath();
            return eFindingStatus.Succeed;
        }
		int i;
        int [] neighbors = null;
		int iNeighbor = 0;
        int numNeighbors = m_TerrainInterface.GetNeighbors(currentNode.Index, ref neighbors);
		float fTempG = 0.0f;
		PathNode neighborNode;
        for (i = 0; i < numNeighbors; i++) {
            iNeighbor = neighbors[i];
            if (iNeighbor == (int)PathFindingGrid.eNeighborDirection.NoNeighbor) {
                continue;
            }
			
            neighborNode = GetNode(iNeighbor);
	
            switch (neighborNode.State)
            {
                case PathNode.eNodeState.Close:
					continue;
                case PathNode.eNodeState.Block:
                    continue;
				
				// If no go on the node before, then record the value and add it to open list.
                case PathNode.eNodeState.UnCheck:
                    RecordNode(neighborNode, currentNode);
                    OpenNode(neighborNode);
                    break;
				
				// If the node is already in open list, then update the score and binary heap.
                case PathNode.eNodeState.Open:
      				fTempG = m_TerrainInterface.GetGValue(currentNode.Index, neighborNode.Index);
                    fTempG += currentNode.GValue;
					// Check the value if less then before.
                    if (fTempG < neighborNode.GValue) {
                        UpdateNode(neighborNode, currentNode, fTempG);
                        // Maintain the binary heap.
                        m_OpenHeap.Remove(neighborNode);
                       m_OpenHeap.Add(neighborNode);
                    }

                    break;

                default:
                    System.Diagnostics.Debug.Assert(false, "Error");
                    break;
            };
        }

        return eFindingStatus.Finding;
    }
	
	public int PerformCalculation(int nWaitingCycles)
    {
        if (m_FindingStatus != eFindingStatus.Finding) {
            return 0;
        }

        int nTime = 0;

        while (nTime < nWaitingCycles) {
            m_FindingStatus = PerformAStarCycle();
            nTime++;
            if (m_FindingStatus == eFindingStatus.Failed || m_FindingStatus == eFindingStatus.Succeed) {
                break;
            } 
        }

        return nTime;
    }
	
	public void InitCalculation(int iStartNode, int iGoalNode)
	{
		if (iGoalNode	< 0 || iGoalNode < 0) {
			m_FindingStatus = eFindingStatus.Failed;
			return;
		}
		UncheckAllNode();
		m_OpenHeap.Clear();
		m_PathNodes.Clear();
		m_StartNode = GetNode(iStartNode);
		m_GoalNode = GetNode(iGoalNode);

        // Put the start node on the open list
        m_StartNode.GValue = 0.0f;
        m_StartNode.HValue = m_TerrainInterface.GetHValue(iStartNode, iGoalNode);
        m_StartNode.FValue = m_StartNode.HValue;
        m_StartNode.Parent = null;
        OpenNode(m_StartNode);

        m_FindingStatus = eFindingStatus.Finding;
	}
	
	public static void DebugDrawPath(Vector3 [] PathPoints, Color color) 
	{
		int i = 0;
		int iLen = PathPoints.Length;
		for(i = 0; i < iLen - 1; i++) {
			UnityEngine.Debug.DrawLine(PathPoints[i], PathPoints[i + 1], color);
		}
	}
}
