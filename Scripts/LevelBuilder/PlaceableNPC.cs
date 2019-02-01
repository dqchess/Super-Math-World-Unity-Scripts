using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class PlaceableNPC : UserEditableObject {

	public string speechToSayToPlayer = "";
	public string characterName = "Noname";
	public GameObject speechAlertThoughtBubble;
	public Sprite characterPortrait;
	public float pitch = 1;

	#region UserEditable 
	public static string speechKey = "NpcSpeech";
	public override SimpleJSON.JSONClass GetProperties(){ 
		//		Dictionary<string,string> properties = new Dictionary<string,string>();
		SimpleJSON.JSONClass N = base.GetProperties();
		string s = Utils.RealToFakeQuotes(speechToSayToPlayer);
		N[speechKey] = s;
//		// commented Debug.Log(name+" returned "+N.ToString()+ " as speech");
		return N;
	}




	public override GameObject[] GetUIElementsToShow(){
		return new GameObject[] { LevelBuilder.inst.placedObjectContextMenucharacterSpeechBubbleButton, LevelBuilder.inst.POCMheightButton };
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
		speechToSayToPlayer = Utils.FakeToRealQuotes(N[speechKey].Value);
	}
	#endregion

	void OnTriggerEnter(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (other.tag == "Player"){
//			if (PlayerNowMessage.inst.showing || PlayerNowMessageWithBox.inst.showing) return;
			string s = Utils.FakeToRealQuotes(speechToSayToPlayer); // speechToSayToPlayer.Replace("^quot^","'");
//			s = s.Replace("^dquot^","\"");
//			// commented Debug.Log("s:"+s);
			string displayString = s;
//			// commented Debug.Log("char saying:"+displayString);
			PlayerNowMessageWithBox.inst.Display(displayString,characterPortrait,transform.position);
			pitch = Random.Range(pitch*0.9f,pitch*1.05f);
			AudioManager.inst.PlayCartoonVoice(transform.position,1,pitch);
			if (speechAlertThoughtBubble.activeSelf){
				speechAlertThoughtBubble.SetActive(false);
			}
		}
	}
}
