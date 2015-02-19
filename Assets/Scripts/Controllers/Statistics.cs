using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Statistics : MonoBehaviour 
{
	public GameObject EntryPrefab;
	public GameObject WinGameMessage;
	public List<EntryCombination> Entries = new List<EntryCombination>();

	public void DisplayWinScreen(string userName)
	{
		gameObject.SetActive (true);
		WinGameMessage.SetActive (true);
		Text MyWinText = WinGameMessage.transform.FindChild ("Text").GetComponent<Text> ();
		MyWinText.text = MyWinText.text.Replace ("$", userName);
	}

	void OnEnable()
	{
		Entries.Clear ();
		foreach (GameController.UserEntry User in GameController.Singleton.Users) 
		{
			StatisticsEntry StatEntry = ((GameObject)Instantiate(EntryPrefab, transform.position, transform.rotation)).GetComponent<StatisticsEntry>();
			EntryCombination NewCombination = new EntryCombination(User, StatEntry);
			Entries.Add(NewCombination);
			StatEntry.GetComponent<RectTransform>().SetParent(transform, false);
			StatEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -Entries.Count * Properties.StatisticsEntrySpawnDistance);
			User.OnStatisticsUpdated += StatEntry.UpdateEntry;
			StatEntry.UpdateEntry(User);
		}
	}

	void OnDisable()
	{
		foreach (EntryCombination Entry in Entries) 
		{
			Entry.MyUserEntry.OnStatisticsUpdated -= Entry.MyStatisticsEntry.UpdateEntry;
			Destroy (Entry.MyStatisticsEntry.gameObject);
		}
		Entries.Clear ();
		WinGameMessage.SetActive (false);
	}

	public class EntryCombination
	{
		public GameController.UserEntry MyUserEntry;
		public StatisticsEntry MyStatisticsEntry;

		public EntryCombination(GameController.UserEntry userEntry, StatisticsEntry statisticsEntry)
		{
			MyUserEntry = userEntry;
			MyStatisticsEntry = statisticsEntry;
		}
	}
}
