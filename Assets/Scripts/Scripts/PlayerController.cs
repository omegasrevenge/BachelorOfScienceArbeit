﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class PlayerController : MonoBehaviour 
{
	[HideInInspector]
	public GameController MyGameController;
	[HideInInspector]
	public Transform Lead;
	[HideInInspector]
	public Weapon MyWeapon;

	public Transform WeaponAnchor;

	void Start()
	{
		MyGameController = GameController.Singleton;
		
		switch (MyGameController.MyStatus) 
		{
		case Properties.PlayerGameStatus.Host:
													if(networkView.isMine)
														{
															MyGameController.PlayerHost = gameObject;
															MyWeapon = ((GameObject)Network.Instantiate (Resources.Load ("Weapon"), Vector3.zero, Quaternion.identity, 1)).GetComponent<Weapon>();
															MyWeapon.PickupNew(0, 0, 0);
															EnableControls();
														}
													else
														{
															MyGameController.PlayerOther = gameObject;
														}
													break;

		case Properties.PlayerGameStatus.Player:
													if(networkView.isMine)
														{
															MyGameController.PlayerOther = gameObject;
															MyWeapon = ((GameObject)Network.Instantiate (Resources.Load ("Weapon"), Vector3.zero, Quaternion.identity, 1)).GetComponent<Weapon>();
															MyWeapon.PickupNew(0, 0, 0);
															EnableControls();
														}
													else
														{
															MyGameController.PlayerHost = gameObject;
														}
													break;
		}
	}

	public void EnableControls()
	{
		Lead = ((GameObject)Instantiate (
			Resources.Load<GameObject> ("CharacterLead"), 
			transform.position, 
			transform.rotation)
		        ).transform;

		GetComponent<AlwaysFollow> ().enabled = true;
		GetComponent<AlwaysFollow> ().Target = Lead;

		transform.FindChild ("Face").gameObject.SetActive (false);

		transform.FindChild ("Camera").gameObject.SetActive (true);
		MyWeapon.transform.parent = WeaponAnchor;
		MyWeapon.transform.localPosition = Vector3.zero;
		MyWeapon.transform.localRotation = Quaternion.identity;
	}

	public void PickupWeapon(int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		if(networkView.isMine)
			MyWeapon.PickupNew (WeaponType, AmmunitionType, SecondaryEffect);
	}

	//[HideInInspector]
	//public int PlayerID;
	//[SerializeField]
	//public string PlayerName = "GD1011";
	//
	//public int MovementSpeed = 10;
	//public int JumpForce = 3;
	//
	//private int _healthPoints = 100;
	//private Transform _healthBar;
	//private float _constantX;
	//private bool _hasCountedDeath = false;
	//
	//private SkillController _skillController;
	//
	//public int HealthPoints
	//{
	//	get
	//	{
	//		return _healthPoints;
	//	}
	//	set
	//	{
	//		_healthPoints = value;
	//		networkView.RPC("SyncInfo", RPCMode.OthersBuffered, _healthPoints);
	//	}
	//}
	//
	//public bool IsDead
	//{
	//	get
	//	{
	//		return HealthPoints <= 0;
	//	}
	//}
	//
	//private Color _color
	//{
	//	get
	//	{
	//		return GetComponent<MeshRenderer>().material.color;
	//	}
	//	set
	//	{
	//		GetComponent<MeshRenderer>().material.color = value;
	//	}
	//}
	//
	//public bool DyingAnimationFinished
	//{
	//	get
	//	{
	//		return _color.a <= 0f;
	//	}
	//}
	//
	//public ConstantForce _push;
	//
	////private dfButton _rightArrow;
	////private dfButton _leftArrow;
	//
	//
	//// Use this for initialization
	//void Start () 
	//{
	//	//_rightArrow = GameObject.Find("Right").GetComponent<dfButton>();
	//	//_leftArrow = GameObject.Find("Left").GetComponent<dfButton>();
	//	_skillController = GameObject.Find ("Content").GetComponent<SkillController>();
	//	_constantX = transform.localPosition.x;
	//	_healthBar = transform.FindChild("Health");
	//	_push = GetComponent<ConstantForce>();
	//	if(networkView.isMine)
	//	{
	//		_skillController.Pos = transform;
	//		_color = new Color(_color.r, _color.g, _color.b+200, _color.a);
	//	}
	//	else
	//	{
	//		_color = new Color(_color.r+200, _color.g, _color.b, _color.a);
	//	}
	//}
	//
	//void OnCollisionStay(Collision collision)
	//{
	//
	//	if(!networkView.isMine) return;
	//
	//	if(collision.gameObject.layer == 8 && Input.GetKeyDown(KeyCode.Space))
	//	{
	//		Jump();
	//	}
	//}
	//
	//void Update () 
	//{
	//	UpdateScore();
	//	KeepGameTwoD();		//It is possible for the physics engine to manipulate the 
	//						//depth axis even though it is frozen in the inspector, this fixes it
	//	UpdateHealthBar();
	//	DeathAnimation();
	//	if(DyingAnimationFinished) Destroy(gameObject);
	//
	//	if(!networkView.isMine) return;
	//
	//	if(IsDead) _skillController.Pos = null; //skillcontroller creates the skilleffects on 
	//											//player position, while it is null it knows the player is dead
	//	HandleInput();
	//}
	//
	//private void DeathAnimation()
	//{
	//	if(IsDead && !DyingAnimationFinished)
	//	{
	//		_color = new Color(_color.r, _color.g, _color.b, _color.a-0.005f);
	//		_healthBar.localScale = new Vector3(_healthBar.localScale.x, 0f, _healthBar.localScale.z);
	//	}
	//}
	//
	//private void KeepGameTwoD()
	//{
	//	if(_constantX != transform.localPosition.x)
	//	{
	//		transform.localPosition = new Vector3(_constantX, transform.localPosition.y, transform.localPosition.z);
	//	}
	//}
	//
	//private void UpdateHealthBar()
	//{
	//	if(!IsDead)
	//	{
	//		_healthBar.localScale = new Vector3(_healthBar.localScale.x, HealthPoints/50f, _healthBar.localScale.z);
	//	}
	//}
	//
	//private void UpdateScore()
	//{
	//	if(IsDead && !_hasCountedDeath)
	//	{
	//		_hasCountedDeath = true;
	//		//if(networkView.isMine)
	//		//{
	//		//	GameObject.Find ("DeathsLabel").GetComponent<dfLabel>().Text = 
	//		//		(int.Parse(GameObject.Find ("DeathsLabel").GetComponent<dfLabel>().Text)+1).ToString();
	//		//}
	//		//else
	//		//{
	//		//	GameObject.Find ("KillsLabel").GetComponent<dfLabel>().Text = 
	//		//		(int.Parse(GameObject.Find ("KillsLabel").GetComponent<dfLabel>().Text)+1).ToString();
	//		//}
	//	}
	//}
	//
	//public void HandleInput()
	//{
	//	if(IsDead) return;
	//
	//	if(Input.GetKey(KeyCode.D)) MoveRight();
	//	
	//	if(Input.GetKey(KeyCode.A)) MoveLeft();
	//	
	//	//if(!Input.anyKey 
	//	//   && (_rightArrow.State != dfButton.ButtonState.Pressed)
	//	//   && (_leftArrow.State != dfButton.ButtonState.Pressed)) _push.force = Vector3.zero;
	//}
	//
	//public void MoveRight()
	//{
	//	if(_push.force.z > 0) return;
	//	transform.localEulerAngles = Vector3.zero;
	//	_push.force = Vector3.forward*MovementSpeed;
	//}
	//
	//public void MoveLeft()
	//{
	//	if(_push.force.z < 0) return;
	//	transform.localEulerAngles = Vector3.up*180;
	//	_push.force = Vector3.back*MovementSpeed;
	//}
	//
	//public void Jump()
	//{
	//	GetComponent<Rigidbody>().AddForce(Vector3.up*JumpForce - new Vector3(0, GetComponent<Rigidbody>().velocity.y,0));
	//}
	//
	//[RPC]
	//public void SyncInfo(int health)
	//{
	//	_healthPoints = health;
	//}
	//
	//void OnTriggerEnter(Collider collider)
	//{
	//	if(!networkView.isMine) return;
	//	
	//	if (collider.tag == "NoCdBuff")
	//	{
	//		GameObject.Find("Content").GetComponent<SkillController>().NoCooldowns = true;
	//		Network.Destroy(collider.gameObject);
	//	}
	//}
}
