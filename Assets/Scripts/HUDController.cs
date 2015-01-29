using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public static HUDController Singleton;

	public GameObject MainMenu;
	public GameObject Lobby;
	public GameObject WaitingForServer;

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
		_lobbyText = Lobby.transform.FindChild ("Connections").GetComponent<Text> ().text;
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
		WaitingForServer.SetActive (false);
		Lobby.SetActive (true);
		PollConnectionsInfo ();
		if (!Network.isServer)
						Lobby.transform.FindChild ("StartGame").gameObject.SetActive (false);
	}

	public void SwitchToMainMenu()
	{
		Debug.Log ("SwitchToMainMenu");
		WaitingForServer.SetActive (false);
		Lobby.SetActive (false);
		MainMenu.SetActive (true);
	}

	public void GameCommence()
	{
		Debug.Log ("GameCommence");
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
		Lobby.transform.FindChild ("Connections").GetComponent<Text> ().text = _lobbyText.Replace ("1", number.ToString ());
	}

	public void PollConnectionsInfo()
	{
		Debug.Log ("REQUESTING INFO FROM SERVER! PollConnectionsInfo: "+Network.connections.ToString());
		networkView.RPC ("RPCPollConnectionsInfo", RPCMode.Server);
	}

	[RPC]
	public void RPCPollConnectionsInfo()
	{
		Debug.Log ("SENDING REQUESTED INFO TO EVERYONE");
		networkView.RPC ("RPCReceiveConnectionsInfo", RPCMode.AllBuffered, Network.connections.Length+1);
	}

	[RPC]
	public void RPCReceiveConnectionsInfo(int count)
	{
		Debug.Log ("GOT THAT INFO! Just received new connections length number: "+count.ToString());
		MyGameController.TotalConnectionNumber = count;
		UpdateLobbyText (count);
	}
}
