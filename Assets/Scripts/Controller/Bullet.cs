using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
	public float _liveTime = 0f;

	public Properties.WeaponTypeEnum WeaponType;

	private bool _gettingDestroyed = false;

	public Properties MyProps
	{
		get
		{
			return GameController.Singleton.MyProperties;
		}
	}

	void Start () 
	{
		if (!networkView.isMine)
						enabled = false;
	}

	void Update()
	{
		_liveTime += Time.deltaTime;
		if(transform.position.y < -501f)
			Network.Destroy (networkView.viewID);
	}

	public void Initialize(int WeaponType)
	{
		networkView.RPC ("RPCInitialized", RPCMode.AllBuffered, WeaponType);
	}

	[RPC]
	public void RPCInitialized(int WeaponType)
	{
		GameObject BulletModel = (GameObject)Instantiate (
			Resources.Load (Properties.BulletModelFolder + "/" + MyProps.BulletModelNames [WeaponType]),
			transform.position,
			transform.rotation);

		BulletModel.transform.parent = transform;
		this.WeaponType = (Properties.WeaponTypeEnum)WeaponType;

		Rigidbody myBody = gameObject.AddComponent<Rigidbody> ();
		myBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
		myBody.useGravity = MyProps.BulletsUseGravity [WeaponType];
		myBody.mass = MyProps.BulletMass [WeaponType];
	}

	public void GetShot(Vector3 TargetPos)
	{
		if(TargetPos != Vector3.zero)
			transform.LookAt (TargetPos); // zero means the raycast hit nothing. Should never happen, but the skybox, for example, cannot be hit by it.

		GetComponent<Rigidbody> ().AddForce (transform.forward * MyProps.BulletFlyingSpeed [(int)WeaponType], ForceMode.Impulse);  //FLYING SPEED!
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (_gettingDestroyed) return;
		if (!networkView.isMine) return;
		if (collision.collider.transform.parent != null && collision.collider.transform.parent.tag == "Bullet") return;

		// LIFETIME AND ON HIT EFFECT!

		if (collision.collider.gameObject.layer == Properties.AvatarLayer) 
			collision.collider.transform.parent.GetComponent<PlayerController> ().GetHit (5); //DAMAGE!

		Network.Destroy (networkView.viewID);
		_gettingDestroyed = true;
	}
}
