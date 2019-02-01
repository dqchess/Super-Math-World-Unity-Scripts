using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // raycast result?
public class ClipboardManager : MonoBehaviour {

	// This script allows you to "save" a group of items to your clipboard.
	// This is json serialized per USER not per LEVEL so a user can move clips from level to level -- they persist.

	public static string clipboardKey = "clipboard";
	public static string clipboardIndexKey = "clip_index";
	public static string clipboardJsonKey = "clip_json";
	public static ClipboardManager inst;

	public SimpleJSON.JSONArray[] clipboardSnips = new SimpleJSON.JSONArray[9]; // there are max 9 clips
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		LoadClipboard();
	}

	public void SaveClipboardSnip(int i, SimpleJSON.JSONArray N){
		clipboardSnips[i] = N;
		UIValueCommClipboard.inst.ClipboardSaved(i);
	}

	public bool HaveAvailableClipboardSlot(){
		foreach(SimpleJSON.JSONArray n in clipboardSnips){
			if (n == null) return true;
		}
		return false;
	}


	public void LoadClipboard(){
		#if UNITY_EDITOR
		LoadClipboardCallback(PlayerPrefs.GetString("Clipboard"));
		#else
		WebGLComm.inst.LoadClipboard();
		#endif
	}

	public void LoadClipboardCallback(string json){
		if (json == "") {
//			Debug.Log("<color=#ff0>No clips</color>");
			return;
		} else {
//			Debug.Log("<color=#ff0>clips:</color>"+json);
		}
		SimpleJSON.JSONClass N = (SimpleJSON.JSONClass)SimpleJSON.JSONNode.Parse(json);
		int i=0;
		foreach(SimpleJSON.JSONClass item in N[clipboardKey].AsArray.Childs){
			SaveClipboardSnip(item[clipboardIndexKey].AsInt,item[clipboardJsonKey].AsArray);
		}
	}

	int GetFirstAvailableClipboardSlot(){
		for (int i=0;i<clipboardSnips.Length;i++){
			SimpleJSON.JSONArray n = clipboardSnips[i];
			if (n == null) return i;
		}
		return -1;
	}

	public int SaveDragParentToClipboard(GameObject draggingParent){
		// User clicked "save clipboard" icon on the marker menu
		// Save to the next available slot if available.

		int slot = GetFirstAvailableClipboardSlot();
		if (slot == -1){
//			Debug.Log("<color=#f00>No clip </color> was avail");
			return slot;
		}


		// Saves to the next available slip if there is one
		// if none, a dialogue pops saying "No more clipboard snips available! Delete one by clicking the red X"

		// move clipboard icon from obj marker menu to clipboard icon in top right.
		GameObject fx = (GameObject)Instantiate(LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).gameObject,LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).position,LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).rotation);
		fx.transform.SetParent(LevelBuilder.inst.markerMenuClipBoardGroup.transform);

		LevelBuilder.inst.AddFXObject(fx,UIValueCommClipboard.inst.clipboardUiElements[slot].position);
//		UIValueCommClipboard.inst.ClipboardSaved(slot);
		List<UserEditableObject> ueos = new List<UserEditableObject>();
		foreach(Transform t in draggingParent.transform){
			UserEditableObject ueo = t.GetComponent<UserEditableObject>();
			if (ueo){
				// should ALWAYS have ueo.. but this is fragile! What if drag parent somehow dragging a piece that ISNT Ueo?
				ueos.Add(ueo);
			}
		}
		SaveClipboardSnip(slot,JsonLevelSaver.inst.SerializeUserEditableObjects(ueos));

		AudioManager.inst.PlayWingFlap(Player.inst.transform.position);

		SaveClipboardToServer();
		return slot;
	}

	void SaveClipboardToServer(){
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[clipboardKey] = new SimpleJSON.JSONArray();
		for (int i=0;i<9;i++){
			if (clipboardSnips[i] != null){
				SimpleJSON.JSONClass item = new SimpleJSON.JSONClass();
				item[clipboardIndexKey].AsInt = i;
				item[clipboardJsonKey]  = clipboardSnips[i];
				N[clipboardKey].Add(item);
			}
			
		}
		#if UNITY_EDITOR
		PlayerPrefs.SetString("Clipboard",N.ToString());
		#else
		WebGLComm.inst.SaveClipboard(N.ToString());
		#endif
	}


	public void PasteDraggingParentFromClipboard(int i){
		// Ireally, really don't like how I'm accessing a bunch of LevelBuilder objects here like draggingparent and current piece and draggingpicesesbucket. Oh well.
		//		AudioManager.inst.LevelBuilderPreview();
		AudioManager.inst.PlayInventoryClose();
		// User saved smething on clipboard and now wants to instantiate it.
		if (clipboardSnips[i] == null){
//			Debug.Log("<color=#f00>No snip </color>at i:"+i);
			return;
		}
		SimpleJSON.JSONArray clipboardArray = clipboardSnips[i];
		List<GameObject> draggingPiecesToCreate = new List<GameObject>();
		List<UserEditableObject> ueos = new List<UserEditableObject>();
		foreach(SimpleJSON.JSONClass n in clipboardArray){
			// Create the objects from string memory.
			UserEditableObject ueo = LevelBuilderObjectManager.inst.PlaceObject(n,SceneSerializationType.Class);
			ueos.Add(ueo); // for group manager
			// Note that this creates the peice wherever it was in world space when the copy was made, so we'll reposition the items to current mouse pos after creation.
			draggingPiecesToCreate.Add(ueo.gameObject);
		}
		LevelBuilder.inst.draggingParent = LevelBuilder.inst.MakeDraggingParentWithPieces(draggingPiecesToCreate); // group these objects together for selection nd select them
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(LevelBuilder.inst.camSky.transform.position,LevelBuilder.inst.camSky.transform.forward,out hit)){
			//			Debug.Log("hitp:"+hit.point);
			LevelBuilder.inst.draggingParent.transform.position = hit.point; // reposition drag parent
			LevelBuilder.inst.currentPiece = LevelBuilder.inst.draggingParent;
			LevelBuilder.inst.BeginMovingObject();
			LevelBuilderGroupManager.inst.MakeGroup(ueos);
			//			PlaceContextMenu(draggingParent);
		} else {
			//			Debug.Log("nohit");
			//			Finisheddr
			Destroy(LevelBuilder.inst.draggingParent);
		}

	}
}
