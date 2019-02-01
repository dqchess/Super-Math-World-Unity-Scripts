using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySceneLoader : MonoBehaviour {

	int sceneIndex = 1;
	void Start(){
		Application.LoadLevel(sceneIndex);
	}
}
