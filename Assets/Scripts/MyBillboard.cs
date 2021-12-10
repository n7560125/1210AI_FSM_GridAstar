using UnityEngine;
using System.Collections;

/*---------------------------------------------------------------
 To generate a plane face to camera, but not always,
 so it's not a real billboard.
 ---------------------------------------------------------------*/
[ExecuteInEditMode]
public class MyBillboard : MonoBehaviour {
	public float m_fWidth = 10.0f;			// Plane width.
	public float m_fHeight = 10.0f;			// Plane height.
#if (UNITY_EDITOR  || UNITY_STANDALONE_OSX)
    private float m_fSaveWidth = 10.0f;		// For editor, saved the Plane width.
	private float m_fSaveHeight = 10.0f;	// For editor, saved the Plane height.
#endif
	
	void Awake () {
		
	}
	
	/*---------------------------------------------------------------
	 Generate the plane.
	 ---------------------------------------------------------------*/
	void Start () {
		MeshFilter mf = (MeshFilter)GetComponent(typeof(MeshFilter)) as MeshFilter;
		if(mf == null) {
			mf = (MeshFilter)this.gameObject.AddComponent(typeof(MeshFilter));
		}
		Mesh mesh = new Mesh();
		mf.mesh = mesh;
		Vector3[] vertices = new Vector3[4];
		float w = m_fWidth/2.0f;
		float h = m_fHeight/2.0f;
		vertices[0] = new Vector3(-w, -h, 0);
		vertices[1] = new Vector3(w, -h, 0);
		vertices[2] = new Vector3(-w, h, 0);
		vertices[3] = new Vector3(w, h, 0);

		mesh.vertices = vertices;
		
		int[] tri = new int[6];
		tri[0] = 0;
		tri[1] = 2;
		tri[2] = 1;
		tri[3] = 2;
		tri[4] = 3;
		tri[5] = 1;
		mesh.triangles = tri;

		//Vector3[] normals = new Vector3[4];
		//normals[0] = -Vector3.forward;
		//normals[1] = -Vector3.forward;
		//normals[2] = -Vector3.forward;
		//normals[3] = -Vector3.forward;
		//mesh.normals = normals;

		uv: Vector2[] uv = new Vector2[4];
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);
		mesh.uv = uv;	
	
		MeshRenderer mr = null;
		mr = (MeshRenderer)this.gameObject.GetComponent(typeof(MeshRenderer));
		if(mr == null) {
		   mr = (MeshRenderer)this.gameObject.AddComponent(typeof(MeshRenderer));
		   mr.material = new Material(Shader.Find("Unlit/Texture"));
		}
		
		//mr.material.color = Color.white;
	}
	
	/*---------------------------------------------------------------
	 Update this only in editor mode.
	 ---------------------------------------------------------------*/
	void Update () {
#if (UNITY_EDITOR)
	    if (m_fWidth != m_fSaveWidth)
	    {
			m_fSaveWidth = m_fWidth;
	        UpdateSize();
	    }
		if (m_fHeight != m_fSaveHeight)
	    {
			m_fSaveHeight = m_fHeight;
	        UpdateSize();
	    }
#else
		
		// Disable the update after invoke this object.
		this.enabled = false;
#endif
		
	}
	
	/*---------------------------------------------------------------
	 Update the billboard size.
	 ---------------------------------------------------------------*/
	void UpdateSize()
	{
		MeshFilter mf = (MeshFilter)GetComponent(typeof(MeshFilter)) as MeshFilter;
		Vector3[] vertices = new Vector3[4];

		float w = m_fWidth/2.0f;
		float h = m_fHeight/2.0f;
		vertices[0] = new Vector3(-w, -h, 0);
		vertices[1] = new Vector3(w, -h, 0);
		vertices[2] = new Vector3(-w, h, 0);
		vertices[3] = new Vector3(w, h, 0);

		mf.sharedMesh.vertices = vertices;
		mf.sharedMesh.RecalculateBounds();
	}
	
	/*---------------------------------------------------------------
	 Set texture to the plane.
	 ---------------------------------------------------------------*/
	public void SetTexture(Texture2D tex)
	{
		MeshRenderer mr = null;
		mr = (MeshRenderer)this.gameObject.GetComponent(typeof(MeshRenderer));
		if(mr != null) {
			if(mr.material.mainTexture != tex) {
				mr.material.mainTexture = tex;	
			}
		}
	}
}
