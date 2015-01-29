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
				.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)); // HERE THE ACCURACY HAS TO BE CALCULATED IN!
		RaycastHit Hit;
		Vector3 TargetPos = Vector3.zero;

		if (Physics.Raycast (Crosshair, out Hit)) 
		{
			TargetPos = Hit.point;
		}

		GameObject Bullet = (GameObject)Network.Instantiate (Resources.Load ("Bullet"), Nozzle.position, Nozzle.rotation, 1);
		Bullet MyBullet = Bullet.GetComponent<Bullet> ();
		MyBullet.Initialize ((int)WeaponType);
		MyBullet.GetShot(TargetPos);
	}

	public void OnDestroy()
	{
		GameController.Singleton.Weapons.Remove (gameObject);
	}
}
