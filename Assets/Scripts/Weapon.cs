using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Weapon : MonoBehaviour 
{

	public Properties.WeaponTypeEnum WeaponType;
	public Properties.AmmunitionTypeEnum AmmunitionType;
	public Properties.SecondaryEffectEnum SecondaryEffect;

	public Transform Nozzle;
	[HideInInspector]
	public GameObject WeaponModel;

	public float CurrentAccuracyDecay = 0f;

	public float Accuracy;
	private float _curAccuracy;
	public int AmmunitionCount;
	public int Damage;

	private float _timerSinceLastAttack = 0f;
	private Properties MyProperties;
	private bool _shootOnlyOnPress = true;

	void Start()
	{
		GameController.Singleton.Weapons.Add (gameObject);
		enabled = networkView.isMine;
	}

	void Update()
	{
		_timerSinceLastAttack += Time.deltaTime;

		if(!Input.GetMouseButton (0))
			CurrentAccuracyDecay -= GameController.Singleton.MyProperties.WeaponTargetingSpeed [(int)WeaponType]*Time.deltaTime;
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);

		if (_shootOnlyOnPress) 
		{
			if(Input.GetMouseButtonDown(0))
			{
				_timerSinceLastAttack = 0f;
				Shoot();
			}
		}
		else if (Input.GetMouseButton (0) && _timerSinceLastAttack >= GameController.Singleton.MyProperties.WeaponAttackSpeedValues[(int)WeaponType]) 
		{
			_timerSinceLastAttack = 0f;
			Shoot();
		}

		WeaponModel.transform.localPosition = Vector3.Lerp (
			WeaponModel.transform.localPosition, 
			Vector3.zero, 
			Time.deltaTime * GameController.Singleton.MyProperties.WeaponRightingAfterShotSpeed [(int)WeaponType]
			);
	}

	public void PickupNew(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		this.AmmunitionType = (Properties.AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;
		networkView.RPC ("RPCCreate", RPCMode.AllBuffered, WeaponType);
	}

	[RPC]
	public void RPCCreate(int WeaponType)
	{
		if (WeaponModel != null)
						Destroy (WeaponModel);

		WeaponModel = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + GameController.Singleton.MyProperties.WeaponModelNames [WeaponType]), 
				transform.position, 
				transform.rotation
				); 

		WeaponModel.transform.parent = transform;
		Nozzle = WeaponModel.transform.FindChild ("Nozzle");
		_shootOnlyOnPress = GameController.Singleton.MyProperties.WeaponAttackSpeedValues [WeaponType] < 0.01f;
	}

	public void Shoot()
	{
		Ray Crosshair = GameController.Singleton.MyPlayer.transform
			.FindChild("Camera").GetComponent<Camera>()
				.ViewportPointToRay(new Vector3(
					Random.Range(0.5f-(CurrentAccuracyDecay/2f), 0.5f+(CurrentAccuracyDecay/2f)), 
					Random.Range(0.5f-(CurrentAccuracyDecay/2f), 0.5f+(CurrentAccuracyDecay/2f)), 
					0));

		RaycastHit Hit;
		Vector3 TargetPos = Vector3.zero;

		if (Physics.Raycast (Crosshair, out Hit)) 
		{
			TargetPos = Hit.point;
		}

		switch (GameController.Singleton.MyProperties.WeaponShootingModes [(int)WeaponType]) 
		{
		case Properties.ShootingMode.Default:
				GameObject Bullet = (GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.position, Nozzle.rotation, 1);
				Bullet MyBullet = Bullet.GetComponent<Bullet> ();
				MyBullet.Initialize ((int)WeaponType);
				MyBullet.GetShot(TargetPos);
			break;
		case Properties.ShootingMode.Cone:
			Nozzle.LookAt(TargetPos);
			Bullet[] _myBullets = new Bullet[Nozzle.childCount];
			for(int i = 0; i < _myBullets.Length; i++)
			{
				_myBullets[i] = ((GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.GetChild(i).position, Nozzle.GetChild(i).rotation, 1)).GetComponent<Bullet>();
				_myBullets[i].Initialize((int)WeaponType);
				_myBullets[i].GetShot(Vector3.zero);
			}
			break;
		}

		GameController.Singleton.MyPlayer.GetComponent<PlayerController> ().
			MyCamera.GetComponent<MouseLook> ().RotateY (GameController.Singleton.MyProperties.CameraRecoilOnShot [(int)WeaponType]); //Camera recoil on shot
		WeaponModel.transform.localPosition -= new Vector3 (0f, 0f, GameController.Singleton.MyProperties.WeaponRecoilBackwardsOnShot[(int)WeaponType]); //WeaponRecoil

		CurrentAccuracyDecay += GameController.Singleton.MyProperties.AccuracyDecay [(int)WeaponType];
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);
	}

	public void OnDestroy()
	{
		GameController.Singleton.Weapons.Remove (gameObject);
	}
}
