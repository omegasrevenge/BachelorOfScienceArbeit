using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public static GameController Singleton;

	public Transform[] SpawnPositions = new Transform[4];

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

	public int MaxNumberAllowedClients = 3;
	public int TotalConnectionNumber = 0;

	private bool _timeout = true;
	
	private Properties _props;

	public Properties MyProperties
	{
		get
		{
			if(_props == null)
				_props = GetComponent<Properties>();
			return _props;
		}
	}

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
			networkView.RPC ("RPCResetGame", RPCMode.AllBuffered);
	}

	public void StartGame()
	{
		MasterServer.UnregisterHost();	//So no one tries to join while game is on going
		networkView.RPC ("RPCSpawnOrder", RPCMode.AllBuffered, Network.connections);
	}

	[RPC]
	public void RPCStartGame()
	{
		CurGameState = Properties.GameState.InGame;
		MyHUD.GameCommence ();
		if(Network.isServer)
			WeaponSpawnPlatform.SetWeaponPlatforms (true);
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
						return SpawnPositions [0];

		float distance = 0f;
		Transform pos = transform;

		Vector3 enemyPos = Vector3.zero;

		foreach(GameObject player in Players)
			enemyPos += player.transform.position;

		enemyPos /= Players.Count;

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
	}

    public void RequestHosts()
	{
		if(HasNetworkConnection) return;
		_timeout = false;
		MasterServer.RequestHostList(Properties.GameType);
		StartCoroutine ("CCountdownTimeout");
	}
	
	public IEnumerator CCountdownTimeout()
	{
		float _curTime = Properties.RequestHostTimeoutLength;

		while (_curTime > 0f) 
		{
			yield return new WaitForEndOfFrame();
			_curTime -= Time.deltaTime;
		}

		if (!HasNetworkConnection) 
		{
			_timeout = true;
			MyHUD.SwitchToMainMenu();
		}
	}

    private void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (_timeout) return;

		if(msEvent == MasterServerEvent.HostListReceived)
		{
			HostData[] data = MasterServer.PollHostList();
			if(data.Length>0) Network.Connect(data[0]);
		}
    }

    private void OnConnectedToServer()
	{
		CurGameState = Properties.GameState.Lobby;
		MyHUD.SwitchToLobby ();
	}
	
	void OnServerInitialized()
	{
		//
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		//
	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		if (Network.connections.Length == 1)
			MyHUD.UpdateLobbyText (1);
		else
			MyHUD.PollConnectionsInfo ();
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) 
	{
		RPCResetGame ();
	}

	[RPC]
	public void RPCResetGame()
	{
		MasterServer.UnregisterHost();	//So the masterserver doesnt get confused when host disconnects before onplayerconnected is called for the first time
		Network.Disconnect();

		MyHUD.SwitchToMainMenu ();
		CurGameState = Properties.GameState.Menu;
		WeaponSpawnPlatform.SetWeaponPlatforms (false);
		
		foreach(GameObject Player in Players) 
			Destroy(Player);
		Players.Clear ();

		foreach (GameObject Weapon in Weapons)
			Destroy(Weapon);
		Weapons.Clear ();
	}

	public void Respawn(float RespawnTime)
	{
		StartCoroutine ("CRespawn", RespawnTime);
	}
	
	public IEnumerator CRespawn(float RespawnTime)
	{
		float _curRespawnTimer = RespawnTime;
		
		while (_curRespawnTimer > 0f) 
		{
			yield return new WaitForEndOfFrame();
			//RESPAWN TIMER FOR UI HERE!
			_curRespawnTimer -= Time.deltaTime;
		}

		if (DestroyCamera) 
		{
			Camera[] _myCams = GameObject.FindObjectsOfType<Camera> ();
			foreach(Camera cam in _myCams)
				Destroy(cam.gameObject);
		}

		SpawnPlayer ();
	}

	public static float GetDistance(Vector3 source, Vector3 target)
	{
		return Mathf.Abs ((target - source).magnitude);
	}
}

