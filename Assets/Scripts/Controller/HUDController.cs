using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public GameObject MainMenu;

	void Start () 
	{

	}

	public void GameStart()
	{
		MainMenu.SetActive (false);
	}
}
