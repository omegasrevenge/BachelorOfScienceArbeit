using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public static GameController Singleton;

	public delegate void UserEvent (UserEntry User);
	public UserEvent OnNewUserEvent;
	public UserEvent OnRemoveUserEvent;

	[HideInInspector]
	public List<Transform> SpawnPositions = new List<Transform> ();
	[HideInInspector]
	public GameObject MyPlayer;
	[HideInInspector]
	public Properties.GameState CurGameState = Properties.GameState.Menu;
	[HideInInspector]
	public bool DestroyCamera = true;
	[HideInInspector]
	public List<GameObject> Players = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> Weapons = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> WeaponSpawnPlatforms = new List<GameObject>();
	[HideInInspector]
	public GameObject CurMap;

	public int PlayersFinishedLoading = 0;

	public List<UserEntry> Users = new List<UserEntry> (); //All currently registered users. Contains info like user name.

	public string SelectedMap = "MapSmallArena";
	public int MaxNumberAllowedClients = 3; // this is here and not in properties because every map could have a different amount
	public int TotalConnectionNumber = 0;

	private bool Timeout = true;


	public HUDController MyHUD
	{
		get
		{
			return HUDController.Singleton;
		}
	}

	public bool HasNetworkConnection
	{
		get
		{
			return Network.connections.Length > 0;
		}
	}

	void Start () 
	{
		Singleton = this;
		Application.runInBackground = true;
		Application.targetFrameRate = 60;
		OnNewUserEvent += RegisterNewUserFunction;
	}
	
	public void RegisterNewUserFunction(UserEntry user)
	{
		user.OnStatisticsUpdated += CheckForVictory;
	}
	
	public void CheckForVictory(UserEntry user)
	{
		if (user.Kills < Properties.KillsToWin) return;

		CurGameState = Properties.GameState.GameOver;
		MyHUD.MyStatistics.DisplayWinScreen (user.UserName);
		MyPlayer.GetComponent<PlayerController> ().MyCamera.GetComponent<MouseLook> ().enabled = false;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			if(Network.isServer)
				networkView.RPC ("RPCResetGame", RPCMode.AllBuffered);
			else
				RPCResetGame();
		}
	}

	public void StartGame()
	{
		if (!HasNetworkConnection) return; //otherwise Network.class gets confused, since you are technically not in a network yet but already want to send RPCs to start game

		MasterServer.UnregisterHost();	//So no one tries to join while game is on going

		if(CurMap == null)
			Network.Instantiate (Resources.Load(SelectedMap), Vector3.zero, Quaternion.identity, 1);
	}

	public void PlayerFinishedLoading()
	{
		networkView.RPC ("RPCPlayerFinishedLoading", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCPlayerFinishedLoading()
	{
		PlayersFinishedLoading++;
		if (Network.isServer && PlayersFinishedLoading == Users.Count)
						StartGameCountDown ();
	}

	public void StartGameCountDown()
	{
		MyHUD.MyCountdown.CountdownOverEvent += GameCommence;
		MyHUD.networkView.RPC ("RPCSwitchToStartingScreen", RPCMode.AllBuffered);
	}

	public void GameCommence() // called upon arena initialized, spawnpoints added, and countdown over, now spawnprocess can begin
	{
		MyHUD.MyCountdown.CountdownOverEvent -= GameCommence;
		networkView.RPC ("RPCSpawnOrder", RPCMode.AllBuffered, Network.connections);
	}

	[RPC]
	public void RPCStartGame()
	{
		Screen.showCursor = false;
		CurGameState = Properties.GameState.InGame;
		MyHUD.GameCommence ();
	}

	public IEnumerator CPlayerSpawnSequence(NetworkPlayer[] order)
	{
		if (Network.isServer) 
		{
			SpawnPlayer ();
			yield return new WaitForEndOfFrame();
		} 
		else 
		{
			while (Players.Count < 1)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		while (Players.Count < order.Length+1) 
		{
			if (order [Players.Count - 1] == Network.player)
				SpawnPlayer ();

			yield return new WaitForEndOfFrame();
		}

		if(Network.isServer)
			networkView.RPC ("RPCStartGame", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCSpawnOrder(NetworkPlayer[] order)
	{
		StartCoroutine ("CPlayerSpawnSequence", order);
	}

	public void SpawnPlayer()
	{
		Transform spawnPos = GetFurthestSpawnPoint ();
		Network.Instantiate(Resources.Load("Character"), 
		                    spawnPos.position, 
		                    spawnPos.rotation,
		                      1);
	}

	public Transform GetFurthestSpawnPoint()
	{
		if (Players.Count < 1)
						return SpawnPositions [Random.Range(0, SpawnPositions.Count)];

		int curSpawnPointIndex = 0;
		float spawnPointDistance = 0f;

		for (int i = 0; i < SpawnPositions.Count; i++) 
		{
			float tallyDistance = 0f;

			foreach(GameObject player in Players)
			{
				tallyDistance += GetDistance(player.transform.position, SpawnPositions[i].position);
			}

			if(tallyDistance > spawnPointDistance)
			{
				curSpawnPointIndex = i;
				spawnPointDistance = tallyDistance;
			}
		}

		return SpawnPositions[curSpawnPointIndex];
	}

    public void CreateGame()
	{
        Network.InitializeServer(MaxNumberAllowedClients, Properties.Port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(Properties.GameType, Properties.GameName);
		CurGameState = Properties.GameState.Lobby;
		MyHUD.SwitchToLobby ();
    }

	public void AbortLobby()
	{
		MyHUD.SwitchToMainMenu ();
		CurGameState = Properties.GameState.Menu;
		if(Network.isServer)
			MasterServer.UnregisterHost();
		if(HasNetworkConnection)
			Network.Disconnect ();

		CleanResetGame ();
	}

    public void RequestHosts()
	{
		if(HasNetworkConnection) return;
		Timeout = false;
		MasterServer.RequestHostList(Properties.GameType);
		StartCoroutine ("CCountdownTimeout");
	}
	
	public IEnumerator CCountdownTimeout()
	{
		yield return new WaitForSeconds(Properties.RequestHostTimeoutLength);

		if (!HasNetworkConnection) 
		{
			Timeout = true;
			MyHUD.SwitchToMainMenu();
		}
	}

    private void OnMasterServerEvent(MasterServerEvent MSEvent)
	{
		if (Timeout) return;

		if(MSEvent == MasterServerEvent.HostListReceived)
		{
			HostData[] data = MasterServer.PollHostList();
			if(data.Length>0) Network.Connect(data[data.Length-1]);
		}
    }

    private void OnConnectedToServer()
	{
		CurGameState = Properties.GameState.Lobby;
		MyHUD.SwitchToLobby ();
		RegisterUser.Register (MyHUD.CurrentNickname);
	}
	
	void OnServerInitialized()
	{
		RegisterUser.Register (MyHUD.CurrentNickname);
	}

	void OnPlayerDisconnected(NetworkPlayer origin) // is called ONLY ON THE SERVER! SAFE TO USE!
	{
		Network.RemoveRPCs (origin);
		Network.DestroyPlayerObjects(origin);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		CleanResetGame (); //if the server leaves the game, his rpcresetgame doesnt get sent. the clients have to clean up after themselves
	}

	[RPC]
	public void RPCResetGame()
	{
		if(Network.isServer)
			MasterServer.UnregisterHost();	//So the masterserver doesnt get confused when host disconnects before onplayerconnected is called for the first time
		Network.Disconnect();

		CleanResetGame ();
	}

	public void CleanResetGame()
	{
		Screen.showCursor = true;
		MyHUD.SwitchToMainMenu ();
		CurGameState = Properties.GameState.Menu;
		
		foreach(GameObject Player in Players) 
			Destroy(Player);
		Players.Clear ();
		
		foreach (GameObject Weapon in Weapons)
			Destroy(Weapon);
		Weapons.Clear ();

		if (CurMap != null)
			Destroy (CurMap);
		SpawnPositions.Clear ();
		WeaponSpawnPlatforms.Clear ();

		foreach (UserEntry User in Users)
				Destroy (User.MyRegisterObject.gameObject);

		Users.Clear ();

		GameObject Cam = GameObject.FindGameObjectWithTag ("MainCamera");
		if(Cam != null)
			Destroy (Cam);

		PlayersFinishedLoading = 0;
	}

	public void Respawn(float respawnTime)
	{
		if (CurGameState == Properties.GameState.GameOver) return;

		StartCoroutine ("CRespawn", respawnTime);
	}
	
	public IEnumerator CRespawn(float respawnTime)
	{
		Countdown.CountDownFrom (respawnTime);
		yield return new WaitForSeconds(respawnTime);
		
		if(CurGameState != Properties.GameState.GameOver)
		{
			if (DestroyCamera) 
			{
				Camera[] MyCams = GameObject.FindObjectsOfType<Camera> ();
				foreach(Camera cam in MyCams)
					Destroy(cam.gameObject);
			}
			
			SpawnPlayer ();
		}
	}

	public static float GetDistance(Vector3 source, Vector3 target)
	{
		return Mathf.Abs ((target - source).magnitude);
	}

	public static UserEntry AddUserEntry(NetworkPlayer user, string userName)
	{
		UserEntry NewUser = new UserEntry (user, userName);

		GameController.Singleton.Users.Add (NewUser);

		if (GameController.Singleton.OnNewUserEvent != null)
			GameController.Singleton.OnNewUserEvent (NewUser);

		return NewUser;
	}

	public static void RemoveUserEntry(int id)
	{
		UserEntry MyUserEntry = GetUserEntry (id);

		GameController.Singleton.Users.Remove (MyUserEntry);

		if (GameController.Singleton.OnRemoveUserEvent != null)
			GameController.Singleton.OnRemoveUserEvent (MyUserEntry);
	}

	public static UserEntry GetUserEntry(int id)
	{
		foreach (UserEntry User in GameController.Singleton.Users)
			if (User.ID == id)
				return User;
		
		return null;
	}

	public static UserEntry GetUserEntry(NetworkPlayer target)
	{
		foreach (UserEntry Entry in GameController.Singleton.Users)
				if (Entry.User == target)
					return Entry;

		return null;
	}

	[System.Serializable]
	public class UserEntry
	{
		public delegate void Updated(UserEntry user);
		public Updated OnStatisticsUpdated;

		public NetworkPlayer User;
		public string UserName = "";
		public int ID;
		public RegisterUser MyRegisterObject;
		public int Kills = 0;
		public int Deaths = 0;

		public UserEntry(NetworkPlayer user, string userName)
		{
			User = user;
			UserName = userName;
		}

		public void UpdateStatistics(int kills, int deaths)
		{
			Kills = kills;
			Deaths = deaths;
			if (OnStatisticsUpdated != null)
				OnStatisticsUpdated (this);
		}
	}
}

