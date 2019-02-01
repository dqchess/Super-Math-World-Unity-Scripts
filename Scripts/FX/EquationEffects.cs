using UnityEngine;
using System.Collections;

public class EquationEffects : MonoBehaviour
{
	public float t;
	
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	public void Reset() {
		t = 0;	
		gameObject.transform.localPosition = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(t < 1) {
			t += UnityEngine.Time.deltaTime / 5;
			gameObject.transform.localScale = new Vector3(1,1,1) * (t * 2 + 1);
			Vector3 pos = gameObject.transform.localPosition;
			pos.y -= UnityEngine.Time.deltaTime * -0.6f;
			gameObject.transform.localPosition = pos;
			CCText text = gameObject.GetComponentInChildren<CCText>();
			Color rgba = text.Color;
			rgba.a = 1 - t;
			text.Color = rgba;
		}
	}
}

