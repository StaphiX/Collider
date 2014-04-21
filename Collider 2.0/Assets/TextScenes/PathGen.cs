using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathGen : MonoBehaviour {
	
	float fRotation = 1.0f;
	float fRadius = 5.0f;
	
	public GameObject goPathSection;
	GameObject goPathObject = null;
	Vector3[] vVertexOffsets;
	Vector2 vScroller;
	
	Vector3 vBasePos = Vector3.zero;
	
	// Use this for initialization
	void Start () {

		Mesh tMesh = goPathSection.GetComponent<MeshFilter>().sharedMesh;
		
		vVertexOffsets = new Vector3[tMesh.vertexCount];
		Vector3 vBasePos = goPathSection.transform.position;
		for(int iVert = 0; iVert < vVertexOffsets.Length; ++iVert)
		{
			vVertexOffsets[iVert] = tMesh.vertices[iVert] - vBasePos;
		}
		
		AddPathSection();
		AddPathSection();
		AddPathSection();
		AddPathSection();
		AddPathSection();
		AddPathSection();
		AddPathSection();
		AddPathSection();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	Vector3 CalcPosition()
	{
		Vector3 vPos = vBasePos;
		vPos.x = vBasePos.x + fRadius * Mathf.Cos(fRotation - Mathf.PI/2);
		vPos.y = vBasePos.y + fRadius * Mathf.Sin(fRotation - Mathf.PI/2);
		
		vBasePos.z += 1.0f;
		
		return vPos;
	}
	
	Quaternion CalcRotation()
	{
		Quaternion qRot = Quaternion.AngleAxis(fRotation * Mathf.Rad2Deg, Vector3.forward);
		fRotation += 0.05f;
		
		return qRot;
	}
	
	void OnGUI()
	{
//		vScroller = GUI.BeginScrollView(new Rect(0,0, Screen.width-2, Screen.height-2), vScroller, new Rect(0,0,Screen.width, Screen.height * 5));
//		
//		for(int iVert = 0; iVert < vVertexOffsets.Length; ++iVert)
//		{
//			if(vVertexOffsets[iVert].z > 0.1f)
//			{
//				GUI.Label(new Rect(200, 5 + (15 * iVert), 100, 20), "FRONT");
//			}
//			else if(vVertexOffsets[iVert].z < -0.1f)
//			{
//				GUI.Label(new Rect(180, 5 + (15 * iVert), 100, 20), "BACK");
//			}
//			GUI.Label(new Rect(5, 5 + (15 * iVert), 100, 20), iVert.ToString() + ": " + vVertexOffsets[iVert].ToString());
//		}
//		
//		GUI.EndScrollView();
	}
	
	void AddPathSection()
	{
		GameObject goNewObject = Instantiate(goPathSection, CalcPosition(), CalcRotation()) as GameObject;

		if(goPathObject != null)
		{
			Mesh pathMesh = goPathObject.GetComponent<MeshFilter>().mesh;
			Vector3[] vPathVerts = pathMesh.vertices;
			Vector2[] vPathUVs = pathMesh.uv;
			Vector3[] vPathNorms = pathMesh.normals;
			int[] vPathTris = pathMesh.triangles;
			
			Mesh thisMesh = goNewObject.GetComponent<MeshFilter>().mesh;
			Vector3[] vThisVerts = thisMesh.vertices;
			Vector2[] vThisUVs = thisMesh.uv;
			Vector3[] vThisNorms = thisMesh.normals;
			int[] vThisTris = thisMesh.triangles;
			
			Transform thisTrans = goNewObject.transform;
			Transform pathTrans = goPathObject.transform;
			
			List<Vector3> newPathVerts = new List<Vector3>(vPathVerts);
			List<Vector2> newPathUVs = new List<Vector2>(vPathUVs);
			List<Vector3> newPathNorms = new List<Vector3>(vPathNorms);
			List<int> newPathTris = new List<int>(vPathTris);
			
			for(int iVert = 0; iVert < vVertexOffsets.Length; ++iVert)
			{
				if(vVertexOffsets[iVert].z > 0.001f) //FRONT
				{
					Vector3 thisPoint = thisTrans.TransformPoint(vVertexOffsets[iVert]);
					thisPoint = pathTrans.InverseTransformPoint(thisPoint);
					
					newPathVerts.Add(thisPoint);
					newPathUVs.Add(vThisUVs[iVert]);
					newPathNorms.Add(vThisNorms[iVert]);
				}
			}
			Vector3[] vNewVertArray = newPathVerts.ToArray();
			for(int iTri = 0; iTri < vThisTris.Length; ++iTri)
			{
				Vector3 thisPoint = thisTrans.TransformPoint(vThisVerts[vThisTris[iTri]]);
				thisPoint = pathTrans.InverseTransformPoint(thisPoint);
				Vector3 vPointToFind = thisPoint;
			 	int iNewTri = FindClosestVert(vPointToFind, vNewVertArray);
				newPathTris.Add(iNewTri);
			}
			pathMesh.vertices = vNewVertArray;
			pathMesh.uv = newPathUVs.ToArray();
			pathMesh.normals = newPathNorms.ToArray();
			pathMesh.triangles = newPathTris.ToArray();
			pathMesh.RecalculateBounds();
			pathMesh.RecalculateNormals();
			
			if(Application.isEditor)
			{
				DestroyImmediate(goNewObject);
			}
			else
			{
				Destroy(goNewObject);
			}
		}	
		else
		{
			goPathObject = goNewObject;
		}
		

	}	
	
	int FindClosestVert(Vector3 vCurrentVert, Vector3[] vNewVerts)
	{
		float fX = vCurrentVert.x;
		float fY = vCurrentVert.y;
		float fZ = vCurrentVert.z;
		
		int iClosest = 0;
		float sqrDist = -1;
		for(int iVert = 0; iVert < vNewVerts.Length; ++iVert)
		{
			float newDist = Mathf.Abs((vCurrentVert - vNewVerts[iVert]).sqrMagnitude);
			
			if(sqrDist < 0 || newDist < sqrDist)
			{
				sqrDist = newDist;
				iClosest = iVert;
				if(sqrDist < 0.0001f)
				{
					return iClosest;
				}
			}
		}
						
		return iClosest;
		
	}
}
