using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDCurrentWeapon : MonoBehaviour 
{
	public Text WeaponText;

	public void ShowWeapon(Weapon Target)
	{
		WeaponText.text = 
				Properties.Singleton.WeaponNames [(int)Target.WeaponType] +
				Properties.CurrentWeaponSeparator +
				Properties.Singleton.AmmunitionNames [(int)Target.AmmunitionType] +
				Properties.CurrentWeaponSeparator +
				Properties.Singleton.SecondaryEffectNames [(int)Target.SecondaryEffect];
			
	}
}
