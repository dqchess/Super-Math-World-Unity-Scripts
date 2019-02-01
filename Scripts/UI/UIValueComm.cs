using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIValueComm : MonoBehaviour {

	// This script will set the properties of the currently selected / placed object
	// Per Level Builder.
	// It calls the UserEditableObject.SetProperties() and is customized per each one with what Key and Value it sets..
	// It has subclasses for what type of UI it supports e.g.
		// Dropdown
		// Text input
	// SetFraction atually has TWO inputs, numerator and denominator, so maybe this script shouldn't live on each one of them

//	public UIValueComm(){
//		OnGameLoaded();
//	}
//
//	virtual public void OnGameLoaded(){
//		
//	}

	virtual public void SetObjectProperties() { AudioManager.inst.PlayClick2();  }

	public void OnEnable(){
		OnMenuOpened();
	}

	virtual public void OnMenuOpened() {}
}
