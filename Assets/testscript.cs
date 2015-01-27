using UnityEngine;
using System.Collections;

public class testscript : MonoBehaviour 
{
	public bool GiveForce = false;
	public float ForceAmount = 1f;

	void Update () 
	{
		if (GiveForce) 
		{
			GiveForce = false;
			GetComponent<Rigidbody>().AddForce(transform.right*ForceAmount, ForceMode.Impulse);
		}
	}

	public void OnTriggerStay(Collider other)
	{
		Debug.Log ("ON TRIGGER STAY!");
	}
}