using UnityEngine;
using System.Collections;

/*---------------------------------------------------------------
 Minimap's controller to control the minimap.
 ---------------------------------------------------------------*/
public class MiniMapController : MonoBehaviour {
	
	/*---------------------------------------------------------------
	 Minimap object's mapper.
	 ---------------------------------------------------------------*/
	struct sMiniMapObject
	{
		public GameObject m_SourceObject;
		public GameObject m_TargetObject;
		public bool m_bShow;
	}
	
	// Instance self
	static private MiniMapController m_Instance = null;
	static public MiniMapController Instance() { return m_Instance; }
	
	public float fXRatio = 1.0f;				// ratio to map the minimap.
	public float fZRatio = 1.0f;				// ratio to map the minimap.
	public float fXOffset = 0.0f;				// offset-x to map the minimap.
	public float fZOffset = 0.0f;				// offset-z to map the minimap.
	private GameObject m_MiniMapPlane = null;   // Minimap plane.
	private Camera m_MiniMapCamera = null;		// Minimap camera.
	private sMiniMapObject m_ControlCharacter;	// Center target.
	private ArrayList m_Team1Targets = null;	// Team1 targets.
	private ArrayList m_Team2Targets = null;	// Team2 targets.
	private GameObject m_MapGroundObject;
	
	/*---------------------------------------------------------------
	 Initialize when being addcomponent.
	 ---------------------------------------------------------------*/
	void Awake()
	{
		m_Instance = this;
		m_MiniMapCamera = null;
		m_ControlCharacter.m_SourceObject = null;
		m_ControlCharacter.m_TargetObject = null;
		m_MiniMapCamera = this.transform.Find("MiniMapCamera").GetComponent<Camera>();
		m_MiniMapPlane = this.transform.Find("MiniMapPlane").gameObject;
		m_Team1Targets = new ArrayList();
		m_Team2Targets = new ArrayList();
	}
	
	/*---------------------------------------------------------------
	 Use this for initialization
	 ---------------------------------------------------------------*/
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		UpdateControlCharacter();
		UpdateAllTargets();
	}
	
	/*---------------------------------------------------------------
	 Update the target position.
	 ---------------------------------------------------------------*/
	private void UpdateSourceToTarget(GameObject src, GameObject tar)
	{
		if(src == null || tar == null) {
			return;	
		}
		Vector3 pos = src.transform.position;
		pos.x = pos.x*fXRatio + fXOffset;
		pos.z = pos.z*fZRatio + fZOffset;
		pos.y = 1.0f;
		tar.transform.position = pos;
	}
	
	public void SetMapGround(GameObject go)
	{
		m_MapGroundObject = go;
	}
	
	/*---------------------------------------------------------------
	 Set the minimap's texture.
	 ---------------------------------------------------------------*/
	public void SetMinimapTexture(GameObject mapObj, int iLV)
	{
	
		if(m_MiniMapPlane != null) {
			if(iLV == 1) {
				m_MapGroundObject = mapObj;
				float fXR = (float)m_MapGroundObject.GetComponent<Collider>().bounds.size.x / (float)m_MiniMapPlane.GetComponent<Collider>().bounds.size.x;
				float fZR = (float)m_MapGroundObject.GetComponent<Collider>().bounds.size.z / (float)m_MiniMapPlane.GetComponent<Collider>().bounds.size.z;

				Texture2D tex = Resources.Load("Level1") as Texture2D;
				Debug.Log(m_MiniMapPlane.GetComponent<Collider>().bounds.size.z);
				m_MiniMapPlane.transform.localScale = new Vector3(fXR, 1.0f, fZR);
				Debug.Log(m_MiniMapPlane.GetComponent<Collider>().bounds.size.z + "..");
				m_MiniMapPlane.GetComponent<Renderer>().material.mainTexture = tex;
				
			} 
		}
	}
	
	/*---------------------------------------------------------------
	 Update center target.
	 ---------------------------------------------------------------*/
	public void UpdateControlCharacter()
	{
		if(m_ControlCharacter.m_SourceObject == null) {
			return;	
		}
		
		Vector3 pos = m_ControlCharacter.m_SourceObject.transform.position;
		pos.x = pos.x*fXRatio + fXOffset;
		pos.z = pos.z*fZRatio + fZOffset;
		float y = m_MiniMapCamera.transform.position.y;
		pos.y = 1.0f;
		m_ControlCharacter.m_TargetObject.transform.position = pos;
		pos.y = y;
		m_MiniMapCamera.transform.position = pos;
	}
	
	/*---------------------------------------------------------------
	 Update all targets map to minimap.
	 ---------------------------------------------------------------*/
	public void UpdateAllTargets()
	{
		
		int iLen = m_Team2Targets.Count;
		int i = 0;
		for(i = 0; i < iLen; i++) {
			sMiniMapObject eObj = (sMiniMapObject)m_Team2Targets[i];
			UpdateSourceToTarget(eObj.m_SourceObject, eObj.m_TargetObject);
		}
		iLen = m_Team1Targets.Count;

		for(i = 0; i < iLen; i++) {
			sMiniMapObject aObj = (sMiniMapObject)m_Team1Targets[i];
			UpdateSourceToTarget(aObj.m_SourceObject, aObj.m_TargetObject);
		}
	}
	
	/*---------------------------------------------------------------
	 Setup the center target.
	 ---------------------------------------------------------------*/
	public void CreateControlCharacter(GameObject go)
	{
		m_ControlCharacter.m_SourceObject = go;
		m_ControlCharacter.m_TargetObject = this.transform.Find("MiniMapControlTarget").gameObject;
	}
	
	/*---------------------------------------------------------------
	 Find team2 target.
	 ---------------------------------------------------------------*/
	public int FindTeam2Object(GameObject go)
	{
		if(go == null) {
			return -1;
		}
		int iCount = m_Team2Targets.Count;
		int i = 0;
		sMiniMapObject obj;
		for(i = 0; i < iCount; i++) {
			obj = (sMiniMapObject)m_Team2Targets[i];	
			if(obj.m_SourceObject == go) {
				break;	
			}
		}
		if(i < iCount) {
			return i;	
		} else {
			return -1;	
		}
	}
	
	/*---------------------------------------------------------------
	 Find Team1 target.
	 ---------------------------------------------------------------*/
	public int FindTeam1Object(GameObject go)
	{
		if(go == null) {
			return -1;
		}
		int iCount = m_Team1Targets.Count;
		int i = 0;
		sMiniMapObject obj;
		for(i = 0; i < iCount; i++) {
			obj = (sMiniMapObject)m_Team1Targets[i];	
			if(obj.m_SourceObject == go) {
				break;	
			}
		}
		if(i < iCount) {
			return i;	
		} else {
			return -1;	
		}
	}
	
	/*---------------------------------------------------------------
	 Remove team2 target.
	 ---------------------------------------------------------------*/
	public void RemoveTeam2Target(GameObject go, bool bHideOnly = true)
	{
		int index = FindTeam2Object(go);
		if(index == -1) {
			return;
		}	
		if(bHideOnly) {
			ShowTeam2Target(index, false);
		} else {
			m_Team2Targets.RemoveAt(index);
		}
	}
	
	/*---------------------------------------------------------------
	 Add team2 target.
	 ---------------------------------------------------------------*/
	public void AddTeam2Target(GameObject go)
	{
		int index = FindTeam2Object(go);
		if(index != -1) {
			ShowTeam2Target(index, true);
			return;
		}	
		//DataManager.Instance().SetUIPath("UI");
		
		GameObject target = (GameObject)Instantiate(Resources.Load("MinimapTeam2Target"));//DataManager.Instance().InstanceGameObject("MiniMapTeam2Target",DataManager.eDataType.UI, true);
		if(target == null) {
			return;
		}
		sMiniMapObject miniObject;
		miniObject.m_SourceObject = go;
		miniObject.m_TargetObject = target;
		miniObject.m_bShow = true;
		m_Team2Targets.Add(miniObject);
	}
	
	/*---------------------------------------------------------------
	 Remove team1 target.
	 ---------------------------------------------------------------*/
	public void RemoveTeam1Target(GameObject go, bool bHideOnly = true)
	{
		int index = FindTeam1Object(go);
		if(index == -1) {
			return;
		}	
		if(bHideOnly) {
			ShowTeam1Target(index, false);
		} else {
			m_Team1Targets.RemoveAt(index);
		}
	}
	
	/*---------------------------------------------------------------
	 Show team1 target.
	 ---------------------------------------------------------------*/
	public void ShowTeam1Target(int index, bool beShow)
	{
		sMiniMapObject miniObject = (sMiniMapObject)m_Team1Targets[index];
		Renderer renderer = miniObject.m_TargetObject.GetComponent(typeof(Renderer)) as Renderer;
		renderer.enabled = beShow;
		miniObject.m_bShow = beShow;
		m_Team1Targets[index] = miniObject;
	}
	
	/*---------------------------------------------------------------
	 Show team2 target.
	 ---------------------------------------------------------------*/
	public void ShowTeam2Target(int index, bool beShow)
	{
		sMiniMapObject miniObject = (sMiniMapObject)m_Team2Targets[index];
		Renderer renderer = miniObject.m_TargetObject.GetComponent(typeof(Renderer)) as Renderer;
		renderer.enabled = beShow;
		miniObject.m_bShow = beShow;
		m_Team2Targets[index] = miniObject;
	}
	
	/*---------------------------------------------------------------
	 Add team1 target.
	 ---------------------------------------------------------------*/
	public void AddTeam1Target(GameObject go)
	{
		int index = FindTeam1Object(go);
		if(index != -1) {
			ShowTeam1Target(index, true);
			return;
		}	
		//DataManager.Instance().SetUIPath("UI");
		GameObject target = (GameObject)Instantiate(Resources.Load("MinimapTeam2Target"));//DataManager.Instance().InstanceGameObject("MiniMapTeam1Target",DataManager.eDataType.UI, true);
		if(target == null) {
			return;
		}
		
		sMiniMapObject miniObject;
		miniObject.m_SourceObject = go;
		miniObject.m_TargetObject = target;
		miniObject.m_bShow = true;
		m_Team1Targets.Add(miniObject);
	}
}
