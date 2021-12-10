using UnityEngine;
using System.Collections;

public class SimpleMoveBox : MonoBehaviour {
	
	public Camera m_Camera;
	public GameObject m_Ground;
	bool bMouseDown = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 vMouse = Input.mousePosition;
		if(Input.GetMouseButtonDown(0)) {
			bMouseDown = true;
		} else if(Input.GetMouseButtonUp(0)) {
			bMouseDown = false;
		}
		if(bMouseDown) {
			RaycastHit info;
			Ray ray = m_Camera.ScreenPointToRay(vMouse);
			if(m_Ground.GetComponent<Collider>().Raycast(ray, out info, 1000.0f)) {
				Vector3 vec = info.point - transform.position;
				vec.y = 0.0f;
				vec.Normalize();
				this.transform.position += vec*0.1f;
			}
		}
	}
}
