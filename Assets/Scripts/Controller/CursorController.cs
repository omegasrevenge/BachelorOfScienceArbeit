using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour 
{

	public Vector3 MoveCursor(Vector3 targetPosition)
	{
		transform.parent.position = new Vector3( targetPosition.x, transform.parent.position.y , targetPosition.z );
		return transform.parent.position;
	}
	//TODO blende cursor nach dem klick wieder aus
}
