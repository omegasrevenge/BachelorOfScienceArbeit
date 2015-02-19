using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class PlayerController : MonoBehaviour 
{
	[HideInInspector]
	public GameController MyGameController;
	[HideInInspector]
	public Transform Lead;
	[HideInInspector]
	public Camera MyCamera;
	[HideInInspector]
	public MeshRenderer[] PlayerRenderer;

	public bool PlayingHitSound = false;

	public WeaponController MyWeapon
	{
		get
		{
			if(MyWeaponInstance == null)
			{
				foreach(GameObject weapon in GameController.Singleton.Weapons)
				{
					if(weapon.networkView.owner == networkView.owner)
					{
						MyWeaponInstance = weapon.GetComponent<WeaponController>();
						break;
					}
				}
			}
			return MyWeaponInstance;
		}
		set
		{
			MyWeaponInstance = value;
		}
	}

	private WeaponController MyWeaponInstance;

	public float CurrentRecollorDuration = 0f;
	
	public int Health;

	public bool Dead = false;

	public Transform WeaponAnchor;

	private bool GotHit = false;
	private bool GotRecollored = false;
	private Color MyHitColor;

	void Start()
	{
		Health = Properties.MaxPlayerHealth;
		PlayerRenderer = gameObject.GetComponentsInChildren<MeshRenderer> ();

		MyGameController = GameController.Singleton;

		MyGameController.Players.Add (gameObject);

		if (!networkView.isMine) return;

		MyGameController.MyPlayer = gameObject;
		HUDController.Singleton.HealthCounter.text = Health.ToString ();
		InitializePlayer ();
	}

	void Update()
	{
		if (GotHit) 
		{
			GotHit = false;
			CurrentRecollorDuration = Properties.RecollorDurationAfterHit;
			foreach(MeshRenderer Rend in PlayerRenderer)
				Rend.material.color = MyHitColor;
			GotRecollored = true;
		}

		CurrentRecollorDuration -= Time.deltaTime;

		if (GotRecollored && CurrentRecollorDuration <= 0f) 
		{
			GotRecollored = false;
			foreach(MeshRenderer Rend in PlayerRenderer)
				Rend.material.color = Color.white;
		}
	}

	public void InitializePlayer()
	{
		Lead = ((GameObject)Instantiate (
			Resources.Load<GameObject> ("CharacterLead"), 
			transform.position, 
			transform.rotation)
		        ).transform;

		GetComponent<AlwaysFollow> ().enabled = true;
		GetComponent<AlwaysFollow> ().Target = Lead;

		transform.FindChild ("Face").gameObject.SetActive (false);


		MyCamera = transform.FindChild ("Camera").GetComponent<Camera> ();
		MyCamera.gameObject.SetActive (true);

		MyWeapon = ((GameObject)Network.Instantiate (Resources.Load ("Weapon"), WeaponAnchor.position, WeaponAnchor.rotation, 1)).GetComponent<WeaponController>();
		MyWeapon.transform.parent = WeaponAnchor;

		if(GameController.GetUserEntry(networkView.owner).Deaths > 0)
		{
			int WeaponType = Random.Range(1, ((int)Properties.WeaponType.Length));
			int AmmunitionType = (int)WeaponController.ChooseAmmunitionType((Properties.WeaponType)WeaponType);
			int SecondaryEffect = (int)WeaponController.ChooseSecondaryEffect((Properties.WeaponType)WeaponType, (Properties.AmmunitionType)AmmunitionType);
			
			MyWeapon.PickupNew(WeaponType, AmmunitionType, SecondaryEffect);
		}
		else
			MyWeapon.PickupDefault ();

	}

	public void GetHit(int damage, int sourceID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, damage, sourceID, weaponType, ammunitionType, killedByDirectHit);
	}
	
	public void GetHit(int damage, int sourceID, int ammunitionType)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, damage, sourceID, 0, ammunitionType, false);
	}
	
	public void GetHit(int damage)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, damage, GameController.GetUserEntry(networkView.owner).ID, 0, 0, false);
	}

	[RPC]
	public void RPCGetHit(int damage, int sourceID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		GotHit = true;
		MyHitColor = damage < 0f ? Properties.Singleton.ColorWhenHealed : Properties.Singleton.ColorWhenHit;
		Health = Mathf.Clamp (Health - damage, 0, Properties.MaxPlayerHealth);

		if (!PlayingHitSound)
						StartCoroutine ("CPlayHitSound");

		if (!networkView.isMine)
						return;

		if (Health == 0) Die (sourceID, weaponType, ammunitionType, killedByDirectHit);

		HUDController.Singleton.HealthCounter.text = Health.ToString ();
	}

	public IEnumerator CPlayHitSound()
	{
		PlayingHitSound = true;

		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int) Properties.Sounds.PlayerHit), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int) Properties.Sounds.PlayerHit],
			Properties.Singleton.SoundDefaultMinDistances [(int) Properties.Sounds.PlayerHit],
			Properties.Singleton.SoundDefaultMaxDistances [(int) Properties.Sounds.PlayerHit]
			);

		yield return new WaitForSeconds (Properties.FrequencyPlayerHitSoundsAllowed);

		PlayingHitSound = false;
	}

	public void Die(int killerID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		if(!Dead)
			networkView.RPC ("RPCDie", 
			                 RPCMode.AllBuffered, 
			                 killerID, 
			                 GameController.GetUserEntry(networkView.owner).ID, 
			                 weaponType, 
			                 ammunitionType, 
			                 killedByDirectHit);
	}

	[RPC]
	public void RPCDie(int killerID, int victimID, int weaponType, int ammunitionType, bool killedByDirectHit)
	{
		if (Dead) return;
		Dead = true;

		
		GameController.UserEntry Killer = GameController.GetUserEntry (killerID);
		GameController.UserEntry Victim = GameController.GetUserEntry (victimID);

		Killer.UpdateStatistics (Killer.Kills + 1, Killer.Deaths);
		Victim.UpdateStatistics (Victim.Kills, Victim.Deaths + 1);

		foreach (Collider Col in gameObject.GetComponentsInChildren<Collider>())
			Col.enabled = false;
		StartCoroutine ("CDeath");
		
		if (networkView.isMine) 
		{
			HUDController.Singleton.MyActionBar.CreateEntry (killerID, victimID, weaponType, ammunitionType, killedByDirectHit);

			GameObject MyCheckPlayerDeath = new GameObject ("DebugPlayerNotDying");
			MyCheckPlayerDeath.AddComponent<CheckPlayerDeath> ().StartCheck (this);
		}
	}

	public IEnumerator CDeath()
	{
		if (networkView.isMine) 
		{
			GetComponent<AlwaysFollow> ().enabled = false;
			Destroy(Lead.gameObject);
		}

		float DyingTime = Mathf.Max (Properties.DyingAnimationLength, 0.001f); //guarantee this cannot be zero.

		float CurDyingTime = DyingTime;
	
		foreach (GameObject Weapon in MyGameController.Weapons) 
		{
			if(Weapon.networkView.owner == networkView.owner)
			{
				MyWeapon = Weapon.GetComponent<WeaponController>();
				break;
			}
		}
		
		if(MyWeapon == null)
		{
			WeaponController[] Weapons = GameObject.FindObjectsOfType<WeaponController>();
			foreach(WeaponController Weapon in Weapons)
				if(Weapon.networkView.owner == networkView.owner)
					{
						MyWeapon = Weapon.GetComponent<WeaponController>();
						break;
					}
		}

		MeshRenderer[] WeaponRenderers = MyWeapon.gameObject.GetComponentsInChildren<MeshRenderer> ();

		Projector Shadow = gameObject.GetComponentInChildren<Projector> ();
		float ShadowMaxStrength = Shadow.farClipPlane;

		while ((CurDyingTime / DyingTime) > 0.01f) 
		{
			yield return new WaitForEndOfFrame();

			CurDyingTime -= Time.deltaTime;

			foreach(MeshRenderer rend in PlayerRenderer)
			{
				Color Col = rend.material.color;
				rend.material.color = new Color(Col.r, Col.g, Col.b, CurDyingTime / DyingTime);
			}
			
			foreach(MeshRenderer Rend in WeaponRenderers)
			{
				Color Col = Rend.material.color;
				Rend.material.color = new Color(Col.r, Col.g, Col.b, CurDyingTime / DyingTime);
			}

			Shadow.farClipPlane = ShadowMaxStrength * (CurDyingTime / DyingTime);
		}

		if (networkView.isMine) 
			DestroyPlayer();
	}
	/// <summary>
	/// Dirty Bugfix. Sometimes CDeath() stops in the middle of execution. This is meant to fix it, along with the CheckPlayerDeath class.
	/// </summary>
	public void DestroyPlayer()
	{
		MyCamera.transform.parent = null;
		MyCamera.GetComponent<MouseLook>().enabled = false;
		
		MyGameController.DestroyCamera = true;
		MyGameController.Respawn(Properties.RespawnTimer);
		Network.Destroy (MyWeapon.networkView.viewID);
		Network.Destroy (networkView.viewID);
	}
	
	public void OnDestroy()
	{
		GameController.Singleton.Players.Remove (gameObject);
	}
}
