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

	private float _xOffset = 0f;
	private float _yOffset = 0f;

	void Update () 
	{
		if (MyWeapon != null) 
		{
			_xOffset = MyWeapon.CurrentAccuracyDecay * Properties.Singleton.UIDimensions.x / 2f;
			_yOffset = MyWeapon.CurrentAccuracyDecay * Properties.Singleton.UIDimensions.y / 2f;
		} 
		else 
		{
			_xOffset = 0f;
			_yOffset = 0f;
		}
		
		CrosshairLeft.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairLeft.rectTransform.localPosition, new Vector3 (-_xOffset, 0f, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairRight.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairRight.rectTransform.localPosition, new Vector3 (_xOffset, 0f, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairUp.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairUp.rectTransform.localPosition, new Vector3 (0f, _yOffset, 0f), 1 / Properties.CrosshairLerpSpeed);
		CrosshairDown.rectTransform.localPosition = 
			Vector3.Lerp(CrosshairDown.rectTransform.localPosition, new Vector3 (0f, -_yOffset, 0f), 1 / Properties.CrosshairLerpSpeed);
	}
}
