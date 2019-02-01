using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



// TODO Serious bug -- Serialization issue. Enum data consistently getting corrupted.
// Relevant thread here: https://www.reddit.com/r/Unity3D/comments/2e9vlg/unity_serialization_is_truly_fubar/
// hrm, maybe the issue is that enum is in wrong scope. Moving it inside now.
// hrm, serialize field for all vars in serializable class? maybe..let's try! relevant thread here: http://forum.unity3d.com/threads/serialization-best-practices-megapost.155352/
// fuck nothing is working, i dont want to do a bunch of mono c# gymnastics to get this to work! I converted my arrays to lists, maybe that will help?
// gonna try serializing my enums this way maybe nexT: http://www.scottwebsterportfolio.com/blog/2015/3/29/enum-serialization-in-unity
// I would pay a lot of money for an asset store solution to serialization. Looking here: http://forum.unity3d.com/threads/released-runtime-serialization-for-unity-extremely-easy-solution-to-save-your-gameplay-data.325812/
// okay.. Scriptable Objects it is. Starting from here: http://gamasutra.com/blogs/MarkWahnish/20150904/252962/Unity_Trick_1__Make_an_inspector_for_any_ScriptableObject.php
// okay.. tried making all the enums strings, but now the problem is the entire serialized class is being cached, enums and all.. damn it.
// okay.. delete library folder? (shrug)
// Cool, nothing worked. Moved the abstract class to a monobehavior and use separate game objects per instruction. THOSE nest in arrays just fine, and I can copy/move them easier too.
// So it's actually better now, although I have no idea how the FUCK to serialize objects in Unity. #wasteoftime






[System.Serializable]
public class UIInstructions{
	[SerializeField] public List<UIInstruction> instructions;
}

public class UIInstructionsHelper : MonoBehaviour {

//	public List<Item> items;


//	[SerializeField] public UIInstructions[] groups;
	[SerializeField] public List<UIInstructions> groups;
//	SerializedProperty
	int instructionsIndex=0;
	int groupIndex=0;

	public string playMessageOne = "Now you're playing the game! Use WASD or arrow keys to move.";
	public string playMessageTwo = "Press TAB to go back to the editor.";

//	public bool helping = false;
	Transform targetHoverTransform;

	void Start(){
//		groups[0].instructions[0].app
	}

	public void Init(){
//		// commented Debug.Log("init. 2");
//		// commented Debug.Log("level builder:"+LevelBuilder.inst);
		instructionsIndex = 0;
		groupIndex = 0;
		InitGroup(0);
		FindObjectOfType<LevelBuilder>().levelBuilderOpenedDelegate += LevelBuilderOpened;
//		LevelBuilder.inst.LevelBuilderObjectPlaced += LevelBuilderObjectPlaced;
		FindObjectOfType<LevelBuilder>().LevelBuilderPreviewClicked += LevelBuilderPreviewClicked;
		HoverHelperManager.inst.hoverParent.SetActive(true);
		// commented Debug.Log("init, move to cur");
		MoveToCurrentInstruction();
	}
	public void LevelBuilderObjectPlaced(GameObject o){
//		// commented Debug.Log("LBO placed:"+o+", o required was:"+CurrentInstruction().go+", event type:"+CurrentInstruction().eventType );
		if (CurrentInstruction().eventType == UIInputType.Placement) {
//		if (CurrentInstruction().eventType == "Placement"){ //UIInstruction.UIInputType.Placement) {
			if (o == CurrentInstruction().go){
				UserFollowedInstruction(o);
			}
		}
	}

	void LevelBuilderPreviewClicked(){
		PlayerNowMessage.inst.Display(playMessageOne,Player.inst.transform.position);
		PlayerNowMessage.inst.DisplayAfterDelay(playMessageTwo,10f);
		PlayerNowMessage.inst.DisplayAfterDelay(playMessageTwo,10f);
	}

	void InitGroup(int i){
		int j=0;
		foreach(UIInstruction ui in groups[i].instructions){
			
//			// commented Debug.Log("group:"+i+", j:"+j);
//			// commented Debug.Log("ui name;"+ui.name);
			DetectMouseClickUI det = ui.go.AddComponent<DetectMouseClickUI>();
			if (ui.alternativeClickGo) ui.alternativeClickGo.AddComponent<DetectMouseClickUI>();
			det.type = ui.eventType;
			j++;
		}
	}


	UIInstruction CurrentInstruction(){
		return groups[groupIndex].instructions[instructionsIndex];
	}
	void MoveToCurrentInstruction(){
//		// commented Debug.Log("move to current:"+CurrentInstruction().title);
//		helping = true;



		// Sometimes, the item we are trying to click is a little farther down the scroll window. Let's detect that and set scroll height appropriately.
		LevelBuilderUIButton uib = CurrentInstruction().go.GetComponent<LevelBuilderUIButton>();
		if (uib){
			PreventClickDrag sr = null; // PreventClickDrag is my method that overrides ScrollRect default .....
			int timesToRecursiveCheckParent = 6;
			Transform currentParent = CurrentInstruction().go.transform;
			while (timesToRecursiveCheckParent > 0 && sr == null) {
				sr = currentParent.parent.GetComponent<PreventClickDrag>();
				timesToRecursiveCheckParent --;
				currentParent = currentParent.parent;
			}
			if (sr){
				
				sr.content.transform.localPosition = sr.contentStartPosition; // snap the scroll pos to the top before potentially modding it.
//				// commented Debug.Log("anchored pos of sr cont:"+sr.content.anchoredPosition.y);
//				// commented Debug.Log("rect anchor y:"+uib.transform.position.y); 
				float ypos = uib.transform.position.y;

				// for world yPos, -461 is at the bottom of the screen, +360 is the top of the screen in real world positions.
				float contentHeight = sr.content.rect.height; 
				// for content height, 575 would represent the entire height of the screen. Not sure how this translates to world units.
				int timesToRecurseMoveScrollbar = 9;
				while (ypos < 0 && timesToRecurseMoveScrollbar > 0){ // ugly
					timesToRecurseMoveScrollbar --;
					ypos = uib.transform.position.y;
//					sr.verticalScrollbar.value -= 0.1f;
					sr.content.anchoredPosition += new Vector2(0,100);
//					// commented Debug.Log("sr vert scrol val:"+sr.verticalScrollbar.value);
				}
//				// commented Debug.Log("scroll rect pos:"+sr.verticalScrollbar.value);
			} else {
//				// commented Debug.Log("no sr found on:"+uib.name);
			}

		}

		// Posiiton the hover helper based on the intruction location.
		targetHoverTransform = CurrentInstruction().targetLocation.transform;
		HoverHelperManager.inst.SetHoverPosition(targetHoverTransform,false,true);// CurrentInstruction().hoverType);
		string title= CurrentInstruction().title;
		string description = CurrentInstruction().description;
		HoverHelperManager.inst.SetHelperText(title,description);
		HoverHelperManager.inst.SetTutorialButtonActive(true);


		TutorialManager.inst.restrictInputBig.SetActive(false);
		TutorialManager.inst.restrictInputSmall.SetActive(false);
//		// commented Debug.Log("both false.");
		switch (CurrentInstruction().restrictInput){
		case RestrictInputUI.Big:
			TutorialManager.inst.restrictInputBig.SetActive(true);
			break;
		case RestrictInputUI.Small:
			TutorialManager.inst.restrictInputSmall.SetActive(true);
			break;
		case RestrictInputUI.None:
			TutorialManager.inst.restrictInputBig.SetActive(true);
			break;
		default:break;
		}


	}

	public void ResizeInputRestrictionPlane(){
		// Disable the circle after lerping.
		switch (CurrentInstruction().restrictInput){
		case RestrictInputUI.Big:
			TutorialManager.inst.restrictInputBig.SetActive(true);
			TutorialManager.inst.restrictInputSmall.SetActive(false);
			break;
		case RestrictInputUI.Small:
			TutorialManager.inst.restrictInputBig.SetActive(false);
			TutorialManager.inst.restrictInputSmall.SetActive(true);
			break;
		case RestrictInputUI.None:
			TutorialManager.inst.restrictInputBig.SetActive(false);
			TutorialManager.inst.restrictInputSmall.SetActive(false);
			break;
		default:break;
		}
//		// commented Debug.Log("and now?");

	}

	float instructionFollowedTimer = 0;
	public void UserFollowedInstruction(GameObject o){


//		// commented Debug.Log("UFI event type:"+CurrentInstruction().eventType.ToString()+", for instruction:"+CurrentInstruction().title);
		if (instructionFollowedTimer > 0) return;
		instructionFollowedTimer = 0.05f;

		if (o == CurrentInstruction().go || o == CurrentInstruction().alternativeClickGo){
			Destroy(o.GetComponent<DetectMouseClickUI>()); // This was a temporary detection component for UI clicks. I know, I know..but it works.
			if (instructionsIndex < groups[groupIndex].instructions.Count - 1){ // instructions have one more step!
				instructionsIndex ++;
				MoveToCurrentInstruction();
			} else {
				// SPECIAL CASE for last instruction -- should I open the video overlay?
				TutorialManager.inst.EndInstructionMode();

			}
		}

	}



	void Update(){
		// For smoother / better lerping, see 
		// http://forum.unity3d.com/threads/a-smooth-ease-in-out-version-of-lerp.28312/
		instructionFollowedTimer -= Time.unscaledDeltaTime;

//		if (CurrentInstruction().eventType == UIInputType.Anything && Input.GetMouseButtonUp(0) && instructionFollowedTimer < -.5f){
//			UserFollowedInstruction(CurrentInstruction().go);
//		}

		// Player re-opened level builder after plaing the game, so look for second group of instructions.


	}
	void LevelBuilderOpened(){
//		// commented Debug.Log("level builder open, from ui instr helper");
		PlayerNowMessage.inst.StopAllCoroutines();
		PlayerNowMessage.inst.Hide();
//		// commented Debug.Log("group index:"+groupIndex+", instr index:"+instructionsIndex);
//		// commented Debug.Log("group len:"+groups.Count+", instr len:"+groups[groupIndex].instructions.Count);
		if (CanMoveToNextGroup()){
//			helping = true;
			groupIndex++;
			InitGroup(groupIndex);
			instructionsIndex = 0;
			MoveToCurrentInstruction();
			HoverHelperManager.inst.hoverParent.SetActive(true);
		} else {
//			// commented Debug.Log("couldn't help.");
//			helping = false;
			TutorialManager.inst.EndInstructionMode();
		}
	}

	bool CanMoveToNextGroup(){
//		// commented Debug.Log("can move? groupindex < groups.len:"+groupIndex+" < " +groups.Count+" ... instindex == curinst.len: "+instructionsIndex+" == "+groups[groupIndex].instructions.Count);
		return groupIndex < groups.Count - 1 && instructionsIndex == groups[groupIndex].instructions.Count - 1;
	}	

	public void UserFinishedPlacingObject(){
		if (CurrentInstruction().eventType == UIInputType.FinishedPlacement) {
			GameObject oo = CurrentInstruction().go; // Redundant..
			UserFollowedInstruction(oo);
		}
	}
}
