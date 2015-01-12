using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TitleScreenMenu : MonoBehaviour //Photon.MonoBehaviour 
{
	//public enum GUIState { Main, Lobby, Game }
	//
	//public static TitleScreenMenu Singleton;
	//
	//public GameObject LobbyMenu;
	//
	//public Camera GUICamera;
	//public GameObject ListItemPrefab;
	//public Transform ListItemSpawn;
	//public tk2dUIItem HostGame;
	//public tk2dUIItem JoinGame;
	//public tk2dUIItem RefreshList;
	//public tk2dUIItem InputName;
	//public tk2dUIItem InputGameName;
	//public tk2dUIItem InputMaxPlayers;
	//public List<tk2dUIItem> HostList;
	//public tk2dUIItem CloseGame;
	//public tk2dUIItem Europe;
	//public tk2dUIItem USA;
	//public tk2dUIItem Asia;
	//
	//[HideInInspector]
	//public int SelectedHostListItem = -1;
	//
	//public GUIState CurrentGUIState;
	// 
	//private tk2dSlicedSprite europeSprite;
	//private tk2dSlicedSprite usaSprite;
	//private tk2dSlicedSprite asiaSprite;
	//
	//void Awake()
	//{
	//	Singleton = this;
	//	europeSprite = Europe.GetComponent<tk2dSlicedSprite> ();
	//	usaSprite = USA.GetComponent<tk2dSlicedSprite> ();
	//	asiaSprite = Asia.GetComponent<tk2dSlicedSprite> ();
	//}
	//
	//void Start () 
	//{
	//	HostGame.OnDownUIItem += OnHostGame;
	//	HostGame.OnDownUIItem += OnFingerDown;
	//	HostGame.OnUpUIItem += OnFingerUp;
	//	
	//	JoinGame.OnDownUIItem += OnJoinGame;
	//	JoinGame.OnDownUIItem += OnFingerDown;
	//	JoinGame.OnUpUIItem += OnFingerUp;
	//	
	//	RefreshList.OnDownUIItem += OnRefreshList;
	//	RefreshList.OnDownUIItem += OnFingerDown;
	//	RefreshList.OnUpUIItem += OnFingerUp;
	//	
	//	InputName.OnDownUIItem += OnInputName;
	//	InputGameName.OnDownUIItem += OnInputGameName;
	//	InputMaxPlayers.OnDownUIItem += OnInputMaxPlayers;
	//	HostList = new List<tk2dUIItem> ();
	//	
	//	CloseGame.OnUpUIItem += OnCloseGame;
	//	CloseGame.OnDownUIItem += OnFingerDown;
	//	CloseGame.OnUpUIItem += OnFingerUp;
	//	
	//	Europe.OnDownUIItem += OnEuropeClicked;
	//	USA.OnDownUIItem += OnUSAClicked;
	//	Asia.OnDownUIItem += OnAsiaClicked;
	//
	//	HighlightSelectedServer ();
	//}
	//
	//void Update () 
	//{
	//	HandleHostListSelection ();
	//}
	//
	//public void HandleHostListSelection()
	//{
	//	foreach (tk2dUIItem item in HostList) item.GetComponent<tk2dSlicedSprite> ().spriteId = 1;
	//
	//	if (SelectedHostListItem == -1) return;
	//
	//	HostList [SelectedHostListItem].GetComponent<tk2dSlicedSprite> ().spriteId = 2;
	//	
	//	if (Input.GetMouseButtonDown (0)) 
	//	{
	//		Ray ray = GUICamera.ScreenPointToRay (Input.mousePosition);
	//		RaycastHit hit;
	//		Physics.Raycast (ray, out hit, Mathf.Infinity);
	//
	//		if(hit.collider == null 
	//		   || (hit.collider.gameObject.name != "JoinGame" && hit.collider.gameObject.name != "HostListItem(Clone)")) 
	//			SelectedHostListItem = -1;
	//	}
	//}
	//
	//public void HighlightSelectedServer()
	//{
	//	europeSprite.spriteId = 1;
	//	usaSprite.spriteId = 1;
	//	asiaSprite.spriteId = 1;
	//	
	//	switch (Settings.Singleton.SelectedServer) 
	//	{
	//	case Settings.ServerChoice.Europe:
	//		europeSprite.spriteId = 2;
	//		break;
	//	case Settings.ServerChoice.USA:
	//		usaSprite.spriteId = 2;
	//		break;
	//	case Settings.ServerChoice.Asia:
	//		asiaSprite.spriteId = 2;
	//		break;
	//	}
	//}
	//
	//public void OnRefreshList(tk2dUIItem source)
	//{
	//	SelectedHostListItem = -1;
	//	foreach (tk2dUIItem item in HostList) DestroyImmediate (item.gameObject);
	//	HostList.Clear ();
	//	if (!PhotonNetwork.insideLobby) return;
	//	for(int index = 0; index < PhotonNetwork.GetRoomList().Length; index++)
	//	{
	//		GameObject newListItem = (GameObject)Instantiate(ListItemPrefab, Vector3.zero, Quaternion.identity);
	//		newListItem.transform.parent = ListItemSpawn.transform.parent;
	//		newListItem.transform.localPosition = ListItemSpawn.transform.localPosition+(ListItemSpawn.transform.localPosition*index);
	//		newListItem.GetComponent<tk2dUIItem>().OnDownUIItem += OnAnyHostListItem;
	//		newListItem.transform.FindChild("GameName").GetComponent<tk2dTextMesh>().text += PhotonNetwork.GetRoomList()[index].name;
	//		newListItem.transform.FindChild("Host").GetComponent<tk2dTextMesh>().text += PhotonNetwork.GetRoomList()[index].customProperties["HostName"];
	//		newListItem.transform.FindChild("Players").GetComponent<tk2dTextMesh>().text += 
	//			PhotonNetwork.GetRoomList()[index].playerCount.ToString()+"/"+PhotonNetwork.GetRoomList()[index].maxPlayers.ToString();
	//		HostList.Add(newListItem.GetComponent<tk2dUIItem>());
	//	}
	//}
	//
	//public void OnHostGame(tk2dUIItem source)
	//{
	//	SelectedHostListItem = -1;
	//	string gameName = InputGameName.transform.GetChild(0).GetComponent<TextField>().MyName;
	//	string count = "";
	//	bool youMayProceed = false;
	//	
	//	while (!youMayProceed) 
	//	{
	//		youMayProceed = true;
	//		foreach(RoomInfo item in PhotonNetwork.GetRoomList())
	//		{
	//			if(gameName+count == item.name)
	//			{
	//				if(count.Length == 0) 
	//					count = "0";
	//				else
	//					count = (Convert.ToInt32(count)+1).ToString();
	//				youMayProceed = false;
	//				break;
	//			}
	//		}
	//	}
	//	PhotonNetwork.playerName = InputName.transform.GetChild (0).GetComponent<TextField> ().MyName;
	//	PhotonNetwork.JoinOrCreateRoom(gameName+count, new RoomOptions(), new TypedLobby());
	//}
	//
	//public void OnJoinGame(tk2dUIItem source)
	//{
	//	if (SelectedHostListItem == -1) return;
	//	
	//	PhotonNetwork.playerName = InputName.transform.GetChild (0).GetComponent<TextField> ().MyName;
	//	
	//	if(!PhotonNetwork.inRoom && PhotonNetwork.GetRoomList().Length > SelectedHostListItem)
	//		PhotonNetwork.JoinOrCreateRoom(PhotonNetwork.GetRoomList()[SelectedHostListItem].name, new RoomOptions(), new TypedLobby());
	//	
	//	SelectedHostListItem = -1;
	//}
	//
	//public void OnInputName(tk2dUIItem source)
	//{
	//	InputName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	//}
	//
	//public void OnInputGameName(tk2dUIItem source)
	//{
	//	InputGameName.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	//}
	//
	//public void OnInputMaxPlayers(tk2dUIItem source)
	//{
	//	InputMaxPlayers.transform.GetChild (0).GetComponent<TextField> ().Toggle ();
	//}
	//
	//public void OnAnyHostListItem(tk2dUIItem source)
	//{
	//	for(int index = 0; index < HostList.Count; index++)
	//	{
	//		if(source == HostList[index]) 
	//		{
	//			SelectedHostListItem = index;
	//			return;
	//		}
	//	}
	//}
	//
	//public void OnEuropeClicked(tk2dUIItem source)
	//{
	//	if (!PhotonNetwork.insideLobby || Settings.Singleton.SelectedServer == Settings.ServerChoice.Europe) return;
	//	
	//	Settings.Singleton.SelectedServer = Settings.ServerChoice.Europe;
	//	PhotonNetwork.Disconnect ();
	//	HighlightSelectedServer ();
	//}
	//
	//public void OnUSAClicked(tk2dUIItem source)
	//{
	//	if (!PhotonNetwork.insideLobby || Settings.Singleton.SelectedServer == Settings.ServerChoice.USA) return;
	//	
	//	Settings.Singleton.SelectedServer = Settings.ServerChoice.USA;
	//	PhotonNetwork.Disconnect ();
	//	HighlightSelectedServer ();
	//}
	//
	//public void OnAsiaClicked(tk2dUIItem source)
	//{
	//	if (!PhotonNetwork.insideLobby || Settings.Singleton.SelectedServer == Settings.ServerChoice.Asia) return;
	//	
	//	Settings.Singleton.SelectedServer = Settings.ServerChoice.Asia;
	//	PhotonNetwork.Disconnect ();
	//	HighlightSelectedServer ();
	//}
	//
	//public void OnCloseGame(tk2dUIItem source)
	//{
	//	GameManager.Singleton.OnCloseGameClicked ();
	//}
	//
	//public void OnFingerDown(tk2dUIItem source)
	//{
	//	if (source.GetComponent<tk2dSprite> () != null)
	//		source.GetComponent<tk2dSprite> ().spriteId = 2;
	//	
	//	if (source.GetComponent<tk2dSlicedSprite> () != null)
	//		source.GetComponent<tk2dSlicedSprite> ().spriteId = 2;
	//}
	//
	//
	//public void OnFingerUp(tk2dUIItem source)
	//{
	//	if (source.GetComponent<tk2dSprite> () != null)
	//		source.GetComponent<tk2dSprite> ().spriteId = 1;
	//	
	//	if (source.GetComponent<tk2dSlicedSprite> () != null)
	//		source.GetComponent<tk2dSlicedSprite> ().spriteId = 1;
	//}
	//
	//void OnJoinedRoom()
	//{
	//	if (!PhotonNetwork.isMasterClient) return;
	//
	//	PhotonNetwork.room.SetPropertiesListedInLobby (new string[]{"HostName"});
	//	ExitGames.Client.Photon.Hashtable myInfo = new ExitGames.Client.Photon.Hashtable (); 
	//	myInfo.Add ("HostName", PhotonNetwork.playerName);
	//	PhotonNetwork.room.maxPlayers = Convert.ToInt32(InputMaxPlayers.transform.GetChild (0).GetComponent<TextField> ().MyName);
	//	PhotonNetwork.room.SetCustomProperties (myInfo);
	//	CurrentGUIState = GUIState.Lobby;
	//	LobbyMenu.SetActive (true);
	//	gameObject.SetActive (false);
	//}
}
