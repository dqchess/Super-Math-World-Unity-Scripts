using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilderSelectableObject : MonoBehaviour {

	// handles the tracking of all objects in the scene that LevelBuilder can interact with.

	virtual public void OnEnable(){
		if (!LevelBuilderObjectManager.inst.bNeedUpdateCachedObjects){
			LevelBuilderObjectManager.inst.UpdateCachedObjects();
		}
	}
	virtual public void OnDisable(){
		if (!LevelBuilderObjectManager.inst.bNeedUpdateCachedObjects){
//			Debug.Log("disable:"+name);
			LevelBuilderObjectManager.inst.UpdateCachedObjects();
		} else{
//			Debug.Log("disabled but not");
		}
	}


}
