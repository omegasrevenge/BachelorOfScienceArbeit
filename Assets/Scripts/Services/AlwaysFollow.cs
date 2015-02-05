using UnityEngine;
using System.Collections;

public class AlwaysFollow : MonoBehaviour 
{
	public Transform Target;

	void Update () 
	{
		if (Target != null && GameController.Singleton.CurGameState != Properties.GameState.GameOver) 
		{
			transform.position = Target.position;
			transform.rotation = Target.rotation;
		}
	}
}
