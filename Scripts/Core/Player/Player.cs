using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum PlayerStartType{
	Pipe,
	Checkpoint,
	StartObject
}

public class Player : MonoBehaviour {

	public static Player inst;
//	public static Transform transform;
	public static bool frozen;
	public static bool playerLookFrozen = false;
	public MouseLook mlook1;
	public MouseLook mlook2;
	public Transform pivot;
	public AudioListener audioListener;
	public Image whiteFlashPanel;
	public Image fadeBlackPanel;
	public Transform defaultParent;
//	public Transform graphics;



	public void Unparent(){
		// don't put it in the scene as it will get destroyed on load.
		// but what about DontDestroyOnLoad? Oh, wait, that function only works sometimes. #unity!
		transform.parent = defaultParent;
	}
	public void SetInstance(){
		inst = this;
	}

	void Start(){
		
//		this.transform = gameObject.transform;
	}


	public void DisableMouseLook() {
		mlook1.enabled = false;
		mlook2.enabled = false;
	}

	public void EnableMouseLook(){
		mlook1.enabled = true;
		mlook2.enabled = true;
	}

	float playerFrozenTimer=0;
	bool needsUnfreezing = false;

	int playerFreeze = 0;
	public void UnfreezePlayer(string source=""){
		playerFreeze --;
//		// commented Debug.Log ("player un-frozen:"+source+", with playerfreeze count:"+playerFreeze);
		if (playerFreeze <= 0){
			playerFreeze = 0;
			needsUnfreezing=true;
			playerFrozenTimer = 0;
		}
	}
	public void FreezePlayer(string source=""){
		playerFreeze ++;
//		// commented Debug.Log ("Freeze from: "+source+", count:"+playerFreeze);
		needsUnfreezing=false;
		//		SetPlayerGravity(true);
		frozen = true;
	}

	int playerLookFreeze = 0;
	public void FreezePlayerFreeLook(){
		// Should only ever be called by CanvasMouseDialogue on game start.
		mlook1.enabled = false;
		mlook2.enabled = false;

//		return;
//		playerLookFreeze++;
//		// commented Debug.Log("free look frozen. ct:"+playerLookFreeze);
//		playerLookFrozen = true;
	}

	public void UnfreezePlayerFreeLook(){
		mlook1.enabled = true;
		mlook2.enabled = true;
//		return;
//		playerLookFreeze--;
//		// commented Debug.Log("UNfree look frozen. ct:"+playerLookFreeze);
//		if (playerLookFreeze <= 0){
//			playerLookFreeze = 0;
//			playerLookFrozen = false;
//		}
	}



	bool fadingBlack = false;
	public void SlowFadeInBlack(){
//		Debug.Log("<color=#f0f>]]]]]]]]]]]fadeinblack</color>");
		fadingBlack = true;
		fadeBlackPanel.color = new Color(0,0,0,1.7f); //Color.black;
	}

	bool flashingwhite = false;
	public void FlashWhite(){
		flashingwhite = true;
		whiteFlashPanel.color = Color.white;
	}



	void Update(){
		if (needsUnfreezing){ // agh duplicate variables!!
			playerFrozenTimer+= Time.deltaTime;
			if (frozen){
				if (playerFrozenTimer > .01f){ // small delay to keep from discharging your weapon while clicking a button and moving from frozen to unfrozen state
//					// commented Debug.Log ("actually unfrozen");
					frozen = false;
					//					SetPlayerGravity(false);
				}
			}
		}
		if (flashingwhite){
			float flashSpeed = 3f;
			whiteFlashPanel.color = Color.Lerp(whiteFlashPanel.color,new Color(1,1,1,0),Time.deltaTime * flashSpeed);
		}

		if (fadingBlack){
			float fadeSpeed = 0.7f;
			fadeBlackPanel.color = Color.Lerp(fadeBlackPanel.color,Color.clear,Time.deltaTime * fadeSpeed);
//			Debug.Log("Color:<color=#f0f>"+fadeBlackPanel.color.a+"</color>");
		}
	}

	public NumberInfo PlayerHoldingNumberInfo(){
		if (PlayerGadgetController.inst.GetCurrentGadget()){
			if (PlayerGadgetController.inst.GetCurrentGadget().GetGadgetName() == "Throw"){
				if (PlayerGadgetController.inst.GetCurrentGadget().GetComponent<GadgetThrow>().numberHeld){
					NumberInfo ni2 = PlayerGadgetController.inst.GetCurrentGadget().GetComponent<GadgetThrow>().numberHeld.GetComponent<NumberInfo>();
					if (ni2){
						return ni2;
					}
				}
			}
		}
		return null;
	}

	public void HidePlayer(){
		foreach(MeshRenderer mr in PlayerCostumeController.inst.curCharInfo.playerMeshes.GetComponentsInChildren<MeshRenderer>()){
			mr.enabled = false;
		}
		foreach(SkinnedMeshRenderer mr in PlayerCostumeController.inst.curCharInfo.playerMeshes.GetComponentsInChildren<SkinnedMeshRenderer>()){
			mr.enabled = false;
		}
	}

	public void ShowPlayer(){
		foreach(MeshRenderer mr in PlayerCostumeController.inst.curCharInfo.playerMeshes.GetComponentsInChildren<MeshRenderer>()){
			mr.enabled = true;
		}
		foreach(SkinnedMeshRenderer mr in PlayerCostumeController.inst.curCharInfo.playerMeshes.GetComponentsInChildren<SkinnedMeshRenderer>()){
			mr.enabled = true;
		}
	}
	public void SetPosition(Vector3 p, Quaternion r){
		Player.inst.transform.position = p;
		Player.inst.transform.rotation 	= r;
	}
	public void SetPosition(Transform t, bool ignoreRotation = false){
		if (!t) return;
//		WebGLComm.inst.Debug("set player pos:"+t.position);
		Player.inst.transform.position = t.position;
		Quaternion rot = t.rotation;
//		WebGLComm.inst.Debug("will set player rot;"+rot+", euler;"+rot.eulerAngles);
		if (!ignoreRotation) {
//			Quaternion rot = new Quaternion();
			rot = Utils.FlattenRotation(t.rotation);
//			WebGLComm.inst.Debug("flattened rot;"+rot+", euler;"+rot.eulerAngles);
		}
		Player.inst.transform.rotation = rot;
	}


	Dictionary<PlayerStartType,Transform> playerStarts = new Dictionary<PlayerStartType,Transform>();
	public void AddPlayerStartPriority(PlayerStartType type,Transform t, string source="default"){
		WebGLComm.inst.Debug("added player start:"+type.ToString()+" from source;"+source);
		if (!playerStarts.ContainsKey(type)) playerStarts.Add(type, t);
		else playerStarts[type] = t;
	}

	public void ClearPlayerStartPositions(){
		// At each level load, clear this first -- we'll populate it with levelbuilder objects that may want to set player start
		playerStarts.Clear();
	}

	public void SetPlayerLocationForGameStarted(){
//		WebGLComm.inst.Debug("setting player start. keys len:"+playerStarts.Count);
		// This is loaded each time the player loads a new level, it is the last thing that is called.
		// It is hierarchical -- player prefers to be at 1) pipe, 2) checkpoint, 3) playerstart, 4) map center
		Transform playerStartT;
		if (playerStarts.ContainsKey(PlayerStartType.Pipe)){
//			WebGLComm.inst.Debug("setting player start pipe.");
			playerStartT = playerStarts[PlayerStartType.Pipe];
		} else if (playerStarts.ContainsKey(PlayerStartType.Checkpoint)){
//			WebGLComm.inst.Debug("setting player start checkp.");
			playerStartT = playerStarts[PlayerStartType.Checkpoint];
		} else if (playerStarts.ContainsKey(PlayerStartType.StartObject)){
//			WebGLComm.inst.Debug("setting player start start obj.");
			playerStartT = playerStarts[PlayerStartType.StartObject];
		} else {
			playerStartT = MapManager.inst.GetPlayerStartPositionForCurrentTerrain(); 
//			WebGLComm.inst.Debug("setting player start center terrain.");
		}
		// Load the inventory regardless of where we started from.
		WebGLComm.inst.LoadPlayerInventory(); // starting from checkpoint? Load last saved inventory. TODO Vadim need to load inventory ONLY if it's for this level..
		if (playerStartT){
//			Debug.Log("setting player start! loc:"+playerStartT.name);
			SetPosition(playerStartT);
		} else {
//			Debug.Log("player start destroyed in this frame?!");
		}
	}




}
