using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIHoverHelp : MonoBehaviour, IPointerExitHandler {

	public string title = "An object";
	public string description = "This object has some mathy properties. If the player does X then Y happens.";




	public void ShowHoverHelp(){

//		UIInstructionsHelper helper = FindObjectOfType<UIInstructionsHelper>();
//		if (helper){
//			if (helper.helping) {
//				// commented Debug.Log("Quit because hover helping.");
//				return; // Don't show this hover help if we are showing other instructions.
//			}
//		}
//		// commented Debug.Log("not this far.");
		LevelBuilder.inst.hoveringButton = this;
		//		// commented Debug.Log("Showing hover help:"+LevelBuilder.inst.hoveringButton.gameObject.name);
		HoverHelperManager.inst.SetHoverPosition(transform,true);
		HoverHelperManager.inst.SetHelperText(title,description);
		HoverHelperManager.inst.SetTutorialButtonActive(false);
		HoverHelperManager.inst.speechBubbleGraphicParent.SetActive(true);
//		LevelBuilder.inst.hoverTitle.text = LevelBuilder.inst.hoveringButton.title;
//		LevelBuilder.inst.hoverDescription.text = LevelBuilder.inst.hoveringButton.description;
//		LevelBuilder.inst.hoverFadingIn = true;
//		//		LevelBuilder.inst.hoverFadingOut = false;
//		LevelBuilder.inst.hoverParent.transform.position = LevelBuilder.inst.hoveringButton.transform.position + new Vector3(50,-50,0); //camUI.ScreenToWorldPoint(Input.mousePosition);

	}

	public void OnPointerExit(PointerEventData eventData){
//		Debug.Log("ointerex");
		LevelBuilder.inst.HoverHelpOff();
//		if (false == TutorialManager.inst.tutorialActive) 
		HoverHelperManager.inst.HideHoverHelp();

	}
}
