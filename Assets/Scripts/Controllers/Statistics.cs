using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Statistics : MonoBehaviour 
{
	public GameObject EntryPrefab;
	public GameObject WinGameMessage;
	public List<EntryCombination> Entries = new List<EntryCombination>();

	public void DisplayWinScreen(string UserName)
	{
		gameObject.SetActive (true);
		WinGameMessage.SetActive (true);
		Text _myWinText = WinGameMessage.transform.FindChild ("Text").GetComponent<Text> ();
		_myWinText.text = _myWinText.text.Replace ("$", UserName);
	}

	void OnEnable()
	{
		Entries.Clear ();
		foreach (GameController.UserEntry user in GameController.Singleton.Users) 
		{
			StatisticsEntry _statEntry = ((GameObject)Instantiate(EntryPrefab, transform.position, transform.rotation)).GetComponent<StatisticsEntry>();
			EntryCombination _newCombination = new EntryCombination(user, _statEntry);
			Entries.Add(_newCombination);
			_statEntry.GetComponent<RectTransform>().SetParent(transform, false);
			_statEntry.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -Entries.Count * Properties.StatisticsEntrySpawnDistance);
			user.OnStatisticsUpdated += _statEntry.UpdateEntry;
			_statEntry.UpdateEntry(user);
		}
	}

	void OnDisable()
	{
		foreach (EntryCombination entry in Entries) 
		{
			entry.MyUserEntry.OnStatisticsUpdated -= entry.MyStatisticsEntry.UpdateEntry;
			Destroy (entry.MyStatisticsEntry.gameObject);
		}
		Entries.Clear ();
		WinGameMessage.SetActive (false);
	}

	public class EntryCombination
	{
		public GameController.UserEntry MyUserEntry;
		public StatisticsEntry MyStatisticsEntry;

		public EntryCombination(GameController.UserEntry UserEntry, StatisticsEntry StatisticsEntry)
		{
			MyUserEntry = UserEntry;
			MyStatisticsEntry = StatisticsEntry;
		}
	}
}
