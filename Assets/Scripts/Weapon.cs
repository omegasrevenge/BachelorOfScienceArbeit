using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Weapon : MonoBehaviour 
{

	public Properties.WeaponTypeEnum WeaponType;
	public Properties.AmmunitionTypeEnum AmmunitionType;
	public Properties.SecondaryEffectEnum SecondaryEffect;

	[HideInInspector]
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
			CurrentAccuracyDecay -= Properties.Singleton.WeaponTargetingSpeed [(int)WeaponType]*Time.deltaTime;
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);

		if (_shootOnlyOnPress) 
		{
			if(Input.GetMouseButtonDown(0))
			{
				_timerSinceLastAttack = 0f;
				if(SecondaryEffect == Properties.SecondaryEffectEnum.Delay)
					StartCoroutine("CDelayShot");
				else
					Shoot();
			}
		}
		else if (Input.GetMouseButton (0) && _timerSinceLastAttack >= Properties.Singleton.WeaponAttackSpeedValues[(int)WeaponType]) 
		{
			_timerSinceLastAttack = 0f;
			if(SecondaryEffect == Properties.SecondaryEffectEnum.Delay)
				StartCoroutine("CDelayShot");
			else
				Shoot();
		}

		WeaponModel.transform.localPosition = Vector3.Lerp (
			WeaponModel.transform.localPosition, 
			Vector3.zero, 
			Time.deltaTime * Properties.Singleton.WeaponRightingAfterShotSpeed [(int)WeaponType]
			);
	}

	public void PickupNew(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		networkView.RPC ("RPCCreate", RPCMode.AllBuffered, WeaponType, AmmunitionType, SecondaryEffect);
	}

	public void PickupDefault()
	{
		WeaponType = Properties.WeaponTypeEnum.Default;
		AmmunitionType = ChooseAmmunitionType (WeaponType);
		SecondaryEffect = ChooseSecondaryEffect (WeaponType, AmmunitionType);
		networkView.RPC ("RPCCreate", RPCMode.AllBuffered, (int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
	}

	[RPC]
	public void RPCCreate(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		if (WeaponModel != null)
						Destroy (WeaponModel);

		WeaponModel = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + Properties.Singleton.WeaponModelNames [WeaponType]), 
				transform.position, 
				transform.rotation
				); 

		this.AmmunitionType = (Properties.AmmunitionTypeEnum)AmmunitionType;
		this.SecondaryEffect = (Properties.SecondaryEffectEnum)SecondaryEffect;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;

		WeaponModel.transform.parent = transform;
		Nozzle = WeaponModel.transform.FindChild ("Nozzle");
		_shootOnlyOnPress = Properties.Singleton.WeaponAttackSpeedValues [WeaponType] < 0.01f;
		foreach (MeshRenderer rend in WeaponModel.GetComponentsInChildren<MeshRenderer>())
			rend.material.color = Properties.Singleton.WeaponColors[SecondaryEffect];
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

		switch (Properties.Singleton.WeaponShootingModes [(int)WeaponType]) 
		{
		case Properties.ShootingMode.Default:
				GameObject Bullet = (GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.position, Nozzle.rotation, 1);
				Bullet MyBullet = Bullet.GetComponent<Bullet> ();
				MyBullet.Initialize ((int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
				MyBullet.GetShot(TargetPos);
			break;
		case Properties.ShootingMode.Cone:
			Nozzle.LookAt(TargetPos);
			Bullet[] _myBullets = new Bullet[Nozzle.childCount];
			for(int i = 0; i < _myBullets.Length; i++)
			{
				_myBullets[i] = ((GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.GetChild(i).position, Nozzle.GetChild(i).rotation, 1)).GetComponent<Bullet>();
				_myBullets[i].Initialize((int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
				_myBullets[i].GetShot(Vector3.zero);
			}
			break;
		}

		GameController.Singleton.MyPlayer.GetComponent<PlayerController> ().
			MyCamera.GetComponent<MouseLook> ().RotateY (Properties.Singleton.CameraRecoilOnShot [(int)WeaponType]); //Camera recoil on shot
		WeaponModel.transform.localPosition -= new Vector3 (0f, 0f, Properties.Singleton.WeaponRecoilBackwardsOnShot[(int)WeaponType]); //WeaponRecoil

		CurrentAccuracyDecay += Properties.Singleton.AccuracyDecay [(int)WeaponType];
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);
	}

	public IEnumerator CDelayShot()
	{
		yield return new WaitForSeconds (Properties.Singleton.DelayEffectDuration [(int)WeaponType]);
		Shoot ();
	}

	public void OnDestroy()
	{
		GameController.Singleton.Weapons.Remove (gameObject);
	}
	
	public static Properties.AmmunitionTypeEnum ChooseAmmunitionType(Properties.WeaponTypeEnum WeaponType)
	{
		Properties.AllowedWeaponEffectCombinations _allowedCombo = Properties.Singleton.WeaponRestrictions [(int)WeaponType];
		return _allowedCombo.AmmunitionTypes [Random.Range (0, _allowedCombo.AmmunitionTypes.Length)];
	}
	
	public static Properties.SecondaryEffectEnum ChooseSecondaryEffect(Properties.WeaponTypeEnum WeaponType, Properties.AmmunitionTypeEnum AmmunitionType)
	{
		Properties.AllowedWeaponEffectCombinations _allowedCombo = Properties.Singleton.WeaponRestrictions [(int)WeaponType];
		Properties.AllowedSecondaryEffects _allowedSecondary = Properties.Singleton.AmmunitionRestrictions [(int)AmmunitionType];
		Properties.SecondaryEffectEnum _myEffect = Properties.SecondaryEffectEnum.None;
		
		do
		{
			_myEffect = _allowedSecondary.SecondaryEffects[Random.Range(0, _allowedSecondary.SecondaryEffects.Length)];
		} 
		while(!_allowedCombo.SecondaryEffects.Contains(_myEffect));
		
		return _myEffect;
	}
}
