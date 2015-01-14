using UnityEngine;

public class GameController : MonoBehaviour
{
	public enum GameState{Menu, InGame}
	public enum PlayerGameStatus{Host, Player}

	public static GameController Singleton;

    public const string GameType = "BachelorOfScience.SalzmannKirill.MDH2015";
	public const int Port = 43654;

	public Transform PlayerCharacterParent;
	public GameObject CharacterPrefab;
	public Transform[] SpawnPositions = new Transform[4];
	
	[HideInInspector]
    public string GameName = "BachelorOfScienceSalzmannKirill";
	[HideInInspector]
	public GameObject PlayerHost;
	[HideInInspector]
	public GameObject PlayerOther;
	[HideInInspector]
	public PlayerGameStatus MyStatus = PlayerGameStatus.Host;
	[HideInInspector]
	public GameState CurGameState = GameState.Menu;

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
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) 
			networkView.RPC ("ResetLevel", RPCMode.All);
		if(!HasNetworkConnection && CurGameState == GameState.InGame)
		    ResetLevel();
	}

	public void SpawnPlayer(PlayerGameStatus status)
	{
		Transform spawnPos = GetFurthestSpawnPoint ();
		GameObject player = (GameObject)Network.Instantiate(Resources.Load("Character"), 
		                                          spawnPos.position, 
		                                          spawnPos.rotation,
		                                            1);
	}

	public Transform GetFurthestSpawnPoint()
	{
		if (MyStatus == PlayerGameStatus.Host && PlayerOther == null)
			return SpawnPositions [0];
		
		if (MyStatus == PlayerGameStatus.Player && PlayerHost == null)
			return SpawnPositions [3];

		float distance = 0f;
		Transform pos = transform;

		Vector3 enemyPos = 
			MyStatus == PlayerGameStatus.Host ? PlayerOther.transform.position : PlayerHost.transform.position;

		for (int i = 0; i < SpawnPositions.Length; i++) 
		{
			float curDist = GetDistance(enemyPos, SpawnPositions[i].position);
			if(curDist > distance)
			{
				distance = curDist;
				pos = SpawnPositions[i];
			}
		}

		return pos;
	}

    public void CreateGame()
	{
        Network.InitializeServer(4, Port, !Network.HavePublicAddress());
        MasterServer.RegisterHost(GameType, GameName);
    }

    public void RequestHosts()
	{
		if(HasNetworkConnection) return;
		MasterServer.RequestHostList(GameType);
    }

    private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if(msEvent == MasterServerEvent.HostListReceived)
		{
			HostData[] data = MasterServer.PollHostList();
			if(data.Length>0) Network.Connect(data[0]);
		}
    }

    private void OnConnectedToServer()
	{
		MyStatus = PlayerGameStatus.Player;
		SpawnPlayer (MyStatus);
		CurGameState = GameState.InGame;
	}
	
	void OnServerInitialized()
	{
		MyStatus = PlayerGameStatus.Host;
		SpawnPlayer (MyStatus);
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		MasterServer.UnregisterHost();	//So no one tries to join while game is on going
		CurGameState = GameState.InGame;
	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		ResetLevel ();
	}

	[RPC]
	public void ResetLevel()
	{
		MasterServer.UnregisterHost();	//So the masterserver doesnt get confused when host disconnects before onplayerconnected is called for the first time
		Network.Disconnect();
		foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) Destroy(player);

		MyHUD.GameEnd ();
		CurGameState = GameState.Menu;
	}

	public static float GetDistance(Vector3 source, Vector3 target)
	{
		return Mathf.Abs ((target - source).magnitude);
	}
}

