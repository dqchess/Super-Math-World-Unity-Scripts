using UnityEngine;
using System.Collections;

public enum ShrinkSoundType {
	Soft,
	Click,
	Mute
}

public class ShrinkAndDisable : MonoBehaviour {

	float t = 0;
	Vector3 origScale;
	bool isEnabled = false;
	public bool instant = false;
	public GameObject background;

	public ShrinkSoundType type = ShrinkSoundType.Soft;
	// Use this for initialization
	void Start () {
		origScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (isEnabled){
			t += Time.unscaledDeltaTime;
			float shrinkSpeed = 5;
			transform.localScale -= Vector3.one * shrinkSpeed * Time.unscaledDeltaTime;
//			// commented Debug.Log("transform localsc:"+transform.localScale);
			if (transform.localScale.x < 0){
				foreach(ShrinkAndDisable sad in GetComponentsInChildren<ShrinkAndDisable>()){
					sad.gameObject.SetActive(false); // in case a parent disabled before a child was finished.
				}
				gameObject.SetActive(false);
				transform.localScale = origScale;
				Stop();
			}
		}
	}

	void OnEnable(){
		isEnabled = false;
	}

	public void Begin(){

		if (!gameObject.activeSelf) return; // already shrunk
		switch(type){
		case ShrinkSoundType.Click:
			AudioManager.inst.PlayClick2();
			break;
			case ShrinkSoundType.Soft:
			AudioManager.inst.PlayCloseMenuSound(name);
			break;
		default:break;
		}
		if (background) background.SetActive(false);
//		// commented Debug.Log("Begin shirnk: "+name+" at "+Time.realtimeSinceStartup);
		if (GetComponent<SinPop>()){
			GetComponent<SinPop>().Stop();
		}
		t = 0;
		if (instant){
			gameObject.SetActive(false);
		} else {
			isEnabled = true;
		}
	}

	public void Stop(){
//		// commented Debug.Log("Stop shirnk: "+name+" at "+Time.realtimeSinceStartup);
//		// commented Debug.Log("disable");

		isEnabled = false;
//		// commented Debug.Log("end shrink");
	}
}
