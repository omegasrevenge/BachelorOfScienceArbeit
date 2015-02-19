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
	
	public static void CreateAt(Transform target, int weaponType, int secondaryEffect)
	{
		Explosion _explosion = ((GameObject)Network.Instantiate (Resources.Load ("Explosion"), target.position, target.rotation, 1)).GetComponent<Explosion>();

		_explosion.networkView.RPC ("RPCInitialize", 
		                RPCMode.AllBuffered, 
		                 Properties.Singleton.ExplosionDamage [weaponType] 
								* Properties.Singleton.BulletDamage [weaponType] 
								* Properties.ExplosionEffectBulletDamageMultiplier
								* ((Properties.SecondaryEffect)secondaryEffect == Properties.SecondaryEffect.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
								* ((Properties.SecondaryEffect)secondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f),
		                target.localScale * Properties.Singleton.ExplosionEndSize [weaponType],
		                Properties.ExplosionLifeTime,
		                1f / Properties.ExplosionLifeTime,
		                secondaryEffect);
	}

	[RPC]
	public void RPCInitialize(float damage, Vector3 endScale, float lifeTime, float scalingSpeed, int secondaryEffect)
	{
		Damage = damage;
		EndScale = endScale;
		LifeTime = lifeTime;
		ScalingSpeed = scalingSpeed;
		SecondaryEffect = (Properties.SecondaryEffect)secondaryEffect;
		CurLifeTime = lifeTime;
		AmmunitionType = Properties.AmmunitionType.Explosive;
		Initialized = true;

		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int)Properties.Sounds.Explosion), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int)Properties.Sounds.Explosion],
			Properties.Singleton.SoundDefaultMinDistances [(int)Properties.Sounds.Explosion] * endScale.magnitude / Properties.DefaultExplosionMagnitude,
			Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.Sounds.Explosion] * endScale.magnitude / Properties.DefaultExplosionMagnitude
			);
	}
	
	void OnTriggerEnter(Collider info)
	{
		if (!networkView.isMine) return;
		
		if (info.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController HitPlayer = info.transform.parent.GetComponent<PlayerController>();
			if(HitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffect.Healing)
				HitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				HitPlayer.GetHit(Mathf.RoundToInt(Damage), GameController.GetUserEntry(networkView.owner).ID, (int)AmmunitionType);
		}
	}
}
