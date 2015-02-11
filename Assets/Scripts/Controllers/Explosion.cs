using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
	public Vector3 EndScale;
	public float ScalingSpeed;
	public float Damage;
	public float LifeTime;
	public float CurLifeTime;
	
	public Properties.AmmunitionType AmmunitionType;
	public Properties.SecondaryEffect SecondaryEffect;

	private MeshRenderer Renderer;
	private bool Initialized = false;

	void Start()
	{
		Renderer = GetComponent<MeshRenderer> ();
	}
	
	void Update()
	{
		if (!Initialized) return;

		transform.localScale = Vector3.Lerp (transform.localScale, EndScale, Time.deltaTime * ScalingSpeed);

		Renderer.material.color = new Color (Renderer.material.color.r, Renderer.material.color.g, Renderer.material.color.b, CurLifeTime/LifeTime);

		CurLifeTime -= Time.deltaTime;

		if (networkView.isMine && CurLifeTime <= 0f)
			Network.Destroy (networkView.viewID);
	}
	
	public static void CreateAt(Transform target, int WeaponType, int SecondaryEffect)
	{
		Explosion _explosion = ((GameObject)Network.Instantiate (Resources.Load ("Explosion"), target.position, target.rotation, 1)).GetComponent<Explosion>();

		_explosion.networkView.RPC ("RPCInitialize", 
		                RPCMode.AllBuffered, 
		                 Properties.Singleton.ExplosionDamage [WeaponType] 
								* Properties.Singleton.BulletDamage [WeaponType] 
								* Properties.ExplosionEffectBulletDamageMultiplier
								* ((Properties.SecondaryEffect)SecondaryEffect == Properties.SecondaryEffect.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
								* ((Properties.SecondaryEffect)SecondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f),
		                target.localScale * Properties.Singleton.ExplosionEndSize [WeaponType],
		                Properties.ExplosionLifeTime,
		                1f / Properties.ExplosionLifeTime,
		                SecondaryEffect);
	}

	[RPC]
	public void RPCInitialize(float Damage, Vector3 EndScale, float LifeTime, float ScalingSpeed, int SecondaryEffect)
	{
		this.Damage = Damage;
		this.EndScale = EndScale;
		this.LifeTime = LifeTime;
		this.ScalingSpeed = ScalingSpeed;
		this.SecondaryEffect = (Properties.SecondaryEffect)SecondaryEffect;
		this.CurLifeTime = LifeTime;
		AmmunitionType = Properties.AmmunitionType.Explosive;
		Initialized = true;

		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int)Properties.Sounds.Explosion), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int)Properties.Sounds.Explosion],
			Properties.Singleton.SoundDefaultMinDistances [(int)Properties.Sounds.Explosion] * EndScale.magnitude / Properties.DefaultExplosionMagnitude,
			Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.Sounds.Explosion] * EndScale.magnitude / Properties.DefaultExplosionMagnitude
			);
	}
	
	void OnTriggerEnter(Collider Info)
	{
		if (!networkView.isMine) return;
		
		if (Info.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController _hitPlayer = Info.transform.parent.GetComponent<PlayerController>();
			if(_hitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffect.Healing)
				_hitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				_hitPlayer.GetHit(Mathf.RoundToInt(Damage), GameController.GetUserEntry(networkView.owner).ID, (int)AmmunitionType);
		}
	}
}
