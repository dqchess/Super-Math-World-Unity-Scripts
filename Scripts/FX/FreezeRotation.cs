using UnityEngine;
using System.Collections;

public class FreezeRotation : MonoBehaviour {
#if !UNITY_IPHONE
	Quaternion fr;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	bool isset=false;
	void LateUpdate () {

		if (Input.GetMouseButtonUp(1)){
			isset=false;
//			GlobalVars.inst.playerPivot.transform.localRotation = Quaternion.identity;
//			GlobalVars.inst.playerGraphics.transform.localRotation=Quaternion.identity;
		}
		if (Input.GetMouseButtonDown(1)){
			fr = transform.rotation;
			isset=true;
		}
		
		if (isset){
			transform.rotation=fr;
		} else if (!Player.frozen) {
			Vector3 r1 = Player.inst.pivot.localRotation.eulerAngles;
//			Vector3 r2 = GlobalVars.inst.playerPivot.transform.localRotation.eulerAngles;
			Quaternion rot = new Quaternion();
			r1 = new Vector3(r1.x,0,0);
			rot.eulerAngles = r1;
			Player.inst.pivot.localRotation = rot; //new Vector3(r1.x,0,0);
			PlayerCostumeController.inst.curCharInfo.playerBodyGraphics.transform.localRotation=Quaternion.identity;
//				= Quaternion.Slerp (GlobalVars.inst.playerPivot.transform.localRotation,Quaternion.identity,Time.deltaTime);
//			GlobalVars.inst.playerGraphics.transform.localRotation=Quaternion.Slerp (GlobalVars.inst.playerGraphics.transform.localRotation,Quaternion.identity,Time.deltaTime);
		}
	}

#endif
}