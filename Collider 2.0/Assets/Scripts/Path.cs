using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path {
	public GameObject m_goPathHead;
	public List<GameObject> m_goPathSection = new List<GameObject>();
	Vector3 m_vPrevHeadPos = Vector3.zero;
	Vector3[] m_vSectionVertexOffset;
	Vector3[] m_vHeadVertexOffset;
	int iObjCount = 0;
	
	float m_fRotation = 0.0f;
	float m_fRotationVelocity = 0.0f;
	float m_fRotationVelocityStep = 0.002f;
	bool m_bCanBoost = true;
	const float FRICTION = 0.98f;
	const float GRAVITY = 0.005f;
	const float DEADZONE = 0.17f;
	const float BOOST = 0.10f;
	public const float RADIUS = 10.0f;
	const int SECTIONCOUNT = 18;
	float m_iTouchTime = 0;
	
	
	public void SetMaterialCol(Color color)
	{
		m_goPathHead.renderer.sharedMaterial.color = color;
	}
	
	// Use this for initialization
	public void Init () {
		
		m_goPathHead = AssetManager.GetObjInstance("PathHead");
		SetHeadPosition(GetPointFromRotation(Vector3.zero, m_fRotation, RADIUS));
			
		GameObject goBase = AssetManager.GetObjBase("PathSection");
		Mesh tMesh = goBase.GetComponent<MeshFilter>().sharedMesh;
		m_vSectionVertexOffset = new Vector3[tMesh.vertexCount];
		Vector3 vBasePos = goBase.transform.position;
		for(int iVert = 0; iVert < m_vSectionVertexOffset.Length; ++iVert)
		{
			m_vSectionVertexOffset[iVert] = tMesh.vertices[iVert] - vBasePos;
		}
		
		goBase = AssetManager.GetObjBase("PathHead");
		tMesh = goBase.GetComponent<MeshFilter>().sharedMesh;
		m_vHeadVertexOffset = new Vector3[tMesh.vertexCount];
		vBasePos = Vector3.zero;
		for(int iVert = 0; iVert < m_vHeadVertexOffset.Length; ++iVert)
		{
			m_vHeadVertexOffset[iVert] = tMesh.vertices[iVert] - vBasePos;
		}
	
		AddSegment(AssetManager.GetObjInstance("PathSection"), GetPointFromRotation(Vector3.zero, m_fRotation, RADIUS));
	}
	
	public void SetHeadPosition(Vector3 vPos)
	{
		Quaternion qRot = CalculateObjRotation(m_goPathHead);
		m_goPathHead.transform.position = vPos;
		m_goPathHead.transform.rotation = qRot;
		SetHeadVerts();
	}
	
	public void AddSegment(GameObject goSection, Vector3 vPos)
	{
		Quaternion qRot = CalculateObjRotation(goSection);
		goSection.transform.rotation = qRot;
		goSection.transform.position = vPos;
		SetSectionVerts(goSection);
		iObjCount += 1;
	}
	
	public void DeleteSegment()
	{
		if(iObjCount > SECTIONCOUNT)
		{
			if(Application.isEditor)
			{
				Mesh.DestroyImmediate(m_goPathSection[0].GetComponent<MeshFilter>().mesh);
				GameObject.DestroyImmediate(m_goPathSection[0]);
			}
			else
			{
				Mesh.Destroy(m_goPathSection[0].GetComponent<MeshFilter>().mesh);
				GameObject.Destroy(m_goPathSection[0]);
			}
			m_goPathSection.RemoveAt(0);
			iObjCount -= 1;
		}	
	}
	
	// Update is called once per frame
	public void Update (Vector3 vTrackPos) {
		
		GetInput();
		CalculateRotation();
		Vector3 vHeadPoint = GetPointFromRotation(vTrackPos, m_fRotation, RADIUS);
		m_vPrevHeadPos = m_goPathHead.transform.position;
		if(Mathf.Abs((vTrackPos - m_goPathSection[iObjCount-1].transform.position).sqrMagnitude) > 1)
		{
			AddSegment(AssetManager.GetObjInstance("PathSection"), m_vPrevHeadPos);
		}
		//Must be done after add segment to ensure they join
		SetHeadPosition(vHeadPoint);
		DeleteSegment();
		
	}
	
	public float GetRotation()
	{
		return m_fRotation;
	}
	
	public Vector3 GetPointFromRotation(Vector3 vCenterPoint, float fRotation, float fRadius)
	{
		Vector3 vPos = vCenterPoint;
		vPos.x = vCenterPoint.x + fRadius * Mathf.Cos(fRotation - Mathf.PI/2);
		vPos.y = vCenterPoint.y + fRadius * Mathf.Sin(fRotation - Mathf.PI/2);
		
		return vPos;
	}
	
	void GetInput()
	{
		bool bLeft = false;
		bool bRight = false;
		bool bBoost = false;
		
		float fMaxTouchSize = 300.0f;
		
		Rect tLeftRect = new Rect(0, 0, Mathf.Min(Screen.height*0.3f, fMaxTouchSize), Screen.height);
		Rect tRightRect = new Rect(Mathf.Min(Screen.height*0.3f, fMaxTouchSize), 0, Mathf.Min(Screen.height*0.3f, fMaxTouchSize), Screen.height);
		Rect tBoostRect = new Rect(Screen.width /2, 0, Screen.width /2, Screen.height);
		
		Vector3 vTouchPos = new Vector3(-1,-1,-1);
		if(Input.GetMouseButton(0))
		{
			vTouchPos = Input.mousePosition;
			m_iTouchTime += 1;
		}
		else
		{
			m_iTouchTime = 0;
		}
		
		if(Input.GetKey(KeyCode.LeftArrow) || InputManager.GetFirstTouchInRect(tLeftRect, out vTouchPos))
			bLeft = true;
		if(Input.GetKey(KeyCode.RightArrow) || InputManager.GetFirstTouchInRect(tRightRect, out vTouchPos))
			bRight = true;
		if(Input.GetKey(KeyCode.DownArrow) || InputManager.GetFirstTouchInRect(tBoostRect, out vTouchPos))
			bBoost = true;
		
		if(bRight)
		{
			m_fRotationVelocity += m_fRotationVelocityStep;
			m_fRotation += m_fRotationVelocityStep;
			if(bBoost && m_bCanBoost)
			{
				m_fRotationVelocity += BOOST;
				m_bCanBoost = false;
			}
		}
		if(bLeft)
		{
			m_fRotationVelocity -= m_fRotationVelocityStep;
			m_fRotation -= m_fRotationVelocityStep;
			if(bBoost && m_bCanBoost)
			{
				m_fRotationVelocity -= BOOST;
				m_bCanBoost = false;
			}
		}
		if(bBoost && m_bCanBoost && !bRight && !bLeft)
		{
			if(m_fRotationVelocity < 0.0f)
				m_fRotationVelocity -= BOOST;
			else
				m_fRotationVelocity += BOOST;
			m_bCanBoost = false;
		}
		if(!bBoost)
		{
			m_bCanBoost = true;
		}

	}
	
	void CalculateRotation()
	{
		m_fRotationVelocity *= FRICTION;
	    if(m_fRotation > DEADZONE) 
                m_fRotationVelocity -= GRAVITY;
        else if(m_fRotation < -DEADZONE)
                m_fRotationVelocity += GRAVITY;
		
		m_fRotation += m_fRotationVelocity;
		
		if(m_fRotation > Mathf.PI)
			m_fRotation -= Mathf.PI*2;
		else if(m_fRotation < -Mathf.PI)
			m_fRotation += Mathf.PI*2;
	}
	
	Quaternion CalculateObjRotation(GameObject goObj)
	{
		return Quaternion.AngleAxis(m_fRotation * Mathf.Rad2Deg, goObj.transform.forward);
	}
	
	
	
	//Path Section Setup
	
	void SetHeadVerts()
	{
		GameObject goNewObject = m_goPathHead;
		GameObject goPrevObject = null;

		if(m_goPathSection.Count >= 1)
		{
			goPrevObject = m_goPathSection[m_goPathSection.Count-1];
			Mesh thisMesh = goNewObject.GetComponent<MeshFilter>().mesh;
			Vector3[] vThisVerts = thisMesh.vertices;
			Transform thisTrans = goNewObject.transform;
			Transform prevTrans = goPrevObject.transform;
			
			SetAttachVerts(m_vHeadVertexOffset,m_vSectionVertexOffset, prevTrans, thisTrans, ref vThisVerts);
			
			thisMesh.vertices = vThisVerts;
			thisMesh.RecalculateBounds();
			thisMesh.RecalculateNormals();
		}	
		
	}
	
	void SetSectionVerts(GameObject goSection)
	{
		GameObject goNewObject = goSection;
		GameObject goPrevObject = null;

		if(m_goPathSection.Count >= 1)
		{
			goPrevObject = m_goPathSection[m_goPathSection.Count-1];
			Mesh thisMesh = goNewObject.GetComponent<MeshFilter>().mesh;
			Vector3[] vThisVerts = thisMesh.vertices;
			Transform thisTrans = goNewObject.transform;
			Transform prevTrans = goPrevObject.transform;
			
			SetAttachVerts(m_vSectionVertexOffset,m_vSectionVertexOffset, prevTrans, thisTrans, ref vThisVerts);
			
			thisMesh.vertices = vThisVerts;
			thisMesh.RecalculateBounds();
			thisMesh.RecalculateNormals();
		}	
		
		m_goPathSection.Add(goNewObject);
	}
	
	void SetAttachVerts(Vector3[] vBaseVerts, Vector3[] vPrevBaseVerts, Transform prevTrans, Transform thisTrans, ref Vector3[] vThisVerts)
	{
		for(int iVert = 0; iVert < vBaseVerts.Length; ++iVert)
		{
			if(vBaseVerts[iVert].z < -0.01f)//BACK
			{
				int iMatchingVert = FindMatchingFrontVert(vBaseVerts, iVert);

				Vector3 prevPoint = prevTrans.TransformPoint(vPrevBaseVerts[iMatchingVert]);
				Vector3 thisPoint = thisTrans.TransformPoint(vBaseVerts[iVert]);
				Vector3 vOffset = thisTrans.InverseTransformDirection(prevPoint - thisPoint);

				vThisVerts[iVert] = vBaseVerts[iVert] + vOffset;
			}
		}
	}
	
	int FindMatchingFrontVert(Vector3[] vBaseVerts, int iOffsetIndex)
	{
		float fX = vBaseVerts[iOffsetIndex].x;
		float fY = vBaseVerts[iOffsetIndex].y;
		
		int iClosest = 0;
		float sqrDist = -1;
		for(int iVert = 0; iVert < m_vSectionVertexOffset.Length; ++iVert)
		{
			if(m_vSectionVertexOffset[iVert].z > 0.01f)//FRONT
			{
				float newDist = Mathf.Abs(m_vSectionVertexOffset[iVert].x - fX + m_vSectionVertexOffset[iVert].y - fY);
				
				if(sqrDist < 0 || newDist < sqrDist)
				{
					sqrDist = newDist;
					iClosest = iVert;
					if(sqrDist < 0.0001f)
					{
						return iClosest;
					}
				}
			}
		}
						
		return iClosest;
		
	}
}
