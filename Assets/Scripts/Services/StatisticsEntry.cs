using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatisticsEntry : MonoBehaviour 
{
	public Text Name;
	public Text Kills;
	public Text Deaths;

	public void UpdateEntry(int Kills, int Deaths)
	{
		this.Kills.text = Kills.ToString ();
		this.Deaths.text = Deaths.ToString ();
	}
}
