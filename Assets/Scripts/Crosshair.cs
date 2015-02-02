using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Crosshair : MonoBehaviour 
{
	public Image CrosshairLeft;
	public Image CrosshairRight;
	public Image CrosshairUp;
	public Image CrosshairDown;
	public Weapon MyWeapon;

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

		CrosshairLeft.rectTransform.localPosition = new Vector3 (-_xOffset, 0f, 0f);
		CrosshairRight.rectTransform.localPosition = new Vector3 (_xOffset, 0f, 0f);
		CrosshairUp.rectTransform.localPosition = new Vector3 (0f, _yOffset, 0f);
		CrosshairDown.rectTransform.localPosition = new Vector3 (0f, -_yOffset, 0f);
	}
}
