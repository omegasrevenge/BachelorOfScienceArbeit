using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour 
{
	public enum ServerChoice { Europe, USA, Asia }

	public static Settings Singleton;

	[HideInInspector]
	public ServerChoice SelectedServer;

	void Awake()
	{
		Singleton = this;
		SelectedServer = (ServerChoice)PlayerPrefs.GetInt ("SelectedServer");
	}

	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt ("SelectedServer", (int)SelectedServer);
	}
}
