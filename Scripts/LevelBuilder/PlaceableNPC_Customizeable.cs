using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using System.Text.RegularExpressions;

public class PlaceableNPC_Customizeable : UserEditableObject {

	public string speechToSayToPlayer = "";
	public string characterName = "Noname";
	public GameObject speechAlertThoughtBubble;

	#region UserEditable 
	public static string speechKey = "NpcSpeech";
	public override SimpleJSON.JSONClass GetProperties(){ 
		//		Dictionary<string,string> properties = new Dictionary<string,string>();
		SimpleJSON.JSONClass N = base.GetProperties();

		string s = Utils.RealToFakeQuotes(speechToSayToPlayer);

//			s = s.Replace("^quot^","'");
//			s = s.Replace("^dquot^","\"");
//		N[speechKey] = Regex.Escape(speechToSayToPlayer);
		N[speechKey] = s;
//		// commented Debug.Log(name+" returned "+N.ToString()+ " as speech");
		return N;
	}




	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] { LevelBuilder.inst.placedObjectContextMenucharacterSpeechBubbleButton };
	}





	/* footpring was: (){
		return 3;
	 */

	public override void OnGameStarted(){
		base.OnGameStarted();

	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		speechToSayToPlayer = N[speechKey].Value;
	}
	#endregion

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			string s = Utils.FakeToRealQuotes(speechToSayToPlayer);

			string displayString = s;
//			// commented Debug.Log("char saying:"+displayString);
//			PlayerNowMessageWithBox.inst.Display(displayString,3,icon);
			if (speechAlertThoughtBubble.activeSelf){
				speechAlertThoughtBubble.SetActive(false);
			}
		}
	}
}
