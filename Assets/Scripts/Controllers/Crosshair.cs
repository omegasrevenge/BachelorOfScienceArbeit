using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Crosshair : MonoBehaviour 
{
	public Image CrosshairLeft;
	public Image CrosshairRight;
	public Image CrosshairUp;
	public Image CrosshairDown;
	public WeaponController MyWeapon;

	private float XOffset = 0f;
	private float YOffset = 0f;

	void Update () 
	{
		if (MyWeapon != null) 
		{
			XOffset = MyWeapon.CurrentAccuracyDecay * Properties.Singleton.UIDimensions.x / 2f;
			YOffset = MyWeapon.CurrentAccuracyDecay * Properties.Singleton.UIDimensions.y / 2f;
		} 
		else 
		{
			XOffset = 0f;
			YOffset = 0f;
		}
		
		CrosshairLeft.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairLeft.rectTransform.localPosition, new Vector3 (-XOffset, 0f, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairRight.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairRight.rectTransform.localPosition, new Vector3 (XOffset, 0f, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairUp.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairUp.rectTransform.localPosition, new Vector3 (0f, YOffset, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairDown.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairDown.rectTransform.localPosition, new Vector3 (0f, -YOffset, 0f), 1 / Properties.CrosshairLerpSpeed);
	}
}
