using UnityEngine;
using System.Collections;

public class HelpTextBox : MonoBehaviour {
	
//	public GameObject top;
//	public GameObject mid;
//	public GameObject bot;
	public GameObject smallTopBox;
	public GameObject bigBox;
	float charInterval=.03f;

	public CCText bigCCText;
	public CCText smallCCText;
	CCText currentText;

	public delegate void EventHandler(string message);
	public event EventHandler OnHide;

	public Renderer bigIcon;

	
	public bool showing = false;
	float t = 0;
	
	void Start() {
		Hide(false);
	}
	
	public void Hide(bool unfreezePlayer = true) {
		bigBox.SetActive(false);
		smallTopBox.SetActive(false);
		showing = false;
		if (unfreezePlayer) {
			Player.inst.UnfreezePlayer("helpclose");
//			Player.inst.UnfreezePlayerFreeLook();
		}
		if (OnHide != null) OnHide("message");
	}

	void ResetTypewriter(string text){
		typewriterText = text;
		typewriterTimer = 0;
		typewriterIndex=0;
	}

	public void TypewriterSay(CCText targetText, string s, float extraTime=1f){
		float timeMod = 1;
		ResetTypewriter(s);
		currentText = targetText;
		t = s.Length * timeMod * charInterval + 6 + extraTime;
		showing=true;
	}

	public void ShowSmall(string text, int sound=1, float extraTime = 1f) {
		Hide ();
		smallTopBox.SetActive(true);

		TypewriterSay(smallCCText,text,extraTime);
		if (sound==1) AudioManager.inst.PlayNotify1();
		else if (sound==2) AudioManager.inst.PlayNotify2();
	}

	public bool showingbig=false;
	public void ShowBig(string text,Texture icon,float extraTime = 1){
		if (showingbig) return;
		Hide ();
		showingbig=true;
//		// commented Debug.Log ("Show big");
		MouseLockCursor.ShowCursor(true,"helpbig");

		bigBox.SetActive(true);
		TypewriterSay(bigCCText,text,extraTime);
		bigIcon.material.mainTexture = icon;
	}

	public void ShowYesNoDialogueBox(string text, Texture icon){
//		FindObjectOfType<DialogueBoxYesNo>().
//		FindObjectOfType<DialogueBoxManager>().yesNoDialogueBox.SetActive(true);
	}

	float typewriterTimer=0;
	int typewriterIndex=0;
	string typewriterText="final text";


	void Update() {
		if(showing) {

			typewriterTimer+=Time.deltaTime;
			if (typewriterTimer > charInterval){
				typewriterIndex=Mathf.Min (typewriterText.Length,typewriterIndex+1);
				typewriterTimer=0;
			}

			currentText.Text = typewriterText.Substring(0,typewriterIndex);

			t -= Time.deltaTime;
			if(t <= 0 && bigCCText != currentText) {
				Hide();
			}
		}

	}




}
