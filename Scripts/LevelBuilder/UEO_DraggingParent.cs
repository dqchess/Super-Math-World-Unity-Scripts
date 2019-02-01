using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GroupedState {
	Grouped,
	Ungrouped
}

public class UEO_DraggingParent : UserEditableObject {

	// this is never available as an object to place but is used as a temporary object for dragging lots of objects around (or rotating them).
	// It will be destroyed at the end of group move/drag action
	// it blocks autosaves

	public GroupedState groupedState = GroupedState.Ungrouped;

	#region UserEditbale
	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[]{
			LevelBuilder.inst.POCMheightButton,
			LevelBuilder.inst.POCMcopyButton
		};

		//		return null;
	}
	#endregion

	public override void OnLevelBuilderObjectMoveFinished(){
//		WebGLComm.inst.Debug("<color=#f0f>DP:</color>move finished");
		base.OnLevelBuilderObjectMoveFinished();
		UnparentAll();
	}

	void UnparentAll(){
//		Debug.Log("unparent all.");
		transform.DetachChildren();
		Destroy(this.gameObject);
	}

	public override void OnMarkerMenuClosed(){
		base.OnMarkerMenuClosed();
		foreach(Transform t in transform){
			UserEditableObject ueo = t.GetComponent<UserEditableObject>();
			if (ueo){
				ueo.OnMarkerMenuClosed();
			}
		}
//		WebGLComm.inst.Debug("<color=#f0f>DP:</color>menu closed");
		UnparentAll();
	}


	public override void OnObjectWasCreatedAsADuplicate(){
		// This object was created as a duplicate, e.g. user pressed "copy" then started clicking on the map to make copies
//		WebGLComm.inst.Debug("<color=#f0f>DP:</color>obj created as dupe");
		UnparentAll();

	}

	public override void OnObjectWasDuplicated(){
		// This object was selected and user wants to duplicate it
//		WebGLComm.inst.Debug("<color=#f0f>DP:</color>obj duped");
		UnparentAll();
	}

	public override void AddTags(List<string> ts){
		foreach(Transform t in transform){
			t.GetComponent<UserEditableObject>().AddTags(ts);
		}
	}

	float selectedParticleFxTimer = 0;
	void Update(){
		selectedParticleFxTimer -= Time.deltaTime;
		if (transform.childCount > 0){
			if (selectedParticleFxTimer < 0){
				selectedParticleFxTimer = 4f;
				foreach(Transform t in transform){
					EffectsManager.inst.BlueRing(t.position,Vector3.zero,30);
				}
			}
		}
	}
}
