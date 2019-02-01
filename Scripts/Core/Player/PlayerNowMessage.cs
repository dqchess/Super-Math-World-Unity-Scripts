using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using System.Text;

public class PlayerNowMessage : MonoBehaviour {

	public enum PlayerNowMessageDisplayMode {
		Hidden,
		Showing,
		Instant
	}

	PlayerNowMessageDisplayMode mode = PlayerNowMessageDisplayMode.Hidden;

	public static PlayerNowMessage inst;
	public Image icon;
	public Text now;
	float t = 0;

	// typewriter vars
	string targetText = "";
	int textIndex = 0;
	float typeTimer = 0f;
	float letterInterval = 0.03f;

	void Start(){
		now.color = new Color(1,1,1,0);
	}

	public void SetInstance(){
		inst = this;
	}

	public void Display(string s){
		Display(s,Player.inst.transform.position);
	}
	public void Display(string s, Vector3 p){
		Display(s,Color.white,p);

	}
	Vector3 lastPlaceSpokenTo = Vector3.zero;
	float sqrDistToClear = 200f; // if you run away the speech disappears.
	float displayTimer = 0;
	string lastThingSaid = "";
	bool usingIcon = false;
	public void Display(string s, Color col, Vector3 p,  Sprite img = null){
		if ( mode == PlayerNowMessageDisplayMode.Showing && s == lastThingSaid) return;


		icon.sprite = img;
		if (img != null){
			usingIcon = true;
			icon.color = Color.white;
		} else {
			usingIcon = false;
		}
		lastThingSaid = s;
		lastPlaceSpokenTo = p;


		now.color = col;
		textIndex = 0;
		targetText = s;
		targetText = Utils.UsePlayerNameInText(targetText);
		mode = PlayerNowMessageDisplayMode.Showing;
		now.color = Color.white;
		now.text = "";
		t = targetText.Length/4f;
		typeTimer = 0;
		if (PlayerTextToSpeech.speechEnabled) PlayerTextToSpeech.inst.Speak(s);
	}

	public void DisplayInstant(string s){
		now.color = Color.white;
		now.text = s;
		mode = PlayerNowMessageDisplayMode.Instant;
		t = 5;

	}

	public void Hide(){
		t = 0;
		now.color = new Color(1,1,1,0);
		mode = PlayerNowMessageDisplayMode.Hidden;
	}


//	StringBuilder sb =  new StringBuilder();
	void Update(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			now.text = "";
			return;
		}

		if ( mode == PlayerNowMessageDisplayMode.Instant){
			t -= Time.deltaTime;
			if (t  < 0){
				Hide();
			}
		}

		if (mode == PlayerNowMessageDisplayMode.Showing) {
			if (Vector3.SqrMagnitude(Player.inst.transform.position-lastPlaceSpokenTo) > sqrDistToClear){
				t = Mathf.Min(t,4);
			}
			typeTimer -= Time.deltaTime;
			now.text = targetText.Substring(0,textIndex+1);
			//				// commented Debug.Log("index:"+textIndex+", ttlen:"+targetText.Length);
			if (targetText[textIndex] == '.') {
				typeTimer += 1.5f; // add a delay for periods.
			}
			if (textIndex < targetText.Length-1) {
				textIndex ++;
			}

			t -= Time.deltaTime;
			if (t < 0){
				float lerpSpeed = 5f;
				Color lerpC = Color.Lerp(now.color,new Color(1,1,1,0),Time.deltaTime * lerpSpeed);
				now.color = lerpC;
				if (usingIcon) icon.color = lerpC;
				if (now.color.a < .01f){
					now.color = new Color(1,1,1,0);
					icon.color = new Color(1,1,1,0);
					Hide();
				}
			}
		}
	}

	public void DisplayAfterDelay(string s, float delay = 1f){
		StartCoroutine(DisplayAfterDelayE(s,delay));
	}

	IEnumerator DisplayAfterDelayE(string s, float delay){
		yield return new WaitForSeconds(delay);
		Display(s,Player.inst.transform.position);
	}
}
