using UnityEngine;
using System.Collections;

public class CheckPlayerDeath : MonoBehaviour 
{
	public void StartCheck(PlayerController player)
	{
		StartCoroutine ("CCheckPlayerDeath", player);
	}

	public IEnumerator CCheckPlayerDeath(PlayerController player)
	{
		yield return new WaitForSeconds (Properties.DyingAnimationLength + 1f);

		if (player != null)
			player.DestroyPlayer ();

		Destroy (gameObject);
	}
}