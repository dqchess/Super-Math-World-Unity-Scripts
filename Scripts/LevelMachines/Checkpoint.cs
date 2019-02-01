using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum CheckpointStatus {
	Ready, // Player has never touched this checkpoint
	Current, // Player ha touched this checkpoint and its the current checkpoint
	Used // Player has touched this checkpoint but they touched another one afterwrads so its no long ercurrent;
}

public class Checkpoint : UserEditableObject {


	public static string checkpointKey = "checkpointKey";

	bool activated = false;
	bool currentCheckpoint = false;

	public Sprite saveIcon;
	public GameObject checkMark;
	public Material greenMaterial;
	public GameObject glowFX;
	public GameObject playerStart;
	public GameObject particles;
	public GameObject risingParticles;
	public CheckpointStatus checkpointStatus = CheckpointStatus.Ready;
	public GameObject checkpointBase;

	#region UserEditable
	public override GameObject[] GetUIElementsToShow(){
		//		// commented Debug.Log("meh?");
		return new GameObject[] { LevelBuilder.inst.POCMheightButton, LevelBuilder.inst.POCMcopyButton };
	}
	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		N[checkpointKey] = checkpointStatus.ToString();
		return N;
	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(checkpointKey)){
			if (N[checkpointKey].Value == "Current"){
				SetCheckpointStatus(CheckpointStatus.Current,"setprop");
			} else if (N[checkpointKey].Value == "Used"){
				SetCheckpointStatus(CheckpointStatus.Used,"setprop");
			}
		}

	}

	public override void OnGameStarted(){
		base.OnGameStarted();

	}

	#endregion

	public CheckpointStatus GetCheckpointStatus(){
		return checkpointStatus;
	}

	public void SetCheckpointStatus(CheckpointStatus status, string source="default"){
//		WebGLComm.inst.Debug("Set chkpt;"+status.ToString()+", source;"+source);
		if (status == CheckpointStatus.Current){
			Player.inst.AddPlayerStartPriority(PlayerStartType.Checkpoint,playerStart.transform);
			PostActivatedFX();
			AudioManager.inst.PlaySuccessBeep();
			foreach(Checkpoint c in FindObjectsOfType<Checkpoint>()){
				// Deactivate the previous current checkpoint if applicable.
				if (c == this) continue; // don't deactivate yourself.
				if (c.GetCheckpointStatus() == CheckpointStatus.Current){
					c.SetCheckpointStatus(CheckpointStatus.Used," nother checkp activated");
				}
			}
		} else if (status == CheckpointStatus.Used){
			PostActivatedFX();
			checkMark.GetComponent<Renderer>().material.color = new Color(.3f,.3f,.3f,.5f);
		}
		checkpointStatus = status;
//		Debug.Log("setstatus;"+status.ToString());
	}


	void OnTriggerEnter(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (checkpointStatus != CheckpointStatus.Ready) return;
		if (GameManager.inst.timeOnThisLevel < 2) return; // awkward, but sometimes the level is restarted and the timing happens ot check a checkpoint if you were standing on one during restart. Bad order or operations I guess
		if (other.CompareTag("Player")) {
			ActivateCheckpoint();
		}

	}
	void ActivateCheckpoint(){
		SetCheckpointStatus(CheckpointStatus.Current,",playerclickdsave");
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.checkpointReached,1);
		particles.SetActive(true);
		risingParticles.SetActive(false);
		PlayerNowMessage.inst.Display("Checkpoint activated!", transform.position);
		JsonLevelSaver.inst.SaveLevel(SceneSerializationType.Instance);
		Inventory.inst.SaveInventory();
		AudioManager.inst.PlayCheckpointSound();
	}


	bool fxFired = false;
	void PostActivatedFX(){
		if (fxFired) return;
		fxFired = true;
		int singles = Random.Range(5,12);
		for(var i=0;i<singles;i++){
			EffectsManager.inst.GemDropX1(transform.position+Vector3.up*4+MathUtils.RandomInsideHalfSphere*2f);
		}
		Material[] mats = checkpointBase.GetComponent<Renderer>().materials;
		mats[1].color = new Color(0,.5f,0,1);
		checkpointBase.GetComponent<Renderer>().materials = mats;
		GetComponent<Collider>().enabled = false;
		Destroy(checkMark.GetComponent<SinHover>());
		checkMark.GetComponent<Renderer>().material = greenMaterial;
//		MoveAlongAxis maa = checkMark.AddComponent<MoveAlongAxis>();
//		maa.dir = Vector3.up;
//		Destroy(glowFX.GetComponent<SinTransparency>());
		glowFX.GetComponent<Renderer>().material.color = new Color(0,0,1,0.24f);
	}



}
