using UnityEngine;
using System.Collections;

public class PathfindingGridComponent : TerrainComponent {
	
	public float 			m_fCellSize = 1.0f;
	public int 				m_nRows = 10;
	public int 				m_nColumns = 10;
	
	// Debug.
	public bool 			m_bDebug = true;
	public Color			m_DebugColor = Color.white;
	
	public PathFindingGrid FindingGrid
	{
		get { return m_TerrainRepresentation as PathFindingGrid; }
	}
	
	// Use this for initialization
	void Awake () {
		m_TerrainRepresentation = new PathFindingGrid();
		
		FindingGrid.Init(transform.position, m_nRows, m_nColumns, m_fCellSize);
		
	}
	
	void Start () {
		RefreshCellState();
	}	
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void RefreshCellState()
	{
		FindingGrid.ResetCellBlockState();
		
		ObstacleComponent[] ObstacleArray = (ObstacleComponent[])GameObject.FindObjectsOfType(typeof(ObstacleComponent));
		foreach (ObstacleComponent obstacle in ObstacleArray)
		{	
			if(obstacle.GetComponent<Collider>() == null) {
				continue;
			}	
			Bounds bounds = obstacle.GetComponent<Collider>().bounds;
			FindingGrid.SetCellStateInRect(bounds, -1);

		}		
	}
	
	
	void OnDrawGizmos()
	{
		Gizmos.color = m_DebugColor;
		if(m_bDebug) {
			BaseGrid.DebugDraw(transform.position, m_nRows, m_nColumns, m_fCellSize, Gizmos.color);
			if(FindingGrid != null) {
				Vector3 cellPos;
				Vector3 size;
				for(int i = 0; i < FindingGrid.GetNodesNumber(); i++) {
					if(FindingGrid.IsNodeBlocked(i)) {
						cellPos = FindingGrid.GetCellCenter(i);
						size = new Vector3(FindingGrid.CellSize, 0.3f, FindingGrid.CellSize);
						UnityEngine.Gizmos.DrawCube(cellPos, size);
					}
				}
			}
			//PathFindingGrid.DebugDrawBlock(FindingGrid);
		}
	}
	
}
