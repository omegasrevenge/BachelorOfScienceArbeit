using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour 
{
	public static CameraManager Singleton;
	[HideInInspector]
	public Transform MyCharacter;
	[HideInInspector]
	public Transform CameraPivot;
	public float MouseSensitivity = 10f;

	void Awake()
	{
		Singleton = this;
		CameraPivot = transform.parent;
	}

	void Update () 
	{
		if (MyCharacter == null) return;

		CameraPivot.transform.position = MyCharacter.transform.position;
		HandleCameraRotation ();
		HandleCameraDistance ();
	}

	public void HandleCameraRotation()
	{
		// falls linke maustaste nicht gedrückt, brich ab
		if (!Input.GetMouseButton (0)) return;

		Vector3 temp = CameraPivot.eulerAngles;
		temp += new Vector3(-Input.GetAxis("Mouse Y")*MouseSensitivity, Input.GetAxis("Mouse X")*MouseSensitivity,0f);

		if (temp.x < 10f || temp.x > 80f) return;

		CameraPivot.eulerAngles = temp;
	}

	public void HandleCameraDistance()
	{
		Vector3 temp = Camera.main.transform.localPosition;
		temp += Camera.main.transform.localPosition.normalized * -Input.GetAxis ("Mouse ScrollWheel") * MouseSensitivity;
		
		if (temp.sqrMagnitude < 50f || temp.sqrMagnitude > 800f) return;

		Camera.main.transform.localPosition = temp;
	}
}
