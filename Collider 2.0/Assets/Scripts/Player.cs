using UnityEngine;
using System.Collections;

public class Player
{
	int m_iNumGates = 0;
	
	public int GetNumGates()
	{
		return m_iNumGates;
	}
	
	public void GateMissed()
	{
		m_iNumGates += 1;
	}
	
	public void GateHit()
	{
		m_iNumGates = 0;
	}
}
