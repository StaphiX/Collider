using UnityEngine;
using System.Collections;

public class InputManager {
	
	//Number of possible touches
	public const int MAXTOUCHES = 4;
	
	private static bool GetIsTouchDevice()
	{
		switch(Application.platform)
		{
		case RuntimePlatform.IPhonePlayer:
			return true;
		case RuntimePlatform.Android:	
			return true;
		case RuntimePlatform.OSXEditor:
			return false;
		case RuntimePlatform.OSXPlayer:
			return false;
		case RuntimePlatform.WindowsPlayer:	
			return false;
		case RuntimePlatform.OSXWebPlayer:	
			return false;
		case RuntimePlatform.OSXDashboardPlayer:	
			return false;
		case RuntimePlatform.WindowsWebPlayer:	
			return false;
		case RuntimePlatform.WiiPlayer:	
			return false;
		case RuntimePlatform.WindowsEditor:	
			return false;
		case RuntimePlatform.XBOX360:	
			return false;
		case RuntimePlatform.PS3:	
			return false;
		case RuntimePlatform.NaCl:	
			return false;
		case RuntimePlatform.FlashPlayer:
			return false;
		default:
			return false;
		}
	}
	
	public static bool GetTouchPoint(out Vector3 vTouchPoint)
	{
		return GetTouchPoint(0,out vTouchPoint);
	}
	
	public static bool GetFirstTouchInRect(Rect tTouchRect, out Vector3 vTouchPoint, int iPadding = 0)
	{
		for(int iTouch  = 0; iTouch < MAXTOUCHES; ++iTouch)
		{
			if(GetTouchPoint(iTouch, out vTouchPoint))
			{
				if(vTouchPoint.x > tTouchRect.x - iPadding &&
					vTouchPoint.y > tTouchRect.y - iPadding &&
					vTouchPoint.x < tTouchRect.x + tTouchRect.width + iPadding &&
					vTouchPoint.y < tTouchRect.y + tTouchRect.height + iPadding)
				{
					return true;
				}
			}
		}
		vTouchPoint = Vector3.zero;
		return false;
	}
	
	public static bool GetAllTouchPoints(out Vector3[] vTouchPoint)
	{
		int iNumberTouches = 0;
		int iMaxTouches = MAXTOUCHES;
		if(!GetIsTouchDevice())
		{
			iMaxTouches = 1;
		}
		Vector3[] vAllTouchPoints = new Vector3[iMaxTouches];
		for(int iTouch = 0; iTouch < iMaxTouches; ++iTouch)
		{
			if(GetTouchPoint(iTouch, out vAllTouchPoints[iTouch]))
				++iNumberTouches;
		}
		vTouchPoint = new Vector3[iNumberTouches];
		for(int iTouch = 0; iTouch < iNumberTouches; ++iTouch)
		{
			vTouchPoint[iTouch] = vAllTouchPoints[iTouch];
		}
		if(iNumberTouches > 0)
			return true;
		else
			return false;
		
	}
	
	public static bool GetTouchPoint(int iTouchIndex,out Vector3 vTouchPoint)
	{
		if(GetIsTouchDevice())
		{
			//Checks if there is any touch began, stationary or moving
			if(Input.touchCount > iTouchIndex)
			{
				if(Input.GetTouch(iTouchIndex).phase != TouchPhase.Ended || 
					Input.GetTouch(iTouchIndex).phase != TouchPhase.Canceled)
				{
					vTouchPoint = Input.touches[iTouchIndex].position;
				}
				else
				{
					vTouchPoint = Vector3.zero;
					//Return false if there is no touch
					return false;
				}
			}
			else
			{
				vTouchPoint = Vector3.zero;
				//Return false if there is no touch
				return false;
			}
		}
		else
		{
			if(Input.GetMouseButton(0))
			{
				vTouchPoint = Input.mousePosition;
			}
			else
			{
				vTouchPoint = Vector3.zero;;
				//Return false if there is no touch
				return false;
			}
		}
		return true;
	}
	
	public static bool GetTouched()
	{
		if(GetIsTouchDevice())
		{
			//Checks if there is any touch began, stationary or moving
			if(Input.touchCount > 0)
			{
				if(Input.GetTouch( 0 ).phase != TouchPhase.Ended || 
					Input.GetTouch( 0 ).phase != TouchPhase.Canceled)
				{
					return true;
				}
				else return false;
			}
			else
			{
				return false;
			}
		}
		else
		{
			if(Input.GetMouseButton(0))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	
	public static int GetTouchPressedDown()
	{
		if(GetIsTouchDevice())
		{
			for(int iTouch = 0; iTouch < MAXTOUCHES; ++iTouch)
			{
				//Checks if there is any touch began, stationary or moving
				if(Input.touchCount > iTouch)
				{
					if(Input.GetTouch(iTouch).phase != TouchPhase.Ended || 
						Input.GetTouch(iTouch).phase != TouchPhase.Canceled)
					{
						return iTouch;
					}
				}
			}
			return -1;
		}
		else
		{
			if(Input.GetMouseButtonDown(0))
			{
				return 0;
			}
			else
			{
				return 0;
			}
		}
	}
	
	public static int GetTouchReleased()
	{
		if(GetIsTouchDevice())
		{
			for(int iTouch = 0; iTouch < MAXTOUCHES; ++iTouch)
			{
				//Checks if there is any touch ended, or canceled
				if(Input.touchCount > iTouch) 
				{
					if(Input.GetTouch(iTouch).phase == TouchPhase.Ended || 
						Input.GetTouch(iTouch).phase == TouchPhase.Canceled)
					{
						return iTouch;
					}
				}
			}
			return -1;
		}
		else
		{
			if(Input.GetMouseButtonUp(0))
			{
				return 0;
			}
			else
			{
				return -1;
			}
		}
	}
}
