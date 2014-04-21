using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager{
	
	int m_iNumPlayers = 0;
	List<Player> m_tPlayers = new List<Player>(); 
	
	public void Init(int iNumPlayers)
	{
		m_iNumPlayers = iNumPlayers;
		for(int iPlayer = 0; iPlayer < iNumPlayers; ++iPlayer)
		{
			m_tPlayers.Add(new Player());
		}
	}
	
	public Player GetPlayer(int iIndex)
	{
		if(iIndex < m_iNumPlayers && iIndex >= 0)
			return m_tPlayers[iIndex];
		else return null;
	}
}
