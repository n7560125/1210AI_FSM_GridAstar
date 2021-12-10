using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class NavigationComponent : MonoBehaviour 
{	
	private PathFindingGrid m_NavigationTerrain;
	private AStar m_AStarPlanner;
	private AStar.eFindingStatus m_Status;
	private Vector3 [] vPath;
	private bool bUpdateBlock;
	public PathFindingGrid NavigationGrid		
	{
		set { m_NavigationTerrain = value; }
		get { return m_NavigationTerrain; }
	}
	public AStar AStarPlanner
	{
		get { return m_AStarPlanner; }
	}
		
	void Awake()
	{
		m_NavigationTerrain = null;
		m_AStarPlanner= new AStar();
		m_Status = AStar.eFindingStatus.Waiting;
		bUpdateBlock = false;
	}
	public void Init(PathFindingGrid grid)
	{
		m_NavigationTerrain = grid;
		m_AStarPlanner.Init(m_NavigationTerrain);
	}
	
	
	public bool MoveToPosition(Vector3 targetPosition)
	{
		int iStartIndex = m_NavigationTerrain.GetNodeIndex(this.transform.position);
		int iDestIndex = m_NavigationTerrain.GetNodeIndex(targetPosition);
		Debug.Log("Start : " + iStartIndex.ToString());
		Debug.Log("Dest : " + iDestIndex.ToString());
		m_AStarPlanner.InitCalculation(iStartIndex, iDestIndex);
		m_AStarPlanner.PerformCalculation(1000);
		
		if(m_AStarPlanner.FindingStatus == AStar.eFindingStatus.Succeed) {
			int iCount = m_AStarPlanner.SolutionPath.Count;
			int index = 0;
			vPath = new Vector3[m_AStarPlanner.SolutionPath.Count];
			foreach(PathNode node in m_AStarPlanner.SolutionPath) {
				vPath[index] = m_NavigationTerrain.GetNodePosition(node.Index);
				index++;
			}
		}
		return true;
	}
	
	
	void Update()
	{
		if(bUpdateBlock == false) {
			m_NavigationTerrain.SyncNodeState(m_AStarPlanner.NodePool);
			bUpdateBlock = true;
		}
		if(m_AStarPlanner.FindingStatus == AStar.eFindingStatus.Succeed) {
			AStar.DebugDrawPath(vPath, Color.cyan);
		}
		
	}
	

}
