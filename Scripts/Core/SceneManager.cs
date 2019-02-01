using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CallbackTarget {
	_LevelBuilder,
	_WebGLComm,
	_Failed // lolz
//	_JsonLevelLoaderClass

}

public enum SceneState {
	Loading,
	Ready
}

public class SceneManager : MonoBehaviour {

	public SceneState sceneState = SceneState.Ready;
	public static string prefsKey = "OnSceneReloadedCallback";
	public static string callbackKey = "CallbackFn";
	public static string callbackTargetKey = "CallbackTarget";
	public static SceneManager inst;

	public void SetInstance(){
		inst = this;
	}


	public void ReloadSceneNow(){
		sceneState = SceneState.Loading;
//		Debug.Log("reloaidng scene now.");
		Application.LoadLevel(Application.loadedLevel);
		StartCoroutine(SceneReadyAfterFrames(2));
	}

	IEnumerator SceneReadyAfterFrames(int frames){
		for(int i=0;i<frames;i++){
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
		sceneState = SceneState.Ready;
	}

	void ReloadSceneWithCallback () {
		WebGLComm.inst.Debug("<color=#f0f>Daemon.Reload</color>");
		ReloadSceneNow();

//		sceneWasReloadedThisFrame = true;
//		LevelBuilderObjectManager.inst.UpdateCachedObjects();
		ExecuteSceneReloadCallback();
	}
	void ExecuteSceneReloadCallback(){
//		WebGLComm.inst.Debug("START: OnSceneReloaded");
		if (PlayerPrefs.HasKey(prefsKey)){
			string json = PlayerPrefs.GetString(prefsKey);
//			MouseLockCursor.ShowCursor(false,"Reloaded scene");
//			#if UNITY_WEBGL && !UNITY_EDITOR
//			#endif
//			WebGLComm.inst.Debug("Has prefs key: "+json);
			SimpleJSON.JSONClass N = (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(json);
			switch(GetCallbackTargetFromString(N[callbackTargetKey])){
			case CallbackTarget._LevelBuilder:
				WebGLComm.inst.Debug("Callback to LevelBuilder");
				LevelBuilder.inst.SendMessage(N[callbackKey].Value);
				break;
			case CallbackTarget._WebGLComm:
				WebGLComm.inst.Debug("Callback to WEBGL");
				WebGLComm.inst.SendMessage(N[callbackKey].Value);	
				break;
			default:
				WebGLComm.inst.Debug("No Callback Target");
				break;
			}
			PlayerPrefs.DeleteKey(prefsKey);
//			CanvasMouseController.inst.ClosePlayerStartDialogue();
//			CanvasMouseController.inst.CloseControlsDialogue();
//			WebGLComm.inst.SetCharacterCostumeOnSceneLoaded();
//			WebGLComm.inst.LoginAsUser(WebGLComm.loggedInAsUser);
		} else {
//			WebGLComm.inst.Debug("Started WITH NO key");
		}
	}

	public void ReloadSceneWithCallbackWithCallback(CallbackTarget cbt, string callbackMethod){
//		callbackTarget = cbt;

		WebGLComm.inst.Debug("Reloading scene callback fn/mthod: "+cbt+",/"+callbackMethod);
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[SceneManager.callbackKey] = callbackMethod;
		N[SceneManager.callbackTargetKey] = GetStringFromCalbackTarget(cbt);
		PlayerPrefs.SetString(prefsKey,N.ToString());
		ReloadSceneWithCallback();
	}

	string GetStringFromCalbackTarget(CallbackTarget cbt){
		switch(cbt){
		case CallbackTarget._LevelBuilder:
			return "_LevelBuilder";
			break;
		case CallbackTarget._WebGLComm:
			return "_WebGLComm";
			break;
		}
		return "none";
	}

	CallbackTarget GetCallbackTargetFromString(string cbt){
		if (cbt == "_LevelBuilder") return CallbackTarget._LevelBuilder;
		if (cbt == "_WebGLComm") return CallbackTarget._WebGLComm;
		return CallbackTarget._Failed; //lol
	}

//	[System.NonSerialized] public bool sceneWasReloadedThisFrame = false;
//	void LateUpdate(){
//		sceneWasReloadedThisFrame  = false;
//	}

}
