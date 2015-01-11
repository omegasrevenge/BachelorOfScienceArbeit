using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RangeCalc : MonoBehaviour 
{
	public enum DimensionCalc {ThreeDimensions, TwoDimensions}

	public DimensionCalc Mode;
	public GameObject origin;
	public GameObject target;

	private GameObject _oldOrigin;
	private GameObject _oldTarget;
	private DimensionCalc _oldMode;

	void Update () 
	{
		if (target == null) return;
		if (target == _oldTarget 
		    && origin == _oldOrigin 
		    && Mode == _oldMode) 
			return;

		_oldTarget = target;
		_oldOrigin = origin;
		_oldMode = Mode;

		Vector3 originPos = origin != null ? origin.transform.position : transform.position;
		Vector3 targetPos = target.transform.position;
		float dist;

		if (Mode == DimensionCalc.TwoDimensions)
		{
			originPos.y = 0f;
			targetPos.y = 0f;
		}

		dist = Mathf.Abs ((targetPos - originPos).magnitude); 


		string originName = origin != null ? origin.name : name;
		string curDimension = Mode == DimensionCalc.ThreeDimensions ? "Three Dimensions. Not ignoring y." : "Two Dimensions. Ignoring y.";

		Debug.Log ("Current Mode is: "+curDimension+" The distance between "+originName+" and "+target.name+" is "+dist+" Unity Length Units. Square of it is: "+(dist*dist)+".");
	}
}
