using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeWarmer : MonoBehaviour {

//	float fireTimer = 0;
	float timer = 0;
	public float warmNo;
	GameObject flame;


	void Start(){
		InitFlame();

	}

	void Update(){
		timer -= Time.deltaTime;
		if (timer < 0){
			timer = .1f;
			warmNo = GetComponent<NumberInfo>().fraction.GetAsFloat();
			if (warmNo < 0) {
				AudioManager.inst.PlayFizzle(transform.position);
				EffectsManager.inst.CreateSmokePuff(transform.position,transform.up);
			}
			float warmSqrNo = warmNo * warmNo;
			foreach(CubeWarm c in FindObjectsOfType<CubeWarm>()){
				float dist = Vector3.Magnitude(c.transform.position-transform.position);
				if (dist < Mathf.Sqrt (warmNo * CubeWarmerManager.radiusFactor)){
					c.Warm(this,warmNo);
				}
//				// commented Debug.Log ("sqrdist : " + dist);
			}
		}

		Collider[] cols = Physics.OverlapSphere(transform.position,5,LayerMask.NameToLayer("Number"));
		foreach(Collider c in cols){
			if (c.GetComponent<NumberInfo>()){
				Destroy (c.gameObject);
			}
		}




	}
	bool flamed=false;
	public void InitFlame(){
		flame = (GameObject)Instantiate(EffectsManager.inst.flame,transform.position,Quaternion.identity);
		InventoryIcon i = gameObject.AddComponent<InventoryIcon>();
//		i.icon = FindObjectOfType<PlayerInventory>().fireIcon;
		flame.transform.parent = transform;
		flame.GetComponent<ParticleEmitter>().emit = true;
		flame.GetComponent<ParticleEmitter>().minSize=10;
		flame.GetComponent<ParticleEmitter>().maxSize=11;
		flamed=true;
	}

	void OnPlayerEquip(){
		if (!flamed) InitFlame();
		flame.GetComponent<ParticleEmitter>().minSize=5;
		flame.GetComponent<ParticleEmitter>().maxSize=6;
	}

	void OnPlayerThrow(){
		if (!flamed) InitFlame();
		flame.GetComponent<ParticleEmitter>().minSize=10;
		flame.GetComponent<ParticleEmitter>().maxSize=11;
	}

}
