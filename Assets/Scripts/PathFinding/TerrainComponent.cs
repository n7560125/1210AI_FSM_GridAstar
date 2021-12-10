using UnityEngine;
using System.Collections;

public abstract class TerrainComponent : MonoBehaviour {
	protected IAStarTerrainInterface m_TerrainRepresentation;
	public IAStarTerrainInterface TerrainRepresentation
	{
		get { return m_TerrainRepresentation; }
	}
}
