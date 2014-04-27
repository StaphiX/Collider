using UnityEngine;
using System.Collections;

public class Glitch1 : MonoBehaviour {

	const int iNumSprites = 35;
	FSprite[] tSprite = new FSprite[iNumSprites];

	// Use this for initialization
	void Start () {
	
		InitFutile();

		for(int iSprite = 0; iSprite < iNumSprites; ++iSprite)
		{
			tSprite[iSprite] = new FSprite("glitch");
			tSprite[iSprite].SetDimensions(Random.value * Futile.screen.width - Futile.screen.width/2, 
			                               Random.value * Futile.screen.height - Futile.screen.height/2,
			                               Random.value * 2000,
			                               Random.value * 100);

			float fRed = 1.0f;
			float fBlue = 1.0f;
			float fGreen = 1.0f;
			if(Random.Range(0.0f,100.0f) > 90.0f)
			{
				fRed = Random.Range(0,2);
				fBlue = Random.Range(0,2);
				fGreen = Random.Range(0,2);
			}

			float fCropX = 0.5f * Random.value;
			float fCropY = 0.5f * Random.value;
			float fCropW = fCropX + Random.value * 0.3f;
			float fCropH = fCropY + Random.value * 0.3f;
			tSprite[iSprite].cropRect = new Rect(fCropX, fCropY, fCropW, fCropH);
			
			Color tColor = new Color(fRed, fBlue, fGreen, 1.0f);
			tSprite[iSprite].color = tColor;
			Futile.stage.AddChild(tSprite[iSprite]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		int iMod = Random.Range(1, iNumSprites);
		for(int iSprite = 0; iSprite < iNumSprites; ++iSprite)
		{
			if(iSprite % iMod == 0)
			{
				int iEffect = Random.Range(0, 3);
				if(iEffect == 0)
				{
					tSprite[iSprite].SetDimensions(Random.value * 2000,
					                               Random.value * 100);
				}
				else if(iEffect == 1)
				{
					float fRed = 1.0f;
					float fBlue = 1.0f;
					float fGreen = 1.0f;
					if(Random.Range(0.0f,100.0f) > 90.0f)
					{
						fRed = Random.Range(0,2);
						fBlue = Random.Range(0,2);
						fGreen = Random.Range(0,2);
					}
					Color tColor = new Color(fRed, fBlue, fGreen, 1.0f);
					tSprite[iSprite].color = tColor;
				}
				else if(iEffect == 2)
				{
					tSprite[iSprite].SetPosition(Random.value * Futile.screen.width - Futile.screen.width/2, 
					                             Random.value * Futile.screen.height - Futile.screen.height/2);
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
