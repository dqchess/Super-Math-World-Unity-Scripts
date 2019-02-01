using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_FactorPlatformStatic : MonoBehaviour {

	public Fraction frac;
	public GameObject platformPrefab;
	public List<GameObject> platforms = new List<GameObject>();
	public float scale = 10;

	float timer = 0;
	void Update(){

	}


	void BuildPlatforms(){
		foreach(GameObject o in platforms){
			Destroy (o);
		}
		platforms.Clear();
		for (int i=1;i<30;i++){
			GameObject plat = (GameObject)Instantiate(platformPrefab);
			plat.transform.parent = transform;
			plat.transform.localPosition = -transform.right + transform.right * i * scale;
//			plat.name = "plat";
			platforms.Add (plat);
//			// commented Debug.Log ("asfloat; " +frac.GetAsFloat());
			int mod = i % (int)(frac.GetAsFloat()) ;

//			// commented Debug.Log ("mod : " + i +"," +frac.GetAsFloat()+" was: " +mod);
			if (mod == 0){

				plat.GetComponent<Collider>().enabled = true;
				plat.GetComponent<Renderer>().material = NumberManager.inst.pos_block;
				plat.GetComponent<Renderer>().material.color = Color.white;
				plat.name = "white";
			} else {
				plat.GetComponent<Collider>().enabled = false;
//				plat.GetComponent<Renderer>().material.color = Color.black;
				plat.name = "black";
			}
		}
	}

	public void ReceiveNumber(GameObject obj){
		frac = obj.GetComponent<NumberInfo>().fraction;
//		obj.SendMessage("StopDying",SendMessageOptions.DontRequireReceiver);
		Destroy(obj.GetComponent<Rigidbody>());
		obj.GetComponent<Collider>().isTrigger=true;
		obj.transform.parent = transform;
		obj.transform.localScale = Vector3.one * 3;

		if (frac.denominator != 0 && frac.numerator != 0){
			BuildPlatforms();
		}
	}

}
