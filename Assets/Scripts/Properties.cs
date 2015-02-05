using UnityEngine;
using System.Collections;

public class Properties : MonoBehaviour 
{
	public static Properties Singleton;

	void Start()
	{
		Singleton = this;
	}

	// PLAYER //
	public const int MaxPlayerHealth = 100;
	public const float DyingAnimationLength = 3f;
	public const float RespawnTimer = 2f;
	public const int AvatarLayer = 12;
	public const float RecollorDurationAfterHit = 0.1f;
	////////////

	// SOUND MANAGER //
	public const string SoundsFolderName = "Sounds";
	public const float FrequencyPlayerHitSoundsAllowed = 1f; //How often can PlayerHit be played. 1f = once per second max.
	
	public enum SoundsEnum
	{
		Gun,
		Rifle,
		Machinegun,
		Shotgun,
		Sniper,
		Bazooka,
		Shrapnel,
		Explosion,
		Bouncy,
		Button,
		PlayerHit
	}

	public readonly string[] SoundFileNames = new string[]
	{
		"Gun_Sound",
		"Rifle_Sound",
		"Machinegun_Sound",
		"Shotgun_Sound",
		"Sniper_Sound",
		"Bazooka_Sound",
		"Shrapnel_Sound",
		"Explosion_Sound",
		"Bouncy_Sound",
		"Button_Sound",
		"PlayerHit_Sound"
	};

	public readonly float[] SoundDefaultVolumes = new float[]
	{
		0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f, 1f, 1f, 0.5f
	};
	
	public readonly float[] SoundDefaultMinDistances = new float[]
	{
		80f, 80f, 80f, 80f, 80f, 150f, 80f, 80f, 80f, 1000f, 50f
	};
	
	public readonly float[] SoundDefaultMaxDistances = new float[]
	{
		400f, 400f, 400f, 400f, 400f, 900f, 250f, 400f, 200f, 1000f, 150f
	};
	///////////////////

	// WEAPON //
	public const string WeaponModelFolder = "WeaponModels";
	public enum WeaponTypeEnum{Default, Rifle, MachineGun, Shotgun, Sniper, Bazooka, Length}
	public readonly string[] WeaponModelNames = new string[]{"DefaultWeaponModel", "RifleModel", "MachinegunModel", "ShotgunModel", "SniperModel", "BazookaModel"};
	public readonly int[] CameraRecoilOnShot = new int[]{1, 2, 1, 3, 1, 3};
	public readonly float[] WeaponRecoilBackwardsOnShot = new float[]{1f, 2f, 1f, 4f, 6f, 20f};
	public readonly float[] WeaponRightingAfterShotSpeed = new float[]{3f, 2f, 4f, 1f, 1f, 0.33f};
	public enum ShootingMode{Default, Cone} //default means it shoots one bullet forwards, cone means it shoots several bullets in slightly different angles (e.g. shotgun)
	public readonly ShootingMode[] WeaponShootingModes = new ShootingMode[]{ShootingMode.Default, ShootingMode.Default, ShootingMode.Default, ShootingMode.Cone, ShootingMode.Default, ShootingMode.Default};
	public const float AccuracyMaxDecay = 0.3f;
	public readonly float[] AccuracyDecay = new float[]{0.03f, 0.05f, 0.0025f, 0.2f, 0.1f, 0.175f};
	public readonly float[] WeaponTargetingSpeed = new float[]{0.2f, 0.3f, 0.2f, 0.2f, 0.5f, 2f}; //how much accuracy is restored per second (in percent of screen)
	public readonly Color[] WeaponColors = new Color[]{Color.white, Color.green, Color.blue, Color.grey}; //Depends on SecondaryEffect
	public readonly int[] WeaponAmmunitionAmount = new int[]{0, 20, 50, 10, 10, 5};
	////////////

	// BULLET //
	public enum AmmunitionTypeEnum{Generic, Bouncy, Shrapnel, Explosive, Length}
	public enum SecondaryEffectEnum{None, Healing, Heavy, Delay, Length}
	public const string BulletModelFolder = "BulletModels";
	public const float BulletAbleToHitDelay = 0.1f;
	public readonly string[] BulletModelNames = new string[]{"DefaultBulletModel", "RifleBulletModel", "MachinegunBulletModel", "ShotgunBulletModel", "SniperBulletModel", "BazookaBulletModel"};
	public readonly int[] BulletDamage = new int[]{10, 25, 10, 12, 35, 50};
	public readonly float[] WeaponAttackSpeedValues = new float[]{0f, 0f, 0.1f, 1f, 1f, 3f};
	public readonly float[] BulletFlyingSpeed = new float[]{1000f, 2400f, 2000f, 1200f, 1500f, 800f};
	public readonly bool[] BulletsUseGravity = new bool[]{false, false, false, false, false, true};
	public readonly int[] BulletMass = new int[]{1, 1, 1, 1, 1, 2};
	public readonly float[] BulletDefaultLifetime = new float[]{5f, 3f, 2f, 1f, 5f, 5f};
	public readonly float[] BulletAmmunitionTypeLifetimeModifier = new float[]{1f, 20f, 0.75f, 0.5f};
	public readonly float[] BulletSecondaryEffectLifetimeModifier = new float[]{1f, 1f, 2f, 1f};
	public readonly Color[] BulletColors = new Color[]{Color.white, Color.cyan, Color.black, Color.red}; //Depends on AmmunitionType
	public readonly float[] ShrapnelSize = new float[]{0f, 2f, 1f, 0f, 3f, 0f};
	public readonly float[] ShrapnelLifeTime = new float[]{0f, 0.3f, 0.15f, 0f, 0.75f, 0f};
	public readonly float[] ShrapnelDamage = new float[]{0f, 0.2f, 0.2f, 0f, 0.3f, 0f}; //Multiplied by Bullet Damage at that time
	public const float ShrapnelEffectBulletDamageMultiplier = 0.75f;
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
	public const float PlatformWeaponRotationSpeed = 100f;
	///////////////////////////

	// GAME CONTROLLER //
	public enum GameState{Menu, Lobby, InGame, GameOver}
	public const string GameType = "BachelorOfScience.SalzmannKirill.MDH2015";
	public const int Port = 43654;
	public const string GameName = "BachelorOfScienceSalzmannKirill";
	public const float RequestHostTimeoutLength = 30f;
	public const float GameStartTimer = 5f;
	public const int KillsToWin = 30;
	/////////////////////

	// UI //
	public const float CrosshairLerpSpeed = 0.2f;
	public readonly Vector2 UIDimensions = new Vector2 (640f, 480f);
	public const string DefaultNickname = "User123";
	public readonly Vector2 ActionBarLineSpawnPos = new Vector2 (10f, 70f);
	public const int ActionBarSpacing = 20;
	public const string ActionBarSeparator = " > ";
	public const float ActionBarEntryLifeTime = 10f;
	public const string CurrentWeaponSeparator = ", ";
	public readonly string[] WeaponNames = new string[]{"Gun", "Rifle", "Machinegun", "Shotgun", "Sniper", "Grenade Launcher"};
	public readonly string[] AmmunitionNames = new string[]{"Default", "Bouncy", "Shrapnel", "Explosive"};
	public readonly string[] SecondaryEffectNames = new string[]{"Generic", "Healing", "Heavy Bullets", "Delayed Shots"};
	public const float StatisticsEntrySpawnDistance = 30f;
	////////

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
