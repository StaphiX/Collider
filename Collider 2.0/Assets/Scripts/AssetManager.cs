using UnityEngine;
using System.Collections;

public class AssetManager : MonoBehaviour {
	
	//Singleton Instance
	public static AssetManager inst;
	public static Transform tWorldTransform;
	
	public GameObject[] gameObjects;
	
	// Use this for initialization
	void Awake () {
		inst = GetComponent<AssetManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static GameObject GetObjBase(string sName)
	{
		for(int iGO = 0 ;iGO < inst.gameObjects.Length; ++iGO)
		{
			if(inst.gameObjects[iGO].name == sName)
				return inst.gameObjects[iGO];
		}
		
		return null;
	}
	
	public static GameObject GetObjInstance(string sName)
	{
		return GetObjInstance(sName, Vector3.zero, Quaternion.identity);
	}
	
	public static GameObject GetObjInstance(string sName, Vector3 vPos, Quaternion qRot)
	{
		for(int iGO = 0 ;iGO < inst.gameObjects.Length; ++iGO)
		{
			if(inst.gameObjects[iGO].name == sName)
				return GameObject.Instantiate(inst.gameObjects[iGO], vPos, qRot) as GameObject;
		}
		
		return null;
	}
}
