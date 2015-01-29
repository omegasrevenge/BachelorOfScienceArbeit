using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public static HUDController Singleton;

	public GameObject MainMenu;
	public GameObject Lobby;
	public GameObject WaitingForServer;

	private Text _myLobbyText;
	private string _lobbyText;
	
	public GameController MyGameController
	{
		get
		{
			return GameController.Singleton;
		}
	}

	void Start () 
	{
		Singleton = this;
		_myLobbyText = Lobby.transform.FindChild ("Connections").GetComponent<Text> ();
		_lobbyText = _myLobbyText.text;
	}

	public void GameStart()
	{
		MainMenu.SetActive (false);
		Lobby.SetActive (true);
		MyGameController.CreateGame ();
	}

	public void GameJoin()
	{
		MainMenu.SetActive (false);
		WaitingForServer.SetActive (true);
		MyGameController.RequestHosts ();
	}

	public void SwitchToLobby()
	{
		_myLobbyText.text = _lobbyText; //if you are client, then leave and open your own game, this does not get otherwise reset
		WaitingForServer.SetActive (false);
		Lobby.SetActive (true);
		PollConnectionsInfo ();
		Lobby.transform.FindChild ("StartGame").gameObject.SetActive (Network.isServer);
	}

	public void SwitchToMainMenu()
	{
		WaitingForServer.SetActive (false);
		Lobby.SetActive (false);
		MainMenu.SetActive (true);
	}

	public void GameCommence()
	{
		MainMenu.SetActive (false);
		Lobby.SetActive (false);
		WaitingForServer.SetActive (false);
	}

	public void CloseGame()
	{
		Application.Quit ();
	}

	public void UpdateLobbyText(int number)
	{
		_myLobbyText.text = _lobbyText.Replace ("1", number.ToString ());
	}

	public void PollConnectionsInfo()
	{
		networkView.RPC ("RPCPollConnectionsInfo", RPCMode.Server);
	}

	[RPC]
	public void RPCPollConnectionsInfo()
	{
		networkView.RPC ("RPCReceiveConnectionsInfo", RPCMode.AllBuffered, Network.connections.Length+1);
	}

	[RPC]
	public void RPCReceiveConnectionsInfo(int count)
	{
		MyGameController.TotalConnectionNumber = count;
		UpdateLobbyText (count);
	}
}
