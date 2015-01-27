using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public static GameController Singleton;

	public Transform[] SpawnPositions = new Transform[4];

	[HideInInspector]
	public GameObject MyPlayer;
	[HideInInspector]
	public GameObject PlayerHost;
	[HideInInspector]
	public GameObject PlayerOther;
	[HideInInspector]
	public Properties.PlayerGameStatus MyStatus = Properties.PlayerGameStatus.Host;
	[HideInInspector]
	public Properties.GameState CurGameState = Properties.GameState.Menu;
	[HideInInspector]
	public bool DestroyCamera = true;
	
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
			networkView.RPC ("RPCResetGame", RPCMode.All);
		if(!HasNetworkConnection && CurGameState == Properties.GameState.InGame)
			RPCResetGame();
	}

	public void StartGame()
	{
		CurGameState = Properties.GameState.InGame;
		SpawnPlayer ();
		if(MyStatus == Properties.PlayerGameStatus.Host)
			WeaponSpawnPlatform.SetWeaponPlatforms (true);
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
		if (MyStatus == Properties.PlayerGameStatus.Host && PlayerOther == null)
			return SpawnPositions [0];
		
		if (MyStatus == Properties.PlayerGameStatus.Player && PlayerHost == null)
			return SpawnPositions [3];

		float distance = 0f;
		Transform pos = transform;

		Vector3 enemyPos = 
			MyStatus == Properties.PlayerGameStatus.Host ? PlayerOther.transform.position : PlayerHost.transform.position;

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
        Network.InitializeServer(4, Properties.Port, !Network.HavePublicAddress());
		MasterServer.RegisterHost(Properties.GameType, Properties.GameName);
    }

    public void RequestHosts()
	{
		if(HasNetworkConnection) return;
		MasterServer.RequestHostList(Properties.GameType);
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
		MyStatus = Properties.PlayerGameStatus.Player;
		StartGame ();
	}
	
	void OnServerInitialized()
	{
		MyStatus = Properties.PlayerGameStatus.Host;
	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		MasterServer.UnregisterHost();	//So no one tries to join while game is on going
		StartGame ();
	}

	void OnPlayerDisconnected(NetworkPlayer player) 
	{
		RPCResetGame ();
	}

	[RPC]
	public void RPCResetGame()
	{
		MasterServer.UnregisterHost();	//So the masterserver doesnt get confused when host disconnects before onplayerconnected is called for the first time
		Network.Disconnect();

		MyHUD.GameEnd ();
		CurGameState = Properties.GameState.Menu;
		WeaponSpawnPlatform.SetWeaponPlatforms (false);
		
		foreach(GameObject Player in GameObject.FindGameObjectsWithTag("Player")) 
			Destroy(Player);
		foreach (GameObject Weapon in GameObject.FindGameObjectsWithTag ("Weapon"))
			Destroy(Weapon);
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

