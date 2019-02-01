using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GadgetFactorSwapperMode {
	Split,
	Join
}

public class GadgetFactorSwapper : Gadget {


	public LineRenderer r1;
	public LineRenderer r2;
	float range = 30;

	GadgetFactorSwapperMode mode = GadgetFactorSwapperMode.Split;
	float fireTimeout = 0;

	override public void Start(){
		base.Start ();
		r1.transform.parent = null;
		r2.transform.parent = null;
	}

	NumberInfo targetA;
	GameObject targetA_resultA;
	GameObject targetA_resultB;
	NumberInfo targetB;
	NumberInfo targetC;
	GameObject targetBC_result;

	override public void GadgetUpdate(){
		base.GadgetUpdate();

	}

	override public void GadgetLateUpdate(){
		fireTimeout -= Time.deltaTime;
		
		if (holdingMouse){
			if (mode == GadgetFactorSwapperMode.Split){
				targetA = CheckLaser(r1,gadgetGraphics.transform.position + (GetAimVector()) * range);
				if (targetA){
					if (!targetA_resultA || !targetA_resultB){
						CreateTargetResultsA();
					}
				} else {
					ClearTargets();
				}
				
			} else if (mode == GadgetFactorSwapperMode.Join){
				targetB = CheckLaser(r1,gadgetGraphics.transform.position + (GetAimVector() + Camera.main.transform.right * .1f ) * range);
				targetC = CheckLaser(r2,gadgetGraphics.transform.position + (GetAimVector() + Camera.main.transform.right * -.1f ) * range);
			}
			
		}
	}

	public override Vector3 GetAimVector(){
		return Camera.main.transform.forward + Camera.main.transform.up * .2f;
	}

	void CreateTargetResultsA(){
		List<int> factors = MathUtils.GetFactors(targetA.fraction.numerator); // Get the factors as an ordered list.
	}

	NumberInfo CheckLaser(LineRenderer r, Vector3 dest){
//		// commented Debug.Log ("checklaser.");
		r.SetVertexCount(2);
		r.SetPosition(0,gadgetGraphics.transform.position);
		r.SetPosition(1,dest);
		Vector3 dirToDest = dest-gadgetGraphics.transform.position;
		RaycastHit hit;
		if (Physics.Raycast(gadgetGraphics.transform.position,dirToDest,out hit,30f)){
			return hit.collider.GetComponent<NumberInfo>();
		}
		else return null;
	}

	
	bool holdingMouse = false;
	override public void MouseButtonDown() {
		if (fireTimeout > 0) return;
//		// commented Debug.Log ("hold true.");
		holdingMouse = true;
	}

	override public void MouseButtonUp(){
		r1.SetVertexCount(0);
		r2.SetVertexCount(0);
		if (mode == GadgetFactorSwapperMode.Join){
			mode = GadgetFactorSwapperMode.Split;
			gadgetGraphics.GetComponent<GadgetFactorSwapperGraphics>().leverTarget = gadgetGraphics.GetComponent<GadgetFactorSwapperGraphics>().leftTarget;
		} else {
			mode = GadgetFactorSwapperMode.Join;
			gadgetGraphics.GetComponent<GadgetFactorSwapperGraphics>().leverTarget = gadgetGraphics.GetComponent<GadgetFactorSwapperGraphics>().rightTarget;
		}
		AudioManager.inst.PlayDoorLever(Player.inst.transform.position);
		holdingMouse = false;
//		// commented Debug.Log ("hold false.");
		ClearTargets();
	}
	
	void ClearTargets(){
		targetA = null;
		if(targetA_resultA) Destroy (targetA_resultA);
		if(targetA_resultB) Destroy(targetA_resultB);
		targetB = null;
		targetC = null;
		if (targetBC_result) Destroy(targetBC_result);
	}


//	{
//		NumberManager.inst.MakeIntoRocket(obj);
//		AudioManager.inst.PlayRocketLaunch(obj.transform.position);
//	}
	

	
	public override void OnEquip(){
		base.OnEquip();
		MascotAnimatorController.inst.HoldRightArmChop(true);
	}

	public override void OnUnequip(){
		base.OnUnequip();
		MascotAnimatorController.inst.HoldRightArmChop(false);
	}

}
