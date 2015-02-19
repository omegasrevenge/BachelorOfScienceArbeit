using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float MaxLifeTime = 0f;
	public float LifeTime = 0f;

	public int MaxBounceCount = 1;
	public int CurrentBounceCount = 0;

	public Properties.WeaponType WeaponType;
	public Properties.AmmunitionType AmmunitionType;
	public Properties.SecondaryEffect SecondaryEffect;

	private bool GettingDestroyed = false;

	void Start () 
	{
		enabled = networkView.isMine;
	}

	void Update()
	{
		LifeTime -= Time.deltaTime;
		if ((LifeTime <= 0f && AmmunitionType != Properties.AmmunitionType.Bouncy) || transform.position.y < -1000f)
		{
			GettingDestroyed = true;
			Network.Destroy (networkView.viewID);
		}
	}

	public void Initialize(int weaponType, int ammunitionType, int secondaryEffect)
	{
		networkView.RPC ("RPCInitialized", RPCMode.AllBuffered, weaponType, ammunitionType, secondaryEffect);
	}

	[RPC]
	public void RPCInitialized(int weaponType, int ammunitionType, int secondaryEffect)
	{
		GameObject BulletModel = (GameObject)Instantiate (
			Resources.Load (Properties.BulletModelFolder + "/" + Properties.Singleton.BulletModelNames [weaponType]),
			transform.position,
			transform.rotation); 

		BulletModel.transform.parent = transform;
		WeaponType = (Properties.WeaponType)weaponType;
		AmmunitionType = (Properties.AmmunitionType)ammunitionType;
		SecondaryEffect = (Properties.SecondaryEffect)secondaryEffect;
		
		Rigidbody myBody = gameObject.AddComponent<Rigidbody> ();
		myBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		myBody.useGravity = SecondaryEffect == Properties.SecondaryEffect.Heavy ? true : Properties.Singleton.BulletsUseGravity [weaponType];
		myBody.mass = Properties.Singleton.BulletMass [weaponType];

		LifeTime = Properties.Singleton.BulletDefaultLifetime [weaponType] 
					* Properties.Singleton.BulletAmmunitionTypeLifetimeModifier [ammunitionType] 
					* Properties.Singleton.BulletSecondaryEffectLifetimeModifier [secondaryEffect];

		MaxLifeTime = LifeTime;

		foreach (MeshRenderer Rend in gameObject.GetComponentsInChildren<MeshRenderer>())
						Rend.material.color = Properties.Singleton.BulletColors [ammunitionType];

		if ((Properties.AmmunitionType)ammunitionType == Properties.AmmunitionType.Bouncy)
						MaxBounceCount = Properties.Singleton.BouncyMaxBounceCount [weaponType];
	}

	public void GetShot(Vector3 targetPos)
	{
		if(targetPos != Vector3.zero)
			transform.LookAt (targetPos); // zero means the raycast hit nothing. Should never happen, but the skybox, for example, cannot be hit by it.

		GetComponent<Rigidbody> ().AddForce (
			transform.forward 
			* Properties.Singleton.BulletFlyingSpeed [(int)WeaponType]
			* (SecondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.Singleton.HeavyEffectSpeedMultiplier[(int)WeaponType] : 1f), 
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
		if(AmmunitionType == Properties.AmmunitionType.Bouncy && CurrentBounceCount + 1 < MaxBounceCount)
			SoundManager.PlayClipAt (
				SoundManager.GetClip ((int)Properties.Sounds.Bouncy), 
				transform.position, 
				Properties.Singleton.SoundDefaultVolumes [(int)Properties.Sounds.Bouncy],
				Properties.Singleton.SoundDefaultMinDistances [(int)Properties.Sounds.Bouncy],
				Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.Sounds.Bouncy]
				);

		if (other.gameObject.layer == Properties.AvatarLayer 
		    && other.transform.parent.GetComponent<PlayerController> ().networkView.isMine
		    && LifeTime > (MaxLifeTime - Properties.BulletAbleToHitDelay))
			return;
		if (GettingDestroyed) return;
		if (!networkView.isMine) return;
		if (other.transform.parent != null && other.transform.parent.tag == "Bullet") return;

		if (other.gameObject.layer == Properties.AvatarLayer) 
		{
			int Damage = Mathf.RoundToInt (Properties.Singleton.BulletDamage [(int)WeaponType] 
			                                * (AmmunitionType == Properties.AmmunitionType.Shrapnel ? Properties.ShrapnelEffectBulletDamageMultiplier : 1f)
			                                * (AmmunitionType == Properties.AmmunitionType.Explosive ? Properties.ExplosionEffectBulletDamageMultiplier : 1f)
			                                * (SecondaryEffect == Properties.SecondaryEffect.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
			                                * (SecondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f));

			if(SecondaryEffect == Properties.SecondaryEffect.Healing && other.transform.parent.networkView.isMine)
				Damage *= -1;

			other.transform.parent.GetComponent<PlayerController> ().GetHit (Damage, GameController.GetUserEntry(networkView.owner).ID, (int)WeaponType, (int)AmmunitionType, true);
		}
		
		if (AmmunitionType == Properties.AmmunitionType.Shrapnel)
						Shrapnel.CreateAt (transform, (int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);

		if (AmmunitionType == Properties.AmmunitionType.Explosive)
						Explosion.CreateAt (transform.GetChild(0), (int)WeaponType, (int)SecondaryEffect);

		CurrentBounceCount++;

		if (CurrentBounceCount >= MaxBounceCount) 
		{
			Network.Destroy (networkView.viewID);
			GettingDestroyed = true;
		}
	}
}
