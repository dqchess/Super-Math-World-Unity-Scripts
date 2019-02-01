using UnityEngine;
using System.Collections;

public class LeverSwitch : MonoBehaviour {

	bool pulled=false;
	int degToRotate=50;


	Vector3 startFwd;
	Quaternion startRot;

	// Use this for initialization
	void Start () {
		startFwd = transform.forward;
		startRot=transform.rotation;
//		FindObjectOfType<CoalTrain>().SetFrozen(true);
	}
	
	// Update is called once per frame
	float timer = 0;
	void Update () {
		timer += Time.deltaTime;
		if (!pulled){
			if (Input.GetMouseButtonDown(0)){
				if (PlayerGadgetController.inst.thrownNumberTimeout < 0){
					if (Vector3.Distance(Player.inst.transform.position,transform.position)<7){
						timer = 0;
						DefaultAction(); 

					}
				}
			}
		}
		if (pulled){
			float ang = Vector3.Angle(transform.forward,startFwd);
//			// commented Debug.Log ("ang: "+ang);
			if (ang<degToRotate){
				float rotSpeed = 40;
				transform.Rotate(-Vector3.right,Time.deltaTime*rotSpeed);
			}
			if (timer > 1.6f){
				DefaultReset ();
			}
		}
	}

	virtual public void DefaultAction(){

	}

	virtual public void DefaultReset(){
		transform.rotation=startRot;
		pulled=false;
	}
}
