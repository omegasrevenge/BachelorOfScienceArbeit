using UnityEngine;
using System.Collections;

public class ArenaController : MonoBehaviour 
{
	void Start () 
	{
		GameController _myCtrl = GameController.Singleton;

		if (_myCtrl.CurMap != null)
			Destroy (_myCtrl.CurMap);

		_myCtrl.CurMap = gameObject;
		_myCtrl.SpawnPositions.Clear (); //its possible it still contains nulls from last map

		foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint")) 
			_myCtrl.SpawnPositions.Add (spawnPoint.transform);

		if(Network.isServer)
			_myCtrl.ArenaSpawned ();
	}
}
