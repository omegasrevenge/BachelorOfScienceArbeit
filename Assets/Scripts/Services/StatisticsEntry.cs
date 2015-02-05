using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatisticsEntry : MonoBehaviour 
{
	public Text Name;
	public Text Kills;
	public Text Deaths;

	public void UpdateEntry(GameController.UserEntry User)
	{
		this.Name.text = User.UserName;
		this.Kills.text = User.Kills.ToString ();
		this.Deaths.text = User.Deaths.ToString ();
	}
}
