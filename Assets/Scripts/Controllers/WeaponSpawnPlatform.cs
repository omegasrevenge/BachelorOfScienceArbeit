using UnityEngine;
using System.Collections;

public class WeaponSpawnPlatform : MonoBehaviour 
{
	public Transform WeaponPivot;

	[HideInInspector]
	public float Timer = 0f;
	[HideInInspector]
	public GameObject MyWeapon;
	[HideInInspector]
	public Properties.WeaponType MyWeaponType;

	void Update () 
	{
		WeaponPivot.transform.localEulerAngles += new Vector3 (0f, Properties.PlatformWeaponRotationSpeed*Time.deltaTime, 0f);

		if (networkView.isMine && MyWeapon == null) 
		{
			Timer += Time.deltaTime;

			if(Timer >= (GameController.Singleton.WeaponSpawnPlatforms.Count * Properties.WeaponSpawnTime) / GameController.Singleton.Users.Count)
			{
				Timer = 0f;
				int WeaponType = Random.Range(1, ((int)Properties.WeaponType.Length));
				networkView.RPC("RPCSummonWeapon", RPCMode.AllBuffered, WeaponType);
			}
		}
	}

	[RPC]
	public void RPCSummonWeapon(int weaponType)
	{
		MyWeaponType = (Properties.WeaponType)weaponType;

		MyWeapon = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + Properties.Singleton.WeaponModelNames [weaponType]), 
				Vector3.zero, 
				Quaternion.identity
				); 

		Transform WeaponCenter = MyWeapon.transform.FindChild ("WeaponModelCenter");
		WeaponCenter.parent = WeaponPivot;
		MyWeapon.transform.parent = WeaponCenter;
		WeaponCenter.localPosition = Vector3.zero;
		WeaponCenter.localRotation = Quaternion.identity;
	}

	[RPC]
	public void RPCDestroyWeapon()
	{
		if (MyWeapon != null)
						Destroy (MyWeapon);
	}

	public void OnTriggerStay(Collider other)
	{
		if (!networkView.isMine) return;

		if (other.collider.gameObject.layer == Properties.AvatarLayer 
		    && MyWeapon != null 
		    && other.transform.parent.GetComponent<PlayerController>().MyWeapon.ShotsQueued == 0) 
		{
			int AmmunitionType = (int)WeaponController.ChooseAmmunitionType(MyWeaponType);
			int SecondaryEffect = (int)WeaponController.ChooseSecondaryEffect(MyWeaponType, (Properties.AmmunitionType)AmmunitionType);

			other.transform.parent.GetComponent<PlayerController>().MyWeapon.PickupNew((int)MyWeaponType, AmmunitionType, SecondaryEffect);

			networkView.RPC("RPCDestroyWeapon", RPCMode.AllBuffered);
		}
	}
}
