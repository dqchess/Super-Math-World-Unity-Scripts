using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;


public class NumberHoopModifyable : MonoBehaviour {

	public Renderer scrollFX;
	public NumberHoop nh;
	public Transform startPoint;
	public Transform endPoint;
	bool eating = false;
	public GameObject eatingNumber;

	public void ReceiveObject(GameObject other){
		if (eating) return;
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni && ni.fraction.denominator == 1){
			AudioManager.inst.PlayCartoonEat(transform.position,1);
			eating = true;
//			eatStartTime
			eatingNumber = ni.gameObject;
			Destroy(eatingNumber.GetComponent<Collider>());
			Destroy(eatingNumber.GetComponent<Rigidbody>());
			eatingNumber.transform.position = startPoint.position;
		}
	}


//	bool scrollFx = false;
	float scrollFxTimer = 0;
	void Update(){
		if (eating){
			float mdd = Time.deltaTime * 2; // eat speed
			eatingNumber.transform.position = Vector3.MoveTowards(eatingNumber.transform.position,endPoint.position,mdd);
			eatingNumber.transform.localScale *= 0.99f;
			if (Vector3.SqrMagnitude(eatingNumber.transform.position-endPoint.position)<.5f){
				nh.SetProperties(JsonUtil.ConvertFractionToJson(Fraction.fractionKey,eatingNumber.GetComponent<NumberInfo>().fraction));
				Destroy(eatingNumber);
				eating = false;
				AudioManager.inst.PlayItemGetSound(); //(transform.position,1,false);
				AudioManager.inst.PlayTimerDing();
//				scrollFx = true;
				scrollFxTimer = 1.5f;
			}
		}
		if (scrollFxTimer > 0){
			scrollFxTimer -= Time.deltaTime;
			Vector2 of = scrollFX.material.GetTextureOffset("_MainTex");
			float scrollspeed = Time.deltaTime * 2f;
			scrollFX.material.SetTextureOffset("_MainTex",new Vector2(of.x + scrollspeed,of.y));
		}
	}
}






