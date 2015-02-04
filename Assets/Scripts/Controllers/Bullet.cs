using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float MaxLifeTime = 0f;
	public float LifeTime = 0f;

	public int MaxBounceCount = 1;
	public int CurrentBounceCount = 0;

	public Properties.WeaponTypeEnum WeaponType;
	public Properties.AmmunitionTypeEnum AmmunitionType;
	public Properties.SecondaryEffectEnum SecondaryEffect;

	private bool _gettingDestroyed = false;

	void Start () 
	{
		enabled = networkView.isMine;
	}

	void Update()
	{
		LifeTime -= Time.deltaTime;
		if ((LifeTime <= 0f && AmmunitionType != Properties.AmmunitionTypeEnum.Bouncy) || transform.position.y < -1000f)
		{
			_gettingDestroyed = true;
			Network.Destroy (networkView.viewID);
		}
	}

	public void Initialize(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		networkView.RPC ("RPCInitialized", RPCMode.AllBuffered, WeaponType, AmmunitionType, SecondaryEffect);
	}

	[RPC]
	public void RPCInitialized(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		GameObject BulletModel = (GameObject)Instantiate (
			Resources.Load (Properties.BulletModelFolder + "/" + Properties.Singleton.BulletModelNames [WeaponType]),
			transform.position,
			transform.rotation); 

		BulletModel.transform.parent = transform;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;
		this.AmmunitionType = (Properties.AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		
		Rigidbody myBody = gameObject.AddComponent<Rigidbody> ();
		myBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		myBody.useGravity = this.SecondaryEffect == Properties.SecondaryEffectEnum.Heavy ? true : Properties.Singleton.BulletsUseGravity [WeaponType];
		myBody.mass = Properties.Singleton.BulletMass [WeaponType];

		LifeTime = Properties.Singleton.BulletDefaultLifetime [WeaponType] 
					* Properties.Singleton.BulletAmmunitionTypeLifetimeModifier [AmmunitionType] 
					* Properties.Singleton.BulletSecondaryEffectLifetimeModifier [SecondaryEffect];

		MaxLifeTime = LifeTime;

		foreach (MeshRenderer rend in gameObject.GetComponentsInChildren<MeshRenderer>())
						rend.material.color = Properties.Singleton.BulletColors [AmmunitionType];

		if ((Properties.AmmunitionTypeEnum)AmmunitionType == Properties.AmmunitionTypeEnum.Bouncy)
						MaxBounceCount = Properties.Singleton.BouncyMaxBounceCount [WeaponType];
	}

	public void GetShot(Vector3 TargetPos)
	{
		if(TargetPos != Vector3.zero)
			transform.LookAt (TargetPos); // zero means the raycast hit nothing. Should never happen, but the skybox, for example, cannot be hit by it.

		GetComponent<Rigidbody> ().AddForce (
			transform.forward 
			* Properties.Singleton.BulletFlyingSpeed [(int)WeaponType]
			* (SecondaryEffect == Properties.SecondaryEffectEnum.Heavy ? Properties.Singleton.HeavyEffectSpeedMultiplier[(int)WeaponType] : 1f), 
			ForceMode.Impulse);
	}

	public void OnTriggerEnter(Collider target)
	{
		Hit (target);
	}

	public void OnCollisionEnter(Collision target)
	{
		Hit (target.collider);
	}

	public void Hit(Collider other)
	{
		if(AmmunitionType == Properties.AmmunitionTypeEnum.Bouncy && CurrentBounceCount + 1 < MaxBounceCount)
			SoundManager.PlayClipAt (
				SoundManager.GetClip ((int)Properties.SoundsEnum.Bouncy), 
				transform.position, 
				Properties.Singleton.SoundDefaultVolumes [(int)Properties.SoundsEnum.Bouncy],
				Properties.Singleton.SoundDefaultMinDistances [(int)Properties.SoundsEnum.Bouncy],
				Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.SoundsEnum.Bouncy]
				);

		if (other.gameObject.layer == Properties.AvatarLayer 
		    && other.transform.parent.GetComponent<PlayerController> ().networkView.isMine
		    && LifeTime > (MaxLifeTime - Properties.BulletAbleToHitDelay))
			return;
		if (_gettingDestroyed) return;
		if (!networkView.isMine) return;
		if (other.transform.parent != null && other.transform.parent.tag == "Bullet") return;

		if (other.gameObject.layer == Properties.AvatarLayer) 
		{
			int _damage = Mathf.RoundToInt (Properties.Singleton.BulletDamage [(int)WeaponType] 
			                                * (AmmunitionType == Properties.AmmunitionTypeEnum.Shrapnel ? Properties.ShrapnelEffectBulletDamageMultiplier : 1f)
			                                * (AmmunitionType == Properties.AmmunitionTypeEnum.Explosive ? Properties.ExplosionEffectBulletDamageMultiplier : 1f)
			                                * (SecondaryEffect == Properties.SecondaryEffectEnum.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
			                                * (SecondaryEffect == Properties.SecondaryEffectEnum.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f));

			if(SecondaryEffect == Properties.SecondaryEffectEnum.Healing && other.transform.parent.networkView.isMine)
				_damage *= -1;

			other.transform.parent.GetComponent<PlayerController> ().GetHit (_damage, GameController.GetUserEntry(networkView.owner).ID, (int)WeaponType, (int)AmmunitionType, true);
		}
		
		if (AmmunitionType == Properties.AmmunitionTypeEnum.Shrapnel)
			Shrapnel.CreateAt (transform, (int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);

		if (AmmunitionType == Properties.AmmunitionTypeEnum.Explosive)
						Explosion.CreateAt (transform.GetChild(0), (int)WeaponType, (int)SecondaryEffect);

		CurrentBounceCount++;

		if (CurrentBounceCount >= MaxBounceCount) 
		{
			Network.Destroy (networkView.viewID);
			_gettingDestroyed = true;
		}
	}
}
