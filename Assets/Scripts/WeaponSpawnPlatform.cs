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
	public int MyWeaponType;

	public bool Working = false;

	public static void SetWeaponPlatforms(bool state)
	{
		foreach (GameObject platform in GameObject.FindGameObjectsWithTag("WeaponSpawnPlatform")) 
		{
			WeaponSpawnPlatform MyPlatform = platform.GetComponent<WeaponSpawnPlatform> ();
			MyPlatform.Working = state;
			MyPlatform.Timer = 0f;
			
			if(MyPlatform.MyWeapon != null)
				Destroy(MyPlatform.MyWeapon);
		}
	}

	void FixedUpdate () 
	{
		WeaponPivot.transform.localEulerAngles += new Vector3 (0f, Properties.Singleton.RotationSpeed*Time.deltaTime, 0f);

		if (Working && MyWeapon == null) 
		{
			Timer += Time.deltaTime;

			if(Timer >= Properties.WeaponSpawnTime)
			{
				Timer = 0f;
				int WeaponType = Random.Range(1, ((int)Properties.WeaponTypeEnum.Length));
				networkView.RPC("RPCSummonWeapon", RPCMode.AllBuffered, WeaponType);
			}
		}
	}

	[RPC]
	public void RPCSummonWeapon(int WeaponType)
	{
		MyWeaponType = WeaponType;

		MyWeapon = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + Properties.Singleton.WeaponModelNames [MyWeaponType]), 
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
		if (!Network.isServer) return;

		if (other.collider.gameObject.layer == Properties.AvatarLayer && MyWeapon != null) 
		{
			int AmmunitionType = (int)Weapon.ChooseAmmunitionType((Properties.WeaponTypeEnum)MyWeaponType);
			int SecondaryEffect = (int)Weapon.ChooseSecondaryEffect((Properties.WeaponTypeEnum)MyWeaponType, (Properties.AmmunitionTypeEnum)AmmunitionType);

			other.transform.parent.GetComponent<PlayerController>().PickupWeapon(MyWeaponType, AmmunitionType, SecondaryEffect);

			networkView.RPC("RPCDestroyWeapon", RPCMode.AllBuffered);
		}
	}
}
