using UnityEngine;
using System.Collections;

public interface IAStarTerrainInterface 
{
	int GetNeighbors(int index, ref int[] neighbors);
	int GetNodesNumber();
    float GetHValue(int sIndex, int dIndex);
	float GetGValue(int sIndex, int dIndex);
	bool IsNodeBlocked(int index);
    int GetNodeIndex(Vector3 pos);
    Vector3 GetNodePosition(int index);
	float GetTerrainHeight(Vector3 position);
	void FillNodesToPool(MyPool<PathNode> pool);
}
