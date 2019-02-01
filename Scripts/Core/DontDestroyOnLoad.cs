using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour {

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnSceneLoaded() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void OnDestroy(){
//		Debug.Log("What:"+this.name);
	}

}
