using UnityEngine;
using System.Collections;

public class CharControl : MonoBehaviour 
{
	public static CharControl MyCharacter;

	public NavMeshAgent MyAgent;

	[HideInInspector]
	public CursorController MyCursorController;

	void Start()
	{
		if(!GetComponent<PhotonView>().isMine)
		{
			enabled = false;
			MyAgent.enabled = false;
			return;
		}

		MyCharacter = this;
	}

	public void MoveCharacter(Vector3 destination)
	{
		if (MyCharacter == null) return;

		MyAgent.destination = MyCursorController.MoveCursor(destination);
	}
}
