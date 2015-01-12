using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyScreenManager : MonoBehaviour 
{

	public GameObject PlayerListItem;
	public GameObject PlayerSlotItem;
	public Transform PlayerListSpawnLocationTeam1;
	
	//public tk2dUIItem StartGame;
	//public tk2dUIItem KickPlayer;

	private Transform PlayerListSpawnLocationTeam2;
	
	private List<Transform> Team1Slots;
	private List<Transform> Team2Slots;

	void Awake()
	{
		Team1Slots = new List<Transform> ();
		Team2Slots = new List<Transform> ();
	}

	void OnEnable () 
	{
		Team1Slots.Clear ();
		Team2Slots.Clear ();

		//if (PhotonNetwork.inRoom) 
		//{
		//	int team1PlayerCount = (int)Mathf.Round (PhotonNetwork.room.maxPlayers/2.0f);
		//	for (int index = 0; index < PhotonNetwork.room.maxPlayers; index++) 
		//	{
		//		if(index < team1PlayerCount)
		//		{
		//			Team1Slots.Add(((GameObject)Instantiate(PlayerSlotItem)).transform);
		//		}
		//	}
		//} else Debug.LogError ("For some reason you started LobbyScreenManager without being in a room.");
	}

	void Update () 
	{
	
	}
}
