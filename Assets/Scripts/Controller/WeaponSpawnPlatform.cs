﻿using UnityEngine;
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
		WeaponPivot.transform.localEulerAngles += new Vector3 (0f, GameController.Singleton.MyProperties.RotationSpeed*Time.deltaTime, 0f);

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
				Resources.Load (Properties.WeaponModelFolder + "/" + GameController.Singleton.MyProperties.WeaponModelNames [MyWeaponType]), 
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
		if (other.transform.parent != null && other.transform.parent.tag == "Player") 
		{
			if(!other.transform.parent.networkView.isMine) return;

			other.transform.parent.GetComponent<PlayerController>().PickupWeapon(
				MyWeaponType,
				Random.Range(0, ((int)Properties.AmmunitionTypeEnum.Length)),
				Random.Range(0, ((int)Properties.SecondaryEffectEnum.Length))
				);

			networkView.RPC("RPCDestroyWeapon", RPCMode.AllBuffered);
		}
	}
}
