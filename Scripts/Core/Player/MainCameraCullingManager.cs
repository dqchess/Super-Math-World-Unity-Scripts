using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MainCameraCullingManager : MonoBehaviour {


	public LayerMask layersToShow;
	public LayerMask hideAll;

	public static MainCameraCullingManager inst;
	public void SetInstance(){
		inst = this;
	}

	void Start() {
		layersToShow = Camera.main.cullingMask;
		float[] distances = new float[32];
		for (int i=0;i<32;i++){
			distances[i] = 200f;
		}
		int ter = LayerMask.NameToLayer("Terrain");
//		Debug.Log("ter:"+ter);
		distances[LayerMask.NameToLayer("Terrain")] = 2000f;
		distances[LayerMask.NameToLayer("TransparentFX")] = 150f;
		Camera.main.layerCullDistances = distances;
		Camera.main.layerCullSpherical = true;
	}

	public void SetCameraMode(PlayMode newMode){
		switch(newMode){
//		Debug.Log("switch:"+newMode);
		case PlayMode.Editor:
			Camera.main.cullingMask = hideAll;
			break;	
		case PlayMode.Player:
			Camera.main.cullingMask = layersToShow;
			break;
		default:break;
		}
	}
}
