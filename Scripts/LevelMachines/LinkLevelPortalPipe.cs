using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinkLevelPortalPipe : UserEditableObject {

	public static string linkLevelKey = "LinkLevel";
//	public string itemName = "Level Portal";
	public string destinationCode = "default";
	public string destinationName = "The Overworld";
	public Transform playerOutT;
	public static string portalDestinationKey = "destinationLevelCode";
	public static string portalNameKey = "destinationLevelName";
	public Renderer levelImage;


	#region UserEditable Inherited Methods
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();
		N[linkLevelKey][portalDestinationKey] = destinationCode;
		N[linkLevelKey][portalNameKey] = destinationName;
		return N;
	}



	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] {
			LevelBuilder.inst.POCMlinkLevelButton,
			LevelBuilder.inst.POCMheightButton
		};
	}

	public override void OnGameStarted(){
		base.OnGameStarted();
		
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		destinationCode = N[linkLevelKey][portalDestinationKey].Value;
		destinationName = N[linkLevelKey][portalNameKey].Value;
		WebGLComm.inst.GetLevelScreenshot(destinationCode,this.gameObject,"SetImage");
	}
	#endregion

	public void SetImage(Texture2D tex){
		WebGLComm.inst.Debug("Screenshot set image:"+tex);
		levelImage.material.mainTexture = tex;
	}


	bool portalActivated = false;
	void OnTriggerEnter(Collider other){
		if (!warmedUp) return; // don't allow this portal to work so early in the game.
		if (!portalActivated){
			if (other.CompareTag("Player")){
				
				JsonLevelSaver.inst.SaveLevel(SceneSerializationType.Instance); // sneakily save the level whenever the player approaches a pipe regardless of their decision at the pipe
				Inventory.inst.SaveInventory();
				if (destinationCode == "") {
					PlayerNowMessage.inst.Display("This portal pipe has no destination!",transform.position);
					return;
				}
		
				PlayerDialogue.inst.ShowPlayerDialogue("Travel to the next level: "+destinationName,"Travel",icon);

				PlayerDialogue.inst.playerPressedOKDelegate += PlayerPressedOK;
				PlayerDialogue.inst.playerPressedCancelDelegate += PlayerPressedCancel;


			}
		}
	}

	float portalActivatedTimer = 0f;
	void PlayerPressedOK(){
		portalActivated = true;
		portalActivatedTimer = 10f;
		AnalyticsManager.inst.SendAnalytics();
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
		WebGLComm.inst.LoadLevelInstanceFromPortal(destinationCode);
		PlayerDialogue.inst.HidePlayerDialogueInstant();
//		PlayerDialogue.inst.ShowLoadingDialogue();

//		// commented Debug.Log("Player pessed ok on portal");
//		PlayerDialogue.inst.HidePlayerDialogue();
	}

	void PlayerPressedCancel(){
		MovePlayerToOutT();
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerPressedOK;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerPressedCancel;
	}

	public void MovePlayerToOutT(){
//		Player.inst.transform.position = playerOutT.position;
		Player.inst.SetPosition(playerOutT);
	}


	bool warmedUp = false;
	float warmUpTimer = 1.0f;
	float playerNotifyRange = 25f;
	void Update(){
		if (portalActivated){
			portalActivatedTimer -= Time.deltaTime;
			if (portalActivatedTimer <0){
				portalActivated = false;
			}
		}
//		if (Vector3.Magnitude(Player.inst.transform.position-transform.position)<playerNotifyRange){
//			PlayerNowMessage.inst.Display("Entering: "+destinationName);
//		}
		if (!warmedUp && warmUpTimer > 0){
			warmUpTimer -= Time.deltaTime;
		} else {
			warmedUp = true;
		}
	}




}
