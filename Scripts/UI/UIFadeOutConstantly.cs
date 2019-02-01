using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFadeOutConstantly : MonoBehaviour {

	public Text t;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	float frames = 0;
	bool fade = false;
	void Update () {
		frames ++;
		if (frames > 30){
			frames = 0;
			if (t.color.a > 0){
				fade = true;
			}
		}
		if (fade){
			float fadeSpeed = 0.5f;
			t.color = Color.Lerp(t.color,new Color(t.color.r,t.color.g,t.color.b,0),Time.unscaledDeltaTime * fadeSpeed);
		}
	}
}
