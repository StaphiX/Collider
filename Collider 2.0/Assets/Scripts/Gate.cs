using UnityEngine;
using System.Collections;

public class Gate{

	GameObject[] m_goGate;
	float m_fCurveTime = 0.0f;
	
	int m_iNumSections = 0;
	float m_fRotation = 0.0f;
	int m_iCurveIndex;
	public static float SPAWNDISTANCE = 85.0f;
	public static float DEFAULTROTATION = Mathf.PI;
	public static float DEFUALTPATHWIDH = 3.1f;
	public static float DEFAULTPATHRADIUS = Path.RADIUS;
	public static float m_fPathSizeR = 0.0f;
	
	public Gate(GameObject goGateSection, int iNumSections, float fRotation, Vector3 vBasePos,float fCurveTime)
	{
		if(m_fPathSizeR == 0.0f)
			CalculatePathSizeRadians();
		
		m_fCurveTime = fCurveTime;
		m_iNumSections = iNumSections;
		m_fRotation = fRotation;
		
		Vector3 vFacePos = vBasePos;
		vFacePos.z += 10;
		
		m_goGate = new GameObject[m_iNumSections];
		m_goGate[0] = goGateSection;
		m_goGate[0].name = "Gate0";
		m_goGate[0].transform.position = vBasePos;
		m_goGate[0].transform.localRotation = GetSectionRotation(vBasePos,vFacePos, 0);
		
		//AddDuplicate(m_goGate[0], 0);
		
		for(int iSection = 1; iSection < m_iNumSections; ++iSection)
		{
			m_goGate[iSection] = Object.Instantiate(m_goGate[0],
				vBasePos, 
			GetSectionRotation(vBasePos, vFacePos, iSection)) as GameObject;
			m_goGate[iSection].name = "Gate"+iSection.ToString();
			
			//AddDuplicate(m_goGate[iSection], iSection);
		}
		
		//m_goCrap = GameObject.Instantiate(TrackManager.goCrapPart, vBasePos, GetSectionRotation(vBasePos,vFacePos, Random.Range(0, 10))) as GameObject;
		
		//float fScale = Random.Range(2.0f, 10.0f);
		//m_goCrap.transform.localScale = new Vector3(fScale, fScale, fScale*2);
		
	}
	
	public void SetCol(int iColX, int iColY)
	{
		for(int iSection = 0; iSection < m_goGate.Length; ++iSection)
		{
			m_goGate[iSection].GetComponent<GateColour>().SetColour(iColX, iColY);
		}
	}
	
	public float GetCurveTime()
	{
		return m_fCurveTime;
	}
				
	private Quaternion GetSectionRotation(Vector3 vBasePos, Vector3 vFacePos, int iSection)
	{
		float fSectionRotation = (iSection * Mathf.PI*2 / 8) + m_fRotation;
		fSectionRotation *= Mathf.Rad2Deg;
		Quaternion qRotation = Quaternion.AngleAxis(fSectionRotation, (vFacePos - vBasePos).normalized); 
		return qRotation;
	}
	
	public int GetCurveIndex()
	{
		return m_iCurveIndex;
	}
	
	public void SetCurveIndex(int iCurve)
	{
		m_iCurveIndex = iCurve;
	}
	
	public void Delete()
	{
		for(int iSection = 0; iSection < m_iNumSections; ++iSection)
		{
			if(Application.isEditor)
				Object.DestroyImmediate(m_goGate[iSection]);
			else
				Object.Destroy(m_goGate[iSection]);
		}
	}
	
	public bool CheckCollison(float fPlayerRotation)
	{
		//Convert to gate rotation range math.pi -> -math.pi
		fPlayerRotation *= -1;
		
		//Check for a collison
		float fSectionRotation = Mathf.PI*2 / 8;
		float fMaxRotation = DEFAULTROTATION - m_fRotation;
		float fMinRotation = fMaxRotation - m_iNumSections * fSectionRotation;
		
		//Account for the size of the path
		if(fMinRotation < fMaxRotation)
		{
			fMinRotation -= m_fPathSizeR/2;
			fMaxRotation += m_fPathSizeR/2;
		}
		else
		{
			fMaxRotation -= m_fPathSizeR/2;
			fMinRotation += m_fPathSizeR/2;
		}
		
		//Get mirrored values incase beacuse the path flips -1 when it pases math.pi
		float fMaxRotation2PI = fMaxRotation;
		if(fMaxRotation < 0)
			fMaxRotation2PI += Mathf.PI*2;
		else if(fMaxRotation > 0)
			fMaxRotation2PI -= Mathf.PI*2;
		
		float fMinRotation2PI = fMinRotation;
		if(fMinRotation < 0)
			fMinRotation2PI += Mathf.PI*2;
		else if(fMinRotation > 0)
			fMinRotation2PI -= Mathf.PI*2;
		
		//Make sure the values are in correct range due to over rotation
		if(fMaxRotation > Mathf.PI * 2 || fMinRotation > Mathf.PI * 2)
		{
			while(fMaxRotation > Mathf.PI * 2 || fMinRotation > Mathf.PI * 2)
			{
				fMaxRotation -= Mathf.PI * 2;
				fMinRotation -= Mathf.PI * 2;
				fMaxRotation2PI -= Mathf.PI * 2;
				fMinRotation2PI -= Mathf.PI * 2;
			}
		}
		else if(fMaxRotation < -(Mathf.PI * 2) || fMinRotation < -(Mathf.PI * 2))
		{
			while(fMaxRotation < -(Mathf.PI * 2) || fMinRotation < -(Mathf.PI * 2))
			{
				fMaxRotation += Mathf.PI * 2;
				fMinRotation += Mathf.PI * 2;
				fMaxRotation2PI += Mathf.PI * 2;
				fMinRotation2PI += Mathf.PI * 2;
			}
		}
		
		if((fPlayerRotation >= fMinRotation &&  fPlayerRotation <= fMaxRotation) ||
			(fPlayerRotation >= fMinRotation2PI &&  fPlayerRotation <= fMaxRotation2PI))
		{
			return true;
		}
		
		return false;
		
	}
	
	private void CalculatePathSizeRadians()
	{
		float fSqrRadius = DEFAULTPATHRADIUS*DEFAULTPATHRADIUS;
		float fSqrWidth = DEFUALTPATHWIDH*DEFUALTPATHWIDH;
		float fCos = (fSqrRadius + fSqrRadius - fSqrWidth); //b2 + c2 - a2
		fCos = fCos / (2*fSqrRadius); // 2*b*c
		m_fPathSizeR = Mathf.Acos(fCos); //Path size in radians
	}
	
	public void GetRotation(ref float fMin, ref float fMax)
	{
		//Check for a collison
		float fSectionRotation = Mathf.PI*2 / 8;
		fMax = DEFAULTROTATION - m_fRotation;
		fMin = fMax - m_iNumSections * fSectionRotation;
	}
	
}
