using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour 
{
	public delegate void OnCountdownOver();
	public OnCountdownOver CountdownOverEvent;

	public Text MyText;

	public static void EveryoneCountDownFrom(float TargetTime)
	{
		HUDController.Singleton.EveryoneCountdown (TargetTime); //HUDController, because we need to use a networkView, and this gameObject cannot have one
	}

	public static void CountDownFrom(float TargetTime)
	{
		HUDController.Singleton.MyCountdown.gameObject.SetActive (true);
		HUDController.Singleton.MyCountdown.StartCoroutine ("CCountDown", TargetTime);
	}

	public IEnumerator CCountDown(float TargetTime)
	{
		while (TargetTime >= 0f) 
		{
			MyText.text = TargetTime.ToString("0.0");
			yield return new WaitForEndOfFrame();
			TargetTime -= Time.deltaTime;
		}

		gameObject.SetActive (false);

		if (CountdownOverEvent != null)
			CountdownOverEvent ();
	}
}
