using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDController : MonoBehaviour 
{
	public static HUDController Singleton;

	public GameObject MainMenu;
	public GameObject Lobby;
	public GameObject WaitingForServer;
	public GameObject InGameUI;
	public GameObject StartingScreen;

	public Crosshair MyCrosshair;
	public Text AmmunitionCounter;
	public Text HealthCounter;
	public Text NicknameInput;
	public Actionbar MyActionBar;
	public HUDCurrentWeapon MyWeaponBar;
	public Countdown MyCountdown;
	public Statistics MyStatistics;

	private List<GameObject> _myScreens;

	[HideInInspector]
	public string CurrentNickname = Properties.DefaultNickname;

	private Text _myLobbyText;
	private string _lobbyText;

	void Start () 
	{
		_myScreens = new List<GameObject> (){MainMenu, Lobby, WaitingForServer, InGameUI, StartingScreen, MyStatistics.gameObject};
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

	public void DeactivateScreens()
	{
		foreach (GameObject screen in _myScreens)
				screen.SetActive (false);
	}

	public void SaveNickname()
	{
		CurrentNickname = NicknameInput.text;
	}

	public void GameStart()
	{
		DeactivateScreens ();
		Lobby.SetActive (true);
		GameController.Singleton.CreateGame ();
	}

	public void GameJoin()
	{
		DeactivateScreens ();
		WaitingForServer.SetActive (true);
		GameController.Singleton.RequestHosts ();
	}

	public void SwitchToLobby()
	{
		_myLobbyText.text = _lobbyText; //if you are client, then leave and open your own game, this does not get otherwise reset
		DeactivateScreens ();
		Lobby.SetActive (true);
		Lobby.transform.FindChild ("StartGame").gameObject.SetActive (Network.isServer);
	}

	public void SwitchToMainMenu()
	{
		GetComponent<AudioListener> ().enabled = true;
		MyCountdown.gameObject.SetActive (false);
		DeactivateScreens ();
		MainMenu.SetActive (true);
	}

	public void SwitchToStartingScreen()
	{
		DeactivateScreens ();
		StartingScreen.SetActive (true);
	}

	public void GameCommence()
	{
		GetComponent<AudioListener> ().enabled = false;
		DeactivateScreens ();
		InGameUI.SetActive (true);
	}
	
	[RPC]
	public void RPCSwitchToStartingScreen()
	{
		SwitchToStartingScreen ();
		Countdown.CountDownFrom (Properties.GameStartTimer);
	}

	public void CloseGame()
	{
		Application.Quit ();
	}

	public void EveryoneCountdown(float TargetTime)
	{
		networkView.RPC ("RPCCountDownFrom", RPCMode.All, TargetTime);
	}

	public void MouseButtonClick()
	{
		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int) Properties.SoundsEnum.Button), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int) Properties.SoundsEnum.Button],
			Properties.Singleton.SoundDefaultMinDistances [(int) Properties.SoundsEnum.Button],
			Properties.Singleton.SoundDefaultMaxDistances [(int) Properties.SoundsEnum.Button]
			);
	}

	[RPC]
	public void RPCCountDownFrom(float TargetTime)
	{
		if (GameController.Singleton.CurGameState == Properties.GameState.Lobby)
			Lobby.SetActive (false);
		Countdown.CountDownFrom (TargetTime);
	}

	public void UpdateLobbyText(GameController.UserEntry User)
	{
		_myLobbyText.text = _lobbyText.Replace ("1", GameController.Singleton.Users.Count.ToString ());
	}
}
