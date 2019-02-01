using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // raycast result?
public class SavedSelectionManager : MonoBehaviour {

	[System.Serializable]
	public class SavedSelection {
		public List<UserEditableObject> objects = new List<UserEditableObject>();
		public int index = 0;
	}

	[SerializeField]
	public SavedSelection[] savedSelections = new SavedSelection[5]; // max is 5
	// This script allows you to "Save" your selected objects so if you select a group you can select that same group later.
	// Local only, not saved to server.

	public static SavedSelectionManager inst;

	public void SetInstance(){
		inst = this;
	}


	public void SaveSelection(int i, List<UserEditableObject> objects){
//		clipboardSnips[i] = N;
	}

	public bool HaveAvailableSlot(){
		foreach(SavedSelection s in savedSelections){
			if (s.objects.Count == 0) return true;
		}
		return false;
	}

	public int GetFirstAvailableSlot(){
		for (int i=0;i<savedSelections.Length;i++){
			if (savedSelections[i].objects.Count == 0) return i;
		}
		return -1; // something went wrong. :-{
	}


	public int SaveDragParentToSelection(GameObject draggingParent){
		// User clicked "save clipboard" icon on the marker menu
		// Save to the next available slot if available.

		int slot = GetFirstAvailableSlot();
		if (slot == -1){
//			Debug.Log("<color=#f00>No clip </color> was avail");
			return slot; // what?S?
		}


		// Saves to the next available slip if there is one
		// if none, a dialogue pops saying "No more clipboard snips available! Delete one by clicking the red X"

		// move clipboard icon from obj marker menu to clipboard icon in top right.
		GameObject fx = (GameObject)Instantiate(LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).gameObject,LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).position,LevelBuilder.inst.markerMenuClipBoardGroup.transform.GetChild(0).rotation);
		fx.transform.SetParent(LevelBuilder.inst.markerMenuClipBoardGroup.transform);

		LevelBuilder.inst.AddFXObject(fx,UIValueCommClipboard.inst.clipboardUiElements[slot].position); // show a visible move from the thing you clicked to where its saved.
//		UIValueCommClipboard.inst.ClipboardSaved(slot);
		List<UserEditableObject> ueos = new List<UserEditableObject>();
		foreach(Transform t in draggingParent.transform){
			UserEditableObject ueo = t.GetComponent<UserEditableObject>();
			if (ueo){
				// should ALWAYS have ueo.. but this is fragile! What if drag parent somehow dragging a piece that ISNT Ueo?
				ueos.Add(ueo);
			}
		}

		AudioManager.inst.PlayWingFlap(Player.inst.transform.position);


		return slot;
	}




	public void SelectThisGroup(int i){
		// The user clicked on one of the selection save icons on the bottom.
		// Select these objects and position the Marker Menu.

	}
}
