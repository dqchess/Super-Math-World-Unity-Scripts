using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public enum NumberFilterType{
	ExactlyX,
	LessThanX,
	GreaterThanX,
	SquaresOnly,
	FactorsOfX,
	MultipleSOfX,
	PrimesOnly,
	CompositesOnly
}

public enum UISubmenuTypes{
	FractionMenu,
	NumberWallSquare,
}

public enum PlayMode {
	Player,
	Editor
}

public enum GameState {
	Playing,
	Editing,
	LoadingInstance,
	LoadingClass,
	SavingInstance,
	SavingClass,
	ChangingMap
}

public class GameManager : MonoBehaviour {


	public GameState state = GameState.Editing;

	public void SetGameState(GameState st, bool instant = false){
		if (!instant){
			StartCoroutine(SetGameStateE(st));
		} else {
//			Debug.Log("setting (instant) state:<color=#0f0>"+st+"</color>");
			state = st;
		}
	}
	IEnumerator SetGameStateE(GameState st){
		// need to wait for end of frame as some objects may be destroyed only betweeen frames 
		// (e.g. don't switch to "edit" mode until all objects finished destroying from "loading" mode.
		yield return new WaitForSeconds(0.01f); 
		
//		Debug.Log("setting game state:<color=#0f0>"+st+"</color>");
		state = st;
	}

	public static int treeBillboardDistance = 500;
//	public static float version = 1.0f; // this was never implemented for the first year of development, so version actually start at 1.1. "unversioned" versions are considered 1.0
//	public static float version = 1.1f; // added more detail to position and rotation, but now rotations are off e.g. saved as 180 but loaded as 179 (super annoying), moving to euler
//	public static float version = 1.101f; // adding zeros for more control over versioning.
//	public static float version = 1.102f; // grouping, then new highlighting, new LevelBuilderObjectManager tracking
	public static float version = 1.103f; // using base64 string for json serialization

	public static string versionKey = "build_version";
	public GameObject waterGroup;
	public GameObject inGameHud;



	public static GameManager inst;
	public float unixTime = -1;
	public AnalyticsManager analyticsManager;
	public AnimalBehaviorManager animalBehaviorManager;
	public AudioManager audioManager;
	public CanvasMouseController canvasMouseController;
	public CameraLayers cameraLayers;
	public ClipboardManager clipboardManager;
	public DebugText debugText;
	public DialogueMapChangeManager dialogueMapChangeManager;
	public EditorTesting editorTesting;
	public EffectsManager effectsManager;
	public BackgroundAudioManager BackgroundAudioManager;
	public EventSystem eventSystem;
	public FlowerManager flowerManager;
	public FramerateTester framerateTester;
	public FPSInputController fps;
	public HeatmapManager heatmapManager;
	public HoverHelperManager hoverHelperManager;
	public GadgetThrow gadgetThrow;


	public InGameHUD inGameHUD;
	public Inventory inventory;
	public JsonLevelLoader jsonLevelLoader;
	public JsonLevelSaver jsonLevelSaver;
	public LevelBuilderObjectManager levelBuilderObjectManager;
	public LevelBuilder levelBuilder;
	public LevelBuilderCamSkyManager levelBuilderCamSkyManager;
	public LevelBuilderEventManager levelBuilderEventManager;
	public LevelBuilderGroupManager levelBuilderGroupManager;
	public LevelBuilderTabManager levelBuilderTabManager;
	public LevelBuilderMessager levelBuilderMessager;
	public MapManager mapManager;
	public MainCameraCullingManager mainCameraCullingManager;
	public MouseLockCursor mouseLockCursor;
	public MascotAnimatorController mascotAnimatorController;
	public MarketUI marketUi;
	public NumberManager numberManager;
	public NumberPool numberPool;
	public PauseMenu pauseMenu;
	public Player player;
	public PlayerUnderwaterController playerUnderwaterController;
	public PlayerWalkDustParticles playerWalkDustParticles;
	public PlayerCostumeController playerCostumeController;
	public PlayerVehicleController playerVehicleController;
	public PlayerNowMessage playerNowMessage;
	public PlayerNowMessageWithBox playerNowMessageWithBox;
	public PlayerGadgetController playerGadgetController;
	public PlayerTextToSpeech playerTextToSpeech;
	public PlayerDialogue playerDialogue;
	public PressKeyDialogue pressKeyDialogue;
//	public SavedSelectionManager savedSelectionManager;
	public SMW_CHEATS smw_cheats;
	public Screenshotter screenshotter;
	public SceneLayerMasks sceneLayerMasks;
	public SceneManager SceneManager;
	public TeacherLevelRestrictionManager teacherLevelRestrictionManager;
//	public TutorialManager tutorialManager;
	public UIValueCommAnimalRules uIValueCommAnimalRules;
	public UltimateToonWaterC ultimateToonWaterC;
	public UIValueCommClipboard uIValueCommClipboard;
	public UpdateManager updateManager;
	public VehicleManager vehicleManager;
	public VideoRecorder videoRecorder;
	public WebGLComm webGlComm;

	private bool gameFrozen = false;
	public bool GameFrozen {
		get {
			return gameFrozen;
		}
		set {
			gameFrozen = value;
		}
	}

	void Start () {

		AudioListener.volume = 0; // No audio until game started.
		SetInstances();

		JsonLevelLoader.inst.onLevelLoadedDelegate += LevelLoaded;


	}

	public void SetInstances(){
		inst = this;
		mainCameraCullingManager.SetInstance();
		levelBuilderMessager.SetInstance();
		clipboardManager.SetInstance();
//		savedSelectionManager.SetInstance();
		updateManager.SetInstance();
		smw_cheats.SetInstance();
		BackgroundAudioManager.SetInstance();
		SceneManager.SetInstance();
		framerateTester.SetInstance();
		editorTesting.SetInstance();
		levelBuilderGroupManager.SetInstance();
		cameraLayers.SetInstance();
		debugText.SetInstance();
		videoRecorder.SetInstance();
		ultimateToonWaterC.SetInstance();
		levelBuilderCamSkyManager.SetInstance();
		analyticsManager.SetInstance();
		fps.SetInstance();
		mapManager.SetInstance();
		levelBuilderTabManager.SetInstance();
		levelBuilderEventManager.SetInstance();
		numberPool.SetInstance();
		playerWalkDustParticles.SetInstance();
		canvasMouseController.SetInstance();
		dialogueMapChangeManager.SetInstance();
		playerCostumeController.SetInstance();
		screenshotter.SetInstance();
		flowerManager.SetInstance();
		mascotAnimatorController.SetInstance();
		animalBehaviorManager.SetInstance();
		effectsManager.SetInstance();
		audioManager.SetInstance();

		numberManager.SetInstance();
		playerNowMessage.SetInstance();
		playerNowMessageWithBox.SetInstance();
		inventory.SetInstance();
		inGameHUD.SetInstance();
		levelBuilder.SetInstance(); // = levelBuilder;
		webGlComm.SetInstance();
		jsonLevelLoader.SetInstance();
		jsonLevelSaver.SetInstance();
		mouseLockCursor.SetInstance();
		vehicleManager.SetInstance();
		pauseMenu.SetInstance();
		player.SetInstance();
		playerUnderwaterController.SetInstance();

		sceneLayerMasks.SetInstance();
		playerDialogue.SetInstance();
		pressKeyDialogue.SetInstance();
		levelBuilderObjectManager.SetInstance();
		uIValueCommAnimalRules.SetInstance();
		uIValueCommClipboard.SetInstance();
		hoverHelperManager.SetInstance();
//		tutorialManager.SetInstance();
		playerVehicleController.SetInstance();
		playerGadgetController.SetInstance();
		playerTextToSpeech.SetInstance();
		SetScrollSpeeds();
//		SetScrollTop();
		heatmapManager.SetInstance();
		teacherLevelRestrictionManager.SetInstance();

		marketUi.SetInstance();

	}

	public void SetThrowGadgetInstance(GadgetThrow g){
		g.SetInstance();
	}

	public void SetScrollTop(){
//		foreach(ScrollRect sr in Resources.FindObjectsOfTypeAll<ScrollRect>()){
//			Canvas.ForceUpdateCanvases();
			//			sr.verticalScrollbar.value = 0;
//			sr.verticalNormalizedPosition = 1;
//			// commented Debug.Log("sr vert val:"+sr.verticalScrollbar.value);
			//			scrollRect.verticalScrollbar.value=0f;
//			Canvas.ForceUpdateCanvases();
//		}
	}

	void SetScrollSpeeds(){
//		return;
		foreach(ScrollRect sr in Resources.FindObjectsOfTypeAll<ScrollRect>()){
			#if UNITY_EDITOR
			sr.scrollSensitivity = 100;
//			// commented Debug.Log("#3");
			#else
			sr.scrollSensitivity = 15; // note that 15? seems to be the lowest value that will detect a single click of the mouse wheel. Weird that the value here is MUcH lower than editor value with similar results
			#endif
		}
	}


	// Update is called once per frame
	public float timeOnThisLevel = 0;
	void Update () {
//		if (Time.frameCount % 1 == 0)
//		{
//			System.GC.Collect();
//		}
		timeOnThisLevel += Time.deltaTime;

	}

	public void RestartLevelTimer(){
		timeOnThisLevel = 0;
	}

//	public bool OperationPermission(float requiredStartupTime = 0){
//		return timeOnThisLevel > requiredStartupTime;
//	}

	public void NewLevelWasLoaded(){
//		WebGLComm.inst.Debug("New level loaded.");
		// A level finished loading so restart the session so that we get the level chagne immediately
		// this isn't working -- send analytics doesn't work.
		AnalyticsManager.inst.SendAnalytics(true);
	}

	public void EndGame(string source){
//		Debug.Log("Game ended, source;"+source);
		// a level was ended, meaning a new level was loaded
		gameStarted = false;
	}

	public bool gameStarted = false;
	public void StartGame(string source){
		if (LevelBuilder.inst.levelBuilderIsShowing){
//			Debug.Log("could not start game, levelbuilder showing.;"+source);
			return;
		}
		// when game first loads and user presses ok on game start, oR when level builder is closed.
		if (gameStarted) {
//			Debug.Log("could not start game; already started."+source);
			return;
		} else {
//			Debug.Log("started game:"+source);
		}
		gameStarted = true;
		timeOnThisLevel = 0;
		FPSInputController.inst.motor.momentum = Vector3.zero;
		AudioListener.volume = 1;
		foreach(UserEditableObject ueo in FindObjectsOfType<UserEditableObject>()){
			
			ueo.OnGameStarted();
		
		}
		SetGameState(GameState.Playing);

	}

	public bool CanDisplayDialogue(){
		return gameStarted
			&& !PauseMenu.paused 
			&& !LevelBuilder.inst.levelBuilderIsShowing 
			&& !PlayerDialogue.inst.showing 
			&& !CanvasMouseController.inst.gameStartedDialogueShowing 
			&& !CanvasMouseController.inst.showing
			&& !Inventory.inst.isShowing;
	}

	public void CloseAllPlayerDialogues(){
//		Debug.Log("Close all diag");
		PauseMenu.inst.HidePauseMenu();
		PlayerDialogue.inst.HidePlayerDialogue();
		CanvasMouseController.inst.CloseCanvasMouseDialogue();
		Inventory.inst.HideInventory();
		PlayerNowMessage.inst.Hide();
		PlayerNowMessageWithBox.inst.Hide();
	}

	public void SetVisibleObjects(PlayMode mode){
//		Debug.Log("set visible objs");
		if (mode == PlayMode.Editor){
//			Debug.Log("hide");
			Inventory.inst.HideBelt();
			RenderSettings.fog = false;
			MapManager.inst.waterFX.SetActive(false);
			MapManager.inst.waterFXskyCam.SetActive(true);
			MapManager.inst.utwc.gameObject.SetActive(false);

			MapManager.inst.SetTerrainTreeBillboardDistance();
			GameManager.inst.inGameHud.SetActive(false);
//			Debug.Log("set vis:"+mode);
			CloseAllPlayerDialogues();
		} else if (mode == PlayMode.Player){
			Inventory.inst.ShowBelt();
//			RenderSettings.fog = true;
			MapManager.inst.waterFXskyCam.SetActive(false);
			MapManager.inst.utwc.gameObject.SetActive(true);
			MapManager.inst.waterFX.SetActive(true);
			MapManager.inst.SetTerrainTreeBillboardDistance();
			inGameHud.SetActive(true);
		}
	}

	public void SetPlayerState(PlayMode mode){
		if (mode == PlayMode.Editor){
			PlayerVehicleController.inst.SetPlayerVehicleState(PlayMode.Editor);
			PlayerUnderwaterController.inst.SetPlayerUnderwater(false);
			Player.inst.FreezePlayer("Level builder shown.");
			MouseLockCursor.ShowCursor(true,"levelbuilder show");
			Player.inst.transform.localScale = Vector3.one * 2;
//			Camera.main.enabled = false;
			MainCameraCullingManager.inst.SetCameraMode(PlayMode.Editor);
//			Camera
			Player.inst.Unparent();
			GameManager.inst.GameFrozen = true;
		} else if (mode == PlayMode.Player){
			Player.inst.transform.localScale = Vector3.one;
			GameManager.inst.GameFrozen = false;
			MouseLockCursor.ShowCursor(false,"LB Hide");
			Player.inst.UnfreezePlayer();
//			CanvasMouseController.inst.ShowRetrapMouseDialogue();//"Play mode","Press TAB to get back to the editor later.");
			MainCameraCullingManager.inst.SetCameraMode(PlayMode.Player);
//			Camera.main.enabled = true;
		}
	}

	public void DestroyAllEphemeralObjects(){
//		WebGLComm.inst.Debug("Destroy objs. class, inst count:");
//		Debug.Log("<color=#f00>Detroying all</color>");
		PlayerVehicleController.inst.PlayerGetOutOfVehicle();

		if (PlayerGadgetController.inst.GetCurrentGadget()) PlayerGadgetController.inst.GetCurrentGadget().OnUnequip();

		PlayerUnderwaterController.inst.SetPlayerUnderwater(false,true);

		// Before destroying stuff, 
		// Mute all things that might make noise when they die, e.g. spikeys, sheep, frogs
		List<UserEditableObject> ueos = Utils.FindObjectsOfTypeInScene<UserEditableObject>();
		foreach(UserEditableObject ueo in ueos){
			foreach(IMuteDestroySound mdestroy in ueo.GetComponents(typeof(IMuteDestroySound))){
				mdestroy.MuteDestroy();
			}
		}


		NumberManager.inst.DestroyAllNumbers();

		foreach(UserEditableObject ueo in ueos){
			if (ueo){
				if (ueo.ignoreDestroyFlags)
				Destroy(ueo);
			}
		}


		Inventory.inst.ClearInventory();


//		WebGLComm.inst.Debug("Destroyed "+j+" level builder placed objs");
		//		// commented Debug.Log("Destroyed "+i+" numbers");

	}

	public delegate void OnLevelWasRestartedDelegate();
	public OnLevelWasRestartedDelegate onLevelWasRestartedDelegate;

	public delegate void OnGameStartedDelegate();
	public OnGameStartedDelegate onGameStartedDelegate;


	public void ReloadLevel(){
//		Debug.Log("Relad");
		// This is not always fired on game started
		// Player pressed reload to last checkpoint from pause menu (more likely, they want an instance of last checkpoint, not to reset whole thing.).
		// OR, player pressed yes to reload after boat number destroyed.
		if (onLevelWasRestartedDelegate != null){
			onLevelWasRestartedDelegate();

		}
//		DestroyAllEphemeralObjects();
		Inventory.inst.HideInventory();
		//		WebGLComm.inst.LoadLevelClass();
		WebGLComm.inst.ReloadLevelInstance();

		CloseAllPlayerDialogues();
	}

	public Sprite questionMarkIcon;
	public void AskRestartLevel(string smallText, string bigText,Sprite icon){
		PlayerDialogue.inst.ShowPlayerDialogue(smallText,bigText,icon);

		PlayerDialogue.inst.playerPressedOKDelegate += AskedRestartLevel_Confirmed;
		PlayerDialogue.inst.playerPressedCancelDelegate += AskedRestartLevel_Cancel;

	}

	void AskedRestartLevel_Confirmed () {
		RestartLevel();
		PlayerDialogue.inst.playerPressedOKDelegate -= AskedRestartLevel_Confirmed;
		PlayerDialogue.inst.playerPressedCancelDelegate -= AskedRestartLevel_Cancel;
	}

	void AskedRestartLevel_Cancel () {
		PlayerDialogue.inst.playerPressedOKDelegate -= AskedRestartLevel_Confirmed;
		PlayerDialogue.inst.playerPressedCancelDelegate -= AskedRestartLevel_Cancel;

	}


	public void ForceRestartLevel(string smallText, string bigText,Sprite icon){
		PlayerDialogue.inst.ShowPlayerDialogue(smallText,bigText,icon);
		#if UNITY_EDITOR
		PlayerDialogue.inst.playerPressedOKDelegate += ConfirmRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate += ConfirmRestartLevel;
		#else
		PlayerDialogue.inst.playerPressedOKDelegate += ForcedRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate += ForcedRestartLevel;
		#endif
	}

	public void ForcedRestartLevel(){
		PlayerDialogue.inst.playerPressedOKDelegate -= ForcedRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate -= ForcedRestartLevel;
		ReloadLevel();
	}

	public void RestartLevel(){ // With dialogue
//		#if UNITY_EDITOR
//		ConfirmRestartLevel();
//		#else

		PlayerDialogue.inst.ShowPlayerDialogue("Are you sure?","Restart this level",LevelBuilder.inst.warningIcon);
		PlayerDialogue.inst.playerPressedOKDelegate += ConfirmRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate += CanceledRestartLevel;
//		#endif
		//		LevelBuilder.inst.ShowLevelBuilder();
	}

	void CanceledRestartLevel(){
		PlayerDialogue.inst.playerPressedOKDelegate -= ConfirmRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate -= CanceledRestartLevel;
	}

	 void ConfirmRestartLevel(){
		PlayerDialogue.inst.playerPressedOKDelegate -= ConfirmRestartLevel;
		PlayerDialogue.inst.playerPressedCancelDelegate -= CanceledRestartLevel;
		RestartLevelActual(); // lol naming this way because we already used "restart level" for the restart dialogue popup
	}

	public void RestartLevelActual(){ // no dialogue just restart it now!
		if (onLevelWasRestartedDelegate != null){
			onLevelWasRestartedDelegate();
		}
		WebGLComm.inst.ResetLevelProgress();
		AudioManager.inst.LevelBuilderPreview();
		Inventory.inst.SaveInventory();
		Inventory.inst.HideInventory();
		JsonLevelLoader.inst.LoadTempLevelJson();
		CloseAllPlayerDialogues();
	}

	public void OnGameStarted(){
		// This is called by LEvelLoader whenever a level is loaded.
		if (onGameStartedDelegate != null){
			onGameStartedDelegate();
		}
		foreach(GameObject o in gameStartedInterfaceObjects){
			if (o){
				foreach(IMyGameStarted mg in o.GetComponents<IMyGameStarted>()){
					mg.GameStarted();
				}
			}
		}
	}

	List<GameObject> gameStartedInterfaceObjects = new List<GameObject>();
	public void AddMyGameStartedInterfaceObject(GameObject o){
		gameStartedInterfaceObjects.Add(o);
	}

	void LevelLoaded(){
		gameStartedInterfaceObjects.Clear();
	}

	[System.NonSerialized] public bool mouseScrollReversed = false;
	public void ToggleMouseScrollReversed(){
		mouseScrollReversed = !mouseScrollReversed;
	}


//	public void OnGameStopped(){
//		// user restarted level
//		// 		from pressing 'ok' on a dialogue, 
//		// 		from loading a new level,
//		// 		from opening level builder
//		//      from restarting level via pause menu
//	}

}
