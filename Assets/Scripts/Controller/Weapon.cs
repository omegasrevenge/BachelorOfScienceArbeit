using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	public const string WeaponModelFolder = "WeaponModels";
	public enum WeaponTypeEnum{Default, Rifle, MachineGun, Shotgun, Sniper, Bazooka, Length}
	public readonly string[] WeaponModelNames = new string[]{"DefaultModel", "RifleModel", "MachinegunModel", "ShotgunModel", "SniperModel", "BazookaModel"};

	public enum AmmunitionTypeEnum{Generic, Bouncy, Shrapnel, Explosive}
	public enum SecondaryEffectEnum{None, Healing, MoreRange, Delay}

	public WeaponTypeEnum WeaponType;
	public AmmunitionTypeEnum AmmunitionType;
	public SecondaryEffectEnum SecondaryEffect;

	public Transform Nozzle;
	public GameObject WeaponBlueprint;

	public float Accuracy;
	public int AmmunitionCount;
	public int Damage;

	public static void Create(WeaponTypeEnum WeaponType, AmmunitionTypeEnum AmmunitionType, SecondaryEffectEnum SecondaryEffect, Vector3 Position, Quaternion Rotation)
	{
		GameObject NewWeapon = (GameObject)Network.Instantiate (Resources.Load ("Weapon"), Position, Rotation, 1);
		NewWeapon.GetComponent<Weapon>().networkView.RPC ("Initialize", RPCMode.AllBuffered, 
		                                                  (int)WeaponType,
		                                                  (int)AmmunitionType,
		                                                  (int)SecondaryEffect
				);
	}

	[RPC]
	public void Initialize(int WeaponType, int AmmunitionType, int SecondaryEffect )
	{
		this.WeaponType = (WeaponTypeEnum)WeaponType;
		this.AmmunitionType  = (AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (SecondaryEffectEnum)SecondaryEffect;
		
		GameObject NewWeaponModel = (GameObject)Instantiate 
			(
			Resources.Load (WeaponModelFolder + "/" + WeaponModelNames [WeaponType]), 
			transform.position, 
			transform.rotation
			);

		//NewWeaponModel.transform.parent = NewWeapon.transform;
		//
		//WeaponScript.Nozzle = NewWeaponModel.transform.FindChild ("Nozzle");
	}

	public void Shoot()
	{

	}
}
