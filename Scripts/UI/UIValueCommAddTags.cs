using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIValueCommAddTags : UIValueComm {


	public InputField tagsInput;

	override public void OnMenuOpened() {
		if (LevelBuilder.inst.currentPiece){
			UserEditableObject ueo = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>();

			tagsInput.text = string.Join(",",ueo.myTags.ToArray());

		}
	}

	public void SetTags(){
		if (LevelBuilder.inst.currentPiece){
			UserEditableObject ueo = LevelBuilder.inst.currentPiece.GetComponent<UserEditableObject>();
			ueo.ClearTags();
			if (tagsInput.text != "") {
				List<string> ts = new List<string>();
				ts.AddRange(tagsInput.text.Replace(" ",string.Empty).Split(','));
				ueo.AddTags(ts);
//				Debug.Log("ueo name;"+ueo.myName+",tags:"+ueo.tags[0]);
			}
		}
	}
}
