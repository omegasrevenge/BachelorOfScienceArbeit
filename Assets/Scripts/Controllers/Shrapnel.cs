﻿using UnityEngine;
using System.Collections;

public class Shrapnel : MonoBehaviour 
{
	public float LifeTime;

	void Start()
	{
		enabled = networkView.isMine;
	}

	void Update()
	{
		LifeTime -= Time.deltaTime;
		if (LifeTime <= 0f)
			Network.Destroy (networkView.viewID);
	}

	public static void CreateAt(Transform target, int WeaponType, int AmmunitionType, int SecondaryEffect)
	{
		GameObject _shrapnel = (GameObject)Network.Instantiate (Resources.Load ("Shrapnel"), target.position, target.rotation, 1);
		_shrapnel.GetComponent<Shrapnel>().networkView.RPC ("RPCInitialize", 
		                                                    RPCMode.AllBuffered, 
		                                                    WeaponType, 
		                                                    AmmunitionType, 
		                                                    SecondaryEffect, 
		                                                    Properties.Singleton.ShrapnelSize [WeaponType]);
	}

	[RPC]
	public void RPCInitialize(int WeaponType, int AmmunitionType, int SecondaryEffect, float ScaleModifier)
	{
		GetComponent<Shrapnel>().LifeTime = Properties.Singleton.ShrapnelLifeTime [WeaponType];
		ShrapnelPiece[] _pieces = GetComponentsInChildren<ShrapnelPiece> ();
		foreach (ShrapnelPiece piece in _pieces)
			piece.Initialize(WeaponType, AmmunitionType, SecondaryEffect);
		transform.localScale *= ScaleModifier;
		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int)Properties.SoundsEnum.Shrapnel), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int)Properties.SoundsEnum.Shrapnel],
			Properties.Singleton.SoundDefaultMinDistances [(int)Properties.SoundsEnum.Shrapnel] * ScaleModifier,
			Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.SoundsEnum.Shrapnel] * ScaleModifier
			);
	}
}
