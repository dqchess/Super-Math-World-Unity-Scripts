using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinPop : MonoBehaviour {
	
	public float factor = 1.02f; // normally 1.02
	float t = 0;
	public float speed = 5.5f; // normally 5.5
	bool isEnabled = false;
	float scale;
	Vector3 startScale;
	public GameObject background;
	
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
		lastTime = Time.realtimeSinceStartup;
		scale = transform.localScale.x;
		if (!gameObject.activeSelf) Destroy(this);
	}
	
	// Update is called once per frame
	float timer = 0;
	float lastTime = 0;
	void Update () {
		if (isEnabled){

			timer += Time.unscaledDeltaTime;
			if (timer > 1) {
				Stop(); 
				transform.localScale = startScale;
			}
			t = Mathf.Min(1, t + Time.unscaledDeltaTime * speed);
			transform.localScale = Vector3.one * scale * (Mathf.Sin(t * 3.14159f) * (factor - 1) + 1);
		}
	}

	public void Begin(){
		Begin(false);
	}
	public void Begin(bool mute){
//		// commented Debug.Log("begin pop");
		if (!mute) AudioManager.inst.PlayEquipNumber();
		if (background) background.SetActive(true);
		gameObject.SetActive(true);
//		// commented Debug.Log("Begin sinpop: "+name+" at "+Time.realtimeSinceStartup);
		if (GetComponent<ShrinkAndDisable>()){
			GetComponent<ShrinkAndDisable>().Stop();
		}
		timer = 0;
		t=0;
		lastTime = Time.realtimeSinceStartup;
		isEnabled = true;
	}

	public void Stop(){
//		// commented Debug.Log("end pop");
//		// commented Debug.Log("Stop sinpop: "+name+" at "+Time.realtimeSinceStartup);
		isEnabled = false;
	}
}
