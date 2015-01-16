using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{

	public Properties.WeaponTypeEnum WeaponType;
	public Properties.AmmunitionTypeEnum AmmunitionType;
	public Properties.SecondaryEffectEnum SecondaryEffect;

	public Transform Nozzle;
	[HideInInspector]
	public GameObject WeaponModel;

	public float Accuracy;
	private float _curAccuracy;
	public int AmmunitionCount;
	public int Damage;

	void Start()
	{
		enabled = networkView.isMine;
	}

	public void PickupNew(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		this.AmmunitionType = (Properties.AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;
		networkView.RPC ("Create", RPCMode.AllBuffered, WeaponType);
	}

	[RPC]
	public void Create(int WeaponType)
	{
		if (WeaponModel != null)
						Destroy (WeaponModel);

		WeaponModel = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + GameController.Singleton.MyProperties.WeaponModelNames [WeaponType]), 
				Vector3.zero, 
				Quaternion.identity
				); 

		WeaponModel.transform.parent = transform;
		WeaponModel.transform.localPosition = Vector3.zero;
		WeaponModel.transform.localRotation = Quaternion.identity;
	}

	public void Shoot()
	{

	}
}
