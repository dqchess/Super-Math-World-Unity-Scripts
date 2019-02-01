using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SinGrowNumber : MonoBehaviour {

	NumberInfo ni;

	public void Init(NumberInfo n){
		ni = n;
		ni.growTimer = 0;
		ni.growing = true;
//		ni.baseScale = 0.5f; //childMeshRenderer.transform.localScale.x;


	}

	void Update(){
		if (ni.growing) UpdateGrowing();
		if (ni.growTimer >= 1){
			Destroy(this);
		}
		ni.growTimer += Time.deltaTime;
	}

	void UpdateGrowing(){
		
		if (!ni.childMeshRenderer) return;
		if(ni.growing && ni.growTimer < 1) {
			//			Debug.Log("update growing:"+name);

			ni.growTimer += UnityEngine.Time.deltaTime * ni.growSpeed;
			float scale = Mathf.Sin(ni.growTimer * Mathf.PI) * ni.growScale + 1;

			Vector3 growingScale = Vector3.one * scale * ni.baseScale;
			ni.childMeshRenderer.transform.localScale = growingScale;

			if (ni.myShape == NumberShape.Cube) ni.digits.transform.localScale = growingScale;
			//			// commented Debug.Log("s:"+growingScale);

			//			digits.localScale = Vector3.one * scale;
			if(ni.growTimer >= 1) {
				ni.PostGrowFX();

				//				digits.localScale = Vector3.one;
				//transform.Find("digits").localScale  = Vector3.one * baseNumberScale;
				//				// commented Debug.Log ("reset child mesh to basescale:"+baseScale);
			}
		}
	}

}
