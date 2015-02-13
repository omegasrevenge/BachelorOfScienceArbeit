using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeaponController : MonoBehaviour 
{
	public Properties.WeaponType WeaponType;
	public Properties.AmmunitionType AmmunitionType;
	public Properties.SecondaryEffect SecondaryEffect;

	[HideInInspector]
	public Transform Nozzle;
	[HideInInspector]
	public GameObject WeaponModel;

	public float CurrentAccuracyDecay = 0f;

	public int CurAmmunition;

	private float TimerSinceLastAttack = 0f; 
	private Properties MyProperties;
	private bool ShootOnlyOnPress = true;

	public int ShotsQueued = 0;

	void Start()
	{
		GameController.Singleton.Weapons.Add (gameObject);
		enabled = networkView.isMine;
		if (networkView.isMine)
			HUDController.Singleton.MyCrosshair.MyWeapon = this;
	}

	void Update()
	{
		if (GameController.Singleton.CurGameState == Properties.GameState.GameOver) return;

		if (CurAmmunition <= 0 && WeaponType != Properties.WeaponType.Default && ShotsQueued == 0) 
			PickupDefault ();

		if (CurAmmunition <= 0 && ShotsQueued > 0) return;

		TimerSinceLastAttack += Time.deltaTime;

		if(!Input.GetMouseButton (0))
			CurrentAccuracyDecay -= Properties.Singleton.WeaponTargetingSpeed [(int)WeaponType]*Time.deltaTime;
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);

		if (ShootOnlyOnPress) 
		{
			if(Input.GetMouseButtonDown(0))
			{
				TimerSinceLastAttack = 0f;
				if(SecondaryEffect == Properties.SecondaryEffect.Delay)
					StartCoroutine("CDelayShot", new int[]{(int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect});
				else
					Shoot((int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
			}
		}
		else if (Input.GetMouseButton (0) && TimerSinceLastAttack >= Properties.Singleton.WeaponAttackSpeedValues[(int)WeaponType]) 
		{
			TimerSinceLastAttack = 0f;
			if(SecondaryEffect == Properties.SecondaryEffect.Delay)
				StartCoroutine("CDelayShot", new int[]{(int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect});
			else
				Shoot((int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
		}

		WeaponModel.transform.localPosition = Vector3.Lerp (
			WeaponModel.transform.localPosition, 
			Vector3.zero, 
			Time.deltaTime * Properties.Singleton.WeaponRightingAfterShotSpeed [(int)WeaponType]
			);
	}

	public void PickupNew(int weaponType, int ammunitionType, int secondaryEffect)
	{
		networkView.RPC ("RPCCreate", RPCMode.AllBuffered, weaponType, ammunitionType, secondaryEffect);
	}

	public void PickupDefault()
	{
		WeaponType = Properties.WeaponType.Default;
		AmmunitionType = ChooseAmmunitionType (WeaponType);
		SecondaryEffect = ChooseSecondaryEffect (WeaponType, AmmunitionType);
		networkView.RPC ("RPCCreate", RPCMode.AllBuffered, (int)WeaponType, (int)AmmunitionType, (int)SecondaryEffect);
	}

	[RPC]
	public void RPCCreate(int weaponType, int ammunitionType, int secondaryEffect)
	{
		if (WeaponModel != null)
						Destroy (WeaponModel);

		WeaponModel = 
			(GameObject)Instantiate (
				Resources.Load (Properties.WeaponModelFolder + "/" + Properties.Singleton.WeaponModelNames [weaponType]), 
				transform.position, 
				transform.rotation
				); 

		AmmunitionType = (Properties.AmmunitionType)ammunitionType;
		SecondaryEffect = (Properties.SecondaryEffect)secondaryEffect;
		WeaponType = (Properties.WeaponType)weaponType;

		WeaponModel.transform.parent = transform;
		Nozzle = WeaponModel.transform.FindChild ("Nozzle");
		ShootOnlyOnPress = Properties.Singleton.WeaponAttackSpeedValues [weaponType] < 0.01f;
		foreach (MeshRenderer rend in WeaponModel.GetComponentsInChildren<MeshRenderer>())
			rend.material.color = Properties.Singleton.WeaponColors[secondaryEffect];
		CurAmmunition = Properties.Singleton.WeaponAmmunitionAmount [weaponType];
		if (networkView.isMine) 
		{
			HUDController.Singleton.AmmunitionCounter.text = CurAmmunition.ToString();
			HUDController.Singleton.MyWeaponBar.ShowWeapon(this);
		}

	}

	public void Shoot(int weaponType, int ammunitionType, int secondaryEffect)
	{
		if (GameController.Singleton.MyPlayer.GetComponent<PlayerController> ().Dead) 
			return;

		CurAmmunition = Mathf.Max (CurAmmunition - 1, 0);
		if(networkView.isMine)
			HUDController.Singleton.AmmunitionCounter.text = CurAmmunition.ToString();

		Ray Crosshair = GameController.Singleton.MyPlayer.transform
			.FindChild("Camera").GetComponent<Camera>()
				.ViewportPointToRay(new Vector3(
					Random.Range(0.5f-(CurrentAccuracyDecay/2f), 0.5f+(CurrentAccuracyDecay/2f)), 
					Random.Range(0.5f-(CurrentAccuracyDecay/2f), 0.5f+(CurrentAccuracyDecay/2f)), 
					0f));

		RaycastHit Hit;
		Vector3 TargetPos = Vector3.zero;
		int RaycastLayerMask = 1 << Properties.WeaponSpawnPlatformLayer;
		RaycastLayerMask = ~RaycastLayerMask;

		if (Physics.Raycast (Crosshair, out Hit, Mathf.Infinity, RaycastLayerMask)) 
		{
			TargetPos = Hit.point;
		}

		switch (Properties.Singleton.WeaponShootingModes [weaponType]) 
		{
		case Properties.ShootingMode.Default:
				GameObject Bullet = (GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.position, Nozzle.rotation, 1);
				Bullet MyBullet = Bullet.GetComponent<Bullet> ();
				MyBullet.Initialize (weaponType, ammunitionType, secondaryEffect);
				MyBullet.GetShot(TargetPos);
			break;
		case Properties.ShootingMode.Cone:
			Nozzle.LookAt(TargetPos);
			Bullet[] MyBullets = new Bullet[Nozzle.childCount];
			for(int i = 0; i < MyBullets.Length; i++)
			{
				MyBullets[i] = ((GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.GetChild(i).position, Nozzle.GetChild(i).rotation, 1)).GetComponent<Bullet>();
				MyBullets[i].Initialize(weaponType, ammunitionType, secondaryEffect);
				MyBullets[i].GetShot(Vector3.zero);
			}
			break;
		}

		GameController.Singleton.MyPlayer.GetComponent<PlayerController> ().
			MyCamera.GetComponent<MouseLook> ().RotateY (Properties.Singleton.CameraRecoilOnShot [weaponType]); //Camera recoil on shot
		WeaponModel.transform.localPosition -= new Vector3 (0f, 0f, Properties.Singleton.WeaponRecoilBackwardsOnShot[weaponType]); //WeaponRecoil

		CurrentAccuracyDecay += Properties.Singleton.AccuracyDecay [weaponType];
		CurrentAccuracyDecay = Mathf.Clamp (CurrentAccuracyDecay, 0f, Properties.AccuracyMaxDecay);

		networkView.RPC ("PlayWeaponSound", RPCMode.AllBuffered, weaponType);
	}

	public IEnumerator CDelayShot(int[] values)
	{
		ShotsQueued++;
		yield return new WaitForSeconds (Properties.Singleton.DelayEffectDuration [values[0]]);
		ShotsQueued--;
		Shoot (values[0], values[1], values[2]);
	}

	[RPC]
	public void PlayWeaponSound(int weaponType)
	{
		SoundManager.PlayClipAt (
			SoundManager.GetClip (weaponType), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [weaponType],
			Properties.Singleton.SoundDefaultMinDistances [weaponType],
			Properties.Singleton.SoundDefaultMaxDistances [weaponType]
			);
	}

	public void OnDestroy()
	{
		GameController.Singleton.Weapons.Remove (gameObject);
	}
	
	public static Properties.AmmunitionType ChooseAmmunitionType(Properties.WeaponType weaponType)
	{
		Properties.AllowedWeaponEffectCombinations AllowedCombo = Properties.Singleton.WeaponRestrictions [(int)weaponType];
		return AllowedCombo.AmmunitionTypes [Random.Range (0, AllowedCombo.AmmunitionTypes.Length)];
	}
	
	public static Properties.SecondaryEffect ChooseSecondaryEffect(Properties.WeaponType weaponType, Properties.AmmunitionType ammunitionType)
	{
		Properties.AllowedWeaponEffectCombinations AllowedCombo = Properties.Singleton.WeaponRestrictions [(int)weaponType];
		Properties.AllowedSecondaryEffects AllowedSecondary = Properties.Singleton.AmmunitionRestrictions [(int)ammunitionType];
		Properties.SecondaryEffect MyEffect = Properties.SecondaryEffect.None;
		
		do
		{
			MyEffect = AllowedSecondary.SecondaryEffects[Random.Range(0, AllowedSecondary.SecondaryEffects.Length)];
		} 
		while(!AllowedCombo.SecondaryEffects.Contains(MyEffect));
		
		return MyEffect;
	}
}
