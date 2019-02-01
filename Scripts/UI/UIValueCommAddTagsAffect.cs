using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class UIValueCommAddTagsAffect : UIValueComm {


	public InputField tagsInput;

	override public void OnMenuOpened() {
		if (LevelBuilder.inst.currentPiece){
			ButtonTagAffector baf = LevelBuilder.inst.currentPiece.GetComponent<ButtonTagAffector>();
			if (baf) {
//				Debug.Log("baf!");
				tagsInput.text = string.Join(",",baf.tagsToSend.ToArray());
			}
		}
	}

	public void SetTags(){
//		Debug.Log("set tags!");
		if (LevelBuilder.inst.currentPiece){
			ButtonTagAffector baf = LevelBuilder.inst.currentPiece.GetComponent<ButtonTagAffector>();
			baf.tagsToSend.Clear();
			if (baf && tagsInput.text != "") {
//				Debug.Log("baf, and not empty!");
				baf.tagsToSend.AddRange(tagsInput.text.Replace(" ",string.Empty).Split(','));
//				Debug.Log("baf tags to send:"+string.Join(",",baf.tagsToSend.ToArray()));
			}
		}
	}
}
