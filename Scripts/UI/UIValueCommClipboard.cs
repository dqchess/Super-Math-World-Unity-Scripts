using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // raycast result?

public class UIValueCommClipboard : MonoBehaviour {

	public Text noClipsText;
	public Transform[] clipboardUiElements;
	public void LoadClip(int i){
		ClipboardManager.inst.PasteDraggingParentFromClipboard(i);
	}

	public static UIValueCommClipboard inst;

	public void SetInstance(){
		inst = this;
	}

	public void ClipboardSaved(int i){
		clipboardUiElements[i].gameObject.SetActive(true);
		noClipsText.gameObject.SetActive(false);
	}

	public void DeleteClipboard(int i){
		// the delete button is appended to the clipboard being hovered over. If clicked a dialogue prompts yes/no really delete, 
		//if yes a poof, clipboard is disabled, cleared from clipboard manager so a new clip can be saved in this slot
		if (ClipboardManager.inst.clipboardSnips[i] != null) {
			
			EffectsManager.inst.CreateSmokePuffBig(clipboardUiElements[i].position,Vector3.zero,5,1.2f );
			AudioManager.inst.PlayPoof(Vector3.zero);
			ClipboardManager.inst.clipboardSnips[i] = null;
		}
		clipboardUiElements[i].gameObject.SetActive(false);

	}

	public void Update(){
		if (Input.GetMouseButtonDown(1)){
			// right click to delete
			foreach(RaycastResult rr in LevelBuilder.inst.objectsHit){
				ClipboardUiElement clipui = rr.gameObject.GetComponent<ClipboardUiElement>();
				if (clipui){
					DeleteClipboard(clipui.index);
					if (!HadAtLeastOneActiveClipboard()){
						noClipsText.gameObject.SetActive(true);
					}
					//					LevelBuilderTabManager.inst.ScrollCurrentTabToTop();
				}
			}
		}
	}

	bool HadAtLeastOneActiveClipboard(){
		foreach(Transform t in clipboardUiElements){
			if (t.gameObject.activeSelf) return true;
		}
		return false;
	}
}
