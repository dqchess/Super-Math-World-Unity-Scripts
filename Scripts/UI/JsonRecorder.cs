using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum JsonRecordingState {
	Ready,
	Recording,
	Saving
}

public class JsonRecorder : MonoBehaviour {


	// Nice to have -- instead of screen caps, just capture the json positions, rotations and scales of all objects every frame. 
	// Then we can reconstruct the scene frame by frame in a server somewhere and record that at a faster speed because it won't be rendering a player camera.
//	public state = JsonRecordingState.Ready;
//	public List<string> jsons = new List<string>();
//	void Update(){
//		if (Input.GetKeyDown(KeyCode.R)){
//			if (state == JsonRecordingState.Ready){
//				BeginRecording();
//			}
//		}
//		if (state == JsonRecordingState.Recording){
////			jsons.Add(Level
//		}
//	}


}
