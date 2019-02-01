using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraLayers : MonoBehaviour {

	public Dictionary<string, bool> masks = new Dictionary<string, bool>();
	public Camera nearCam;
	public static CameraLayers inst;


	public void SetInstance(){
		inst = this;
	}

	public void ShowThisLayer(string layer, bool flag){
		if (flag) {
			Camera.main.cullingMask |= 1 << LayerMask.NameToLayer(layer);
		}
		else {
			Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer(layer));
		}
		if (!masks.ContainsKey(layer)) masks.Add (layer,flag);
		else masks[layer] = flag;
//		// commented Debug.Log("camera culling mask:"+Camera.main.cullingMask);
	}

	void Update(){
//		if (Input.GetKeyDown(KeyCode.O)){
//			ShowThisLayer ("Color1",false);
//			ShowThisLayer("Color2",true);
//		} else if (Input.GetKeyDown(KeyCode.P)){
//			ShowThisLayer("Color1",true);
//			ShowThisLayer("Color2",false);
//		}
	}

	public void ShowPlayer(){
//		playerOutline.SetActive(false);
		Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Player");
		Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("PlayerOutline"));
	}
	public void HidePlayer(){
//		playerOutline.SetActive(true);
		Camera.main.cullingMask &=  ~(1 << LayerMask.NameToLayer("Player"));
		Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("PlayerOutline");
	}

}
