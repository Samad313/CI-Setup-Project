using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;


public class DeletePlayerPrefs : ScriptableObject
{
	[ MenuItem( "PlayerPrefs/Delete Player Prefs" ) ]
	static void DeletePlayerPrefsFunction()
	{
		PlayerPrefs.DeleteAll();
		
		//DEBUG:
		Debug.Log( "DEBUG: <color=yellow>Player Prefs Deleted.</color>" );
	}

	//[ MenuItem( "CarJacker/Clear Game State" ) ]
	//static void ClearGameState()
	//{
	//	File.Delete( Application.persistentDataPath + "/g.s" );
		
	//	//DEBUG:
	//	Debug.Log( "DEBUG: <color=yellow>Game State Cleared.</color>" );
	//}
}
