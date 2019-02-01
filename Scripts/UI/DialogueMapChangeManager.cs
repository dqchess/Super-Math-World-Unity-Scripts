using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueMapChangeManager : MonoBehaviour {

	public static DialogueMapChangeManager inst;
	public GameObject confirmButton;
	public Image confirmImage;
	public Text confirmText;
	public UIBooleanSlider slider;

	public void SetInstance(){
		inst = this;
	}

	void OnEnable(){
//		// commented Debug.Log("awake");
		confirmButton.SetActive(false);
		slider.TurnOn();
//		// commented Debug.Log("confirmL"+confirmButton.activeSelf);
	}

//	void Start(){
//		// commented Debug.Log("start"+confirmButton.activeSelf);
//	}
	public Transform mapsList;

	public GameObject confirmDialogue;
	// Player is presented with ~8 map icons they can pick
	// When clicked, it should stay highlighted.
	// After one is selected, the DONE button appears. You cannot deselect all maps just click away and it closes the window.
	// Click DONE button, and it prompts you to SAVE your level first or CONTINUE WITHOUT SAVING (your work will be lost!)
	// After SAVE COMPLETE from the server, change the map on callback.
	// Change the map as before. Easy peasy!


	Transform terrainT;
	public void PreSelectTerrainByTransform(Button b){
		terrainT = b.GetComponent<MapTransformReference>().map;
		foreach(Button bb in mapsList.GetComponentsInChildren<Button>()){
			bb.image.color = new Color(1,1,1,0.8f);
		}
		b.image.color = Color.white;
		b.gameObject.GetComponent<SinPop>().Begin();
		confirmButton.SetActive(true);
		confirmText.text = b.GetComponentInChildren<Text>().text;
		confirmImage.sprite = b.GetComponent<Image>().sprite;
//		sg.factor = 1.05f;
//		sg.speed = 10f;

//		confirmDialogue.GetComponent<SinPop>().Begin();
	}

	public void ConfirmSelection(){
		if (terrainT == null){
			// commented Debug.LogError("Terrain index was -1");
			return;
		}
		GetComponent<ShrinkAndDisable>().Begin();

		ChangeTerrain();
	}




	void ChangeTerrain(){
		GameManager.inst.SetGameState(GameState.ChangingMap,true);
		// TODO Vaidm DRY: This achieves the same functionality as "Create New Level" button on website, but with a specific map.
		// commented Debug.Log("terrain changed:"+terrainT.name);
		if (slider.GetSliderValue() == true){
			SceneManager.inst.ReloadSceneNow();
			Player.inst.ClearPlayerStartPositions();
			GameManager.inst.DestroyAllEphemeralObjects();
		}


		WebGLComm.inst.ClearBrowserLevelCodeForNewLevel();
//		WebGLComm.inst.ClearUnityLevelCode();
//		WebGLComm.inst.UpdateUnityWithCurrentLevelCode(); // Make sure server knows we changed the map and don't save over the previous map/level.

		MapManager.inst.SelectTerrainByTransform(terrainT);
		MapManager.inst.SetCameraZoomByTerrainSize();
		Player.inst.SetPlayerLocationForGameStarted(); // should* result in moving the player to the center of this map, since there won't be any UEO to set player to, since they've all been cleared.
		JsonLevelSaver.inst.ResetUndoHistory("map changer dialogue");
		LevelBuilder.inst.ActionCenterOnPlayer();
		JsonLevelSaver.inst.ResetLevelNameAndDescription();
		LevelBuilderGroupManager.inst.ClearGroups();
		GameManager.inst.SetGameState(GameState.Editing);

	}

}
