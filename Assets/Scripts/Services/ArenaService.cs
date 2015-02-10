using UnityEngine;
using System.Collections;

public class ArenaService : MonoBehaviour 
{
	void Start () 
	{
		GameController MyGameController = GameController.Singleton;

		if (MyGameController.CurMap != null)
			Destroy (MyGameController.CurMap);

		MyGameController.CurMap = gameObject;
		MyGameController.SpawnPositions.Clear (); //its possible it still contains nulls from last map
		MyGameController.WeaponSpawnPlatforms.Clear ();
		
		foreach (GameObject platform in GameObject.FindGameObjectsWithTag("WeaponSpawnPlatform")) 
			MyGameController.WeaponSpawnPlatforms.Add (platform);

		foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("SpawnPoint")) 
			MyGameController.SpawnPositions.Add (spawnPoint.transform);

		MyGameController.PlayerFinishedLoading ();
	}
}
