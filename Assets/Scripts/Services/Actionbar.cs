using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Actionbar : MonoBehaviour 
{
	public GameObject ActionbarPrefab;

	[HideInInspector]
	public List<Text> ActionBarEntries = new List<Text>();

	public void CreateEntry(int killerID, int victimID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		networkView.RPC ("RPCCreateEntry", RPCMode.AllBuffered, killerID, victimID, weaponType, ammunitionType, killedByDirectHit);
	}

	[RPC]
	public void RPCCreateEntry(int killerID, int victimID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		foreach (Text Entry in ActionBarEntries)
			Entry.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (0f, Properties.ActionBarSpacing);

		GameObject NewEntry = (GameObject)Instantiate(ActionbarPrefab, 
		                                               ActionbarPrefab.GetComponent<RectTransform>().position, 
		                                               ActionbarPrefab.GetComponent<RectTransform>().rotation);

		NewEntry.GetComponent<RectTransform> ().SetParent (GetComponent<RectTransform> (), false);
		NewEntry.GetComponent<RectTransform> ().anchoredPosition = Properties.Singleton.ActionBarLineSpawnPos; 

		NewEntry.GetComponent<Text> ().text = 
						GameController.GetUserEntry (killerID).UserName + 
						Properties.ActionBarSeparator + 
						(killedByDirectHit ? Properties.Singleton.WeaponNames [weaponType] : Properties.Singleton.AmmunitionNames [ammunitionType]) + 
						Properties.ActionBarSeparator + 
						GameController.GetUserEntry (victimID).UserName;

		ActionBarEntries.Add (NewEntry.GetComponent<Text>());

		StartCoroutine ("CDeleteActionBarEntry", NewEntry);
	}

	public IEnumerator CDeleteActionBarEntry(GameObject entry)
	{
		yield return new WaitForSeconds(Properties.ActionBarEntryLifeTime);

		if(entry != null)
		{
			ActionBarEntries.Remove (entry.GetComponent<Text> ());
			Destroy (entry);
		}
	}

	void OnDisable()
	{
		foreach (Text Entry in ActionBarEntries)
						Destroy (Entry.gameObject);
		ActionBarEntries.Clear ();
	}
}
