using UnityEngine;
using System.Collections;

public class UnparentOnStart : MonoBehaviour, IMyGameStarted {


	void Start(){
	
		GameManager.inst.AddMyGameStartedInterfaceObject(this.gameObject);
//		GameManager.inst.NewLevelWasLoaded += DestroyMe;
//		JsonLevelLoader.inst.onLevelLoadedDelegate += DestroyMe;
	}
	public void GameStarted(){
		transform.parent = null;
//		Debug.Log("gamestrated.");
	}

//	void DestroyMe(){
//		if (gameObject) Destroy(gameObject);
//	}



}

