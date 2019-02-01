using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DanMeyerCubeClicker : MonoBehaviour {

	// Use this for initialization
	public LevelMachine_DanMeyerCubes lm;
	public MouseLook mouseLook;
	Camera cam;
	void Start(){
		cam = GetComponent<Camera>();
	}
	void OnEnable(){
		UpdateClickableStateForAllCubes();
	}

	bool rotating = false;
	public UIBooleanSlider slider;
	// Update is called once per frame

	bool cachedToggleValue = false;

	void UpdateClickableStateForAllCubes(){
		if (slider.GetSliderValue() != cachedToggleValue){
			cachedToggleValue = slider.GetSliderValue();
			foreach(DanMeyerCube dmc1 in lm.cubes){
				dmc1.GetComponent<Collider>().enabled = cachedToggleValue != dmc1.cubeActive; // only allow cubes to be clicked depending on their on/off status combined with slider status
				// e.g. if slider is set to "Destroy" we don't want already destroyed cubes colliding with the mouse
				// in this way we can "penetrate" to the center of the cube without edge cube colliders always getting in the way.

			}
		}
	}

	void Update () {
		if (Input.GetMouseButtonUp(0)){
			UpdateClickableStateForAllCubes();

		}

		if (Input.GetKeyDown(KeyCode.Escape)){
			if (rotating){
				MouseLockCursor.ShowCursor(true,"end edit dan meyer cube while rotating");
			}
			lm.EndEditing(); // will disable this object, preventing further updates from happening after the following return; statement
			return;
		}

		if (Input.GetMouseButtonDown(1)){
			mouseLook.enabled = true;
			rotating = true; // mouse look
			MouseLockCursor.ShowCursor(false,"eidting dan meyer cube");
		}
		if (Input.GetMouseButtonUp(1)){
			mouseLook.enabled = false;
			rotating = false;
			MouseLockCursor.ShowCursor(true,"eidting dan meyer cube");
		}

		if (rotating) return;

//		RaycastHit[] hits;
		List<DanMeyerCube> cubesToAffect = new List<DanMeyerCube>();
		foreach(RaycastHit hit in Physics.RaycastAll( cam.ScreenPointToRay(Input.mousePosition),300)) {
			DanMeyerCube dmc = hit.collider.GetComponent<DanMeyerCube>();
//			if (!dmc) {
//				// maybe they clicked the toggle!
//				UIBooleanSlider = 
//			}
			if (dmc){
				// we are hovering over a cube while in editing mode.
				// Hotkeys are SHIFT, ALT, CTRL for columns, x rows, z rows
				foreach(DanMeyerCube dm2 in lm.cubes){
					// columns let's say
					// get all cubes in this column.
					if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)){
						//ALL of them.
//						if ((int)dm2.indexedPosition.y == (int)dmc.indexedPosition.y  (int)dm2.indexedPosition.z == (int)dmc.indexedPosition.z){
						cubesToAffect.Add(dm2);
						LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
//						}
					} else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)){
						// HORIZONTAL PLANE 
						if ((int)dm2.indexedPosition.y == (int)dmc.indexedPosition.y){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					} else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)){
						// VERTICAL Z PLANE 
						if ((int)dm2.indexedPosition.z == (int)dmc.indexedPosition.z){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					} else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt)){
						// VERTICAL X PLANE 
						if ((int)dm2.indexedPosition.x == (int)dmc.indexedPosition.x){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					} else if (Input.GetKey(KeyCode.LeftShift)){
						// Z ROW ONLY

						if ((int)dm2.indexedPosition.y == (int)dmc.indexedPosition.y && (int)dm2.indexedPosition.z == (int)dmc.indexedPosition.z){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					} else if (Input.GetKey(KeyCode.LeftControl)){
						// X ROW ONLY
						if ((int)dm2.indexedPosition.x == (int)dmc.indexedPosition.x && (int)dm2.indexedPosition.y == (int)dmc.indexedPosition.y){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					} else if (Input.GetKey(KeyCode.LeftAlt)){
						// COLUMN ONLY
						if ((int)dm2.indexedPosition.x == (int)dmc.indexedPosition.x && (int)dm2.indexedPosition.z == (int)dmc.indexedPosition.z){
							cubesToAffect.Add(dm2);
							LevelBuilder.inst.MakeHighlightFX2(dm2.transform,"dan meyer cube high");
						}
					}
				} 
				if (cubesToAffect.Count == 0){
					// no hotkeys pressed so we didn't add the "focus" cube under the mouse cursor.
					cubesToAffect.Add(dmc);
					LevelBuilder.inst.MakeHighlightFX2(dmc.transform,"dan meyer cube high");
				}
				if (Input.GetMouseButtonDown(0)){
//					Debug.Log("<color=#00f>Affecting</color>Affecting:"+cubesToAffect.Count+"cubes.");
					bool active = !slider.GetSliderValue(); // cache this value for .000001s savings. By the way slider value == true is DESTROY in this setup.
					foreach(DanMeyerCube dm3 in cubesToAffect){
						if (active) dm3.TurnCubeOff(active); // note we are comparing ONLY the active value of the exact hovering cube, so all column/row cubes are set to the SAME bool value, not "flipped"
						else dm3.TurnCubeOn(active);
					}
					lm.UpdateCountText();
				}
				break; // the foreach doesn't continue after this.
			}


		}
	}
}
