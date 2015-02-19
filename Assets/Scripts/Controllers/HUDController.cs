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

	private List<GameObject> MyScreens;

	[HideInInspector]
	public string CurrentNickname = Properties.DefaultNickname;

	private Text MyLobbyText;
	private string LobbyText;

	void Start () 
	{
		MyScreens = new List<GameObject> (){MainMenu, Lobby, WaitingForServer, InGameUI, StartingScreen, MyStatistics.gameObject};
		Singleton = this;
		MyLobbyText = Lobby.transform.FindChild ("Connections").GetComponent<Text> ();
		LobbyText = MyLobbyText.text;

		if (GameController.Singleton != null) 
		{
			GameController.Singleton.OnNewUserEvent += UpdateLobbyText;
			GameController.Singleton.OnRemoveUserEvent += UpdateLobbyText;
		} 
		else 
		{
			GameController MyGameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			MyGameController.OnNewUserEvent += UpdateLobbyText;
			MyGameController.OnRemoveUserEvent += UpdateLobbyText;
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
		foreach (GameObject Page in MyScreens)
				Page.SetActive (false);
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
		MyLobbyText.text = LobbyText; //if you are client, then leave and open your own game, this does not get otherwise reset
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

	public void MouseButtonClick()
	{
		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int) Properties.Sounds.Button), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int) Properties.Sounds.Button],
			Properties.Singleton.SoundDefaultMinDistances [(int) Properties.Sounds.Button],
			Properties.Singleton.SoundDefaultMaxDistances [(int) Properties.Sounds.Button]
			);
	}

	public void UpdateLobbyText(GameController.UserEntry User)
	{
		MyLobbyText.text = LobbyText.Replace ("1", GameController.Singleton.Users.Count.ToString ());
	}
}
