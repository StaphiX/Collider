using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class AlgoritmTest : MonoBehaviour {
	
	List<SearchPoint> searchPoints = new List<SearchPoint>();
	string sLength = "500";
	Bezier tRandomBezier;
	float fCameraTime = 0.0f;
	float fSpeed = 3.0f;
	
	// Use this for initialization
	void Start () {
		tRandomBezier = new Bezier(
			new Vector3(0,0,0),
			new Vector3(0,0,500/3),
			new Vector3(0,0,-500/3),
			new Vector3(0,0,500)
			);
		fCameraTime = GetEstTimeFromDistance(fSpeed, fCameraTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		
		int iY = 5;
		int iX = 5;
		int iW = 200;
		int iH = 20;
		sLength = GUI.TextField(new Rect(iX, iY, iW, iH), sLength);
		int iLength = 0;
		if(sLength.Length > 0)
			iLength = Convert.ToInt32(sLength);
		if(GUI.Button(new Rect(iX + iW, iY, iW, iH), "Refresh"))
		{
			RandomiseCurve((float)iLength);
		}

		
		for(int iRow = 0; iRow < searchPoints.Count; ++iRow)
		{
			for(int iCol = 0; iCol < 3; ++iCol)
			{
				int iBoxX = iX + iCol * iW;
				int iBoxY = iY + (iRow+1) * (iH);
				if(iCol == 0)
				{
					GUI.Box(new Rect(iBoxX, iBoxY, iW, iH), iRow.ToString());
				}
				if(iCol == 1)
				{
					GUI.Box(new Rect(iBoxX, iBoxY, iW, iH), searchPoints[iRow].fTime.ToString());
				}
				else if(iCol == 2)
				{
					GUI.Box(new Rect(iBoxX, iBoxY, iW, iH), searchPoints[iRow].fDistance.ToString());
				}
				
			}
		}
		
	}
	
	float GetEstTimeFromDistance(float fDistanceToGet, float fStartTime)
	{
		Vector3 vStartPoint = tRandomBezier.GetPointAtTime(fStartTime);
		float fTimeStep = 0.001f;
		float fTolerance = 0.001f;
		float fCurrentTime = fStartTime + fTimeStep;
		Vector3 vCurrentPoint = tRandomBezier.GetPointAtTime(fCurrentTime);
		float fCurrentDistance = (vCurrentPoint - vStartPoint).magnitude;
		
		searchPoints.Clear();
		searchPoints.Add(new SearchPoint(fCurrentTime, fCurrentDistance));
		fCurrentTime = fDistanceToGet / fCurrentDistance * (fCurrentTime - fStartTime) + fStartTime;
		vCurrentPoint = tRandomBezier.GetPointAtTime(fCurrentTime);
		fCurrentDistance = (vCurrentPoint - vStartPoint).magnitude;
		searchPoints.Add(new SearchPoint(fCurrentTime, fCurrentDistance));
		
		while((fCurrentDistance > fDistanceToGet + fTolerance || fCurrentDistance < fDistanceToGet - fTolerance) && searchPoints.Count < 20)
		{
			fCurrentTime = fDistanceToGet / fCurrentDistance * (fCurrentTime - fStartTime) + fStartTime;
			vCurrentPoint = tRandomBezier.GetPointAtTime(fCurrentTime);
			fCurrentDistance = (vCurrentPoint - vStartPoint).magnitude;
			searchPoints.Add(new SearchPoint(fCurrentTime, fCurrentDistance));
		}
		
		return fCurrentTime;
		
	}
	
	public void RandomiseCurve(float fLength)
	{
		//tRandomBezier = new Bezier(
		//	new Vector3(0,0,0),
		//	UnityEngine.Random.onUnitSphere * fLength - UnityEngine.Random.onUnitSphere * fLength/2,
		//	UnityEngine.Random.onUnitSphere * fLength - UnityEngine.Random.onUnitSphere * fLength/2,
		//	new Vector3(0,0,fLength)
		//	);
		fCameraTime = GetEstTimeFromDistance(fSpeed, fCameraTime);
	}
}



struct SearchPoint
{
	public SearchPoint(float _time, float _distance)
	{
		fTime = _time;
		fDistance = _distance;
	}
	
	public float fTime;
	public float fDistance;
}
