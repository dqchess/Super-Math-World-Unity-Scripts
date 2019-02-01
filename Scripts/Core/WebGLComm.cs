using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;


public class WebGLComm : MonoBehaviour {


	


	public GameObject openGameMakerButton;


//	public string currentLevelCode = "";
	public string levelCodeToUseForLoadLevelClass = "";

	[DllImport("__Internal")]
	private static extern void Hello();
	
	[DllImport("__Internal")]
	private static extern void HelloString(string str);
	
	[DllImport("__Internal")]
	private static extern void PrintFloatArray(float[] array, int size);
	
	[DllImport("__Internal")]
	private static extern int AddNumbers(int x, int y);
	
	[DllImport("__Internal")]
	private static extern string StringReturnValueFunction();
	
	[DllImport("__Internal")]
	private static extern void BindWebGLTexture(int texture);

	[DllImport("__Internal")]
	private static extern void LoginCheck(string s);

	[DllImport("__Internal")]
	private static extern void CookieSet(string s);



	public void GetDisplayMainRenderingResolution(){
		Debug(Display.main.renderingWidth+"px by "+Display.main.renderingHeight);
	}

	public void GetScreenResolution(){
		Debug(Screen.width+"px by "+Screen.height);
	}


	public void DisplayMainSetRenderingResolution(string xy){
		int xx = int.Parse(xy.Split(',')[0]);
		int yy = int.Parse(xy.Split(',')[1]);
		Display.main.SetRenderingResolution(xx,yy);
	}

	public void ScreenSetResolution(string xy){
		int xx = int.Parse(xy.Split(',')[0]);
		int yy = int.Parse(xy.Split(',')[1]);
		Screen.SetResolution(xx,yy,false);
	}

	public void GetMainCameraViewport(){
		Debug("main cam viewport x:"+Camera.main.rect.x+",y:"+Camera.main.rect.y+", w:"+Camera.main.rect.width+"h:"+Camera.main.rect.height);
	}

	public void SetMainCameraViewport(string xy){
		float x = float.Parse(xy.Split(',')[0]);
		float y = float.Parse(xy.Split(',')[1]);
		float w = float.Parse(xy.Split(',')[2]);
		float h = float.Parse(xy.Split(',')[3]);
		Debug("xywh;"+x+","+y+","+w+","+h);
		Camera.main.rect = new Rect(x,y,w,h);
		Debug("cam main pixel rect;"+Camera.main.pixelRect);
		Debug("cam main rect;"+Camera.main.rect);
	}

	public void GetSkyCamViewport(){
		Debug("main cam viewport x:"+LevelBuilder.inst.camSky.rect);
	}

	public void SetSkyCameraViewport(string xy){
		int x = int.Parse(xy.Split(',')[0]);
		int y = int.Parse(xy.Split(',')[1]);
		int w = int.Parse(xy.Split(',')[2]);
		int h = int.Parse(xy.Split(',')[3]);
		Debug("xywh;"+x+","+y+","+w+","+h);
		LevelBuilder.inst.camSky.rect = new Rect(x,y,w,h);
		Debug("sky cam pixel rect;"+LevelBuilder.inst.camSky.pixelRect);
		Debug("sky cam rect;"+LevelBuilder.inst.camSky.rect);
	}

	public static bool captureKeyboardInputWasSet = false;
	void Start() {
		#if !UNITY_EDITOR && UNITY_WEBGL
		if (!captureKeyboardInputWasSet) WebGLInput.captureAllKeyboardInput = false;
		#endif
//		WebGLInput.captureAllKeyboardInput = false;
//		OpenLevelBuilder("");
		#if UNITY_EDITOR

		#else
		Application.ExternalEval( " GAME_LOADER.GameLoaded(); " );
		// commented Debug.Log("Game loaded: Start called from WebGL script. Making (first?) session!");

		#endif
//		GetUnixTimeFromServer();
		// Hello();
		
		// HelloString("This is a string.");
		
//		float[] myArray = new float[10];
//		PrintFloatArray(myArray, myArray.Length);

//		var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
//		BindWebGLTexture(texture.GetNativeTextureID());


		// The following MIGHT solve a bug where if the user tries to leave the page but window.onbeforeunload catches it (a javascript trick to ask user are you SURE you want to close the page?")
		// Before, that would cause stuff to unload even tho app hadn't quit, slowing the game to a crawl.
		// from https://support.gamesparks.net/support/discussions/topics/1000063753


//			string eval = "";
//			eval += "function OnApplicationQuit()";
//			eval += "{";
//			eval += "   GetUnity().SendMessage('" + gameObject.name + "', 'OnApplicationQuit', '');";
//			eval += "   return true;";
//			eval += "}";
//			eval += "window.onbeforeunload = OnApplicationQuit;";
//			Application.ExternalEval(eval);


		// GAme started but mouse is probably not trapped in the window yet.
//		CanvasMouseController.inst.ShowRetrapMouseDialogue("Game loaded!","Press W to move forwards. Space to jump. Click mouse to throw.");
		CanvasMouseController.inst.ShowGameStartedDialogue();
		DetectBrowser();

	}
//	public void OnApplicationQuit()
//	{
//		new EndSessionRequest().SetDurable(true).Send((response) =>{}); // no response expected
//	}




	// The game should update the current level code to NOTHING "" when changing the map, so that Python server will generate a new UUID level code and you won't be saving over an old level that had a different map


	public void SetCaptureKeyboardInput(string s){
//		Debug("Set cap:"+s);
		bool f = s == "true" ? true : false;
		#if !UNITY_EDITOR && UNITY_WEBGL
		WebGLInput.captureAllKeyboardInput = f;
		captureKeyboardInputWasSet = true; // ..what.. well, sicne we have to reload the scene and need to only set this when entire game loaded..sheesh. Just put this stuff in a pre-scene instead of special casing each variable you want to set only once per game session
		#endif
	}

	public void OpenLevelBuilder(string s){
		LevelBuilder.inst.ShowLevelBuilder();
	}

//	public void StartCreatorTutorial(int i){
//		// commented Debug.Log("skip.");
//		TutorialManager.inst.ActivateTutorial(TutorialManager.inst.tutorial_create[i]); //.SetActive(true);
//	}
//
//	public void StartPlayerTutorial(int i){
//		// commented Debug.Log("skip.");
//		TutorialManager.inst.ActivateTutorial(TutorialManager.inst.tutorial_play[i]);
//	}

	int swapType = 0;

	void Update(){
		
		if (SMW_CHEATS.inst.memoryMonitor){
			Debug("mem @ "+Time.time+GetTotalMemory());
		}
	}


	public static string loggedInAsUser = "Default";
	public void LoginAsUser(string un){
//		// commented Debug.Log("logging in as: "+username);
		loggedInAsUser = un;
//		Debug("loged in as:"+un);
		InGameHUD.inst.SetUserAndLevelInfoText(loggedInAsUser,"Unsaved Level!");
		LevelBuilder.inst.playerName.text = loggedInAsUser;
	}



	public void ReportABug(){
		Application.ExternalEval("ReportABug()");
	}

	public void ShowCursor(string f){
		bool flag = f == "true";
		MouseLockCursor.ShowCursor(flag,"external");
	}

	public void CanvasClick(string s=""){
		
//		MouseLockCursor.ShowCursor(true);
//		MouseLockCursor.ShowCursor(false);
//		// commented Debug.Log("Canvas click:"+s);
//		Screen.lockCursor = true;
	}


	public void PointerWasJustLocked(){
		PauseMenu.inst.HidePauseMenu();
		CanvasMouseController.inst.CloseCanvasMouseDialogue();
//		// commented Debug.Log("js called pointer was just locked.");

	}

	public void CanvasMouseOut(string s=""){
//		// commented Debug.Log("canvas mouise out, LB inst:"+LevelBuilder.inst);
//		// commented Debug.Log("canvas mouise out, LB inst is show:"+LevelBuilder.inst.levelBuilderIsShowing);

		CanvasMouseController.inst.ShowRetrapMouseDialogue(); //"Mouse escaped!","Did you tell Chrome \nor Firefox to \nALLOW mouse lock?",true);//"Mouse escaped!","Click anywhere to lock mouse in the window.");
//		Cursor.visible = true;
//		if (PauseMenu.inst) PauseMenu.inst.ShowPauseMenu();
	}

	public void Autosave(string levelJSON){
		Debug("WEBGLComm autosave");
		#if UNITY_EDITOR

		UpdateAutosaveText("");
		#endif
		Application.ExternalEval("LEVEL_SAVER.AutosaveLevelJSON('"+levelJSON+"','"+JsonLevelSaver.inst.GetSavedLevelName()+"');");
	}


	public void Autosave(SimpleJSON.JSONClass N){
		#if UNITY_EDITOR
		UpdateAutosaveText("");
		#endif
		Application.ExternalEval("LEVEL_SAVER.AutosaveLevelJSON('"+N+"');");
	}

	public void AutosaveCallback(string nothing){
		UpdateAutosaveText(nothing); // gotta pass those variables. It's a government variables program.
		JsonLevelLoader.inst.state = LoadingState.Ready;
	}

	public void UpdateAutosaveText(string nothing){
		JsonLevelSaver.inst.UpdateAutosaveText("Autosaved");
	}

	public void SaveLevelClassToServer(string levelJSON, string screenshotByteArray){


//		JsonLevelSaver.inst.SaveLevelClassCallback("OFFLINE");
		Application.ExternalEval("LEVEL_SAVER.SaveLevelClass('"+levelJSON+"','"+screenshotByteArray+"','"+JsonLevelSaver.inst.GetSavedLevelName()+"');");
	}

	public void SaveLevelClassCallback(string nothing=""){ // js function expects an arg to send to unity.
		JsonLevelSaver.inst.SaveLevelClassCallback();
	}

	public void VerifyValidSave(string levelName){
		#if UNITY_EDITOR
		//		LoadLevel(LevelJsonSamples.json1);
		VerifyValidSaveCallback("true");
		#endif
		Application.ExternalEval("LEVEL_SAVER.VerifyValidSave('"+levelName+"');");

	}

	public void VerifyValidSaveCallback(string sflag){
		bool flag = sflag == "true";
		JsonLevelSaver.inst.VerifyValidSaveCallback(flag);

	}

	public void ReloadLevelInstance(){
		// Player reset from pause menu
		#if UNITY_EDITOR

		#endif
		Application.ExternalEval("LEVEL_LOADER.ReloadLevelInstance();");

	}


	// Hate to expose these vars, but necessary (?) for ez callback flow
	// we could store them in the playerprefs json in onscenereloaded btu we're needlessly moving a big string (the level json) which will slow everything down.
	static bool loadingFromPipe = false;
	static string previousLevelCode = "none_yet";
	public void LoadLevelInstanceFromPortalCallback(string json){
		WebGLComm.inst.Debug("Load lev inst from port calbk");
//		Debug("UTY LoadLevelJson(pipe). cur level code:"+currentLevelCode);
		// old level code should still hold, because new code shouldn't be set until after this call completes in JS. right? Race condition?!
		loadingFromPipe = true;
		previousLevelCode = oldLevelCodeForPortal;

		JsonLevelLoader.inst.LoadLevel(json,SceneSerializationType.Instance,loadingFromPipe,false,previousLevelCode); // Note that Unity's current level code won't be updated because the SetUnityLevelCode delegate was never assigned

	}


	public void LoadLevelClassFromJSON(string json){
		
		// callback for LoadLevelClass
//		WebGLComm.inst.Debug("UTY.webglcomm.loadlevelclassjson(json)");
//		JsonLevelLoader.inst.onLevelLoadedDelegate += UpdateUnityWithCurrentLevelCode; 
//		Application.LoadLevel(Application.loadedLevel);
		WebGLComm.inst.Debug("Load lev class json");
		JsonLevelLoader.inst.LoadLevel(json);
		WebGLComm.inst.Debug("Load lev class json 2");

		
	}

	public void LoadEmptyLevel(){
		JsonLevelLoader.inst.LoadLevel("",SceneSerializationType.Class,false);
	}

	public void LoadLevelClass(){
		// TODO: hover loading screen.
		#if UNITY_EDITOR 
		LoadAnotherLevelClass();
		#else
		#endif
//		LevelBuilder.inst.firstTimeOpened = true; // will center on player first time this is loaded. 
		Application.ExternalEval("LEVEL_LOADER.LoadLevelClass();"); // should load whatever the "currentLevelCode" is.

	}

	public void LoadLevelInstanceFromJSON(string json){
		WebGLComm.inst.Debug("Load lev instance json");
		JsonLevelLoader.inst.LoadLevel(json,SceneSerializationType.Instance);
	}


	public void LoadLevelInstanceFromPortal(string code){
		#if UNITY_EDITOR
		string json = PlayerPrefs.HasKey("SavedInstance") ? PlayerPrefs.GetString("SavedInstance") : PlayerPrefs.GetString("SavedClass");
		LoadLevelInstanceFromPortalCallback(json);
		#endif
		Application.ExternalEval("LEVEL_LOADER.LoadLevelInstanceFromPortal('"+code+"');");
//		Debug("UTY portal activated. Destcode:"+code);
	}

	public void CheckLockCursor(){
		Application.ExternalEval("CheckLockCursor()");
	}

	public void CheckLockCursorCallback(string flagstring){
		bool flag = flagstring == "true" ? true : false;
//		PauseMenu.inst.TryResumeCallback(flag);
	}

	public void ApplyFilters(string json){
		if (Application.isPlaying) return;
		SimpleJSON.JSONNode n = SimpleJSON.JSON.Parse(json);
		foreach(SimpleJSON.JSONNode node in n["filters"].AsArray.Childs){
			// commented Debug.Log("node:"+node.AsObject.ToString()+",totring;"+node.ToString()+",value:"+node.Value);
		}
	}

	public static WebGLComm inst;
	public void SetInstance(){
		inst = this;
	}

	public void PlayerPressedStart(string flag){
		MouseLockCursor.ShowCursor(false,"web gl player pressed start");
	}

	public void LinkHelpEMAIL()
	{
		Application.ExternalEval("window.open('mailto:team@supermathworld.com?subject=Question%20on%Super%20Math%20World','_blank')");
//		string t =
//			"mailto:team@supermathworld.com?subject=Question%20on%Super%20Math%20World";
//		Application.ExternalCall("MailTo('"+t+"');");
	}

	public void ArbitraryEval(string s){
		Application.ExternalEval(s);
	}


	public void GetCodes(){
		Application.OpenURL(GameConfig.websiteRoot + "get_level_codes/");
	}


	public void GetUsersLevelCodesCallback(string s){
		string[] codes = s.Split(',');

	}

	public void LoadAnotherLevelInstance(){
		// User pressed "LOAD" from the PauseMenu
		#if UNITY_EDITOR
		//		LoadLevel(LevelJsonSamples.json1);
		string json = PlayerPrefs.HasKey("SavedInstance") ? PlayerPrefs.GetString("SavedInstance") : PlayerPrefs.GetString("SavedClass");
		LoadLevelInstanceFromJSON(json);

		#endif
		Application.ExternalEval("LEVEL_SELECT.PopulateLevelList_LoadLevelInstance()");


	}


	public void LoadAnotherLevelClass(){
		// User pressed "LOAD" from the LevelBuilderUI
		#if UNITY_EDITOR
		//		LoadLevel(LevelJsonSamples.json1);
		string json = PlayerPrefs.GetString("SavedClass");
		LoadLevelClassFromJSON(json);

		#endif


		if (LevelBuilder.inst.levelBuilderIsShowing){ 
			Application.ExternalEval("LEVEL_SELECT.PopulateLevelList_LoadLevelClassForEditor()");
		} else {
			Application.ExternalEval("LEVEL_SELECT.PopulateLevelList_LoadLevelClass()");
		}

	}




	public void GetLevelListForPortal(){
		#if UNITY_EDITOR
		//		// commented Debug.Log("unity editor - skipping external open level list.");
		GetLevelListForPortalCallback("AxFaA");
		#endif
		Application.ExternalEval("LEVEL_SELECT.PopulateLevelList_SetPortalDetination()");
	}


	public void GetLevelListForPortalCallback(string code){
		FindObjectOfType<UIValueCommLinkLevel>().levelCodeText.text = code;
		GetLevelNameFromCode(code);
		FindObjectOfType<UIValueCommLinkLevel>().SetObjectProperties();
	}

	public void GetLevelNameFromCode(string code){
		#if UNITY_EDITOR
//		// commented Debug.Log("unity editor validating code.");
		string levelName = "offline";
		GetLevelNameFromCodeCallback(levelName);
		#endif
		Application.ExternalEval("GetLevelNameFromCode('"+code+"')");
	}

	public void GetLevelNameFromCodeCallback(string levelName){
//		// commented Debug.Log("callback w levelName;"+levelName);
		FindObjectOfType<UIValueCommLinkLevel>().levelNameText.text = levelName;
		FindObjectOfType<UIValueCommLinkLevel>().SetObjectProperties();

	}


	static string characterJson = "";
	public void SetCharacterCostume(string json){
		characterJson = json;
		SetCharacterCostumeOnSceneLoaded();
	}

	public void SetCharacterCostumeOnSceneLoaded(){
		// This needs to be called each time the scene is reloaded since the character object is destroyed
		// we dont' use DontDestroyOnLoad because A) we built it wrong and dont want to refactor that much today and B) partially reloading scene to clear as much memory as possible (gc) to prevent out of mem error
		PlayerCostumeController.inst.InitCharacter(characterJson);
		PlayerCostumeController.inst.SetCharacterMaterials(characterJson);
		LevelBuilder.inst.levelBuilderAvatarCostumeController.InitCharacter(characterJson);
		LevelBuilder.inst.levelBuilderAvatarCostumeController.SetCharacterMaterials(characterJson);
	}

	public void SendAnalytics(string json){
		#if UNITY_EDITOR
//		int fps = 
		#else
		Application.ExternalEval("STUDENT_SESSION.UnityAnalyticsReport('"+json+"');");
		#endif
	}





	public void SaveSingleJpgForVideo(int i, string data){
		Application.ExternalEval( "SaveSingleJpgForVideo('"+i+"','"+data+"');");
	}




	public void InitVideoRecordingCallback(string s){
		VideoRecorder.inst.VideoRecordingInitialized();
	}

	public void RecordingCompleted(string s){
		VideoRecorder.inst.RecordingCompleted();
	}



//	public void GetUnixTimeCallback(string unixTimeString){
//		GameManager.inst.unixTime = float.Parse(unixTimeString);
//	}

	public void GetTeacherLevel(){
		#if UNITY_EDITOR
		#endif
		Application.ExternalEval("GetTeacherLevel()");
	}

	public void GetTeacherLevelCallback(int level){
		TeacherLevelRestrictionManager.inst.SetTeacherLevel(level);
	}

	float memoryUsedLastTime = 0;

	public void DebugOrange(string s){
		#if UNITY_EDITOR
		UnityEngine.Debug.Log("<color=#f70>"+s+"</color>");
		#else
		Application.ExternalEval("UnityDebug('"+s+"','#f70');");
		#endif
	}

	public void Debug(string s){

		#if !UNITY_EDITOR
		// For production build strip out "<color=$ff"> info as it isn't interpreted by the js console.
		s = System.Text.RegularExpressions.Regex.Replace(s,"<.*?>",string.Empty,System.Text.RegularExpressions.RegexOptions.Multiline);

		#endif

		float memoryUsedThisTime = Mathf.RoundToInt(System.GC.GetTotalMemory(false)/1024/1024*100)/100f;
		s = "[ " + memoryUsedThisTime.ToString() + " MB ]  "+s;
		if (s.Length>600) s=s.Substring(0,600);

		#if UNITY_EDITOR
		if (SMW_CHEATS.inst.cheatsEnabled) UnityEngine.Debug.Log(s);
		#else
		Application.ExternalEval("UnityDebug('"+s+"');");
		#endif

	}

//	public void RequestCurrentLevelCode(){
////		WebGLComm.inst.Debug("UTY.webglcomm.updateunitywithcurrentlevelcode()");
//		JsonLevelLoader.inst.onLevelLoadedDelegate -= UpdateUnityWithCurrentLevelCode; 
//		#if UNITY_EDITOR
//		#else
//		Application.ExternalEval("LEVEL_LOADER.UpdateUnityWithCurrentLevelCode();");
//		#endif
//	}

//	public void UpdateUnityWithCurrentLevelCodeCallback(string code){
////		Debug("Unity got new level code;"+code+" with old code;"+currentLevelCode);
//		currentLevelCode = code;
//	}

	public void LoadPlayerInventory(){
		#if UNITY_EDITOR
		//		LoadLevel(LevelJsonSamples.json1);
		string json = PlayerPrefs.GetString("Inventory");
		LoadPlayerInventoryCallback(json);
//		UnityEngine.Debug.Log("loading inv:"+json);
		#else
		Application.ExternalEval("PLAYER_INVENTORY.Load();");
		#endif
	}

	public void LoadPlayerInventoryCallback(string json){
		Inventory.inst.LoadInventory(json);
	}

	public void SavePlayerInventory(string json){
		Application.ExternalEval("PLAYER_INVENTORY.Save('"+json+"');");
	}



	public void SaveLevelInstanceToServer(string json){
		Application.ExternalEval("LEVEL_SAVER.SaveLevelInstance('"+json+"');");
	}
	public void SaveLevelInstanceObjectToServer(SimpleJSON.JSONClass N){
		
		Application.ExternalCall("LEVEL_SAVER.SaveLevelInstanceObject",N);
	}

	public void SaveToServerComplete(){
		JsonLevelLoader.inst.state = LoadingState.Ready;
	}

	public void LogOut(){
		Application.ExternalEval("LogOut()");
	}

	public void ClearBrowserLevelCodeForNewLevel(){
		Application.ExternalEval("LEVEL_LOADER.ClearBrowserCodeForNewLevel()");
	}

//	public void ClearServerLevelCode(){
//		Application.ExternalEval("LEVEL_LOADER.SetCurrentLevelCode('')");
//	}
//	public void ClearUnityLevelCode(){
//		currentLevelCode = "";
//	}




	public void LevelSelectCanceled(string n){
		// User pressed LOAD, then pressed CANCEL
		// Clear the delegate that was set up when we tried to load a level by clicking LOAD
//		JsonLevelLoader.inst.onLevelLoadedDelegate -= UpdateUnityWithCurrentLevelCode; 
	}

	public void RetrapMouseHelp(){
		Application.ExternalEval("Alert(\"Super Math World needs to trap the mouse in order to play. In your Chrome or Firefox browser, you must click 'Always Allow' when prompted to lock the mouse cursor. We apologize for any inconvenience! If you need additional help, email us directly team@supermathworld.com . Thanks\");");
	}

	public void ResetUndoHistory(string s=""){
//		Debug("reset undo history:"+s);
		JsonLevelSaver.inst.ResetUndoHistory();
	}

	public void ResetLevelProgress(){
		// When player restarts the level, lose their checkpoint progress
		#if UNITY_EDITOR

		#else
		Application.ExternalEval("LEVEL_LOADER.ResetLevelProgress();");
		#endif
	}


//	public void SendTownBatteryEvent(){
//		// Filling a town battery is considered a "level completion event."
//		// We record this event so that we may let the teacher know if the user "completed" this level.
//		#if UNITY_EDITOR
//
//		#else
//		Application.ExternalEval("STUDENT_SESSION.RecordTownBatteryCompleted();");
//		#endif
//	}

	public void FinishedVideoRecording(string audioN, string fps){
		Application.ExternalEval("FinishVideoRecording('"+audioN+"','"+fps+"')");
	}
	public void InitVideoRecording(){
		Application.ExternalEval("InitVideoRecording()"); // we must wait for Python to give us a video directory to save our shots to before beginning recording.
	}

	public void PlayEnvironmentAudio(string path){
		Application.ExternalEval("PlayEnvironmentAudio('"+path+"')"); // will init javascript to play audio located at PATH on our server.
	}

	public void PauseEnvironmentAudio(){
		Application.ExternalEval("PauseEnvironmentAudio()"); // pause audio if any was playing.
	}

	public void PlayEditorAudio(string path){
		// User pressed play audio in editor.
		Application.ExternalEval("PlayEditorAudio('"+path+"')"); // will init javascript to play audio located at PATH on our server.
	}

	public void PauseEditorAudio(){
		Application.ExternalEval("PauseEditorAudio()");
	}

	public string oldLevelCodeForPortal = "";
	public void StoreOldLevelCodeForPortal(string code){
		oldLevelCodeForPortal = code;
	}

	public void PlayerCompletedLevel(){
//		Debug("finish");
		Application.ExternalEval("STUDENT_SESSION.RecordPlayerCompletedLevelEvent();");
	}

	public void PlayerWindowFocused(string flag){
		if (flag == "true") {
			// Player just switched to window. Stop input
			FPSInputController.inst.motor.inputMoveDirection = Vector3.zero;
		}
		
	}

	public void DetectBrowser(){
		Application.ExternalEval("DetectBrowser()");// 'SendMessageToUnity(\'DetectBrowserCallback\')');");
	}

	public string clientBrowser = "";
	public void DetectBrowserCallback(string browser){
		clientBrowser = browser;
	}

	public void GetHeatmapDataForLevel(){
		Application.ExternalEval("GetHeatmapDataForLevel()");// 'SendMessageToUnity(\'DetectBrowserCallback\')');");
	}

	public void GetHeatmapDataForLevelCallback(string json){
		HeatmapManager.inst.SetHeatmapData(json);
	}

	public void EnableHeatmapDebug(){
		HeatmapManager.inst.debug = true;
	}

	public void DisableHeatmapDebug(){
		HeatmapManager.inst.debug = false;
	}

	public Texture2D invisibleTex;

	public void SaveClipboard(string json){
		Application.ExternalEval("CLIPBOARD.Save('"+json+"');");
	}

	public void LoadClipboard(){
		Application.ExternalEval("CLIPBOARD.Load()");
	}

	public void LoadClipboardCallback(string json){
		ClipboardManager.inst.LoadClipboardCallback(json);
	}

	public bool jsDebugEnabled = false;
	public void EnableJsDebug(){
		// can activate from js console
		jsDebugEnabled = true;
	}

	GameObject tempCallbackObj;
	string tempCallbackMethod; // lol because passing these around in an asynch javascript/ajax is annoying 
	public void GetLevelScreenshot(string code, GameObject callbackObj, string callbackMethod){
		tempCallbackObj = callbackObj;
		tempCallbackMethod= callbackMethod;
//		#if UNITY_EDITOR
//		GetLevelScreenshotCallback("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");
//		#endif
		Application.ExternalEval("LEVEL_LOADER.GetLevelScreenshot('"+code+"');");
	}

	public void GetLevelScreenshotCallback(string im){

		Texture2D shot = new Texture2D(Screenshotter.resWidth, Screenshotter.resHeight, TextureFormat.RGB24, true);
		byte[] bArray = System.Convert.FromBase64String(im);
		shot.LoadImage(bArray);
		tempCallbackObj.SendMessage(tempCallbackMethod,shot,SendMessageOptions.DontRequireReceiver);


	}

	public void LoadLevelClassForEditor(){
		Application.ExternalEval("LEVEL_LOADER.LoadLevelClassForEditor();");
	}

	public void LoadLevelClassForEditorCallback(string json){
		JsonLevelLoader.inst.LoadLevel(json);
	}

	#region build_testing // these are written for Brian to circumvent the mouse click required input by user, to simulate the clicking of OK in dialogue box
	public void ClickDialogueOK(string message){
		
		PlayerDialogue.inst.PlayerPressedOK();
	}

	public void SetCurrentLevelCodeUnity(string c){
		SMW_CHEATS.inst.debugText1.text = "curlev:"+c;
	}
	public void SetAutosaveInterval(string s){
		JsonLevelSaver.inst.recursiveSaveInterval = float.Parse(s);
		JsonLevelSaver.inst.AutosaveNow();
	}
	public string GetTotalMemory(string b="f"){
		bool flag = b == "true";
		string mem = System.GC.GetTotalMemory(flag).ToString();
//		Debug("mem:"+mem);
		return mem;
	}

//	public void PremiumCheck(){
//		Application.ExternalEval('PREMIUM.CheckStatus()');
//	}
//
//	public void PremiumCheckCallback(string flagstring){
//		bool flag = flagstring == "true";
//
//	}
//

	public void GetEnvironmentAudiosFromBrowser(){
		#if UNITY_EDITOR
		GetEnvironmentAudiosFromBrowserCallback();
		#else
		Application.ExternalEval("GetEnvironmentAudiosFromBrowser()");
		#endif
	}


	public void GetEnvironmentAudiosFromBrowserCallback(string json= "{\"tracks\":[{\"name\":\"Mysterious Woods\",\"file\":\"mysterious_woods.mp3\"},{\"name\":\"Sacred Temple\",\"file\":\"sacred_temple.mp3\"},{\"name\":\"Dreamscape\",\"file\":\"dreamscape.mp3\"},{\"name\":\"Ancient Gladiator\",\"file\":\"ancient_gladiator.mp3\"},{\"name\":\"Jungle\",\"file\":\"jungle_ambient.mp3\"},{\"name\":\"None\",\"file\":\"none.mp3\"}]}"){
		Debug("Recieved audios from browser:"+json);		
		SimpleJSON.JSONClass N = (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(json);
		BackgroundAudioManager.inst.PopulateBackgroundAudiosFromBrowser(N);


	}

	#endregion

}