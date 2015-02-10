using UnityEngine;
using System.Collections;

public class ShrapnelPiece : MonoBehaviour 
{
	public Properties.WeaponType WeaponType;
	public Properties.AmmunitionType AmmunitionType;
	public Properties.SecondaryEffect SecondaryEffect;

	public float Damage;

	public void Initialize(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		this.AmmunitionType = (Properties.AmmunitionType)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffect)SecondaryEffect;
		this.WeaponType = (Properties.WeaponType)WeaponType;
		Damage = Properties.Singleton.ShrapnelDamage [WeaponType] 
			* Properties.Singleton.BulletDamage [WeaponType] 
			* Properties.ShrapnelEffectBulletDamageMultiplier
			* ((Properties.SecondaryEffect)SecondaryEffect == Properties.SecondaryEffect.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
			* ((Properties.SecondaryEffect)SecondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f);
		GetComponent<Rigidbody> ().AddForce (transform.forward * Properties.ShrapnelFlyingSpeed, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision Info)
	{
		if (!networkView.isMine) return;

		if (Info.collider.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController _hitPlayer = Info.collider.transform.parent.GetComponent<PlayerController>();
			if(_hitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffect.Healing)
				_hitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				_hitPlayer.GetHit(Mathf.RoundToInt(Damage), GameController.GetUserEntry(networkView.owner).ID, (int)AmmunitionType);
		}

		networkView.RPC ("RPCDestroyShrapnelPiece", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCDestroyShrapnelPiece()
	{
		Destroy (gameObject);
	}
}
