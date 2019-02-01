using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GadgetBoomerang : Gadget {

	List<Collider> touched = new List<Collider>();
	public Material squareMat;
	public Transform pivot;
	Quaternion originalRotation;
	bool flying = false;
	float rotateSpeed = 350;
	Vector3 thrownDirection;
	float timeThrown = 0;
	float speed = 30;
	float totalFlytime = 3f;
	public GameObject flyingBoomerangPrefab;
	override public void MouseButtonDown(){
		Fire();
	}

	public override void Fire(){
//		Debug.Log("fire!");
		if (flying) return;
		flying = true;
		GameObject flyingBoomerang = (GameObject)Instantiate(flyingBoomerangPrefab,gadgetGraphics.transform.position,Quaternion.identity);
		gadgetGraphics.GetComponent<Renderer>().enabled = false;
//		Debug.Log("flying!");
		flyingBoomerang.transform.forward = Player.inst.transform.right;
		flyingBoomerang.GetComponent<GadgetBoomerangFlyingObject>().Init (totalFlytime);
		timeThrown = Time.time;
		MascotAnimatorController.inst.HoldRightArm(false);
	}


	public override void GadgetUpdate() {
//		// commented Debug.Log ("gadget updating... "+Time.time);
		if (flying){
			if (Time.time > timeThrown + totalFlytime){
				Catch();
			}
		}
		base.GadgetUpdate();

	}

	public override void OnEquip(){
		base.OnEquip();
		MascotAnimatorController.inst.HoldRightArmChop(true);
//		Catch();
	}

	void Catch(){
		flying = false;
		MascotAnimatorController.inst.HoldRightArmChop(true);
		if (gadgetGraphics){
			gadgetGraphics.GetComponent<Renderer>().enabled = true;
		}
	}

	public override void OnUnequip(){
		base.OnUnequip();
//		MascotAnimatorController.inst.HoldRightArm(false);
	}
}
