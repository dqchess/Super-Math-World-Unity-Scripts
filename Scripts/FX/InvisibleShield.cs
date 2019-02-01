using UnityEngine;
using System.Collections;

public class InvisibleShield : MonoBehaviour {

	public Vector2 startScale = new Vector2(30,30);
	public float scaleModSpeed = .1f;
	float interval = 1.3f;

	void OnCollisionEnter(Collision hit){
		//		// commented Debug.Log ("nothing");
		//		if (hit.collider.gameObject.GetComponent<NumberInfo>()){
		//			// commented Debug.Log ("number info hit me.");
		//			GetComponent<Renderer>().material.SetColor("_MainColor",new Color(.5f,.5f,.7f,1f));
		GetComponent<Renderer>().material.SetColor("_TintColor",new Color(1,1,1,1f));
		GetComponent<Renderer>().material.SetTextureScale("_MainTex",startScale);
		shieldTimer = interval;
		AudioManager.inst.PlayElectricDischarge1(transform.position);
//		// commented Debug.Log("hit");
		//		} else {
		//			// commented Debug.Log ("hit collide rno ni:"+hit.collider.name);
		//		}
	}


	float shieldTimer = .7f;

	void Update(){
		if (shieldTimer > 0){
			shieldTimer -= Time.deltaTime;
			Color lerpFrom = GetComponent<Renderer>().material.GetColor("_TintColor");
			Color lerpTo = new Color(.5f,.5f,.7f,0f); // clear
			float lerpSpeed = 3f;
			Color result = Color.Lerp (lerpFrom,lerpTo,Time.deltaTime * lerpSpeed);
			GetComponent<Renderer>().material.SetColor("_TintColor",result);
			Vector2 newScale = startScale + Vector2.one * (1 + interval - shieldTimer);	
			GetComponent<Renderer>().material.SetTextureScale("_MainTex",newScale);
			//			// commented Debug.Log ("result;"+result);
		} else if (GetComponent<Renderer>().material.GetColor("_TintColor").a != 0) {
			GetComponent<Renderer>().material.SetColor("_TintColor",new Color(0,0,0,0));
//			// commented Debug.Log("set to zero..wtf");
		}
	}

	public void PlayerTouched(){
		OnCollisionEnter(null);
	}
}
