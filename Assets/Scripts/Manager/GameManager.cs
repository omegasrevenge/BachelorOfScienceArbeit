using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	public static GameManager Singleton;

	public GameObject RightSpawnPoint;
	public GameObject LeftSpawnPoint;
	public Transform MovementBeacon;
	public GameObject CharacterPrefab;

	[HideInInspector]
	public GameObject MyMainCharacter;

	void Awake()
	{
		Singleton = this;
		Application.runInBackground = true;
		Application.targetFrameRate = 60;
	}

	void Update () 
	{
		//if (!PhotonNetwork.connected && !PhotonNetwork.connecting) OnNetworkDisconnect();

		//if (MyMainCharacter == null && PhotonNetwork.connected && PhotonNetwork.inRoom) 
		//{
		//	TitleScreenMenu.Singleton.CurrentGUIState = TitleScreenMenu.GUIState.Game;
		//	TitleScreenMenu.Singleton.gameObject.SetActive(false);
		//	TitleScreenMenu.Singleton.gameObject.SetActive(false); TODO lobbymenu
		//	MyMainCharacter = PhotonNetwork.Instantiate(CharacterPrefab.name, RightSpawnPoint.transform.position, RightSpawnPoint.transform.rotation, 0);
		//	MyMainCharacter.GetComponent<CharControl>().MyCursorController = MovementBeacon.GetChild(0).GetComponent<CursorController>();
		//	CameraManager.Singleton.MyCharacter = MyMainCharacter.transform;
		//}
	}

	public void OnCloseGameClicked()
	{

		//switch (TitleScreenMenu.Singleton.CurrentGUIState) 
		//{
		//case TitleScreenMenu.GUIState.Main:
		//	Application.Quit ();
		//	break;
		//case TitleScreenMenu.GUIState.Lobby:
		//	TitleScreenMenu.Singleton.CurrentGUIState = TitleScreenMenu.GUIState.Main;
		//	TitleScreenMenu.Singleton.gameObject.SetActive (true);
		//	if(PhotonNetwork.inRoom)
		//		PhotonNetwork.LeaveRoom();
		//	break; // this break and the 4 lines above exist because there may come a time when i need it TODO
		//case TitleScreenMenu.GUIState.Game:
		//	TitleScreenMenu.Singleton.CurrentGUIState = TitleScreenMenu.GUIState.Main;
		//	TitleScreenMenu.Singleton.gameObject.SetActive (true);
		//	if(PhotonNetwork.inRoom)
		//		PhotonNetwork.LeaveRoom();
		//	break;
		//}
	}
	
	private void OnNetworkDisconnect()
	{
		string newIp = "";
		switch (Settings.Singleton.SelectedServer) 
		{
		case Settings.ServerChoice.Europe:
			newIp += "app-eu.exitgamescloud.com";
			break;
		case Settings.ServerChoice.USA:
			newIp += "app-us.exitgamescloud.com";
			break;
		case Settings.ServerChoice.Asia:
			newIp += "app-asia.exitgamescloud.com";
			break;
		}
		//PhotonNetwork.PhotonServerSettings.ServerAddress = newIp;
		//PhotonNetwork.ConnectUsingSettings("1.0");
	}
}
