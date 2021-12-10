using UnityEngine;
using System.Collections;

public class ObstacleComponent : MonoBehaviour {
	
	public enum eObstacleType	
	{
		BLOCKER = 0,
		ROCK_ROAD,
	}
	public bool bEnable = true;
	public eObstacleType eType = eObstacleType.BLOCKER;
	
	void Start()
	{
		bEnable = true;
		
	}
	
}
