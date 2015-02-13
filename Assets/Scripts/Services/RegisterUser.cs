using UnityEngine;
using System.Collections;

public class RegisterUser : MonoBehaviour 
{
	public GameController.UserEntry MyUserEntry;

	public static void Register(string userName)
	{
		RegisterUser Reg = ((GameObject)Network.Instantiate(
			Resources.Load("RegisterUser"), 
			Vector3.zero, 
			Quaternion.identity, 1))
			.GetComponent<RegisterUser>();

		Reg.networkView.RPC ("RPCAdmitInfo", RPCMode.AllBuffered, userName);
	}

	/// <summary>
	/// Allocates a unique ID for every single User currently connected to the Network. Is already automatically called when someone joins.
	/// </summary>
	public static void InitializeIDs()
	{
		if (!Network.isServer) return;
		
		for (int i = 0; i < GameController.Singleton.transform.childCount; i++) 
		{
			RegisterUser RegUserObj = GameController.Singleton.transform.GetChild(i).GetComponent<RegisterUser>();

			RegUserObj.networkView.RPC("RPCUpdateID", RPCMode.AllBuffered, i);
		}
	}

	[RPC]
	public void RPCUpdateID(int newID)
	{
		MyUserEntry.ID = newID;
	}

	[RPC]
	public void RPCAdmitInfo(string UserName)
	{
		transform.parent = GameController.Singleton.transform;
		MyUserEntry = GameController.AddUserEntry (networkView.owner, UserName);
		MyUserEntry.MyRegisterObject = this;
		if (Network.isServer)
			RegisterUser.InitializeIDs ();
	}

	void OnDestroy()
	{
		GameController.RemoveUserEntry (MyUserEntry.ID);
	}
}
