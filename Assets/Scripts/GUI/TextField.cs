using UnityEngine;
using System.Collections;

public class TextField : MonoBehaviour 
{
	//public enum TextFieldSetting { Default, NumbersOnly }
	//
	//public TextFieldSetting Setting;
	//
	//public bool ToggleNow = false;
	//
	//[HideInInspector]
	//public bool showingLine = false;
	//[HideInInspector]
	//public tk2dTextMesh targetTextMesh;
	//
	//public string MyName = "Peter";
	//
	//private float actualElapsedTime = 0f;
	//private float elapsedTime;
	//
	//void Awake()
	//{
	//	//targetTextMesh = GetComponent<tk2dTextMesh> ();
	//}
	//
	//public void Toggle()
	//{
	//	if (ToggleNow) return;
	//
	//	tk2dTextMesh inputTextfield = GetComponent<tk2dTextMesh> ();
	//	showingLine = true;
	//	inputTextfield.text = MyName+"|";
	//	inputTextfield.Commit ();
	//	ToggleNow = true;
	//	actualElapsedTime = 0f;
	//	elapsedTime = 0f;
	//}
	//
	//void Update () 
	//{
	//	if ((Input.GetMouseButtonDown (0) || Input.GetKeyDown(KeyCode.Return)) && ToggleNow && actualElapsedTime >= 0.5f) 
	//	{
	//		ToggleNow = false;
	//
	//		if (showingLine) 
	//		{
	//			showingLine = false;
	//			//string newText = targetTextMesh.text;
	//			//newText = newText.Remove(newText.Length-1);
	//			//targetTextMesh.text = newText;
	//			//targetTextMesh.Commit();
	//		}
	//		//MyName = targetTextMesh.text;
	//	}
	//
	//	if (!ToggleNow) return;
	//	actualElapsedTime += Time.deltaTime;
	//
	//	HandleBlinkingEnd ();
	//	HandleKeypadInput ();
	//}
	//
	//public void AddCharToTextfield(string text, bool removeChar = false)
	//{
	//	if (removeChar) 
	//	{
	//		string newText = targetTextMesh.text;
	//		if(newText.Length < 1 || (newText.Length == 1 && newText == "|")) return;
	//		if(newText[newText.Length - 1].ToString() != "|")
	//		{
	//			newText = newText.Remove (newText.Length - 1);
	//		}
	//		else
	//		{
	//			newText = newText.Remove (newText.Length - 2);
	//			newText += "|";
	//		}
	//		targetTextMesh.text = newText;
	//		targetTextMesh.Commit ();
	//		return;
	//	}
	//
	//	if (targetTextMesh.text.Length >= 25) return;
	//	
	//	if (showingLine) 
	//	{
	//		string newText = targetTextMesh.text;
	//		newText = newText.Remove (newText.Length - 1);
	//		newText += text + "|";
	//		targetTextMesh.text = newText;
	//		targetTextMesh.Commit ();
	//	} 
	//	else 
	//	{
	//		targetTextMesh.text += text;
	//		targetTextMesh.Commit();
	//	}
	//}
	//
	//public void HandleBlinkingEnd()
	//{
	//	elapsedTime += Time.deltaTime;
	//
	//	if(elapsedTime >= 1f)
	//	{
	//		elapsedTime = 0f;
	//		if(showingLine)
	//		{
	//			showingLine = false;
	//			string newText = targetTextMesh.text;
	//			newText = newText.Remove(newText.Length-1);
	//			targetTextMesh.text = newText;
	//			targetTextMesh.Commit();
	//		}
	//		else
	//		{
	//			showingLine = true;
	//			targetTextMesh.text += "|";
	//			targetTextMesh.Commit();
	//		}
	//	}
	//}
	//
	//public void HandleKeypadInput()
	//{
	//	string addText = "";
	//
	//	if(Input.GetKeyDown(KeyCode.Backspace))
	//		AddCharToTextfield("", true);
	//	
	//	addText = "";
	//	addText = HandleNumberKeys ();
	//	
	//	if(addText.Length > 0)
	//		AddCharToTextfield(addText);
	//
	//	if (Setting == TextFieldSetting.NumbersOnly) return;
	//
	//	addText = HandleLetterKeys ();
	//
	//	if(addText.Length > 0)
	//		AddCharToTextfield(addText);
	//
	//}
	//
	//public string HandleLetterKeys()
	//{
	//	string addText = "";
	//
	//	if(Input.GetKeyDown(KeyCode.A))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "A";
	//		else
	//			addText += "a";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.B))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "B";
	//		else
	//			addText += "b";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.C))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "C";
	//		else
	//			addText += "c";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.D))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "D";
	//		else
	//			addText += "d";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.E))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "E";
	//		else
	//			addText += "e";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.F))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "F";
	//		else
	//			addText += "f";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.G))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "G";
	//		else
	//			addText += "g";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.H))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "H";
	//		else
	//			addText += "h";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.I))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "I";
	//		else
	//			addText += "i";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.J))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "J";
	//		else
	//			addText += "j";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.K))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "K";
	//		else
	//			addText += "k";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.L))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "L";
	//		else
	//			addText += "l";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.M))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "M";
	//		else
	//			addText += "m";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.N))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "N";
	//		else
	//			addText += "n";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.O))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "O";
	//		else
	//			addText += "o";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.P))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "P";
	//		else
	//			addText += "p";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Q))
	//	{
	//		if(Input.GetKeyDown(KeyCode.AltGr))
	//			addText += "@";
	//		else 
	//			if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//				addText += "Q";
	//			else
	//				addText += "q";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.R))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "R";
	//		else
	//			addText += "r";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.S))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "S";
	//		else
	//			addText += "s";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.T))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "T";
	//		else
	//			addText += "t";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.U))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "U";
	//		else
	//			addText += "u";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.V))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "V";
	//		else
	//			addText += "v";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.W))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "W";
	//		else
	//			addText += "w";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.X))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "X";
	//		else
	//			addText += "x";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Y))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "Y";
	//		else
	//			addText += "y";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Z))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += "Z";
	//		else
	//			addText += "z";
	//	}
	//	
	//	if (Input.GetKeyDown (KeyCode.Space))
	//		addText += " ";
	//
	//	return addText;
	//}
	//
	//
	//public string HandleNumberKeys()
	//{
	//	string addText = "";
	//
	//	if(Input.GetKeyDown(KeyCode.Alpha0))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "=";
	//		else
	//			addText += "0";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha1))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "!";
	//		else
	//			addText += "1";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha2))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "'";
	//		else
	//			addText += "2";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha3))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "§";
	//		else
	//			addText += "3";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha4))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "$";
	//		else
	//			addText += "4";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha5))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "%";
	//		else
	//			addText += "5";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha6))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "&";
	//		else
	//			addText += "6";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha7))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "/";
	//		else
	//			addText += "7";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha8))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : "(";
	//		else
	//			addText += "8";
	//	}
	//	
	//	if(Input.GetKeyDown(KeyCode.Alpha9))
	//	{
	//		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
	//			addText += (int)Setting == 1 ? "" : ")";
	//		else
	//			addText += "9";
	//	}
	//
	//	return addText;
	//}
}
