using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Track {
	
	static int CURVEDISTANCE = 500;
	Bezier m_tCurve;
	List<Gate> m_tGates = new List<Gate>();
	int m_iCurrentGate;
	
	public Track(Bezier tLastCurve)
	{
		m_tCurve = CreateCurve(tLastCurve);
		if(tLastCurve != null)
			SpawnGatesFromCurve();
	}
	
	Bezier CreateCurve(Bezier tLastCurve)
	{
		if(tLastCurve == null)
		{ 
			Vector3[] vControlPos = new Vector3[4];
			vControlPos[0] = new Vector3(0,0,0);
			vControlPos[1] = new Vector3(0,0,CURVEDISTANCE/3);
			vControlPos[2] = new Vector3(0,0,-CURVEDISTANCE/3);
			vControlPos[3] = new Vector3(0,0,CURVEDISTANCE);
			return new Bezier(vControlPos[0], vControlPos[1], vControlPos[2], vControlPos[3]);
		}
		else
		{;
			Vector3[] vControlPos = new Vector3[4];
			vControlPos[0] = tLastCurve.p3;
			vControlPos[1] = -tLastCurve.p2;
			vControlPos[2] = new Vector3(0,0,-CURVEDISTANCE/3);
			vControlPos[3] = new Vector3(vControlPos[0].x,vControlPos[0].y,vControlPos[0].z + CURVEDISTANCE);
			return new Bezier(vControlPos[0], vControlPos[1], vControlPos[2], vControlPos[3]);
		}
	}
	
	public Bezier GetBezier()
	{
		return m_tCurve;	
	}
	
	public void SpawnGatesFromCurve()
	{
		int iNumGates = (int)(500 / Gate.SPAWNDISTANCE);
		for(int iGate = 0; iGate < iNumGates; ++iGate)
		{
			float fSpawnTime = 1.0f / (float)iNumGates * iGate;
			Vector3 vSpawnPos = m_tCurve.GetPointAtTime(fSpawnTime);
			AddGate(vSpawnPos, fSpawnTime);
		}
		m_iCurrentGate = 0;
	}
	
	public void AddGate(Vector3 vSpawnPos, float fSpawnTime)
	{
		int iNumSections = Random.Range(1,4) + Random.Range(0,3) + Random.Range(0,3);
		float fRotation = Random.Range(0, 0.1f) + Random.Range(0, (Mathf.PI*2-0.1f)); //Maximum is mathf.pi*2
		
		m_tGates.Add(new Gate(AssetManager.GetObjInstance("GateSection"), 
			iNumSections, 
			fRotation, 
			vSpawnPos, fSpawnTime));
		
		int iColXOffset = Random.Range(0,128);
		int iColYOffset = Random.Range(0,128);
		m_tGates[m_tGates.Count-1].SetCol(iColXOffset,iColYOffset);
		//TrackManager.iGateColX += iColXOffset;
		//TrackManager.iGateColY += iColYOffset;
	}
	
	public void Delete()
	{
		foreach(Gate tGate in m_tGates)
		{
			tGate.Delete();
		}
		m_tGates.Clear();
	}
	
	public bool CheckGateCollison(float fPlayerTime, float fPlayerRotation)
	{
		bool bReturn = false;
		if(GetCurrentGateValid() == true)
		{
			if(m_tGates[m_iCurrentGate].CheckCollison(fPlayerRotation))
				bReturn = true;
			else
				bReturn = false;
		}
		
		return bReturn;
	}
	
	public bool CheckPassedGate(float fPlayerTime)
	{
		if(GetCurrentGateValid() == true)
		{
			if(m_tGates[m_iCurrentGate].GetCurveTime() < fPlayerTime)
			{
				return true;
			}
		}
		
		return false;
	}
			
	public void IncGate()
	{
		if(++m_iCurrentGate >= m_tGates.Count)
		{
			m_iCurrentGate = m_tGates.Count;
		}
	}
	
	public int GetCurrentGate()
	{
		return m_iCurrentGate;
	}
	
	public bool GetCurrentGateValid()
	{
		if(m_iCurrentGate >= 0 && m_iCurrentGate < m_tGates.Count)
			return true;
		
		return false;
	}
	
	public void GetGateRotation(ref float fMin, ref float fMax)
	{
		if(m_iCurrentGate < m_tGates.Count)
			m_tGates[m_iCurrentGate].GetRotation(ref fMin, ref fMax);
		else
		{
			fMin = 100;
			fMax = 100;
		}
	}

}
