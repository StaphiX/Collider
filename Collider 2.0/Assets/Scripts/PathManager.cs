using UnityEngine;
using System.Collections;

public class PathManager{
	
	PlayerPath m_tPath;
	
	// Use this for initialization
	public void Init () {
		m_tPath = new PlayerPath();
		m_tPath.Init();
	}
	
	public void Update(Vector3 vTrackPos)
	{
		m_tPath.Update(vTrackPos);
	}
	
	public float GetPathRotation()
	{
		return m_tPath.GetRotation();
	}
	
	public void SetMaterialCol( Color color)
	{
		m_tPath.SetMaterialCol(color);
	}
}
