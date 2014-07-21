using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierTrack {

	List<TrackCurve> m_tCurve = new List<TrackCurve>(); //List of curves that make up this track
	List<AttachmentPoint> m_tPoints = new List<AttachmentPoint>(); //The places on the track that objects can attach to
	List<MovementPoint> m_tMovementTimes = new List<MovementPoint>();

	public int GetCurveCount()
	{
		if(m_tCurve != null && m_tCurve.Count > 0)
			return m_tCurve.Count;

		return 0;
	}

	public void GeneratePoints()
	{
		int iCurveCount = m_tCurve.Count;
		float fCurrentTime = 0.0f;
		float fSpacingOverflow = 0.0f; //Adjusts spacing between curves
		for(int iCurve = 0; iCurve < iCurveCount; ++iCurve)
		{
			TrackCurve tCurve = m_tCurve[iCurve];
			while(fCurrentTime >= 0 && fCurrentTime <= 1.0f)
			{
				fCurrentTime = tCurve.tBezier.GetEstTimeFromDistance(tCurve.fSpacing - fSpacingOverflow, fCurrentTime);
				if(fSpacingOverflow > 0.0f) //Use the overflow value only once
					fSpacingOverflow = 0.0f;

				if(fCurrentTime < 1.0f)
				{
					MovementPoint tMovePoint = new MovementPoint();
					tMovePoint.iCurveIndex = iCurve;
					tMovePoint.fMoveTime = fCurrentTime;
					m_tMovementTimes.Add(tMovePoint);

//					AttachmentPoint tPoint = new AttachmentPoint();
//					tPoint.iCurveIndex = iCurve;
//					tPoint.fTimePoint = fCurrentTime;
//					tPoint.vVectorPoint = tCurve.tBezier.GetPointAtTime(fCurrentTime);
//					m_tPoints.Add(tPoint);
				}
			}
			if(fCurrentTime >= 1.0f)
			{
				float fLastTime = m_tMovementTimes[m_tMovementTimes.Count-1].fMoveTime;

				Vector3 vStartPos = tCurve.tBezier.GetPointAtTime(fLastTime);
				Vector3 vEndPos = tCurve.tBezier.GetPointAtTime(1.0f);
				float fDistance = (vEndPos - vStartPos).magnitude;
				fSpacingOverflow = tCurve.fSpacing - fDistance;
			}
		}
	}
}

public class MovementPoint
{
	public int iCurveIndex;
	public float fMoveTime;
}

public class TrackCurve
{
	public TrackCurve(Bezier _tBezier, float _fSpacing, float _fAttachmentSpacing)
	{
		tBezier = _tBezier;
		fSpacing = _fSpacing;
		fAttachmentSpacing = _fAttachmentSpacing;
	}
	public Bezier tBezier;
	public float fSpacing;
	public float fAttachmentSpacing;
}
