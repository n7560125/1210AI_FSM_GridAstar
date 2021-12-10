using UnityEngine;
using System.Collections;

public class Main2 : MonoBehaviour {
	public GameObject m_ControlObject;
	public GameObject m_Ground;
	public GameObject m_FOWObject;
	
	// Use this for initialization
	void Start () {
		MiniMapController minimap = MiniMapController.Instance();
		minimap.SetMinimapTexture(m_Ground, 1);
		minimap.CreateControlCharacter(m_ControlObject);
		FOWGrid fow = FOWGrid.Instance();
		fow.InitData();
		fow.CreateGrid(0.75f, 0);
		fow.AddTarget(m_ControlObject, 5.0f);
		fow.AddTarget(GameObject.Find("Cube1"), 2.0f);
		fow.AddTarget(GameObject.Find("Cube2"), 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
		FOWGrid.Instance().UpdateFOW();
	}
}
