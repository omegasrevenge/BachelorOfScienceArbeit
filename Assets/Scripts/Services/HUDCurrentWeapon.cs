using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDCurrentWeapon : MonoBehaviour 
{
	public Text WeaponText;

	public void ShowWeapon(WeaponController target)
	{
		WeaponText.text = 
				Properties.Singleton.WeaponNames [(int)target.WeaponType] +
				Properties.CurrentWeaponSeparator +
				Properties.Singleton.AmmunitionNames [(int)target.AmmunitionType] +
				Properties.CurrentWeaponSeparator +
				Properties.Singleton.SecondaryEffectNames [(int)target.SecondaryEffect];
			
	}
}
