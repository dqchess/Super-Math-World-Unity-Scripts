using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelBuilderMessager : MonoBehaviour {

	public enum MessageFadeState {
		In,
		Showing,
		Out,
		Hidden

		
	}

	public static LevelBuilderMessager inst;
	public MessageFadeState fadeState = MessageFadeState.Out; // fade out on start.
	public Transform messageParent;
	public Text messageText;

	public void SetInstance(){
		inst = this;
	}


	float displayTime = 1f;
	public void Display(string message){
		Display(message,3f);
	}
	public void Display(string message, float showForSeconds){
//		Debug.Log("Display:"+message);
		displayTime = showForSeconds;
		fadeState = MessageFadeState.In;
		messageText.text = message;
		t = displayTime;
	}


	float t = 0;
	void Update(){
		switch(fadeState){
		case MessageFadeState.In:
			if (Utils.FadeUiElements(messageParent,1f,10f)){
				fadeState = MessageFadeState.Showing;
			}
			break;
		case MessageFadeState.Showing:
			t -= Time.deltaTime;
			if (t < 0){
				fadeState = MessageFadeState.Out;
			}
			break;
		case MessageFadeState.Out:
			if (Utils.FadeUiElements(messageParent,0f,10f)){
				fadeState = MessageFadeState.Hidden;
			}
			break;
		default:break;
		}
	}
}
