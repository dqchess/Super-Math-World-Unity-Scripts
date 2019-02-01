using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugobj : MonoBehaviour {

	int index = 0;
	Transform[] allChilds;
	void OnEnable(){
		allChilds = transform.GetComponentsInChildren<Transform>();

	}

	void Update(){
		if (SMW_CHEATS.inst.cheatsEnabled && Input.GetKeyDown(KeyCode.C)){
			if (allChilds[index] != this.transform){
				foreach(Transform t in allChilds){
					t.gameObject.SetActive(true);
				}
				allChilds[index].gameObject.SetActive(false);
				WebGLComm.inst.Debug("Disabled:"+allChilds[index].name);
			}
			index ++;
			if (index > allChilds.Length-1) index = 0;
		}
	}
}
