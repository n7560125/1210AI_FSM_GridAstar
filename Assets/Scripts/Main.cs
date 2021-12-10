using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	public GameObject m_ControlObject;
	public GameObject m_Ground;
	
	// Use this for initialization
	void Start () {
		MiniMapController.Instance().SetMinimapTexture(m_Ground, 1);
		MiniMapController.Instance().CreateControlCharacter(m_ControlObject);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
