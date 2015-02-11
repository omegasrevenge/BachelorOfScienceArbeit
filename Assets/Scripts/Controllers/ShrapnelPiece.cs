using UnityEngine;
using System.Collections;

public class ShrapnelPiece : MonoBehaviour 
{
	public Properties.WeaponType WeaponType;
	public Properties.AmmunitionType AmmunitionType;
	public Properties.SecondaryEffect SecondaryEffect;

	public float Damage;

	public void Initialize(int weaponType, int ammunitionType, int secondaryEffect)
	{
		AmmunitionType = (Properties.AmmunitionType)ammunitionType;
		SecondaryEffect = (Properties.SecondaryEffect)secondaryEffect;
		WeaponType = (Properties.WeaponType)weaponType;

		Damage = Properties.Singleton.ShrapnelDamage [weaponType] 
			* Properties.Singleton.BulletDamage [weaponType] 
			* Properties.ShrapnelEffectBulletDamageMultiplier
			* ((Properties.SecondaryEffect)secondaryEffect == Properties.SecondaryEffect.Delay ? Properties.DelayEffectDamageMultiplier : 1f)
			* ((Properties.SecondaryEffect)secondaryEffect == Properties.SecondaryEffect.Heavy ? Properties.HeavyEffectDamageMultiplier : 1f);
		GetComponent<Rigidbody> ().AddForce (transform.forward * Properties.ShrapnelFlyingSpeed, ForceMode.Impulse);
	}

	void OnCollisionEnter(Collision Info)
	{
		if (!networkView.isMine) return;

		if (Info.collider.gameObject.layer == Properties.AvatarLayer) 
		{
			PlayerController HitPlayer = Info.collider.transform.parent.GetComponent<PlayerController>();
			if(HitPlayer.networkView.isMine && SecondaryEffect == Properties.SecondaryEffect.Healing)
				HitPlayer.GetHit(Mathf.RoundToInt((-1) * Damage));
			else
				HitPlayer.GetHit(Mathf.RoundToInt(Damage), GameController.GetUserEntry(networkView.owner).ID, (int)AmmunitionType);
		}

		networkView.RPC ("RPCDestroyShrapnelPiece", RPCMode.AllBuffered);
	}

	[RPC]
	public void RPCDestroyShrapnelPiece()
	{
		Destroy (gameObject);
	}
}
