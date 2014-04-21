using UnityEngine;
using System.Collections;

public class GateColour : MonoBehaviour
{
	const int iTexWidth = 128;
	const float fPixelSize = 1.0f / (float)iTexWidth;
	
	Mesh m_mGateMesh;
	Vector2[] m_vBaseUVs;
	
	public void Start()
	{
		if(m_mGateMesh == null)
		{
			m_mGateMesh = GetComponent<MeshFilter>().mesh;
			m_vBaseUVs = m_mGateMesh.uv;
		}
		
		//SetColour(42, 54);
	}

	public void SetColour(int iX, int iY)
	{
		if(m_mGateMesh == null)
		{
			m_mGateMesh = GetComponent<MeshFilter>().mesh;
			m_vBaseUVs = m_mGateMesh.uv;
		}
		
		iX = Mathf.Clamp(iX, 0, iTexWidth-1);
		iY = Mathf.Clamp(iY, 0, iTexWidth-1);
		iY = (iTexWidth-1) - iY;
		
		Vector2[] vNewUV = new Vector2[m_vBaseUVs.Length];
		for(int iVert = 0; iVert < m_vBaseUVs.Length; ++iVert)
		{
			float fX = m_vBaseUVs[iVert].x / (float)iTexWidth;
			float fY = m_vBaseUVs[iVert].y / (float)iTexWidth;
			
			fX += fPixelSize * iX;
			fY += fPixelSize * iY;
			
			vNewUV[iVert] = new Vector2(fX, fY);
		}
		
		m_mGateMesh.uv = vNewUV;
	}
	
}
