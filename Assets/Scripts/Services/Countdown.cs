using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour 
{
	public delegate void OnCountdownOver();
	public OnCountdownOver CountdownOverEvent;

	public Text MyText;

	public static void CountDownFrom(float targetTime)
	{
		HUDController.Singleton.MyCountdown.gameObject.SetActive (true);
		HUDController.Singleton.MyCountdown.StartCoroutine ("CCountDown", targetTime);
	}

	public IEnumerator CCountDown(float targetTime)
	{
		while (targetTime >= 0f) 
		{
			MyText.text = targetTime.ToString("0.0");
			yield return new WaitForEndOfFrame();
			targetTime -= Time.deltaTime;
		}

		gameObject.SetActive (false);

		if (CountdownOverEvent != null)
			CountdownOverEvent ();
	}
}
