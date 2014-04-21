using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	
	TrackManager m_tTrackManager;
	PathManager m_tPathManager;
	PlayerManager m_tPlayerManager;
	
	bool m_bRed;
	
	// Use this for initialization
	void Start () {
		m_tTrackManager = new TrackManager();
		m_tTrackManager.Init();
		
		m_tPathManager = new PathManager();
		m_tPathManager.Init();
		
		m_tPlayerManager = new PlayerManager();
		m_tPlayerManager.Init(1);

		//camera.transform.position = new Vector3(vBezPositions[iNumPositions/2].x, vBezPositions[iNumPositions/2].y+20, vBezPositions[iNumPositions/2].z);
		//camera.transform.LookAt(vBezPositions[iNumPositions/2]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate()
	{
		m_bRed = CheckTrack();
		Vector3 vTrackPos = m_tTrackManager.GetNextTrackPoint();
		Vector3 vCameraPos = m_tTrackManager.GetCameraPoint();

		camera.transform.position = vCameraPos;
		
		camera.transform.LookAt(vTrackPos);
		
		m_tPathManager.Update(vTrackPos);
	}
	
	bool CheckTrack()
	{
		float fPlayerTime = m_tTrackManager.GetCurrentTime();
		float fPlayerRotation = m_tPathManager.GetPathRotation();
		
		bool bPassedGate = false;
		if(m_tTrackManager.CheckPassedGate(fPlayerTime))
			bPassedGate = true;
		
		bool bCollision = m_tTrackManager.CheckGateCollison(fPlayerTime, fPlayerRotation);
		if(bCollision == false && bPassedGate == true)
			m_tPlayerManager.GetPlayer(0).GateMissed();
		else if(bCollision == true && bPassedGate == true)
			m_tPlayerManager.GetPlayer(0).GateHit();
		
		if(bPassedGate)
			m_tTrackManager.IncGate();
		
		bool bRemainingGates = false;
		bRemainingGates = m_tTrackManager.GetCurrentGateIsValid();
		
		bool bDeleted = m_tTrackManager.CheckForDelete();
		
		if(bDeleted && bRemainingGates) //There are gates that havent been checked
		{
			if(bCollision == false)
				m_tPlayerManager.GetPlayer(0).GateMissed();
			else if(bCollision == true)
				m_tPlayerManager.GetPlayer(0).GateHit();
		}
		
		return bCollision;
	}
	
	void OnGUI()
	{
		if(m_tPlayerManager != null)
		{
			if(m_tPlayerManager.GetPlayer(0) != null)
			{
				GUI.Label(new Rect(10,10, 100, 100), m_tPlayerManager.GetPlayer(0).GetNumGates().ToString());
			}
		}
		
		//DEBUG CODE TO HIGHLIGHT COLLISIONS
//		if(m_tPathManager != null)
//		{
//			if(m_bRed)
//			{
//				m_tPathManager.SetMaterialCol(Color.red);
//			}
//			else
//				m_tPathManager.SetMaterialCol(Color.white);
//		}
	}
}
