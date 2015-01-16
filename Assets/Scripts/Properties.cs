using UnityEngine;
using System.Collections;

public class Properties : MonoBehaviour 
{
	// WEAPON //
	public const string WeaponModelFolder = "WeaponModels";
	public enum WeaponTypeEnum{Default, Rifle, MachineGun, Shotgun, Sniper, Bazooka, Length}
	public readonly string[] WeaponModelNames = new string[]{"DefaultWeaponModel", "RifleModel", "MachinegunModel", "ShotgunModel", "SniperModel", "BazookaModel"};
	public enum AmmunitionTypeEnum{Generic, Bouncy, Shrapnel, Explosive, Length}
	public enum SecondaryEffectEnum{None, Healing, MoreRange, Delay, Length}
	////////////

	// WEAPON SPAWN PLATFORM //
	public const float WeaponSpawnTime = 10f;
	public float RotationSpeed = 100f;
	///////////////////////////

	// GAME CONTROLLER //
	public enum GameState{Menu, InGame}
	public enum PlayerGameStatus{Host, Player}
	public const string GameType = "BachelorOfScience.SalzmannKirill.MDH2015";
	public const int Port = 43654;
	public const string GameName = "BachelorOfScienceSalzmannKirill";
	/////////////////////
}
