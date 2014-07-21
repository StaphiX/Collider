using UnityEngine;
using System.Collections;

public class Checkpoint 
{
	AttachmentPoint m_tPoint;
}

public class AttachmentPoint
{	
	public int iCurveIndex; //The curve index in the track to attach to
	public float fTimePoint; //The timeoffset on this curve
	public Vector3 vVectorPoint; //The vector offset from this curve
}
