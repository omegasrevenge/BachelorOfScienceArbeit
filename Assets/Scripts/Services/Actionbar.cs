using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Actionbar : MonoBehaviour 
{
	public GameObject ActionbarPrefab;

	[HideInInspector]
	public List<Text> ActionBarEntries = new List<Text>();

	public void CreateEntry(int KillerID, int VictimID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		networkView.RPC ("RPCCreateEntry", RPCMode.AllBuffered, KillerID, VictimID, WeaponType, AmmunitionType, KilledByDirectHit);
	}

	[RPC]
	public void RPCCreateEntry(int KillerID, int VictimID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		foreach (Text entry in ActionBarEntries)
			entry.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (0f, Properties.ActionBarSpacing);

		GameObject _newEntry = (GameObject)Instantiate(ActionbarPrefab, 
		                                               ActionbarPrefab.GetComponent<RectTransform>().position, 
		                                               ActionbarPrefab.GetComponent<RectTransform>().rotation);

		_newEntry.GetComponent<RectTransform> ().SetParent (GetComponent<RectTransform> (), false);
		_newEntry.GetComponent<RectTransform> ().anchoredPosition = Properties.Singleton.ActionBarLineSpawnPos; 

		_newEntry.GetComponent<Text> ().text = 
						GameController.GetUserEntry (KillerID).UserName + 
						Properties.ActionBarSeparator + 
						(KilledByDirectHit ? Properties.Singleton.WeaponNames [WeaponType] : Properties.Singleton.AmmunitionNames [AmmunitionType]) + 
						Properties.ActionBarSeparator + 
						GameController.GetUserEntry (VictimID).UserName;

		ActionBarEntries.Add (_newEntry.GetComponent<Text>());

		StartCoroutine ("CDeleteActionBarEntry", _newEntry);
	}

	public IEnumerator CDeleteActionBarEntry(GameObject Entry)
	{
		yield return new WaitForSeconds(Properties.ActionBarEntryLifeTime);

		if(Entry != null)
		{
			ActionBarEntries.Remove (Entry.GetComponent<Text> ());
			Destroy (Entry);
		}
	}

	void OnDisable()
	{
		foreach (Text entry in ActionBarEntries)
						Destroy (entry.gameObject);
		ActionBarEntries.Clear ();
	}
}
