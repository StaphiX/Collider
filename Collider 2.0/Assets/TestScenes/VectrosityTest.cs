using UnityEngine;
using System.Collections;
using Vectrosity;

public class VectrosityTest : MonoBehaviour {

	public Material tMat = null;

	Vector3[] vGate = {new Vector3(0f, 16.618f, -1.176f), new Vector3(-4.301f, 16.052f, -1.176f), new Vector3(0f, 16.618f, -1.176f), new Vector3(4.301f, 16.052f, -1.176f), new Vector3(4.301f, 16.052f, -1.176f), new Vector3(8.309f, 14.392f, -1.176f), new Vector3(8.309f, 14.392f, -1.176f), new Vector3(11.751f, 11.751f, -1.176f), new Vector3(8.309f, 14.392f, 1.176f), new Vector3(11.751f, 11.751f, 1.176f), new Vector3(8.309f, 14.392f, 1.176f), new Vector3(4.301f, 16.052f, 1.176f), new Vector3(0f, 16.618f, 1.176f), new Vector3(4.301f, 16.052f, 1.176f), new Vector3(-4.301f, 16.052f, 1.176f), new Vector3(0f, 16.618f, 1.176f)};
	VectorLine vLine;

	// Use this for initialization
	void Start () {

		vLine = new VectorLine("GATE", vGate, tMat, 250.0f, LineType.Discrete, Joins.Weld);
		vLine.Draw3D();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
