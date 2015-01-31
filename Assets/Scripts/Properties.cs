using UnityEngine;
using System.Collections;

public class Properties : MonoBehaviour 
{
	public static Properties Singleton;

	public bool UpdateNow = false;

	void Start()
	{
		Singleton = this;
	}

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
	public const float RecollorDurationAfterHit = 0.1f;
	////////////

	// WEAPON //
	public const string WeaponModelFolder = "WeaponModels";
	public enum WeaponTypeEnum{Default, Rifle, MachineGun, Shotgun, Sniper, Bazooka, Length}
	public readonly string[] WeaponModelNames = new string[]{"DefaultWeaponModel", "RifleModel", "MachinegunModel", "ShotgunModel", "SniperModel", "BazookaModel"};
	public readonly int[] CameraRecoilOnShot = new int[]{1, 2, 1, 3, 1, 3};
	public readonly float[] WeaponRecoilBackwardsOnShot = new float[]{1f, 2f, 1f, 4f, 6f, 20f};
	public readonly float[] WeaponRightingAfterShotSpeed = new float[]{3f, 2f, 4f, 1f, 1f, 0.33f};
	public enum ShootingMode{Default, Cone} //default means it shoots one bullet forwards, cone means it shoots several bullets in slightly different angles (e.g. shotgun)
	public readonly ShootingMode[] WeaponShootingModes = new ShootingMode[]{ShootingMode.Default, ShootingMode.Default, ShootingMode.Default, ShootingMode.Cone, ShootingMode.Default, ShootingMode.Default};
	public const float AccuracyMaxDecay = 0.4f;
	public readonly float[] AccuracyDecay = new float[]{0.03f, 0.1f, 0.005f, 0.2f, 0.1f, 0.175f};
	public readonly float[] WeaponTargetingSpeed = new float[]{0.2f, 0.3f, 0.2f, 0.2f, 0.5f, 2f}; //how much accuracy is restored per second (in percent of screen)
	public readonly Color[] WeaponColors = new Color[]{Color.white, Color.green, Color.blue, Color.grey}; //Depends on SecondaryEffect
	////////////

	// BULLET //
	public enum AmmunitionTypeEnum{Generic, Bouncy, Shrapnel, Explosive, Length}
	public enum SecondaryEffectEnum{None, Healing, Heavy, Delay, Length}
	public const string BulletModelFolder = "BulletModels";
	public readonly string[] BulletModelNames = new string[]{"DefaultBulletModel", "RifleBulletModel", "MachinegunBulletModel", "ShotgunBulletModel", "SniperBulletModel", "BazookaBulletModel"};
	public readonly int[] BulletDamage = new int[]{5, 10, 3, 3, 15, 30};
	public readonly float[] WeaponAttackSpeedValues = new float[]{0f, 0f, 0.1f, 1.5f, 1f, 3f};
	public readonly float[] BulletFlyingSpeed = new float[]{800, 1200f, 1000f, 800f, 1500f, 600f};
	public readonly bool[] BulletsUseGravity = new bool[]{false, false, false, false, false, true};
	public readonly int[] BulletMass = new int[]{1, 1, 1, 1, 1, 2};
	public readonly float[] BulletDefaultLifetime = new float[]{5f, 3f, 2f, 1f, 5f, 5f};
	public readonly float[] BulletAmmunitionTypeLifetimeModifier = new float[]{1f, 2f, 0.75f, 0.5f};
	public readonly float[] BulletSecondaryEffectLifetimeModifier = new float[]{1f, 1f, 2f, 1f};
	public readonly Color[] BulletColors = new Color[]{Color.white, Color.cyan, Color.black, Color.red}; //Depends on AmmunitionType
	public readonly float[] ShrapnelSize = new float[]{0f, 2f, 1f, 0f, 3f, 0f};
	public readonly float[] ShrapnelLifeTime = new float[]{0f, 0.2f, 0.1f, 0f, 0.5f, 0f};
	public readonly float[] ShrapnelDamage = new float[]{0f, 0.5f, 0.5f, 0f, 0.8f, 0f}; //Multiplied by Bullet Damage at that time
	public const float ShrapnelEffectBulletDamageMultiplier = 0.5f;
	public const float ShrapnelFlyingSpeed = 1500f;
	public readonly int[] BouncyMaxBounceCount = new int[]{0, 5, 3, 2, 10, 0};
	public readonly float[] HeavyEffectSpeedMultiplier = new float[]{0f, 0.5f, 0.5f, 0.5f, 0.5f, 0f};
	public const float HeavyEffectDamageMultiplier = 2f;
	public readonly float[] DelayEffectDuration = new float[]{0f, 1f, 1f, 1f, 0f, 0f};
	public const float DelayEffectDamageMultiplier = 2f;
	public readonly float[] ExplosionDamage = new float[]{0f, 0.5f, 0.5f, 0f, 0.5f, 1f}; //Multiplied by Bullet Damage at that time
	public const float ExplosionEffectBulletDamageMultiplier = 0.5f;
	public const float ExplosionLifeTime = 0.5f;
	public readonly float[] ExplosionEndSize = new float[]{0f, 50f, 30f, 0f, 50f, 30f}; //How many times bigger than the bullet
	public readonly AllowedWeaponEffectCombinations[] WeaponRestrictions = new AllowedWeaponEffectCombinations[]
	{
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.Default, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Generic}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None}
		),
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.Rifle, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Generic, AmmunitionTypeEnum.Bouncy, AmmunitionTypeEnum.Shrapnel, AmmunitionTypeEnum.Explosive}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.MachineGun, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Generic, AmmunitionTypeEnum.Bouncy, AmmunitionTypeEnum.Shrapnel, AmmunitionTypeEnum.Explosive}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.Shotgun, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Generic, AmmunitionTypeEnum.Bouncy}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.Sniper, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Generic, AmmunitionTypeEnum.Bouncy, AmmunitionTypeEnum.Shrapnel, AmmunitionTypeEnum.Explosive}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing}
		),
		new AllowedWeaponEffectCombinations(
			WeaponTypeEnum.Bazooka, 
			new AmmunitionTypeEnum[]{AmmunitionTypeEnum.Explosive}, 
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing}
		)
	};

	public readonly AllowedSecondaryEffects[] AmmunitionRestrictions = new AllowedSecondaryEffects[]
	{
		new AllowedSecondaryEffects(
			AmmunitionTypeEnum.Generic,
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedSecondaryEffects(
			AmmunitionTypeEnum.Bouncy,
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedSecondaryEffects(
			AmmunitionTypeEnum.Shrapnel,
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		),
		new AllowedSecondaryEffects(
			AmmunitionTypeEnum.Explosive,
			new SecondaryEffectEnum[]{SecondaryEffectEnum.None, SecondaryEffectEnum.Healing, SecondaryEffectEnum.Heavy, SecondaryEffectEnum.Delay}
		)
	};
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

	public class AllowedWeaponEffectCombinations
	{
		public WeaponTypeEnum WeaponType;
		public AmmunitionTypeEnum[] AmmunitionTypes;
		public SecondaryEffectEnum[] SecondaryEffects;

		public AllowedWeaponEffectCombinations(WeaponTypeEnum WeaponType, AmmunitionTypeEnum[] AmmunitionTypes, SecondaryEffectEnum[] SecondaryEffects)
		{
			this.WeaponType = WeaponType;
			this.AmmunitionTypes = AmmunitionTypes;
			this.SecondaryEffects = SecondaryEffects;
		}
	}

	public class AllowedSecondaryEffects
	{
		public AmmunitionTypeEnum AmmunitionType;
		public SecondaryEffectEnum[] SecondaryEffects;
		
		public AllowedSecondaryEffects(AmmunitionTypeEnum AmmunitionType, SecondaryEffectEnum[] SecondaryEffects)
		{
			this.AmmunitionType = AmmunitionType;
			this.SecondaryEffects = SecondaryEffects;
		}
	}

}
