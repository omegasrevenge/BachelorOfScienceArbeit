using UnityEngine;
using System.Collections;

public class ShrapnelPiece : MonoBehaviour 
{
	public Properties.WeaponTypeEnum WeaponType;
	public Properties.AmmunitionTypeEnum AmmunitionType;
	public Properties.SecondaryEffectEnum SecondaryEffect;

	public float Damage;

	public void Initialize(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		this.AmmunitionType = (Properties.AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;
		Damage = Properties.Singleton.ShrapnelDamage [WeaponType] 
			* Properties.Singleton.BulletDamage [WeaponType] 
			* Properties.ShrapnelEffectBulletDamageMultiplier
			* ((Properties.SecondaryEffectEnum)SecondaryEffect == Properties.SecondaryEffectEnum.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
			* ((Properties.SecondaryEffectEnum)SecondaryEffect == Properties.SecondaryEffectEnum.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f);
		GetComponent<Rigidbody> ().AddForce (transform.forward * Properties.ShrapnelFlyingSpeed, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision Info)
	{
		if (!networkView.isMine) return;

		if (Info.collider.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController _hitPlayer = Info.collider.transform.parent.GetComponent<PlayerController>();
			if(_hitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffectEnum.Healing)
				_hitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				_hitPlayer.GetHit(Mathf.RoundToInt(Damage));
		}

		networkView.RPC ("RPCDestroyShrapnelPiece", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCDestroyShrapnelPiece()
	{
		Destroy (gameObject);
	}
}
