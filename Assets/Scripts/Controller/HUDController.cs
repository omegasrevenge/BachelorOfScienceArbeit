using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDController : MonoBehaviour 
{
	public static HUDController Singleton;

	public GameObject MainMenu;
	
	public GameController MyGameController
	{
		get
		{
			return GameController.Singleton;
		}
	}

	void Start () 
	{
		Singleton = this;
	}

	public void GameStart()
	{
		MainMenu.SetActive (false);
		MyGameController.CreateGame ();
	}

	public void GameJoin()
	{
		//Weapon Filler = new GameObject("...").AddComponent<Weapon>();
		//Debug.Log ("THIS IS THE URL: "+Weapon.WeaponModelFolder + "/" + Filler.WeaponModelNames [1]);
		//Resources.Load (Weapon.WeaponModelFolder + "/" + Filler.WeaponModelNames [1]);
		//Destroy (Filler.gameObject);
		MainMenu.SetActive (false);
		MyGameController.RequestHosts ();
	}

	public void GameEnd()
	{
		MainMenu.SetActive(true);
	}

	public void CloseGame()
	{
		Application.Quit ();
	}
}
