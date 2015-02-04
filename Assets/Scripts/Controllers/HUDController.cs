using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public static HUDController Singleton;

	public GameObject MainMenu;
	public GameObject Lobby;
	public GameObject WaitingForServer;
	public GameObject InGameUI;

	public Crosshair MyCrosshair;
	public Text AmmunitionCounter;
	public Text HealthCounter;
	public Text NicknameInput;
	public Actionbar MyActionBar;
	public HUDCurrentWeapon MyWeaponBar;
	public Countdown MyCountdown;
	public Statistics MyStatistics;

	[HideInInspector]
	public string CurrentNickname = Properties.DefaultNickname;

	private Text _myLobbyText;
	private string _lobbyText;

	void Start () 
	{
		Singleton = this;
		_myLobbyText = Lobby.transform.FindChild ("Connections").GetComponent<Text> ();
		_lobbyText = _myLobbyText.text;

		if (GameController.Singleton != null) 
		{
			GameController.Singleton.OnNewUserEvent += UpdateLobbyText;
			GameController.Singleton.OnRemoveUserEvent += UpdateLobbyText;
		} 
		else 
		{
			GameController _myGameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			_myGameController.OnNewUserEvent += UpdateLobbyText;
			_myGameController.OnRemoveUserEvent += UpdateLobbyText;
		}
	}

	void Update()
	{
		if (GameController.Singleton.CurGameState == Properties.GameState.InGame) 
		{
			if(Input.GetKeyDown(KeyCode.Tab) && !MyStatistics.gameObject.activeSelf)
			{
				MyStatistics.gameObject.SetActive(true);
			}

			if(Input.GetKeyUp(KeyCode.Tab) && MyStatistics.gameObject.activeSelf)
			{
				MyStatistics.gameObject.SetActive(false);
			}
		}
	}

	public void SaveNickname()
	{
		CurrentNickname = NicknameInput.text;
	}

	public void GameStart()
	{
		MainMenu.SetActive (false);
		Lobby.SetActive (true);
		InGameUI.SetActive (false);
		GameController.Singleton.CreateGame ();
	}

	public void GameJoin()
	{
		MainMenu.SetActive (false);
		InGameUI.SetActive (false);
		WaitingForServer.SetActive (true);
		GameController.Singleton.RequestHosts ();
	}

	public void SwitchToLobby()
	{
		_myLobbyText.text = _lobbyText; //if you are client, then leave and open your own game, this does not get otherwise reset
		WaitingForServer.SetActive (false);
		MyCountdown.gameObject.SetActive (false);
		InGameUI.SetActive (false);
		Lobby.SetActive (true);
		Lobby.transform.FindChild ("StartGame").gameObject.SetActive (Network.isServer);
	}

	public void SwitchToMainMenu()
	{
		MyCountdown.gameObject.SetActive (false);
		InGameUI.SetActive (false);
		WaitingForServer.SetActive (false);
		Lobby.SetActive (false);
		MainMenu.SetActive (true);
	}

	public void GameCommence()
	{
		MainMenu.SetActive (false);
		Lobby.SetActive (false);
		WaitingForServer.SetActive (false);
		InGameUI.SetActive (true);
	}

	public void CloseGame()
	{
		Application.Quit ();
	}

	public void EveryoneCountdown(float TargetTime)
	{
		networkView.RPC ("RPCCountDownFrom", RPCMode.All, TargetTime);
	}

	[RPC]
	public void RPCCountDownFrom(float TargetTime)
	{
		if (GameController.Singleton.CurGameState == Properties.GameState.Lobby)
			Lobby.SetActive (false);
		Countdown.CountDownFrom (TargetTime);
	}

	public void UpdateLobbyText()
	{
		_myLobbyText.text = _lobbyText.Replace ("1", GameController.Singleton.Users.Count.ToString ());
	}
}
