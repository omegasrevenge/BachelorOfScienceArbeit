using UnityEngine;

public class NetworkChar : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
	private Transform myTransform;

	void Awake()
	{
		myTransform = transform;
	}

	void Update()
	{
		if (photonView.isMine) return;

		myTransform.position = Vector3.Lerp(myTransform.position, this.correctPlayerPos, Time.deltaTime * 5);
		myTransform.rotation = Quaternion.Lerp(myTransform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(myTransform.position);
			stream.SendNext(myTransform.rotation);
		}
		else
		{
			this.correctPlayerPos = (Vector3)stream.ReceiveNext();
			this.correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
}