using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNowMessageWithBox : MonoBehaviour {


	public static PlayerNowMessageWithBox inst;
	public Text now;
	public Image box;
//	public Image boxOutline;
	public Image portrait;
	public Image portraitSheen;
	// typewriter vars
	string targetText = "";
	int textIndex = 0;
	float typeTimer = 0f;
	float letterInterval = 0.015f;


	float t = 0;
	void Start(){
		now.color = new Color(1,1,1,0);
		SetAllColors(now.color);
	}

	public void SetInstance(){
		inst = this;
	}

	Vector3 lastPlaceSpokenTo;
	float sqrDistToClear = 10000f; // if you run away the speech disappears.
	string lastThingSaid = "";
	public void Display(string s, Sprite icon,Vector3 p){
		if (showing && s == lastThingSaid) return;
		lastThingSaid = s;
		lastPlaceSpokenTo = p;
//		if (PlayerNowMessage.inst.showing) return;
//		Debug.Log("dipslay.."+Time.time);
		portrait.sprite = icon;
//		// commented Debug.Log("disp:"+s);
		now.text = "";
		now.color = Color.black;
		box.color = new Color(1,1,1,1);
		portrait.color = Color.white;
//		portraitSheen.color = new Color(1,1,1,0.65f);
//		boxOutline.color = Color.black;
		targetText = s;
		targetText = Utils.UsePlayerNameInText(targetText);
		textIndex = 0;
		typeTimer = 0;
		showing = true;

		t = 2 + targetText.Length/8f;
		if (PlayerTextToSpeech.speechEnabled) PlayerTextToSpeech.inst.Speak(s);

	}


	public void Hide(){
		t = 0;
		SetAllColors(new Color(1,1,1,0));
		showing = false;
	}

	public bool showing = false;
	bool waitedExtraSecondsThisCharacter = false; // prevents duplicate waits for each "." in "..."
	int waitedExtraSecondsCounter = 0; // wait at least 5 characters before daring to pause again!
	void Update(){
		if (showing) {
			if (Vector3.SqrMagnitude(Player.inst.transform.position-lastPlaceSpokenTo) > sqrDistToClear){
				t = Mathf.Min(t,4);
			}
			typeTimer -= Time.deltaTime;
			if (typeTimer < 0 ){
				typeTimer = letterInterval;
				now.text = targetText.Substring(0,textIndex+1);
//				// commented Debug.Log("index:"+textIndex+", ttlen:"+targetText.Length);
				if (waitedExtraSecondsThisCharacter){
					waitedExtraSecondsCounter ++;
					if (waitedExtraSecondsCounter > 5){
						waitedExtraSecondsThisCharacter = false;
						waitedExtraSecondsCounter = 0;
					}
				}
				if (textIndex > 5){
					if ( targetText.Length > 5 // only try to pause if text is already long
						&& (
							OnePeriodOnly()
							|| targetText[textIndex] == '!'  // after any !
							|| targetText[textIndex] == '?')  // after any ?
						&& !waitedExtraSecondsThisCharacter) { // but not twice in a row within 5 seconds
						typeTimer += 1.5f; // add a delay for periods, exclanation marks and quetions.
						waitedExtraSecondsThisCharacter = true;
					}
				}
				if (textIndex < targetText.Length-1) {
					textIndex ++;
				}
			}


			t -= Time.deltaTime;
			if (t < 0){
				float lerpSpeed = 5f;
				now.color = Color.Lerp(now.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
				box.color = Color.Lerp(box.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
//				boxOutline.color = Color.Lerp(boxOutline.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
				portrait.color = Color.Lerp(portrait.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
//				Color lerpC = Color.Lerp(now.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
//				SetAllColors(lerpC);


				if (now.color.a < .01f){
					SetAllColors(new Color(1,1,1,0));

					showing = false;
				}
			}
		}
	}


	bool OnePeriodOnly(){
		return  targetText[textIndex] == '.' // after any .
			&& textIndex < targetText.Length - 4 // not at end
			&& targetText[textIndex+1] != '.'; // not with another period following it.
	}

//	bool AfterThreePeriods(){
//		return targetText[textIndex] == '.'
//			&& textIndex > 3 // not at beginning
//			&& 
//	}


	void SetAllColors(Color lerpC){
		now.color = lerpC;
		box.color = lerpC;
//		boxOutline.color = lerpC;
		portrait.color = lerpC;
//		portraitSheen.color = lerpC;

	}


}
