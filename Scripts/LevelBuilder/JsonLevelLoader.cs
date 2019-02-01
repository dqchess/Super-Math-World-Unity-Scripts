using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public enum LoadingState {
	Ready,
	LoadingClass,
	SavingClass,
	LoadingInstance,
	SavingInstance
}


public class JsonLevelLoader : MonoBehaviour {

/*
 * 
 * Saving and loading of levels flow
 * If you loaded an existing level and saved and level.user = request.user, replace it
 * If you loaded an existing level and saved and level.user != request.user, create new, save, set new currentlevelcode
 * If you created a new level and saved, create new currentlevelcode
 * If you changed the map, create new and set new currentlevelcode
 * 
 * 
 * TODOS
 * 	Undo should compare the two jsons and only change the diff
 * 
*/
	public LoadingState state = LoadingState.Ready;

	public static JsonLevelLoader inst;
	// For UI element key variables for JSON
	public bool firstLoadCompleted = false;
	public float thisLevelVersion = 0.0f;


	public void SetInstance(){
		inst = this;
	}

	public static bool clearedTempJson = false; // need to clear this only when game loaded the first time but not on subsequent scene reloads... this is definitly not the right way to do it
	void Start(){
		#if !UNITY_EDITOR
		if (!clearedTempJson){
			clearedTempJson = true;
			WebGLComm.inst.Debug("<color=#0f0>Loader:</color>Delete tempjson key");
			PlayerPrefs.DeleteKey("tempLevelJson");
		}
		#endif
	}
	public GameObject loadingDialogue;


	bool SearchForPipeToPlacePlayerThere(string previousLevelCode){
//		WebGLComm.inst.Debug("UTY search portal.");
		bool debugPortal = true;
		// Player entered a pipe, and maybe the level he's loading (level B) has a pipe that links back to the level he's currently on (level A).
		// If a pipe is found on new level B that links back to level A, place the player at this pipe instead of at the "Start" position.
		// Otherwise, keep "needssetplayerposition" as true and place player normally.
		if (debugPortal) WebGLComm.inst.Debug("Trying to look for a portal pipe with previous code;"+previousLevelCode);
		if (previousLevelCode != ""){
			int j = 0;
			// We just loaded a new level -- look through all those pieces to see if we find a matching portal pipe.
			foreach(LinkLevelPortalPipe pp in FindObjectsOfType<LinkLevelPortalPipe>()){
				if (pp.destroyedThisFrame) continue;
				if (pp.destinationCode == previousLevelCode) { // wow the fragileness
					if (debugPortal) WebGLComm.inst.Debug("success! Matched "+previousLevelCode+" and moving player.");
					Player.inst.AddPlayerStartPriority(PlayerStartType.Pipe,pp.playerOutT);
					return true;
				} else {
					if (debugPortal)  WebGLComm.inst.Debug("failed Matched "+previousLevelCode+" != "+pp.destinationCode);
				}
				j++;
			}
		}
		return false;
	}

	public SimpleJSON.JSONClass SetGlobalLevelProperties(SimpleJSON.JSONClass N, bool centerOnPlayer){
		WebGLComm.inst.Debug("LevelLoader.SetGlobalProperties.");
		// This is where GameStarted, either beacuse user loaded a level instance from Load screen, restarted a level, lor pressed 'play' from editor.




		if (N["AnimalBehaviorRules"] != null) AnimalBehaviorManager.inst.SetAnimalRules(N["AnimalBehaviorRules"]);

		if (N["Name"] == null || N["Name"] == "") {
			LevelBuilder.inst.levelNameInput.text = "Unnamed Level";
		} else {
			LevelBuilder.inst.levelNameInput.text = N["Name"];
			InGameHUD.inst.SetUserAndLevelInfoText(WebGLComm.loggedInAsUser,N["Name"].Value);
//			WebGLComm.inst.UpdateClientBrowserWithCurrentLevelName();
		}

		string terrainName = N["Terrain"].Value;

		MapManager.inst.SelectTerrainByName(terrainName,centerOnPlayer);
		//		CheckExistingObjects();

//		if (!LevelBuilder.inst.levelBuilderIsShowing){
////			foreach(UserEditableObject in LevelBuilderObjectManager.inst.GetPlacedObjects(SceneSerializationType.Instance)){
//			foreach(UserEditableObject ueo in FindObjectsOfType<UserEditableObject>()){
//				ueo.OnGameStarted();
//			}
//		}
		GameManager.inst.StartGame("loader");
		// TODO: Move a bunch of this stuff above to game manager ongamestarted.
		StartCoroutine(GameStartedAfterSeconds(.2f)); // can't do this same frame because old stuff will get touched and new instantiated stuff isnt there yet.


		return N;
	}

	IEnumerator GameStartedAfterSeconds(float s){
//		WebGLComm.inst.Debug("game started after seconds:"+s);
		yield return new WaitForSeconds(s);
		GameManager.inst.OnGameStarted();
		Player.inst.SetPlayerLocationForGameStarted();
//		WebGLComm.inst.Debug("game started finished after seconds:"+s);
	}

	public bool levelIsBeingLoaded = false;
	public void LoadLevel(string json, SceneSerializationType type=SceneSerializationType.Class, bool loadingFromPipe=false, bool centerOnPlayer=true, string previousLevelCode=""){
	
//		Debug.Log("<color=#fff> BREAKPOINT </color>");
		Player.inst.SlowFadeInBlack();
		SceneManager.inst.ReloadSceneNow();
//		Application.LoadLevel(Application.loadedLevel);
		StartCoroutine(LoadLevelE(json,type,loadingFromPipe,centerOnPlayer,previousLevelCode));
	}
	IEnumerator LoadLevelE(string json, SceneSerializationType type=SceneSerializationType.Class, bool loadingFromPipe=false, bool centerOnPlayer=true, string previousLevelCode=""){
		yield return new WaitForEndOfFrame(); // wAit for scene to reload (needs 2 frames)
		yield return new WaitForEndOfFrame(); // wAit for scene to reload
		if (type == SceneSerializationType.Class) state = LoadingState.LoadingClass;
		if (type == SceneSerializationType.Instance) state = LoadingState.LoadingInstance;
		GameManager.inst.EndGame("json level loader e");
		WebGLComm.inst.Debug("<color=#0f0>Loading</color> json of len:"+json.Length+", sertype;"+type+",str;");
		if (type == SceneSerializationType.Class) GameManager.inst.SetGameState(GameState.LoadingClass);
		else if (type == SceneSerializationType.Instance) GameManager.inst.SetGameState(GameState.LoadingInstance); // we are duplicating LoadingState.LoadingInstance .. heh
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();


//		Debug.Log("LOAD:"+json);
//		WebGLComm.inst.Debug("UTY load level type:"+type);

//		Debug.Log("level being loaded");
		Player.inst.ClearPlayerStartPositions();
		if (LevelBuilder.inst.levelBuilderIsShowing){
			loadingDialogue.SetActive(true);
		}

//		if (json.Length > 25) WebGLComm.inst.Debug("Load level:"+type.ToString()+",json:"+json.Substring(0,45)+" cp;"+centerOnPlayer+", loadfrompipe:"+loadingFromPipe+",oldcode:"+previousLevelCode);
//		else WebGLComm.inst.Debug("Load level but no json:"+type.ToString()+",loadfrompipe;"+loadingFromPipe);


		GameManager.inst.DestroyAllEphemeralObjects(); // inventory destroyed here.
		if (json == "") {
			// Assume a new 
			MapManager.inst.SelectTerrainByName("Flatland");
			JsonLevelSaver.inst.ResetLevelNameAndDescription();
			WebGLComm.inst.Debug("json null, loading flatland");
			LevelBuilder.inst.ActionCenterOnPlayer();
//			LevelBuilder.inst.loadingLevelObjectScreen.SetActive(false);
		} else {
//			N = 
			N = JsonUtil.GetJsonFromString(json);
//			yield break;

			SetJsonVersion(N);
//			Debug.Log("<color=#f0f>OBJS LD:"+N["Objects"].Count+"</color>");


	//		WebGLComm.inst.Debug("N tostring:"+N.ToString());
			if (type == SceneSerializationType.Instance){
				centerOnPlayer = false;
				LoadLevelInstance(N,loadingFromPipe,previousLevelCode);
//				WebGLComm.inst.GetPlayerInventory();
			} else if (type == SceneSerializationType.Class){
				LoadLevelClass(N,centerOnPlayer);
			}
		}
		centerOnPlayer = !firstLoadCompleted; // only center on player on the first open of levlebuilder.
		firstLoadCompleted = true;
		SetGlobalLevelProperties(N,centerOnPlayer);

		if (N.GetKeys().Contains("BackgroundAudio")){
			string ba = N["BackgroundAudio"];
			BackgroundAudioManager.inst.SetGameBackgroundAudio(ba);
//			WebGLComm.inst.Debug("got background audio:"+ba+" for load lev:"+type);
			//				BackgroundAudioManager.inst.PlayEnvironmentAudio(N["BackgroundAudio"]);
		} else {
//			WebGLComm.inst.Debug("no key for bg audio on load lev "+type);
		}
		if (type == SceneSerializationType.Instance || !LevelBuilder.inst.levelBuilderIsShowing){
			// If we loaded an instance, play audio immediately.
			BackgroundAudioManager.inst.PlayGameBackgroundAudio();
		}

		if (N.GetKeys().Contains(LevelBuilderGroupManager.groupManagerKey)){
			LevelBuilderGroupManager.inst.SetProperties(N[LevelBuilderGroupManager.groupManagerKey].AsArray);
		}



		OnLevelLoaded(type);
		if (type == SceneSerializationType.Class){
			// awkward way to make sure the json we just loaded is now saved locally.
			JsonLevelLoader.inst.SetTempLevelJsonPlayerPrefs(json);

		}

	}




	public delegate void OnLevelLoadedDelegate();
	public OnLevelLoadedDelegate onLevelLoadedDelegate;
	public void OnLevelLoaded(SceneSerializationType type){
//		Debug.Log("On level loaded delegate firing");
		loadingDialogue.SetActive(false);
//		WebGLComm.inst.Debug("UTY.levelloader.onlevel loaded (deleagte)");
		GameManager.inst.RestartLevelTimer();

		if (onLevelLoadedDelegate != null) {
//			WebGLComm.inst.Debug("UTY.levelloader delegate:"+onLevelLoadedDelegate.ToString());
			onLevelLoadedDelegate();
		}
		if (LevelBuilder.inst.levelBuilderIsShowing && LevelBuilder.inst.firstTimeOpened){
			LevelBuilder.inst.ActionCenterOnPlayer();
		}

		GameManager.inst.NewLevelWasLoaded();


		if (LevelBuilder.inst.levelBuilderIsShowing){
			GameManager.inst.SetGameState(GameState.Editing); 
		} else {
			GameManager.inst.SetGameState(GameState.Playing); 
		}

		state = LoadingState.Ready;

//		StartCoroutine(GameStartedAfterSeconds(.2f)); // can't do this same frame because old stuff will get touched and new instantiated stuff isnt there yet.
//		Player.inst.SetPlayerLocationForGameStarted();

	}

//	public bool levelClassFinishedLoading = false; // todo: Move this to an enum for state, e.g. LevelLoadState.LoadingClass, .LoadingInstance, .LoadedClass, .LoadedInstance, None

	SimpleJSON.JSONClass LoadLevelInstance(SimpleJSON.JSONClass N, bool loadingFromPipe = false, string previousLevelCode = ""){
//		WebGLComm.inst.Debug("UTY.levelloader. load level instance.");
		int i=0;
		foreach(SimpleJSON.JSONClass levelObj in N["Objects"].AsArray.Childs){
			
			LevelBuilderObjectManager.inst.PlaceObject(levelObj,SceneSerializationType.Instance);
			i++;
		}

//		WebGLComm.inst.Debug("Placed "+i+" objects for LEVEL INSTANCE for level:"+N["Name"]);

		if (loadingFromPipe) {

			SearchForPipeToPlacePlayerThere(previousLevelCode); // yep, this is bad. Need to have unique IDs for pipes in the json and handle this in the json, not by searching through placed gameobjects' properties.
		}

		return N;


	}


	SimpleJSON.JSONClass LoadLevelClass(SimpleJSON.JSONClass N, bool centerOnPlayer = true){
		// ttransition2
//		WebGLComm.inst.Debug("UTY.levelloader. load leve Class.");

		// TODO: Figure out what the screenshot camera is and preload it
		// Screenshotter.DefaultCAmera.position,rotation = x

		LevelBuilder.inst.currentPiece = null; // lose previous reference in case of leftover bs from last edit session.
		LevelBuilder.inst.UserFinishedPlacingObject();



		if (N["Tags"] == null || N["Tags"] == "") {
			LevelBuilder.inst.levelTagsInput.text = "None";
		} else {
			LevelBuilder.inst.levelTagsInput.text = N["Tags"];
		}
		if (N["Description"] == null || N["Description"] == "") {
			LevelBuilder.inst.levelDescriptionInput.text = "No description";
		} else {
			LevelBuilder.inst.levelDescriptionInput.text = Utils.FakeToRealQuotes(N["Description"]);
		}
	
	
		int i=0;
		int totalObjectsToPlace = N["Objects"].AsArray.Childs.Count();
		string objectsString = "";
		Debug.Log("objs len:"+totalObjectsToPlace);
		Debug.Log("objs contetsn:"+N["Objects"].ToString());
		foreach(SimpleJSON.JSONClass levelObj in N["Objects"].AsArray.Childs){
//			Debug.Log("n array:"+i+", name:"+levelObj.ToString());
			i++;
//			if (i > 5) continue;
//			if (levelObj["name"].Value == "Stella" || levelObj["name"].Value == "Generic Character") continue; // oops, these are severely corrupted somehow and placing a ton of them
//			Debug.Log("placing:"+levelObj["name"]);
//			Debug.Log("placing;"+levelObj["name"].Value);
			Debug.Log("N name;"+levelObj["name"]);
			UserEditableObject ueo = LevelBuilderObjectManager.inst.PlaceObject(levelObj,SceneSerializationType.Class);
			objectsString += levelObj["name"].Value+",";
			UpdateProgress(i,totalObjectsToPlace);
			if (ueo){
				LevelBuilderEventManager.inst.ReconnectBrokenUuidsForUndeletedObjects(ueo,ueo.GetUuid());
			} else {
				WebGLComm.inst.Debug("<color=#f00>Missing:</color>"+levelObj["name"]);
			}
		}
//		Debug.Log("<color=#00f>JSON</color>: "+N.ToString());


		if (N["ScreenshotCameraInfo"] != null && N["ScreenshotCameraInfo"] != ""){
			Screenshotter.inst.SetScreenshotCameraInfo((SimpleJSON.JSONClass)N["ScreenshotCameraInfo"]);
		}
//		WebGLComm.inst.Debug("Placed "+i+" objects for LEVEL CLASS for level:"+N["Name"]);

//		LevelBuilder.inst.loadingLevelObjectScreen.SetActive(false);
		OnLevelClassLoaded();
		return N;


	}

	void OnLevelClassLoaded(){
//		JsonLevelSaver.inst.ResetUndoHistory();
	}

//	// This is for debugging in case the expected objects count and type doesn't match reality
//	void CheckExistingObjects(){
//		StartCoroutine(CheckExistingObjectsE());
//	}
//	IEnumerator CheckExistingObjectsE(){
//		yield return new WaitForSeconds(.01f); // one frame really, can't do same frame because old objs havent finished destroying yet.
//		string existingNames = "";
//		int count = 0;
//		foreach(UserEditableObject ueo in FindObjectsOfType<UserEditableObject>()){
//			if (ueo.transform.parent == null) {
//				existingNames += ueo.myName+",";
//				count++;
//			}
//		}
////		// commented Debug.Log("There exist "+count+" object:"+existingNames);
//	}

	void UpdateProgress(int i, int totalObjectsToPlace){
		int progressCompleted = Mathf.FloorToInt(i / totalObjectsToPlace * 20);
		int progressRemaining = 20 - progressCompleted;
		string progressText = "";
		for(int j=0;j<progressCompleted;j++){
			progressText += "O";
		}
		for(int j=0;j<progressRemaining;j++){
			progressText += "_";
		}
//		LevelBuilder.inst.loadingLevelObjectsProgressText.text = progressText;
	}


	public void LoadTempLevelJson(bool centerOnPlayer = true){
//		GameManager.inst.SetGameState(GameState.LoadingClass);
//		return;
//		WebGLComm.inst.Debug("load temp json: START");
		string json = "";
		if (PlayerPrefs.HasKey("tempLevelJson")){
			json = PlayerPrefs.GetString("tempLevelJson");
//			Debug.Log("json:"+PlayerPrefs.GetString("tempLevelJson"));
//			WebGLComm.inst.Debug("<color=#0f0>Loader</color>:load temp json: Have key");
		}

		if (json == ""){
			WebGLComm.inst.Debug("<color=#0f0>Loader</color>:temp Json null");
//			Debug.Log("hi");
			WebGLComm.inst.LoadLevelClassForEditor(); // tries to get json from current level code, if possible (if not will just load level with blank json)
//			LoadLEvel((); 
//			#if UNITY_EDITOR
//			LoadLevel("");
//			#else
//			#endif
		} else {
			LoadLevel(json,SceneSerializationType.Class,false,centerOnPlayer);
//			Debug.Log("loading from index:"+PlayerPrefs.GetInt("currentSaveIndex"));
//			Debug.Log("jsonLOAD:"+json);
		}
	}



	void Update(){
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.D)){
			WebGLComm.inst.LoadLevelInstanceFromJSON(LevelJsonSamples.json8); //,SceneSerializationType.Instance);
		}
		#endif
	}


	void SetJsonVersion(SimpleJSON.JSONClass N){
		if (N.GetKeys().Contains(GameManager.versionKey)){
			thisLevelVersion = N[GameManager.versionKey].AsFloat;
			WebGLComm.inst.Debug("This json had <color=#0f0> version </color>:"+thisLevelVersion);
		} else {
			thisLevelVersion = 1.0f;
			WebGLComm.inst.Debug("This json did not have a <color=#0f0> version </color> so we assume it's 1.0:"+thisLevelVersion);
		}

	}


	public void SetTempLevelJsonPlayerPrefs(string json, string source = "default"){
		if (json == ""){
			WebGLComm.inst.Debug("<color=#0f0>Loader</color>:Clear temp level json()");
		} else {
			WebGLComm.inst.Debug("<color=#0f0>Loader</color>:Set temp level json len("+json.Length+")");
		}
		if (json == "" || state == LoadingState.Ready){
			
			PlayerPrefs.SetString("tempLevelJson",json);
		} else {
			WebGLComm.inst.Debug("<color=#0f0>Loader</color>:Could not set temp level json, loading state was;"+state+", json was;"+json.Substring(0,40));
		}
	}


}
