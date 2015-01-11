using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour 
{
	public Camera GUICamera;

	void Awake()
	{
		GUICamera = GetComponent<Camera> ();
	}

	void Update ()
	{
		HandleRightMouseButton ();
		HandleLeftMouseButton ();
	}

	public void HandleRightMouseButton()
	{
		if (Input.GetMouseButtonDown(1)) 
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) 
			{
				if(hit.collider.gameObject.layer == 8 && CharControl.MyCharacter != null)
					CharControl.MyCharacter.MoveCharacter(hit.point);  
			}
		}
	}
	
	public void HandleLeftMouseButton()
	{
		//if (Input.GetMouseButtonDown(0)) 
		//{
		//	Ray ray = GUICamera.ScreenPointToRay (Input.mousePosition);
		//	RaycastHit hit;
		//	
		//	if (Physics.Raycast (ray, out hit, Mathf.Infinity)) 
		//	{
		//		if(hit.collider.gameObject.layer == 11 && hit.collider.GetComponent<tk2dButton>() != null)
		//		{
		//			StartCoroutine(hit.collider.GetComponent<tk2dButton>().coHandleButtonPress(-1));
		//		}
		//	}
		//}
	}
}
