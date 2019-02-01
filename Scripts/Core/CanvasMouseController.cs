using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasMouseController : MonoBehaviour {


	public static CanvasMouseController inst;
	public Text title;
	public Text description;
	public GameObject hardHelp;
	public GameObject softHelp;
	float softHelpTimer = 0f;
	int softHelpTimes = 0;
	public GameObject retrapMouseDialogueChrome;
	public GameObject retrapMouseDialogueFirefox;
	public GameObject gameStartedDialogue;
	public GameObject controlsDialogue;

	// Todo merge this with PlayerDialogue

	public void SetInstance(){
		inst = this;
	}
	public bool showing = false;

	public void ShowControls(){
		if (!controlsDialogue.activeSelf){
			controlsDialogue.SetActive(true);
			Player.inst.FreezePlayerFreeLook();
			MouseLockCursor.ShowCursor(true,"controls open");
			Player.inst.FreezePlayer("showcontrols");
			Inventory.inst.DisableBelt();
		}
	}


	public void ShowRetrapMouseDialogue(bool showPause = false){
		if (LevelBuilder.inst){
			if (LevelBuilder.inst.levelBuilderIsShowing) {
				
				return;
			}
		}
		if (gameStartedDialogueShowing) return;
		if (PauseMenu.paused) return;
		if (Inventory.inst.isShowing) return;
		if (PlayerDialogue.inst.showing) return;
		softHelpTimer = 5f;
		softHelpTimes ++;
		if (softHelpTimes > 2){
			hardHelp.SetActive(true);
			softHelp.SetActive(false); 

		} else {
			hardHelp.SetActive(false);
			softHelp.SetActive(true);
		}
		Inventory.inst.DisableBelt(); 


		if (showPause) PauseMenu.inst.ShowPauseMenu();

		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.showMouseRetrapHelp,1);
		retrapMouseDialogueChrome.GetComponent<SinPop>().Begin();
//		if (WebGLComm.inst.clientBrowser == "Chrome") 
//		else if (WebGLComm.inst.clientBrowser == "Firefox") retrapMouseDialogueFirefox.GetComponent<SinPop>().Begin();
//		else {
//			#if UNITY_EDITOR
//			return; // hmm
//		}
		showing=true;
//		Debug.Log("opened");
//		title.text = t;
//		description.text = d;

	}

	public void CloseCanvasMouseDialogue(){
		Inventory.inst.UpdateBeltPosition();
		if (retrapMouseDialogueChrome.activeSelf) retrapMouseDialogueChrome.GetComponent<ShrinkAndDisable>().Begin();
		if (retrapMouseDialogueFirefox.activeSelf) retrapMouseDialogueFirefox.GetComponent<ShrinkAndDisable>().Begin();
//		Debug.Log("Closed");
		showing=false;
	}
	public void RetrapMouseButtonPressed(){
		if (!LevelBuilder.inst.levelBuilderIsShowing){
			//			// commented Debug.Log("retrap pressed, mouse shows ct before retrap:"+MouseLockCursor.mouseShows);
			MouseLockCursor.inst.LockCursor();
			//			MouseLockCursor
			//			MouseLockCursor.mouseShows = 0; // shouldn't be exposded. ugh
			
		}
	}

	public void ShowGameStartedDialogue(){
		gameStartedDialogueShowing = true;
		Inventory.inst.DisableBelt(); 
		
		gameStartedDialogue.SetActive(true);
		Player.inst.FreezePlayerFreeLook();
	}
	

	public bool gameStartedDialogueShowing = false;
	void Update(){

		// Show soft help at first, if shown too many times too quickly, show harder help (more complex, hit "always allow" for firefox").
		softHelpTimer -= Time.deltaTime;
		if (softHelpTimer < 0) softHelpTimes = 0;

		if (showing && LevelBuilder.inst.levelBuilderIsShowing){
			CloseCanvasMouseDialogue();
		}
		if (Input.GetMouseButtonDown(0)){
			if ((retrapMouseDialogueChrome.activeSelf || retrapMouseDialogueFirefox.activeSelf) && !PauseMenu.paused) CloseCanvasMouseDialogue();
		}
		if (gameStartedDialogueShowing || controlsDialogue.activeSelf){
			if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
				ClosePlayerStartDialogue();
//				CloseControlsDialogue();	
			}
		} 
	}

	public void ClosePlayerStartDialogue(){
		if (gameStartedDialogue.activeSelf){
			gameStartedDialogue.GetComponent<ShrinkAndDisable>().Begin();
			Player.inst.UnfreezePlayerFreeLook();
			Inventory.inst.UpdateBeltPosition();
			gameStartedDialogueShowing = false;
			GameManager.inst.StartGame("close player start");
		}
	}

	public void CloseControlsDialogue(){
		if (controlsDialogue.activeSelf){
//			Inventory.inst.UpdateBeltPosition
			Inventory.inst.UpdateBeltPosition();
			controlsDialogue.SetActive(false);
			Player.inst.UnfreezePlayer("contr closed");
			Player.inst.UnfreezePlayerFreeLook();
			MouseLockCursor.ShowCursor(false,"controls closed");
		}
	}

}
