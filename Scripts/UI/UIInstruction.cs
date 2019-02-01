using UnityEngine;
using System.Collections;

public enum UIInputType {
	MouseDown,
	MouseUp,
	Placement,
	FinishedPlacement,
	Anything
}

public enum RestrictInputUI {
	Big,
	Small,
	None
}

public enum HoverHelperType {
	Bottom,
	Left
}

public class UIInstruction : MonoBehaviour {
	public string title;
	public string description;
	public GameObject go;
	public GameObject alternativeClickGo;
	public Transform targetLocation;
//	public HoverHelperType hoverType = HoverHelperType.Left;
	public UIInputType eventType = UIInputType.MouseDown;
	public RestrictInputUI restrictInput = RestrictInputUI.Big;
}
