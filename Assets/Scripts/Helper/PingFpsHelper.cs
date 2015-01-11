using UnityEngine;
using System.Collections;

public class PingFpsHelper : MonoBehaviour 
{
	//public tk2dTextMesh MyPing;
	//public tk2dTextMesh MyFps;
	//
	//public int numFrames = 0;
	//public float elapsedTime = 0f;
	//
	//void Start () 
	//{
	//
	//}
	//
	//void Update () 
	//{
	//	elapsedTime += Time.deltaTime;
	//	numFrames++;
	//
	//	if (elapsedTime >= 1f) 
	//	{
	//		MyFps.text = "Fps: "+numFrames;
	//		MyFps.Commit();
	//		elapsedTime = 0f;
	//		numFrames = 0;
	//
	//		if (!PhotonNetwork.inRoom) return;
	//		MyPing.text = "Ping: "+PhotonNetwork.GetPing ();
	//		MyPing.Commit ();
	//	}
	//
	//}
}
