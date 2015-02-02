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
	
	private int Health;

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
		MyWeapon.PickupDefault ();

	}

	public void PickupWeapon(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		MyWeapon.PickupNew (WeaponType, AmmunitionType, SecondaryEffect);
	}

	public void GetHit(int Damage)
	{
		if(!Dead)
			networkView.RPC ("RPCGetHit", RPCMode.AllBuffered, Damage);
	}

	[RPC]
	public void RPCGetHit(int Damage)
	{
		_gotHit = true;
		Health = Mathf.Clamp (Health - Damage, 0, Properties.MaxPlayerHealth);
		_myHitColor = Damage < 0f ? Color.green : Color.red;

		if (!networkView.isMine)
						return;

		if (Health == 0) Die ();

		HUDController.Singleton.HealthCounter.text = Health.ToString ();
	}

	public void Die()
	{
		Dead = true;
		networkView.RPC ("RPCDie", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCDie()
	{
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
				MyWeapon = weapon.GetComponent<Weapon>();
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
