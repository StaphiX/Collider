using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This is a class that holds all the data for a segment
//This is saved, loaded and used at runtime

public class GameSegment 
{
	BezierTrack m_tBezierTrack; //This is the main path through the track for the camera to follow and objects to attach to.
	List<Checkpoint> m_tCheckpoint; //This is the parts to avoid or hit on the track

	
}
