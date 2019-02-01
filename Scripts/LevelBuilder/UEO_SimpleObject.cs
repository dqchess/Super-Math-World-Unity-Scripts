using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UEO_SimpleObject : UserEditableObject {

//	public string objectName;

//	public int gridSnap = 10;
	public bool copy = true;
	public bool height = true;
//	public bool rotationOffset = false;  // for rotating big concrete pads to fix zfigthing when people place them so that surfaces overlap.
//	public bool scaleManipulator = false;
	public bool hasFraction = false;
	public bool textTrigger = false;
	public bool cannon = false;
	public bool gemDrop = false;


	public bool carryAdditionalPiecesOnMove = false;

	public string ueo_notes; // strictly for my reference later in editor -- lets me know something unique about this prefab object
	#region UserEditable 
	public override SimpleJSON.JSONClass GetProperties(){ 
		SimpleJSON.JSONClass N = base.GetProperties();// new SimpleJSON.JSONClass();
//		Debug.Log("gettin prop...:"+N.ToString());

		if (cannon){
			N[ConveyerWhoosherSpeeder.speedKey].AsInt = Mathf.RoundToInt(GetComponentInChildren<ConveyerWhoosherSpeeder>().speed);

		}
		if (textTrigger){
			// Todo : factor this into all UserEditableObject so you can add text triggers to any UEO.
			PlayerNowMessageTrigger tt = GetComponentInChildren<PlayerNowMessageTrigger>();
			if (tt){
				N[PlayerNowMessageTrigger.textTriggerKey] = Utils.RealToFakeQuotes(tt.thingToSay);
			} else {
				// commented Debug.LogError("Getprop UEO: No text script is a child of "+myName+", but it's labeled as text trigger.");
			}
		}
		return N;

	}

	public override void OnLevelBuilderObjectCreated(){
		
		base.OnLevelBuilderObjectCreated();
//		if (rotationOffset){
//			Quaternion rot = transform.rotation;
//			rot.eulerAngles = new Vector3(rot.eulerAngles.x + .04f,rot.eulerAngles.y + .04f,rot.eulerAngles.z + .04f); // offset for zfighting
//			transform.rotation = rot;
//		}
	}

//	public override void OnLevelBuilderObjectPlaced(){
//	}

//	/	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> ui = new List<GameObject>();
		ui.AddRange(base.GetUIElementsToShow());
		if (copy) ui.Add(LevelBuilder.inst.POCMcopyButton);
		if (height) ui.Add(LevelBuilder.inst.POCMheightButton);
		if (hasFraction) ui.Add(LevelBuilder.inst.POCMFractionButton);
		if (textTrigger) ui.Add(LevelBuilder.inst.POCMTextTriggerButton);
		if (cannon) ui.Add(LevelBuilder.inst.POCMModCannonButton);
//		if (gemDrop) ui.Add(LevelBuilder.inst.POCMGemDropButton);
		return ui.ToArray();

//		return null;
	}


//	public override float GridSnapSpacing(){
//		return gridSnapS;
//	}


	/* footpring was: (){
		return 1;
	 */


	public override void OnGameStarted(){
		base.OnGameStarted();

	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);

		if (cannon && N.GetKeys().Contains(ConveyerWhoosherSpeeder.speedKey)){
			GetComponentInChildren<ConveyerWhoosherSpeeder>().speed = N[ConveyerWhoosherSpeeder.speedKey].AsInt;

		}
		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
		//		// commented Debug.Log("props:"+props);
		//		SimpleJSON.JSONNode n = SimpleJSON.JSON.Parse(props);
//		// commented Debug.Log("set prop received:"+N.ToString());


		if (textTrigger && N.GetKeys().Contains(PlayerNowMessageTrigger.textTriggerKey)){
			PlayerNowMessageTrigger tt = GetComponentInChildren<PlayerNowMessageTrigger>();
			if (tt){ 
				tt.thingToSay = Utils.FakeToRealQuotes(N[PlayerNowMessageTrigger.textTriggerKey].Value);
			} else {
				// commented Debug.LogError("Set prop fail, no speech, couldn't say '"+N[PlayerNowMessageTrigger.textTriggerKey].Value+"' on "+myName);
			}
		}






	}



	#endregion
}
