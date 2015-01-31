using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour 
{
	public Vector3 EndScale;
	public float ScalingSpeed;
	public float Damage;
	public float LifeTime;
	public float CurLifeTime;

	public Properties.SecondaryEffectEnum SecondaryEffect;

	private MeshRenderer _renderer;

	void Start()
	{
		_renderer = GetComponent<MeshRenderer> ();
	}
	
	void Update()
	{
		transform.localScale = Vector3.Lerp (transform.localScale, EndScale, Time.deltaTime * ScalingSpeed);

		_renderer.material.color = new Color (_renderer.material.color.r, _renderer.material.color.g, _renderer.material.color.b, CurLifeTime/LifeTime);

		CurLifeTime -= Time.deltaTime;

		if (networkView.isMine && CurLifeTime <= 0f)
			Network.Destroy (networkView.viewID);
	}
	
	public static void CreateAt(Transform target, int WeaponType, int SecondaryEffect)
	{
		Explosion _explosion = ((GameObject)Network.Instantiate (Resources.Load ("Explosion"), target.position, target.rotation, 1)).GetComponent<Explosion>();
		
		_explosion.Damage = Properties.Singleton.ExplosionDamage [WeaponType] 
				* Properties.Singleton.BulletDamage [WeaponType] 
				* Properties.ExplosionEffectBulletDamageMultiplier
				* ((Properties.SecondaryEffectEnum)SecondaryEffect == Properties.SecondaryEffectEnum.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
				* ((Properties.SecondaryEffectEnum)SecondaryEffect == Properties.SecondaryEffectEnum.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f);

		_explosion.EndScale = target.localScale * Properties.Singleton.ExplosionEndSize[WeaponType];
		_explosion.LifeTime = Properties.ExplosionLifeTime;
		_explosion.ScalingSpeed = 1f / Properties.ExplosionLifeTime;
		_explosion.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		_explosion.CurLifeTime = Properties.ExplosionLifeTime;
	}
	
	void OnTriggerEnter(Collider Info)
	{
		if (!networkView.isMine) return;
		
		if (Info.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController _hitPlayer = Info.transform.parent.GetComponent<PlayerController>();
			if(_hitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffectEnum.Healing)
				_hitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				_hitPlayer.GetHit(Mathf.RoundToInt(Damage));
		}
	}
}
