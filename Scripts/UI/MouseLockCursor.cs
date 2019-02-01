using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseLockCursor : MonoBehaviour {

	public static MouseLockCursor inst;
	public static int mouseShows=0;
	public GameObject eventSystem;


	public void SetInstance(){
		inst = this;
	}
	void Start(){
		#if UNITY_WEBGL && !UNITY_EDITOR
		ShowCursor(true, "mouselock start"); // Freeze the mouse look until player pressed Start. WebGL comm should always call cursor shows when player presses "play now".
		#endif

		UpdateCursor();
//		eventSystem.SetActive(false);
	}



	public static void ShowCursor(bool flag, string source="default"){ // forbid locking the cursor if backpack is selected.
		if (flag) mouseShows++;
		else mouseShows--;
		UpdateCursor();
		if (mouseShows > 0){
			if (GameManager.inst) GameManager.inst.eventSystem.gameObject.SetActive(true);
		} else {
			if (GameManager.inst) GameManager.inst.eventSystem.gameObject.SetActive(false);
		}
//		if (SMW_CHEATS.inst.cheatsEnabled) WebGLComm.inst.Debug("SHOCURSOR:"+flag+" ct:"+mouseShows+" from source:"+source);
//		Debug.Log("SHOCURSOR:"+flag+" ct:"+mouseShows+" from source:"+source);
//		if (mouseShows > 0) inst.eventSystem.SetActive(true); // do'nt need to detect menu clicks if there's no cursor, it's expensives.
//		else inst.eventSystem.SetActive(false);
//		Debug.Log ("ShowCursor: "+flag+","+source+"   mouse shows:"+mouseShows);

//		if (flag) {
////			if (!Player.frozen) {
//			Player.inst.FreezePlayer("mouselock");
//			Player.inst.FreezePlayerFreeLook();
////			}
//		}
//		else {
//			
//			Player.inst.UnfreezePlayer("mouselock");
//			Player.inst.UnfreezePlayerFreeLook();
//		}
//


	}

	bool permalock = false;
	void Update(){
//		Debug.Log("cursor:"+mouseShows);
		
//		if (Input.GetKeyDown(KeyCode.N)){
//			permalock = !permalock;
//			PlayerNowMessage.inst.Display("Perm:" +permalock+", mouseshows:"+mouseShows);
//		}
//
//		if (Input.GetKeyDown(KeyCode.M)){
//			Cursor.visible = false;
//			Cursor.lockState = CursorLockMode.Locked;
//			PlayerNowMessage.inst.Display("Lockonce, mouseshows:"+mouseShows);
//		}
////		// commented Debug.Log("mousehsow;"+mouseShows);
//		if (permalock){
//			Cursor.visible = false;
//			Cursor.lockState = CursorLockMode.Locked;
//		}

//		UpdateCursor();
		if (Input.GetMouseButtonDown(0) || LevelBuilder.inst.levelBuilderIsShowing){
			UpdateCursor();
		}
	}

	static void UpdateCursor () {
//		Debug.Log("cursor lock. mouseshow:"+mouseShows);
		if (mouseShows == 0){
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
//			Time.timeScale = 1;
		}
		else if (mouseShows > 0){
			if (LevelBuilder.inst){
				if (!LevelBuilder.inst.levelBuilderIsShowing) {
					Cursor.visible = true;
				} else {
//					// commented Debug.Log("levelbuilder inst showing");
				}
			} else {
				Cursor.visible = true;
			}
			Cursor.lockState = CursorLockMode.Confined;
//			Time.timeScale = 0;
		}
	}

	public void LockCursor(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}
}
