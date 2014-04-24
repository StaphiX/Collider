using UnityEngine;
using System.Collections;

public class Glitch1 : MonoBehaviour {

	FSprite[] tSprite = new FSprite[100];

	// Use this for initialization
	void Start () {
	
		InitFutile();

		for(int iSprite = 0; iSprite < 100; ++iSprite)
		{
			tSprite[iSprite] = new FSprite("blank");
			tSprite[iSprite].SetDimensions(Random.value * Screen.width - Screen.width/2, 
			                               Random.value * Screen.height - Screen.height/2,
			                               Random.value * 300,
			                               Random.value * 300);
			float fRed = Random.Range(0,2);
			float fBlue = Random.Range(0,2);
			float fGreen = Random.Range(0,2);

			Color tColor = new Color(fRed, fBlue, fGreen, 1.0f);
			tSprite[iSprite].color = tColor;
			Futile.stage.AddChild(tSprite[iSprite]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		int iMod = Random.Range(1, 50);
		for(int iSprite = 0; iSprite < 100; ++iSprite)
		{
			if(iSprite % iMod == 0)
			{
				int iEffect = Random.Range(0, 3);
				if(iEffect == 0)
				{
					tSprite[iSprite].SetDimensions(Random.value * 300,
					                               Random.value * 300);
				}
				else if(iEffect == 1)
				{
					float fRed = Random.Range(0,2);
					float fBlue = Random.Range(0,2);
					float fGreen = Random.Range(0,2);
					Color tColor = new Color(fRed, fBlue, fGreen, 1.0f);
					tSprite[iSprite].color = tColor;
				}
				else if(iEffect == 2)
				{
					tSprite[iSprite].SetPosition(Random.value * Screen.width - Screen.width/2, 
					                             Random.value * Screen.height - Screen.height/2);
				}
			}
		}
	}

	void InitFutile() {
		FutileParams fParams = new FutileParams(true, true, false, false);
		int iWidth = 960;
		int iHeight = 640;
		fParams.AddResolutionLevel(Mathf.Max(iWidth,iHeight), 1, 1, "");
		fParams.origin = new Vector2(0.5f, 0.5f);
		
		Futile.instance.Init(fParams);

		string sPath = "Atlases" + "/" + "UI" + "/" + "UI";
		Futile.atlasManager.LoadAtlas(sPath);
	}


}
