using UnityEngine;
using System.Collections;

public class RegisterUser : MonoBehaviour 
{
	public GameController.UserEntry MyUserEntry;

	public static void Register(RequiredInformation Info)
	{
		RegisterUser _reg = ((GameObject)Network.Instantiate(
			Resources.Load("RegisterUser"), 
			Vector3.zero, 
			Quaternion.identity, 1))
			.GetComponent<RegisterUser>();

		_reg.networkView.RPC ("RPCAdmitInfo", RPCMode.AllBuffered, Info.UserName);
	}

	/// <summary>
	/// Allocates a unique ID for every single User currently connected to the Network. Is already automatically called when someone joins.
	/// </summary>
	public static void InitializeIDs()
	{
		if (!Network.isServer) return;
		
		for (int i = 0; i < GameController.Singleton.transform.childCount; i++) 
		{
			RegisterUser _regUserObj = GameController.Singleton.transform.GetChild(i).GetComponent<RegisterUser>();

			_regUserObj.networkView.RPC("RPCUpdateID", RPCMode.AllBuffered, i);
		}
	}

	[RPC]
	public void RPCUpdateID(int NewID)
	{
		MyUserEntry.ID = NewID;
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

	public class RequiredInformation
	{
		public string UserName;
		public int ID;

		public RequiredInformation(string UserName)
		{
			this.UserName = UserName;
		}
	}
}
