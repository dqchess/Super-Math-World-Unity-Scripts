using UnityEngine;
using System.Collections;

public class FullScreenManager : MonoBehaviour {


	// As of Mar 15 we are disabiling Unity's ability to "go fullscreen" and instead we rely on user browser going fullscreen 
	// This script exists on a disabled gameobject in the scene

	float timer = 0;
	float interval = .5f;
	public Sprite icon;
	public GameObject enableFullscreenButton;
	public GameObject disableFullscreenButton;

	public static FullScreenManager inst;
	public void SetInstance(){
		inst = this;
	}

	public void ToggleFullScreen(){
		

		if (!Screen.fullScreen){
			EnableFullscreen();

		} else {
			DisableFullscreen();
		}
	}

	public void UpdateButtonVisibility(){
		if (Screen.fullScreen){
			disableFullscreenButton.SetActive(true);
			enableFullscreenButton.SetActive(false);
		} else {
			enableFullscreenButton.SetActive(true);
			disableFullscreenButton.SetActive(false);
		}
	}

	public void EnableFullscreen(){
//		Debug.Log("screen not full.");
		// Screen was not fullscreen. Ask player if they want to go fullscreen, but secretly enable fullscreen already 
		// This is because setting fullscreen has no effect until the NEXT player action, hopefully they press OK.
		// If they press cancel, no biggie, just un-fullscreen them.
		Screen.fullScreen = true;
		PauseMenu.inst.HidePauseMenu();
		if (!LevelBuilder.inst.levelBuilderIsShowing){
			PlayerDialogue.inst.playerPressedOKDelegate += PlayerPressedOK;
			PlayerDialogue.inst.playerPressedCancelDelegate += PlayerPressedCancel;
			PlayerDialogue.inst.playerPressedBackboardDelegate += PlayerPressedBackboard;
			PlayerDialogue.inst.ShowPlayerDialogue("Fullscreen is enabled. OK?","",icon);
		}

		AudioManager.inst.PlayInventoryOpen();
		UpdateCameraFullscreenState();
		timer = 1; 
		UpdateButtonVisibility();
	}

	public void DisableFullscreen(){
//		Debug.Log("screen was full, making false.");
		timer = 1; 
		Screen.fullScreen = false;
		UpdateCameraFullscreenState();
		UpdateButtonVisibility();
	}

	void PlayerPressedBackboard(){
		timer = 1f;
		Screen.fullScreen = true;
		UpdateCameraFullscreenState();
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
		PlayerDialogue.inst.playerPressedBackboardDelegate -= PlayerPressedBackboard;
	}

	void PlayerPressedOK(){
		timer = 1f;
		Screen.fullScreen = true;
		UpdateCameraFullscreenState();
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
		PlayerDialogue.inst.playerPressedBackboardDelegate -= PlayerPressedBackboard;
	}

	void PlayerPressedCancel(){
		timer = 1f;
		Screen.fullScreen = false;
		UpdateCameraFullscreenState();
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
		PlayerDialogue.inst.playerPressedBackboardDelegate -= PlayerPressedBackboard;
	}

	void Update(){
		timer -= Time.deltaTime;
		if (timer < 0) {
			timer = 1f;
			// should check for screen.width vs previous frame screen width
			UpdateCameraFullscreenState();

		}

	}

	void UpdateCameraFullscreenState(){
//		Debug.Log("updating "+Resources.FindObjectsOfTypeAll<AspectUtility>().Length+" cameras");
		foreach(AspectUtility au in Resources.FindObjectsOfTypeAll<AspectUtility>()){
			au.SetCamera(); // expensive
		}
	}

}
