using UnityEngine;
using System.Collections;

public class LevelMonster_Shield : MonoBehaviour {



	void OnCollisionEnter(Collision hit){
//		// commented Debug.Log ("nothing");
//		if (hit.collider.gameObject.GetComponent<NumberInfo>()){
//			// commented Debug.Log ("number info hit me.");
//			GetComponent<Renderer>().material.SetColor("_MainColor",new Color(.5f,.5f,.7f,1f));
		if (hit.collider.GetComponent<DoesExplodeOnImpact>()) return;
		ShieldFX();
			
//		} else {
//			// commented Debug.Log ("hit collide rno ni:"+hit.collider.name);
//		}
	}
	void ShieldFX(){
		needsRevert = true;
		GetComponent<Renderer>().material.SetColor("_Color",new Color(1,1,1,1f));
		shieldTimer = .7f;
		AudioManager.inst.PlayElectricDischarge1(transform.position);
	}

	float shieldTimer = 1.4f;
	bool needsRevert = false;
	void Update(){
		if (shieldTimer > 0){
			shieldTimer -= Time.deltaTime;
			Color lerpFrom = GetComponent<Renderer>().material.GetColor("_Color");
			Color lerpTo = new Color(.5f,.5f,.7f,0f); // clear
			float lerpSpeed = 2.2f;
			Color result = Color.Lerp (lerpFrom,lerpTo,Time.deltaTime * lerpSpeed);
			GetComponent<Renderer>().material.SetColor("_Color",result);
//			// commented Debug.Log ("result;"+result);
		} else if (needsRevert){
			needsRevert = false;
			GetComponent<Renderer>().material.SetColor("_Color",new Color(1,1,1,0));
		}
	}

	public void PlayerTouched(){
		ShieldFX();
		float playerBackOffPushDistance = Random.Range(4f,25f);
		Vector3 flatVector = Vector3.Normalize(Utils.FlattenVector(Player.inst.transform.position - transform.position)) * playerBackOffPushDistance;
//		// commented Debug.Log("push:"+flatVector);
			
		Player.inst.transform.position += flatVector;
	}
}
