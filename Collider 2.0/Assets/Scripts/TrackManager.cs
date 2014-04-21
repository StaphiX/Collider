using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackManager
{
	List<Track> m_tTrackCurves = new List<Track>();
	int m_iMinCurves = 2;
	int m_iCurveCount = 0;
	
	float m_fCurrentTime = 0.0f;
	int m_iCurrentTrack = 0;
	float m_fSpeed = 1.0f;
	float m_fCameraTime = 0.0f;
	const int CAMERAPOINTS = 25;
	float[] m_fNextCameraTime = new float[CAMERAPOINTS];
	int[] m_iNextCameraTrack = new int[CAMERAPOINTS];
	int m_iCameraTrack = 0;
	int m_iCameraTimeIndex = 0;
	
	public static int iGateColX = 69;
	public static int iGateColY = 69;
	
	List<GameObject> m_goLineRender = new List<GameObject>();
	
	// Use this for initialization
	public void Init () {

		for (int iCurve = 0; iCurve < m_iMinCurves; ++iCurve)
		{
			AddCurve();
		}
	}
	
	public float GetCurrentTime()
	{
		return m_fCurrentTime;
	}
	
	private Bezier GetCurve(int iIndex)
	{
		return m_tTrackCurves[iIndex].GetBezier();
	}
	
	private Track GetTrack(int iIndex)
	{
		return m_tTrackCurves[iIndex];
	}
	
	public Track GetCurrentTrack()
	{
		return m_tTrackCurves[m_iCurrentTrack];
	}
	
	void AddCurve()
	{
		if(m_iCurveCount < 1)
			m_tTrackCurves.Add(new Track(null));
		else
			m_tTrackCurves.Add(new Track(GetCurve(m_iCurveCount-1)));
		++m_iCurveCount;
		
		//Setup renderer
		
		/*
		m_goLineRender.Add(new GameObject());
		m_goLineRender[m_goLineRender.Count-1].AddComponent<LineRenderer>();
		int iPointCount = Bezier.NUMPOINTS;
		LineRenderer tLineRenderer = m_goLineRender[m_goLineRender.Count-1].GetComponent<LineRenderer>();
		tLineRenderer.SetVertexCount(iPointCount+1);
		for(int iPoint = 0; iPoint < iPointCount; ++iPoint)
		{
			float fTime = 1.0f / iPointCount * iPoint;
			tLineRenderer.SetPosition(iPoint, m_tTrackCurves[m_iCurveCount-1].GetPointAtTime(fTime));
		}
		tLineRenderer.SetPosition(iPointCount, m_tTrackCurves[m_iCurveCount-1].GetPointAtTime(1.0f));
		*/
	}
	
	public Vector3 GetNextTrackPoint()
	{
		float fNewTime = GetCurve(m_iCurrentTrack).GetEstTimeFromDistance(m_fSpeed, m_fCurrentTime);
		if(fNewTime >= 1.0f)
		{
			Vector3 vStartPos = GetCurve(m_iCurrentTrack).GetPointAtTime(m_fCurrentTime);
			Vector3 vEndPos = GetCurve(m_iCurrentTrack).GetPointAtTime(1.0f);
			float fDistance = (vEndPos - vStartPos).magnitude;
			float fDistanceToGet = m_fSpeed - fDistance;
			//Move camera tracks
			if(++m_iCurrentTrack < m_iCurveCount)
			{
				m_fCurrentTime = 0.0f;
				fNewTime = GetCurve(m_iCurrentTrack).GetEstTimeFromDistance(fDistanceToGet, m_fCurrentTime);
				AddCurve();
			}
			else
			{
				--m_iCurrentTrack;
			}
		}

		m_fCurrentTime = fNewTime;
		
		m_fNextCameraTime[m_iCameraTimeIndex] = m_fCurrentTime;
		m_iNextCameraTrack[m_iCameraTimeIndex] = m_iCurrentTrack;
		
		if(++m_iCameraTimeIndex >= CAMERAPOINTS)
			m_iCameraTimeIndex = 0;
		
		return GetCurve(m_iCurrentTrack).GetPointAtTime(m_fCurrentTime);
	}
	
	public Vector3 GetCameraPoint()
	{
		int iThisCameraTimeIndex = m_iCameraTimeIndex-CAMERAPOINTS;
		if(iThisCameraTimeIndex < 0)
			iThisCameraTimeIndex += CAMERAPOINTS;
		
		if(m_iCameraTrack != m_iNextCameraTrack[iThisCameraTimeIndex])
			m_iCameraTrack = m_iNextCameraTrack[iThisCameraTimeIndex];
		m_fCameraTime = m_fNextCameraTime[iThisCameraTimeIndex];
		
		return GetCurve(m_iCameraTrack).GetPointAtTime(m_fCameraTime);
	}
	
	public bool CheckForDelete()
	{
		if(m_iCameraTrack > 0)
		{
			for(int iTrack = 0; iTrack < m_iCameraTrack; ++iTrack)
			{
				RemoveTrack(0);
				--m_iCurveCount;
				
				--m_iCurrentTrack;
				--m_iCameraTrack;
				
				for(int iNextTrack = 0; iNextTrack < CAMERAPOINTS; ++iNextTrack)
				{
					--m_iNextCameraTrack[iNextTrack];
				}
			}
			
			return true;
		}
		
		return false;
	}
	
	public bool GetCurrentGateIsValid()
	{
		if(m_iCurveCount > 0)
		{
			return m_tTrackCurves[m_iCurrentTrack].GetCurrentGateValid();
		}
		
		return false;
	}
	
	public void RemoveTrack(int iIndex)
	{
		m_tTrackCurves[iIndex].Delete();
		m_tTrackCurves.RemoveAt(iIndex);
	}
	
	public bool CheckGateCollison(float fPlayerTime, float fPlayerRotation)
	{
		if(m_iCurveCount > 0)
		{
			if(m_tTrackCurves[m_iCurrentTrack].GetCurrentGateValid())
				return m_tTrackCurves[m_iCurrentTrack].CheckGateCollison(fPlayerTime, fPlayerRotation);
			else if(m_tTrackCurves[m_iCurrentTrack+1].GetCurrentGateValid())
				return m_tTrackCurves[m_iCurrentTrack+1].CheckGateCollison(fPlayerTime, fPlayerRotation);
		}
		
		return false;
	}
	
	public bool CheckPassedGate(float fPlayerTime)
	{
		if(m_iCurveCount > 0)
			return m_tTrackCurves[m_iCurrentTrack].CheckPassedGate(fPlayerTime);
		
		return false;
	}
	
	public void IncGate()
	{
		if(m_iCurveCount > 0)
			m_tTrackCurves[m_iCurrentTrack].IncGate();
	}
	
	public void GetGateRotation(ref float fMin, ref float fMax)
	{
		m_tTrackCurves[m_iCurrentTrack].GetGateRotation(ref fMin, ref fMax);
		if(fMin == 100 && fMax == 100)
			m_tTrackCurves[m_iCurrentTrack+1].GetGateRotation(ref fMin, ref fMax);
	}
	
	public bool GetGateCollision(float fPlayerTime, float fPlayerRotation)
	{
		if(m_tTrackCurves[m_iCurrentTrack].GetCurrentGateValid())
			return m_tTrackCurves[m_iCurrentTrack].CheckGateCollison(fPlayerTime, fPlayerRotation);
		else
			return m_tTrackCurves[m_iCurrentTrack+1].CheckGateCollison(fPlayerTime, fPlayerRotation);
	}
}
