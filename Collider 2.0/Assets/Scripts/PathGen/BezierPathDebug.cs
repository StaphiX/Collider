using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierPathDebug : MonoBehaviour {

	public List<Bezier> m_tBezier = new List<Bezier>();
	public LineRenderer m_tLine;
	public GameObject m_goMarker;
	public List<GameObject> m_goBezierMarker = new List<GameObject>();
	int TESTPOINTS = 100;

	// Use this for initialization
	void Start () 
	{
		for(int i = 0; i < 4; ++ i)
			m_goBezierMarker.Add(Instantiate(m_goMarker) as GameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(m_tBezier != null && m_tBezier.Count > 0)
		{
			int iIndex = m_tBezier.Count-1;
			GetBezierUpdate(m_tBezier[iIndex]);
		}

	}


	void UpdateObjectPositions()
	{
		int iLineCount = m_tBezier.Count;
		m_tLine.SetVertexCount(iLineCount * TESTPOINTS + 1);
		for(int iLine = 0; iLine < iLineCount; ++iLine)
		{
			int iNumPointsToShow = TESTPOINTS;
			if(iLine == iLineCount-1)
				iNumPointsToShow++;
			for(int iPoint = 0; iPoint < iNumPointsToShow; ++iPoint)
			{
				float fTimeToGet = 1.0f / (float)TESTPOINTS * iPoint;
				Vector3 vPoint = m_tBezier[iLine].GetPointAtTime(fTimeToGet);
				int iPointIndex = iLine * TESTPOINTS + iPoint;
				m_tLine.SetPosition(iPointIndex, vPoint);
			}
		}
	}
	
	void AddPath()
	{
		Vector3 p0 = GetBezierStartPoint();
		Vector3 p3 = p0;
		p3.z += 100;
		Vector3 p1 = GetBezierStartHandlePoint(p0, p3);
		Vector3 p2 = p1 + p0 - p3;
		p3.x += Random.Range(0, 30.0f);
		p3.y += Random.Range(0, 30.0f);

		Bezier tBezier = new Bezier(p0, p1, p2, p3);
		m_tBezier.Add(tBezier);
		SetMarkerPositions(tBezier);
		UpdateObjectPositions();
	}

	void OnGUI()
	{
		if(GUI.Button(new Rect(5, 5,100, 50), "SPAWN \n TRACK"))
		{
			AddPath();
		}
	}

	Vector3 GetBezierStartPoint()
	{
		Vector3 vPoint = Vector3.zero;
		if(m_tBezier != null && m_tBezier.Count > 0)
		{
			int iIndex = m_tBezier.Count-1;
			vPoint = m_tBezier[iIndex].p3;
		}

		return vPoint;
	}

	Vector3 GetBezierStartHandlePoint(Vector3 vStartPoint, Vector3 vEndPoint)
	{
		Vector3 vPoint = (vEndPoint - vStartPoint)/2;
		if(m_tBezier != null && m_tBezier.Count > 0)
		{
			int iIndex = m_tBezier.Count-1;
			vPoint = -m_tBezier[iIndex].p2;
		}
		
		return vPoint;
	}

	void SetMarkerPositions(Bezier tBezier)
	{
		m_goBezierMarker[0].transform.position = tBezier.p0;
		m_goBezierMarker[1].transform.position = tBezier.p0 + tBezier.p1;
		m_goBezierMarker[2].transform.position = tBezier.p3 + tBezier.p2;
		m_goBezierMarker[3].transform.position = tBezier.p3;
	}

	void GetBezierUpdate(Bezier tBezier)
	{
		bool bHasChanged = false;
		if(m_goBezierMarker[0].transform.hasChanged)
		{
			tBezier.p0 = m_goBezierMarker[0].transform.position;
			bHasChanged = true;
		}
		if(m_goBezierMarker[3].transform.hasChanged)
		{
			tBezier.p3 = m_goBezierMarker[3].transform.position;
			bHasChanged = true;
		}
		if(m_goBezierMarker[1].transform.hasChanged)
		{
			tBezier.p1 = m_goBezierMarker[1].transform.position - tBezier.p0;
			bHasChanged = true;
		}
		if(m_goBezierMarker[2].transform.hasChanged)
		{
			tBezier.p2 = m_goBezierMarker[2].transform.position - tBezier.p3;
			bHasChanged = true;
		}

		if(bHasChanged)
			UpdateObjectPositions();
	}
}
