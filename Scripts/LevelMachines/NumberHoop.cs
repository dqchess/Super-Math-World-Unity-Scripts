using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;

public enum HoopType{
	Multiply,
	Add,
	Exponent,
	FractionEquivalence
}


public class NumberHoop : NumberModifier {


//	public GameObject hoopInsides;
//	public HoopType ht = HoopType.Multiply;
	public bool absoluteValue = false;
	public bool debug = true;
	public Text numberTextBack;
	public Text numberTextFront;

	public string currentEquation;

	public Dictionary<Collider,Vector3> lastRelativePoint;
	public Dictionary<Collider, bool> triggered;

	public NumberModifier.ModifyOperation modifyOp = x => x;

	public override void Start () {
		base.Start();
		lastRelativePoint = new Dictionary<Collider, Vector3>();
		triggered = new Dictionary<Collider, bool>();
	}

	public virtual void SetHoopText(){



//		}

		//		if (numberTextFront.text.Length >= 3) numberTextFront.transform.localScale *= .7f;
		//		if (numberTextBack.text.Length >= 3) numberTextBack.transform.localScale *= .7f;
	}

	//	public void SetFraction(int n, int d){
	//		SetHoopFraction(new 
	//	}



	virtual public void OnTriggerEnter(Collider other) {
		if (SMW_CHEATS.inst.cheatsEnabled){
//			WebGLComm.inst.DebugOrange("Hoop trigger enter:"+other.name);
		}
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		// here too, in case the object hits the trigger over the half way point

		CheckCrossing(other);
	}

	virtual public void OnTriggerExit(Collider other) {
//		WebGLComm.inst.DebugOrange("Hoop trigger exit:"+other.name);
		CheckCrossing(other);
	}

	virtual public void OnTriggerStay(Collider other) {
//		WebGLComm.inst.DebugOrange("Hoop trigger stay:"+other.name);
		CheckCrossing(other);
	}
//
	public void CheckCrossing(Collider other) {
//		// commented Debug.Log("check:"+other.name);
		RecordPosition record = other.GetComponent<RecordPosition>();
		if(record == null) { return; }


		GameObject obj = other.gameObject;
		Vector3 pastPoint = transform.InverseTransformPoint(record.lastPosition);
		Vector3 nowPoint = transform.InverseTransformPoint(other.transform.position);
//		// commented Debug.Log("pp,np:"+pastPoint+","+nowPoint);

		// resource number exceptions
		if (other.GetComponent<ResourceNumber>()){
			other.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(pastPoint-nowPoint)*50); // bounce off
			return;
		}	

		if(pastPoint.z < 0 && nowPoint.z >= 0) { 
//			WebGLComm.inst.DebugOrange("Hoop trigger forwards:"+other.name);
			UseHoop(obj, record, true);
		}
		else if (pastPoint.z >= 0 && nowPoint.z < 0) {
//			Debug.Log("Backwards! - " + obj.name);
//			WebGLComm.inst.DebugOrange("Hoop trigger backwards:"+other.name);
			UseHoop(obj, record, false);
		} else {
//			WebGLComm.inst.DebugOrange("Hoop trigger neither:"+other.name);
//			Debug.Log("neither!:"+nowPoint+","+pastPoint);
		}
	}

	virtual public void UseHoop(GameObject obj, RecordPosition record, bool direction){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		//// commented Debug.Log(record.lastPosition);
		record.nowPosition = obj.transform.position;
		record.lastPosition = obj.transform.position; // Do this to avoid any double triggering

		if (obj.GetComponent<Collider>().enabled==false) return; // still works on player!

		AudioManager.inst.PlayShortBop(transform.position);
		EffectsManager.inst.CreateRingEffect2(transform.position,1,.5f,Color.white);
		Vector3 fxPos = obj == Player.inst.gameObject ? obj.transform.position + Vector3.up * 2 : obj.transform.position; // player fx need to be offset up as players position is on the ground, vs a number passing through position is in the center
		EffectsManager.inst.CreateSmallPurpleExplosion(fxPos,1.5f,.4f);

	}



	public override void PostModifyNumber (Fraction original, NumberInfo ni)
	{
//		base.PostModifyNumber (original, ni);
//		MonsterAIRevertNumber revert = ni.GetComponent<MonsterAIRevertNumber>();
//		if(revert) { revert.origFrac = ni.fraction; }
	}	

	public override Fraction GetModifiedFraction (Fraction original)
	{
		return modifyOp(original);
	}

	public override string GetEquation (Fraction original)
	{
		return original + " " + currentEquation + " = " + GetModifiedFraction(original);
	}
}






