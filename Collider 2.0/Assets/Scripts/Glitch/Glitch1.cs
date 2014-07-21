using UnityEngine;
using System.Collections;

public class Glitch1 : MonoBehaviour {

	string[] sGlitchList = new string[] 
	{
		"Scan1", "Scan2", "Scan3", "Scan4", 
		"ScanStatic1", "ScanStatic2", "ScanStatic3", "ScanStatic4", "ScanStatic5", "ScanStatic6", "ScanStatic7",
	};
	FSprite[] tSprite;

	// Use this for initialization
	void Start () {
		InitFutile();

		InitScanLines();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void InitFutile() {
		FutileParams fParams = new FutileParams(true, true, false, false);
		int iWidth = 960;
		int iHeight = 640;
		fParams.AddResolutionLevel(Mathf.Max(iWidth,iHeight), 1, 1, "");
		fParams.origin = new Vector2(0.5f, 0.5f);
		
		Futile.instance.Init(fParams);

		string sPath = "Atlases" + "/" + "Glitch" + "/" + "Glitch";
		Futile.atlasManager.LoadAtlas(sPath);
	}

	void InitScanLines()
	{
		string sSprite = sGlitchList[Random.Range(0, sGlitchList.Length)];
		int iTexW = 128;
		float fScreenW = Futile.screen.width;

		tSprite = new FSprite[1];
		tSprite[0] = new FSprite(sSprite);
		tSprite[0].SetDimensions(fScreenW, (float)iTexW * Random.Range(0.5f, 2.0f));
		tSprite[0].SetPosition(0,0);

		Futile.stage.AddChild(tSprite[0]);
	}

}
