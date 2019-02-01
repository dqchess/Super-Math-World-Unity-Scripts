using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PauseMenu : MonoBehaviour {

	public static PauseMenu inst;
	public GameObject pauseMenu;
	public GameObject holdButton;
	public Text broswerLock;
	public bool canPause=true;
	public static bool paused=false;
	public GameObject statsButton;
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		statsButton.SetActive(false);
//		Debug.Log("start, pause:"+paused);
		paused = false;
		Time.timeScale = 1;
	}

	public void ShowPauseMenu(){
		// TODO we really need a DIALOGUE MANAGER.
//		Debug.Log("show");
		if (paused) return;
		if (!GameManager.inst.gameStarted) return;
		Inventory.inst.DisableBelt(); 
		
		Inventory.inst.HideInventory(true);
		if (!GameManager.inst.CanDisplayDialogue()) return;
		MarketUI.inst.HideMarket(); // ugh use interface for closing all these.

		pauseMenu.GetComponent<SinPop>().Begin();
		Time.timeScale = 0;
//		WebGLComm.inst.Debug("paused at "+Time.time+", scale : 0");
//		tryResume = false;
		if (LevelBuilder.inst) {
			if (LevelBuilder.inst.levelBuilderIsShowing) return;
		}
//		Debug.Log("pause true");
		paused = true;
		PlayerDialogue.inst.HidePlayerDialogue();
		CanvasMouseController.inst.CloseCanvasMouseDialogue();
		CanvasMouseController.inst.CloseControlsDialogue();
		CanvasMouseController.inst.ClosePlayerStartDialogue();

//		AudioListener.pause = true;
		MouseLockCursor.ShowCursor(true,"SHOW PAUSE");
		Player.inst.FreezePlayer("PAUSE presesed.");
		foreach(CCText t in pauseMenu.GetComponentsInChildren<CCText>()){
			t.enabled = true;
		}
//		Player.
//		// commented Debug.Log("paused..");
	}

	public void UserClickedContinue(){
		HidePauseMenu();
	}

	public void HidePauseMenu(bool force = false){
		if (!paused && !force) return;
		Inventory.inst.UpdateBeltPosition();
		MouseLockCursor.ShowCursor(false,"HIDE PAUSE");
//		Inventory.inst.ShowInventory(true);
		Time.timeScale = 1;
//		Inventory.inst.beltParent.gameObject.SetActive(true);
//		Inventory.inst.ShowInventory(true);
		paused = false;
		pauseMenu.GetComponent<ShrinkAndDisable>().Begin(); 
//		Time.timeScale = 1;
//		Camera.main.GetComponent<AudioListener>().pause = false;
//		AudioListener.pause = false;
		if (Player.inst) Player.inst.UnfreezePlayer("PAUSE un-pressed.");
		foreach(CCText t in pauseMenu.GetComponentsInChildren<CCText>()){
			t.enabled = false; // an unfortunate, fragile, way to disable all cctext when pause menu is being closed so that those CCText do not appear "on top" of other ui elements
			// they will appear "on top" because we moved CCText towards the camear in Z-plane in pause menu so that they would be displayed correctly to user
			// We use CCText with real Z position here because Unity's UI text does not scale correctly and looks weird kearning/thickness at large scales
		}


	}


	void Update(){
//		if (Input.GetKeyDown(KeyCode.Tab)){
//			WebGLComm.inst.Debug("time;"+Time.timeScale+", gamestdiag;"+CanvasMouseController.inst.gameStartedDialogueShowing+", paused;"+paused+", levbuildshow;"+LevelBuilder.inst.levelBuilderIsShowing);
//		}
		if ((Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab)) && !CanvasMouseController.inst.gameStartedDialogueShowing) {
			if (!paused && !LevelBuilder.inst.levelBuilderIsShowing){
				ShowPauseMenu();
			} else {
			}
		}
	}

	public void EnableStatsButton(){
		statsButton.SetActive(true);
	}

}
