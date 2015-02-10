using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatisticsEntry : MonoBehaviour 
{
	public Text Name;
	public Text Kills;
	public Text Deaths;

	public void UpdateEntry(GameController.UserEntry user)
	{
		Name.text = user.UserName;
		Kills.text = user.Kills.ToString ();
		Deaths.text = user.Deaths.ToString ();
	}
}
