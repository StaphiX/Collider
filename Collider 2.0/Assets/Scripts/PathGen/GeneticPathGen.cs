using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneticPathGen : MonoBehaviour {

	public List<GeneticBezier> tBezier = new List<GeneticBezier>();
	public LineRenderer m_tLine = null;
	int iMaxGeneration = 30;
	int TESTPOINTS = 200;

	//Camera
	float fCameraTime = 0.0f;
	float fMovementTime = 0.0f;
	float fMovementSpeed = 20.0f;
	Vector3 tCameraPos = Vector3.zero;
	Vector3 tCameraLookAt = new Vector3(0.0f, 0.0f, 0.1f);

	// Use this for initialization
	void Start () {

		SpawnAllBeziers();
		camera.transform.position = tCameraPos;
		camera.transform.LookAt(tCameraLookAt);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(InputManager.IsKeyDown(KeyCode.LeftArrow))
		{
			fMovementSpeed = 50.0f;
		}
		else
		{
			fMovementSpeed = 0.0f;
		}

		if(InputManager.IsKeyDown(KeyCode.Y))
		{
			int iCurve = Mathf.FloorToInt(fCameraTime);
			tBezier[iCurve].iFitness += 50;
		}

		SetCameraPoint();
		if(fCameraTime >= iMaxGeneration-1)
		{
			NewGeneration();
			fCameraTime = 0.0f;
		}
	}

	void SpawnAllBeziers()
	{
		if(tBezier.Count < 1)
		{
			tBezier.Add(new GeneticBezier(Vector3.zero, new Vector3(0, 0,50)));
		}
		Bezier tLastCurve = tBezier[0].tBezier;
		if(m_tLine != null)
			m_tLine.SetVertexCount(iMaxGeneration * TESTPOINTS + 1);
		for(int iCurve = 0; iCurve < iMaxGeneration; ++iCurve)
		{
			if(iCurve > 0)
			{
				Vector3 p0 = tLastCurve.p3;
				Vector3 p1 = -tLastCurve.p2;
				tBezier.Add(new GeneticBezier(p0, p1));
				tLastCurve = tBezier[iCurve].tBezier;
			}

			if(m_tLine != null)
			{
				int iNumPointsToShow = TESTPOINTS;
				if(iCurve == iMaxGeneration-1)
					iNumPointsToShow++;
				for(int iPoint = 0; iPoint < iNumPointsToShow; ++iPoint)
				{
					float fTimeToGet = 1.0f / (float)TESTPOINTS * iPoint;
					Vector3 vPoint = tLastCurve.GetPointAtTime(fTimeToGet);
					int iPointIndex = iCurve * TESTPOINTS + iPoint;
					m_tLine.SetPosition(iPointIndex, vPoint);
				}
			}
		}
	}

	void SetCameraPoint()
	{
		int iCurveIndex = Mathf.FloorToInt(fCameraTime);
		float fTime = fCameraTime - iCurveIndex;

		Vector3 tPos = tBezier[iCurveIndex].tBezier.GetPointAtTime(fTime);

		float fLookAtTime = fCameraTime + 0.1f;
		iCurveIndex = Mathf.FloorToInt(fLookAtTime);
		fTime = fLookAtTime - iCurveIndex;

		Vector3 tLookAt = tBezier[iCurveIndex].tBezier.GetPointAtTime(fTime);

		camera.transform.position = Vector3.Lerp(tCameraPos, tPos, fMovementTime);
		camera.transform.LookAt(Vector3.Lerp(tCameraLookAt, tLookAt, fMovementTime));

		fMovementTime += fMovementSpeed * Time.deltaTime;

		if(fMovementTime >= 1.0f)
		{
			tCameraPos = tPos;
			tCameraLookAt = tLookAt;
			fMovementTime = fMovementTime - 1.0f;
			fCameraTime += 0.01f;
		}

		camera.transform.position += camera.transform.TransformDirection(Vector3.up) * 3;
	}

	public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
	{
		bool equal = true;
		
		if (Mathf.Abs (v1.x - v2.x) > precision) equal = false;
		if (Mathf.Abs (v1.y - v2.y) > precision) equal = false;
		if (Mathf.Abs (v1.z - v2.z) > precision) equal = false;
		
		return equal;
	}

	void NewGeneration()
	{
		List<GeneticBezier> tNewPop = GenSelection();

		tBezier.Clear();
		tBezier = new List<GeneticBezier>(tNewPop);
	}

	List<GeneticBezier> GenSelection()
	{
		int iTotalFitness = 0;
		foreach(GeneticBezier tItem in tBezier)
		{
			iTotalFitness += tItem.iFitness;
		}

		List<GeneticBezier> tNewPop = new List<GeneticBezier>();

		int iCount = tBezier.Count;
		tNewPop.Add(tBezier[iCount-1].Clone(false));

		GeneticBezier tParentA = null;
		GeneticBezier tParentB = null;
		for(int iCurve = 1; iCurve < iCount; ++iCurve)
		{
			int iFitnessCount = 0;
			int iRndFit = Random.Range(0, iTotalFitness);

			for(int iSelection = 0; iSelection < iCount; ++iSelection)
			{
				GeneticBezier tItem = tBezier[iSelection];
				iFitnessCount += tItem.iFitness;
				if(iRndFit < iFitnessCount || iSelection == iCount-1)
				{
					if(iCurve % 2 == 0)
					{
						tParentA = tItem;
					}
					else
					{
						tParentB = tItem;
						tNewPop.Add(CrossoverAndMutation(tParentA, tParentB));
						tNewPop.Add(CrossoverAndMutation(tParentB, tParentA));
					}
				}
			}
		}

		return tNewPop;


	}

	GeneticBezier CrossoverAndMutation(GeneticBezier tParentA, GeneticBezier tParentB)
	{
		if(tParentA == null)
			return tParentB;
		if(tParentB == null)
			return tParentA;

		GeneticBezier tNewBezier = tParentA.Clone(true);

		//Crossover
		if(Random.Range(0.0f, 1.0f) > 0.7f)
		{
			tNewBezier.p3 = tParentB.p3;
		}
		if(Random.Range(0.0f, 1.0f) > 0.7f)
		{
			tNewBezier.p2 = tParentB.p2;
		}

		//Mutation
		if(Random.Range(0.0f, 1.0f) > 0.95f)
		{
			tNewBezier.p3 += Random.insideUnitSphere * 40;
		}
		if(Random.Range(0.0f, 1.0f) > 0.95f)
		{
			tNewBezier.p2 += Random.insideUnitSphere * 40;
		}
		if(Random.Range(0.0f, 1.0f) > 0.95f)
		{
			tNewBezier.fMinDistance += Random.Range(-100.0f, 100.0f);
			tNewBezier.p3 = tNewBezier.tBezier.p0 + Random.insideUnitSphere * tNewBezier.fMinDistance;
			tNewBezier.p2 = Random.insideUnitSphere * 40;
		}

		return tNewBezier;
	}

	void OnGUI()
	{
		GUI.Label(new Rect(5, 5, 100, 20), fCameraTime.ToString());
	}
}


public class GeneticBezier
{
	public float fMinDistance = 500.0f;
	public Bezier tBezier;
	public Vector3 p2 = -Vector3.one;
	public Vector3 p3 = -Vector3.one;
	public int iFitness = 1;

	public GeneticBezier(Vector3 p0, Vector3 p1, Vector3 _p2, Vector3 _p3,int _iFitness)
	{
		p2 = _p2;
		p3 = _p3;
		Init(p0, p1);
		iFitness = _iFitness;
	}

	public GeneticBezier(Vector3 p0, Vector3 p1, int _iFitness)
	{
		Init (p0, p1);
		iFitness = _iFitness;
	}

	public GeneticBezier(Vector3 p0, Vector3 p1)
	{
		Init (p0, p1);
		iFitness = 1;
	}

	public GeneticBezier Clone(bool bResetFitness)
	{
		if(bResetFitness == false)
			return new GeneticBezier(tBezier.p0, tBezier.p1, p2, p3, iFitness);
		else
			return new GeneticBezier(tBezier.p0, tBezier.p1, p2, p3,  1);
	}

	public void Init(Vector3 p0, Vector3 p1)
	{
		if(p2 == -Vector3.one)
			CreateP2();
		if(p3 == -Vector3.one)
			CreateP3(p0);
		tBezier = new Bezier(p0, p1, p2, p3);
	}

	public void CreateP3(Vector3 p0)
	{
		float fY = Random.Range(0, fMinDistance-2);
		float fX = Random.Range(0, fMinDistance-1- fY);
		float fZ = fMinDistance - fY - fX;

		p3 = p0 + new Vector3(fX, fY, fZ);
	}

	public void CreateP2()
	{
		p2 = Random.insideUnitSphere * fMinDistance;
	}
}