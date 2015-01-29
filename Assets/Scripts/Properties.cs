using UnityEngine;
using System.Collections;

public class Properties : MonoBehaviour 
{
	public bool UpdateNow = false;

	void Update()
	{
		if (UpdateNow) 
		{
			UpdateNow = false;
			networkView.RPC("UpdateValues", RPCMode.AllBuffered, WeaponAttackSpeedValues, BulletFlyingSpeed);
		}
	}

	[RPC]
	public void UpdateValues(float[] val1, float[] val2)
	{
		//WeaponAttackSpeedValues = val1;
		//BulletFlyingSpeed = val2;
		//Debug.Log ("UPDATED VALUES!");
	}

	// PLAYER //
	public const int MaxPlayerHealth = 100;
	public const float DyingAnimationLength = 3f;
	public const float RespawnTimer = 2f;
	public const int AvatarLayer = 12;
	////////////

	// WEAPON //
	public const string WeaponModelFolder = "WeaponModels";
	public const string BulletModelFolder = "BulletModels";
	public enum WeaponTypeEnum{Default, Rifle, MachineGun, Shotgun, Sniper, Bazooka, Length}
	public readonly string[] WeaponModelNames = new string[]{"DefaultWeaponModel", "RifleModel", "MachinegunModel", "ShotgunModel", "SniperModel", "BazookaModel"};
	public readonly string[] BulletModelNames = new string[]{"DefaultBulletModel", "RifleBulletModel", "MachinegunBulletModel", "ShotgunBulletModel", "SniperBulletModel", "BazookaBulletModel"};
	public readonly float[] WeaponAttackSpeedValues = new float[]{0f, 0f, 0.1f, 1.5f, 1f, 3f};
	public readonly float[] BulletFlyingSpeed = new float[]{400, 1200f, 1000f, 800f, 1500f, 400f};
	public readonly bool[] BulletsUseGravity = new bool[]{false, false, false, false, false, true};
	public readonly int[] BulletMass = new int[]{1, 1, 1, 1, 1, 2};
	public enum AmmunitionTypeEnum{Generic, Bouncy, Shrapnel, Explosive, Length}
	public enum SecondaryEffectEnum{None, Healing, MoreRange, Delay, Length}
	////////////

	// WEAPON SPAWN PLATFORM //
	public const float WeaponSpawnTime = 10f;
	public float RotationSpeed = 100f;
	///////////////////////////

	// GAME CONTROLLER //
	public enum GameState{Menu, Lobby, InGame}
	public const string GameType = "BachelorOfScience.SalzmannKirill.MDH2015";
	public const int Port = 43654;
	public const string GameName = "BachelorOfScienceSalzmannKirill";
	public const float RequestHostTimeoutLength = 10f;
	public const float GameStartTimer = 5f;
	/////////////////////
}
