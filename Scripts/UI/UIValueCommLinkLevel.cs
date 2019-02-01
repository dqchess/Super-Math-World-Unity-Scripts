using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class UIValueCommLinkLevel : UIValueComm {

	public Text levelCodeText;
	public Text levelNameText;

	override public void OnMenuOpened() {
		SimpleJSON.JSONClass N = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().GetProperties();
		if (N.GetKeys().Contains(LinkLevelPortalPipe.linkLevelKey)){
			levelCodeText.text = N[LinkLevelPortalPipe.linkLevelKey][LinkLevelPortalPipe.portalDestinationKey].Value;
			levelNameText.text = N[LinkLevelPortalPipe.linkLevelKey][LinkLevelPortalPipe.portalNameKey].Value;
		}
	}

	override public void SetObjectProperties() { 
		// commented Debug.Log("setting obj properties for portal:");
		if (levelCodeText.text.Length == 0 || levelNameText.text.Length == 0) {
			
			return;
		}
		base.SetObjectProperties();
		SimpleJSON.JSONClass N = new SimpleJSON.JSONClass();
		N[LinkLevelPortalPipe.linkLevelKey][LinkLevelPortalPipe.portalDestinationKey] = levelCodeText.text; 
		N[LinkLevelPortalPipe.linkLevelKey][LinkLevelPortalPipe.portalNameKey] = levelNameText.text;
		LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>().SetProperties(N);
		// commented Debug.Log("success, N:"+N.ToString());
	}



	public void OnEnable(){
		OnMenuOpened();
	}

}
