using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeWarm : MonoBehaviour {

//	bool warming = false;

	bool warming = false;
	bool warmed = true;
	Renderer rend;
	public bool transparent = false;
	List<CubeWarmer> cubeWarmers = new List<CubeWarmer>();

	void Start(){
		rend = gameObject.GetComponent<Renderer>();
//		rend = transform.FindChild("mesh").GetComponent<Renderer>(); 
		rend.material.color = new Color(.1f,.1f,.1f);
		if (transparent) {
			rend.material.color = new Color(0,0,0,0);
		}
	}

	float warmTimer = 0;
	Color targetLerpColor;
	float colorLerpTime = 0;
	void Update(){
		colorLerpTime -= Time.deltaTime;
		warmTimer -= Time.deltaTime;
		if (warmTimer < 0){
//			// commented Debug.Log ("warming false on : "+name);
			Cool ();
		}

		if (colorLerpTime > 0){
			if (rend.material.HasProperty("_Color")){
				rend.material.color = Color.Lerp (rend.material.color,targetLerpColor,Time.deltaTime/colorLerpTime*4);
	//			// commented Debug.Log ("target col:" + targetLerpColor);
				if (colorLerpTime < 0.05f){
					rend.material.color = targetLerpColor;
					colorLerpTime = 0;
				}
			}
		}

	}

	void Cool(){
//		// commented Debug.Log ("cool");
		warmed = false;
		GetComponent<Collider>().isTrigger=true;
		colorLerpTime = 1;
		targetLerpColor = new Color(.1f,.1f,.1f,0);
		warming = false;
		warmth = 0;
		cubeWarmers = new List<CubeWarmer>();
	}

	float warmth = 0;
	public void Warm(CubeWarmer cw, float warmNo){
//		// commented Debug.Log ("warm");
		warmTimer = 1;
		UpdateWarmth();
		if (!cubeWarmers.Contains(cw)){
			cubeWarmers.Add (cw);
			warmth += warmNo;
			if (warmth > 0) {
				warming = true;
				warmed = true;
				colorLerpTime = 1;
				targetLerpColor = Color.white;
//				// commented Debug.Log ("Color white on: " +name);
				GetComponent<Collider>().isTrigger=false;
			}
			else warming = false;
//			// commented Debug.Log ("warming with warmno: " + warmNo + " ! warmth: " + warmth);
//			// commented Debug.Log ("Warmth : "+warmth+ " on " + nsdwame);
		}
	}

	void UpdateWarmth(){
		warmth = 0;
		List<CubeWarmer> toRemove = new List<CubeWarmer>();
		foreach(CubeWarmer ccw in cubeWarmers){
			if (ccw==null) {
				toRemove.Add (ccw);
			} else if (Vector3.Magnitude(transform.position - ccw.transform.position) > ccw.warmNo * CubeWarmerManager.radiusFactor){
				toRemove.Add (ccw);
				warmth -= ccw.warmNo;
			}
		}
		foreach(CubeWarmer ccw in toRemove){
			cubeWarmers.Remove (ccw);
		}
		foreach(CubeWarmer ccw in cubeWarmers){
			warmth += ccw.warmNo;
		}

	}

}
