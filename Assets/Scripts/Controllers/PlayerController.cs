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

	public Weapon MyWeapon
	{
		get
		{
			if(_myWeapon == null)
			{
				foreach(GameObject weapon in GameController.Singleton.Weapons)
				{
					if(weapon.networkView.owner == networkView.owner)
					{
						_myWeapon = weapon.GetComponent<Weapon>();
						break;
					}
				}
			}
			return _myWeapon;
		}
		set
		{
			_myWeapon = value;
		}
	}

	private Weapon _myWeapon;

	public float CurrentRecollorDuration = 0f;
	
	public int Health;

	public bool Dead = false;

	public Transform WeaponAnchor;

	private bool _gotHit = false;
	private bool _gotRecollored = false;
	private Color _myHitColor;

	void Start()
	{
		Health = Properties.MaxPlayerHealth;
		PlayerRenderer = gameObject.GetComponentsInChildren<MeshRenderer> ();

		MyGameController = GameController.Singleton;

		MyGameController.Players.Add (gameObject);

		if (!networkView.isMine) return;

		MyGameController.MyPlayer = gameObject;
		InitializePlayer ();
		HUDController.Singleton.HealthCounter.text = Health.ToString ();
	}

	void Update()
	{
		if (_gotHit) 
		{
			_gotHit = false;
			CurrentRecollorDuration = Properties.RecollorDurationAfterHit;
			foreach(MeshRenderer rend in PlayerRenderer)
				rend.material.color = _myHitColor;
			_gotRecollored = true;
		}

		CurrentRecollorDuration -= Time.deltaTime;

		if (_gotRecollored && CurrentRecollorDuration <= 0f) 
		{
			_gotRecollored = false;
			foreach(MeshRenderer rend in PlayerRenderer)
				rend.material.color = Color.white;
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

		MyWeapon = ((GameObject)Network.Instantiate (Resources.Load ("Weapon"), WeaponAnchor.position, WeaponAnchor.rotation, 1)).GetComponent<Weapon>();
		MyWeapon.transform.parent = WeaponAnchor;

		if(GameController.GetUserEntry(networkView.owner).Deaths > 0)
		{
			int WeaponType = Random.Range(1, ((int)Properties.WeaponType.Length));
			int AmmunitionType = (int)Weapon.ChooseAmmunitionType((Properties.WeaponType)WeaponType);
			int SecondaryEffect = (int)Weapon.ChooseSecondaryEffect((Properties.WeaponType)WeaponType, (Properties.AmmunitionType)AmmunitionType);
			
			MyWeapon.PickupNew(WeaponType, AmmunitionType, SecondaryEffect);
		}
		else
			MyWeapon.PickupDefault ();

	}

	public void GetHit(int Damage, int SourceID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, Damage, SourceID, WeaponType, AmmunitionType, KilledByDirectHit);
	}
	
	public void GetHit(int Damage, int SourceID, int AmmunitionType)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, Damage, SourceID, 0, AmmunitionType, false);
	}
	
	public void GetHit(int Damage)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, Damage, GameController.GetUserEntry(networkView.owner).ID, 0, 0, false);
	}

	[RPC]
	public void RPCGetHit(int Damage, int SourceID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		_gotHit = true;
		Health = Mathf.Clamp (Health - Damage, 0, Properties.MaxPlayerHealth);
		_myHitColor = Damage < 0f ? Color.green : Color.red;

		if (!PlayingHitSound)
						StartCoroutine ("CPlayHitSound");

		if (!networkView.isMine)
						return;

		if (Health == 0) Die (SourceID, WeaponType, AmmunitionType, KilledByDirectHit);

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

	public void Die(int KillerID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		if(!Dead)
			networkView.RPC ("RPCDie", 
			                 RPCMode.AllBuffered, 
			                 KillerID, 
			                 GameController.GetUserEntry(networkView.owner).ID, 
			                 WeaponType, 
			                 AmmunitionType, 
			                 KilledByDirectHit);
	}

	[RPC]
	public void RPCDie(int KillerID, int VictimID, int WeaponType, int AmmunitionType, bool KilledByDirectHit)
	{
		if (Dead) return;
		Dead = true;

		if(networkView.isMine)
			HUDController.Singleton.MyActionBar.CreateEntry (KillerID, VictimID, WeaponType, AmmunitionType, KilledByDirectHit);
		
		GameController.UserEntry _killer = GameController.GetUserEntry (KillerID);
		GameController.UserEntry _victim = GameController.GetUserEntry (VictimID);

		_killer.UpdateStatistics (_killer.Kills + 1, _killer.Deaths);
		_victim.UpdateStatistics (_victim.Kills, _victim.Deaths + 1);

		foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
			col.enabled = false;
		StartCoroutine ("CDeath");
	}

	public IEnumerator CDeath()
	{
		if (networkView.isMine) 
		{
			GetComponent<AlwaysFollow> ().enabled = false;
			Destroy(Lead.gameObject);
		}

		float _dyingTime = Properties.DyingAnimationLength > 0.001f ? Properties.DyingAnimationLength : 0.001f;

		float _curDyingTime = _dyingTime;
	
		foreach (GameObject weapon in MyGameController.Weapons) 
		{
			if(weapon.networkView.owner == networkView.owner)
			{
				MyWeapon = weapon.GetComponent<Weapon>();
				break;
			}
		}
		
		if(MyWeapon == null)
		{
			Weapon[] _weapons = GameObject.FindObjectsOfType<Weapon>();
			foreach(Weapon weapon in _weapons)
				if(weapon.networkView.owner == networkView.owner)
					{
						MyWeapon = weapon.GetComponent<Weapon>();
						break;
					}
		}

		MeshRenderer[] weaponRenderers = MyWeapon.gameObject.GetComponentsInChildren<MeshRenderer> ();

		Projector _shadow = gameObject.GetComponentInChildren<Projector> ();
		float _shadowMaxStrength = _shadow.farClipPlane;

		while ((_curDyingTime / _dyingTime) > 0f) 
		{
			yield return new WaitForEndOfFrame();

			_curDyingTime -= Time.deltaTime;

			foreach(MeshRenderer rend in PlayerRenderer)
			{
				Color _col = rend.material.color;
				rend.material.color = new Color(_col.r, _col.g, _col.b, _curDyingTime / _dyingTime);
			}
			
			foreach(MeshRenderer rend in weaponRenderers)
			{
				Color _col = rend.material.color;
				rend.material.color = new Color(_col.r, _col.g, _col.b, _curDyingTime / _dyingTime);
			}

			_shadow.farClipPlane = _shadowMaxStrength * (_curDyingTime / _dyingTime);
		}

		if (networkView.isMine) 
		{
			MyCamera.transform.parent = null;
			MyCamera.GetComponent<MouseLook>().enabled = false;

			MyGameController.DestroyCamera = true;
			MyGameController.Respawn(Properties.RespawnTimer);
			Network.Destroy (MyWeapon.networkView.viewID);
			Network.Destroy (networkView.viewID);
		}
	}
	
	public void OnDestroy()
	{
		GameController.Singleton.Players.Remove (gameObject);
	}
}
