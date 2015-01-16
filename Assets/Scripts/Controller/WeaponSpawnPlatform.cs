using UnityEngine;
using System.Collections;

public class WeaponSpawnPlatform : MonoBehaviour 
{
	public const float WeaponSpawnTime = 10f;

	public Transform WeaponPivot;

	public float RotationSpeed = 100f;

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

	void Update () 
	{
		WeaponPivot.transform.localEulerAngles += new Vector3 (0f, RotationSpeed*Time.deltaTime, 0f);

		if (Working && MyWeapon == null) 
		{
			Timer += Time.deltaTime;

			if(Timer >= WeaponSpawnPlatform.WeaponSpawnTime)
			{
				Timer = 0f;
				int WeaponType = Random.Range(1, ((int)Weapon.WeaponTypeEnum.Length)-1);
				networkView.RPC("SummonWeapon", RPCMode.AllBuffered, WeaponType);
			}
		}
	}

	[RPC]
	public void SummonWeapon(int WeaponType)
	{
		MyWeaponType = WeaponType;
		Weapon Filler = new GameObject("...").AddComponent<Weapon>();
		MyWeapon = 
			(GameObject)Instantiate (
				Resources.Load (Weapon.WeaponModelFolder + "/" + Filler.WeaponModelNames [MyWeaponType]), 
				Vector3.zero, 
				Quaternion.identity
				); 
		Destroy (Filler.gameObject);

		Transform WeaponCenter = MyWeapon.transform.FindChild ("WeaponModelCenter");
		WeaponCenter.parent = WeaponPivot;
		MyWeapon.transform.parent = WeaponCenter;
		WeaponCenter.localPosition = Vector3.zero;
		WeaponCenter.localRotation = Quaternion.identity;
	}

	[RPC]
	public void DestroyWeapon()
	{
		if (MyWeapon != null)
						Destroy (MyWeapon);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") 
		{
			// HERE THE WEAPON GETS PICKED UP!
			networkView.RPC("DestroyWeapon", RPCMode.AllBuffered);
		}
	}
}
