using UnityEngine;
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

	public static void CreateAt(Transform target, int weaponType, int ammunitionType, int secondaryEffect)
	{
		GameObject Shrapnel = (GameObject)Network.Instantiate (Resources.Load ("Shrapnel"), target.position, target.rotation, 1);
		Shrapnel.GetComponent<Shrapnel>().networkView.RPC ("RPCInitialize", 
		                                                    RPCMode.AllBuffered, 
		                                                    weaponType, 
		                                                    ammunitionType, 
		                                                    secondaryEffect, 
		                                                    Properties.Singleton.ShrapnelSize [weaponType]);
	}

	[RPC]
	public void RPCInitialize(int weaponType, int ammunitionType, int secondaryEffect, float scaleModifier)
	{
		GetComponent<Shrapnel>().LifeTime = Properties.Singleton.ShrapnelLifeTime [weaponType];
		ShrapnelPiece[] Pieces = GetComponentsInChildren<ShrapnelPiece> ();
		foreach (ShrapnelPiece Piece in Pieces)
			Piece.Initialize(weaponType, ammunitionType, secondaryEffect);
		transform.localScale *= scaleModifier;
		SoundManager.PlayClipAt (
			SoundManager.GetClip ((int)Properties.Sounds.Shrapnel), 
			transform.position, 
			Properties.Singleton.SoundDefaultVolumes [(int)Properties.Sounds.Shrapnel],
			Properties.Singleton.SoundDefaultMinDistances [(int)Properties.Sounds.Shrapnel] * scaleModifier,
			Properties.Singleton.SoundDefaultMaxDistances [(int)Properties.Sounds.Shrapnel] * scaleModifier
			);
	}
}
