using UnityEngine;
using System.Collections;

public class GadgetSword : Gadget {

	bool swinging=false;
	float swingTime;
	float swingDuration = 0.5f;
	bool hitNumberOnThisSwing=false;
	
	bool parameterized = false;
	
	bool collected = false;
	Fraction collectedFrac = new Fraction(2,1);


	public override void GadgetOnTriggerEnter(Collider other){
		if (other.GetComponent<NumberInfo>()){
			// The player swung the sword and hit a number, so cut it in half.
			CutInHalf(other.GetComponent<Collider>());
		}
	}

	public override void MouseButtonDown(){
		Fire();
	}

	
	

	public override bool CanCollectNumber (NumberInfo ni)
	{
		return !collected && parameterized;
	}
	

	
	public override void OnEquip(){
		
//		// commented Debug.Log ("equip sword at "+Time.time);
		base.OnEquip();
		MascotAnimatorController.inst.HoldRightArmChop(true);
		if (!LevelBuilder.inst.levelBuilderIsShowing && !Inventory.inst.isShowing) AudioManager.inst.EquipSword();
		swinging = false;
//		foreach(Transform t in PlayerGadgetController.inst.currentWeapon.weaponObject.transform){
//			t.SendMessage("OnEnable",SendMessageOptions.DontRequireReceiver);
//		}
//		PlayerGadgetController.inst.swordGraphics.SetActive(true);
		//		Player.GetComponent<PlayerGadgetController>().weaponsAnims.ThrowStatic();
	}

	public override void OnUnequip(){
		base.OnUnequip();
//		// commented Debug.Log ("sword done");
//		MascotAnimatorController.inst.HoldRightArmChop(false);
	}
	
	public override void Fire ()
	{
		if(parameterized && !collected) { return; }
		base.Fire();
		if (Player.frozen) return;
		if (!swinging){
//			// commented Debug.Log ("swing");
			/*
			 * what I wnat:
			 * I've saved a quaternion and position for the sword at the end of its swing. for first half, lerp to these values, second half, lerp away from them back 
			 * to the original values, then allow swing again.
			 * */
			MascotAnimatorController.inst.SwingSword();
			swinging=true;
			gadgetGraphics.GetComponent<BoxCollider>().enabled=true;

			swingTime=Time.time;
			AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.timesSwordSwung,1); // userData.timesSwordSwung++;
			hitNumberOnThisSwing=false;
			
			AudioManager.inst.PlayNumberThrow();
		} else {
			//			// commented Debug.Log("can't swing, already swinigng");
		}
	}
	
	override public void PlayOnEquipAudio(){
		AudioManager.inst.EquipSword();

	}

	
//	public override void OnCollisionEnterWeapon(Collision hit){
//		//		if (!Network.isClient){
//		if (SMW_GF.inst.IsNumber(hit.collider)){
//			CutInHalf(hit.collider);
//		}
//		//		}
//	}

	bool IsValidSwordTarget(GameObject other){
		return 
			other.GetComponent<Rigidbody>() 
			&& !other.GetComponent<Rigidbody>().isKinematic
			&& !other.GetComponent<EnemyBrick>()
			&& !other.GetComponent<MonsterAIBase>()
			;
	}

	void CutInHalf(Collider collider){
//		// commented Debug.Log ("cut1. swinging:"+swinging+", hitnumberonthisswing:"+hitNumberOnThisSwing);
		if (swinging && !hitNumberOnThisSwing){// && bNeedsSphereCheck){
//			// commented Debug.Log ("cut12");	
			hitNumberOnThisSwing=true;
			AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.numberChopped,1); //++;
			
			GameObject other = collider.gameObject;
			NumberInfo ni = other.GetComponent<NumberInfo>();
			if (ni){
//				// commented Debug.Log ("cut13");
				//				if (ni.myShapes

				if (IsValidSwordTarget(other)){
					

//					// commented Debug.Log ("cut14");
					other.SendMessage("OnCutInHalf",SendMessageOptions.DontRequireReceiver);
					other.SendMessage("OnDestroy",SendMessageOptions.DontRequireReceiver);
//					// commented Debug.Log("hit ni wihtout kin:"+ni.gameObject.name);
					Fraction result = Fraction.Multiply(ni.fraction, new Fraction(1,2), true); // Calculate the resulting fraction as half of the original
					ni.SetNumber(result);
					if (!ni.gameObject.GetComponent<SwordChoppedNumber>()){
						ni.gameObject.AddComponent<SwordChoppedNumber>(); // when cloned, both left and right nums will have this, which prevents them from combining with other sword chopped numbers for a hile.
					}
					ni.GetComponent<Rigidbody>().drag = 1;
					//					Fraction result2 = Fraction.Subtract(ni.fraction, result1, true);
					
					//					// commented Debug.Log ("frac subtract 1/2048, 1/4096:"+
					
					NumberShape numShape = ni.myShape;
					float splitDistance = ni.transform.localScale.x/1.4f; // 1.5f;
					//					if (ni.numberIsAlive) splitDistance += ni.transform.localScale.x;
					Vector3 rightDir = Player.inst.transform.right*splitDistance;
					Vector3 rightPos = other.transform.position + rightDir + Vector3.up; // new locations are just "player relative" left and right of the original object (slices resulting from being sliced in half should be in positions that "make sense" and or "feel right")
					Vector3 leftPos = other.transform.position + -rightDir + Vector3.up;
					
					GameObject numLeft = (GameObject)Instantiate (ni.gameObject,leftPos,Quaternion.identity);
					GameObject numRight = (GameObject)Instantiate(ni.gameObject,rightPos,Quaternion.identity); // NumberManager.inst.CreateNumber(result2,rightPos,numShape,true, ni.isPercent);

					// weird unresolved bug todo vadim
					// When sword breaks a 1 into 1/2, the 1/2 is not rendered in resulting spheres 
					// If you chop the 1/2 it does render 1/4 in the resulting spheres
					// if you combine two 1/4 to 1/2 it does render
					// debug yields that vertex count of cctext when its not being rendered is zero
					// probably due to instantiating the object and messing with cctext in the same frame, something is not getting initialized.
					// solution?? interestingly, set the number BEFORE instantiating the copies. e.g. when you chop a 1, set the 1 to 1/2, then instantiate two copies of it and destroy the original
					// something about instantiating and THEN setting the number was causing CCText to product empty vertex arrays for the text object

					ni.GetComponent<Collider>().enabled = false;
//					foreach(CCText t in ni.GetComponentsInChildren<CCText>()){
////						t.enabled = true;
//					}
					//					// commented Debug.Log ("result 1, 2: "+result1+","+result2);
//					numLeft.GetComponent<NumberInfo>().SetNumber(result1);
//					numRight.GetComponent<NumberInfo>().SetNumber(result2);
//					numLeft.transform.localScale = Vector3.one * ni.transform.localScale.x;
//					numRight.transform.localScale = Vector3.one *ni.transform.localScale.x;
					
					
					//					if (ni.numberIsAlive){
					//						numLeft.name += "--Left";
					//						numRight.name += "--Right";
					//					
					////						float extraSpace=15;
					//						numLeft.transform.localScale = ni.transform.localScale*0.79370052598f; // the half-volume radius
					//						numRight.transform.localScale = ni.transform.localScale*0.79370052598f;
					//						// Make that number seek its partner and attempt to re-combine into one spikey.
					////						NumberManager.inst.TurnNumberIntoMonster(numLeft,ni.mt);
					////						NumberManager.inst.TurnNumberIntoMonster(numRight,ni.mt);
					//						
					//						numLeft.transform.parent=ni.transform.parent;
					//						numRight.transform.parent=ni.transform.parent;
					//						
					////						NumberInfo ni1 = numLeft.GetComponent<NumberInfo>();
					////						NumberInfo ni2 = numRight.GetComponent<NumberInfo>();
					////						ni1.SetMatchingPartner(numRight);
					////						ni2.SetMatchingPartner(numLeft);
					////						ni1.SetMaxNumber(ni.maxNumber);
					////						ni2.SetMaxNumber(ni.maxNumber); // TODO: This should be refactored? It's a way for spikeys to keep track of whether or not they should
					////						ni1.seekPartner=true;
					////						ni2.seekPartner=true;	
					//						// seek partners and try to recombine with other spikeys with the same number or not.
					//						
					//						// Set MARN to the original NI value, so that after it combines adn is no longer "seeking", it will not MARN back to the half value.
					////						MonsterAIRevertNumber marn1 = numLeft.GetComponent<MonsterAIRevertNumber>();
					////						MonsterAIRevertNumber marn2 = numRight.GetComponent<MonsterAIRevertNumber>();
					////						if (marn1) { marn1.origFrac=ni.fraction; marn1.bNeedsRevert=false; }
					////						if (marn2) { marn2.origFrac=ni.fraction; marn2.bNeedsRevert=false; }
					//
					////						// commented Debug.Log("num left ("+numLeft.name+") origfrac: "+numLeft.GetComponent<MonsterAIRevertNumber>().origFrac);
					//						
					//						
					//					}
					
					collected = false;
					collectedFrac = new Fraction(2,1);											
					
					// make the numbers roll away from each other a bit after chopping
					float rollForce = 300f;
					//					if (ni.numberIsAlive) rollForce*=2;
					numLeft.GetComponent<Rigidbody>().AddForce(-rightDir*rollForce);
					numRight.GetComponent<Rigidbody>().AddForce(rightDir*rollForce);
					
					//					if(Network.isServer) {
					//						ni.NetworkDestroy();
					//					}
					//					else {
					NumberManager.inst.DestroyOrPool(ni);
					//					}					
					
				}
			}
		}
	}
	
	//	bool bNeedsSphereCheck=false;
	public override void GadgetUpdate(){
		base.GadgetUpdate();
		if (swinging){
			
			if (Time.time > swingTime + swingDuration){
				swinging=false;
				gadgetGraphics.GetComponent<BoxCollider>().enabled=false;
			}
		}
	}
}
