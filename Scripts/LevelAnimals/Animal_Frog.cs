using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TongueState {
	Ready,
	Extending,
	Retracting
}
public class Animal_Frog : Animal {


	#region UserEditable 


	public override void OnGameStarted(){
		base.OnGameStarted();
	}



	// upoffset 		return 2; //transform.localScale.y*1.1f + 1f;

	#endregion

	public override void Init(){
		base.Init();
		line.SetWidth(.2f,.2f);
		InitSinGrowAttributes(0.1f,true);
	}



	// 
	/* **** 
	 * 
	 * Frog heavior. 
Doesn't move, just rotates towards targets in range
Flicks tongue out and eats numbers that are in range and match its animalpreference.

*/

	public override void DrawTargetLine(){
		return; // don't use this method.. lol


	}


	float canSetTargetTimer = 0f;



	float soundTimer = 0;
	float soundInterval = 10f;
	int soundChanceWithTarget = 80;
	int soundChanceWithoutTarget = 4;
	public override void AnimalUpdate(){
//		Debug.Log("up 1");
		if (animalDisabled) return;
		soundTimer -= Time.deltaTime;
		if (soundTimer < 0){
			soundTimer = Random.Range(soundInterval/2f,soundInterval*2f);
			int newPercent = target == null ? soundChanceWithoutTarget : soundChanceWithTarget;
			if (Random.Range(0,100) > (100 - newPercent)){
				AudioManager.inst.PlayRandomFrogSound(transform.position);
			}
		}

//		Debug.Log("up 2");
		if (!canSetTarget){
//			Debug.Log("can't yet.");
			canSetTargetTimer -= Time.deltaTime;
			if (canSetTargetTimer < 0){
				canSetTarget = true;
			}
		}
		if (!tonguing){
			base.TryGetTarget();
		}
//		Debug.Log("up 3");
		TryLoseTarget(); // Lose the target it its too far
		//		CheckTargetActive();
		if (target != null) {
			if (target.transform != null){

				CheckCloseToTargetAndEat();
				if (target != null && target.transform && tonguing) {
					//					// commented Debug.Log("tonguing, with targe;t"+target.transform);
					Tongue();

				} 
			}
		}
	}

	bool tonguing =false;
	public TongueState tongueState = TongueState.Ready;
//	float distToTongue = 0;

	public Transform tongueAnchor;
	public Transform tongueEnd;
	bool finishedExtending = false;


	float tongueRetractSpeed = 35f;
	bool TongueFinishedExtending(){
		bool tongueReachedTarget = Vector3.Distance(target.transform.position,tongueEnd.position) < 2 ? true : false;
		bool tongueReachedMaxLength = Vector3.Distance(tongueEnd.position,tongueAnchor.position) > targetSeekRadius ? true : false;
		//		// commented Debug.Log("reached?"+tongueReachedTarget+", max?"+tongueReachedMaxLength);
		return tongueReachedMaxLength || tongueReachedTarget;
	}

	void ExtendTongueTowardsTarget(){
		transform.LookAt(target.transform);
		transform.rotation = Utils.FlattenRotation(transform.rotation);
		float deltaDistance = Vector3.Distance(target.transform.position,tongueEnd.position);
		// Tongue is extending towards target
		//			Debug.Log("tonguing hasn't reached max.");
		line.SetVertexCount(2);
		line.SetWidth(.3f,.3f);
		line.SetPosition(0,tongueAnchor.position);
		line.SetPosition(1,tongueEnd.position);
//		float tongueExtendSpeed = 70f;
		float tongueExtendSpeed = Vector3.Distance(tongueEnd.position,target.transform.position) < 8f ? 300f : 70f;
		tongueEnd.transform.position = Vector3.MoveTowards(tongueEnd.transform.position,target.transform.position,Time.deltaTime *tongueExtendSpeed);

	}

	void RetractTongueTowardsMouth(){
		
		line.SetVertexCount(2);
		line.SetPosition(0,tongueAnchor.position);
//		line.SetPosition(1,tongueEnd.position);
		line.SetWidth(.3f,.3f);
		Vector3 nearFactor = Vector3.Normalize(tongueAnchor.position - tongueEnd.position);
		line.SetPosition(1,tongueEnd.position + nearFactor);
//					tongueEnd.transform.position = Vector3.Lerp(tongueEnd.transform.position,tongueAnchor.position,Time.deltaTime * tongueSpeed);
		tongueEnd.transform.position = Vector3.MoveTowards(tongueEnd.transform.position,tongueAnchor.transform.position,Time.deltaTime * tongueRetractSpeed);

	}

	bool TongueReachedMouth(){
		float distFromTargetToMaw = Vector3.Distance(tongueEnd.position,tongueAnchor.position);
		bool flag = distFromTargetToMaw < 0.5f;
//		Debug.Log("flag:"+flag);
		return flag;
	}

	void ConvertTargetToEdibleForm(){
//		Debug.Log("a");
		AudioManager.inst.PlayFrogStick(target.transform.position);
		if (target.type == TargetType.Player || target.type == TargetType.PlayerInventory){
			bool loseTarget = true;

			if (target.type == TargetType.Player){

				Debug.Log("player targ");
				// hmm, problem: this converts the first ammo found where the target may have been a different ammo
				// seems like if you had ammo in multiblaster that frog likes, that is targeted, then you fire it away, there is another ammo in a different gadget that gets eaten now
				if (target.gadget){
					Debug.Log("Target had gadget;"+target.gadget);
					foreach(Ammo a in target.gadget.GetAmmoInfo()){
						// TODO make an ammo type of gadget and store a reference to which gadget we're stealing ammo from
						// HERE instead we'll just reference the gadget which is a part of the AnimalTarget class
						// Verify that the ammo in this gadget IS EDIBLE.
						if (AnimalBehaviorManager.inst.CheckValidTarget(this.GetAnimalTargetPreference(),this,a.ammoValue)){

							GameObject newTarget = target.gadget.DropOneAmmo(a);
							if (newTarget){
								canSetTarget = true;
								
								SetTarget(newTarget.transform,TargetType.Food);
								//						// commented Debug.Log("new target:"+newTarget);
								target.transform.parent = tongueEnd;
								target.transform.localPosition = Vector3.zero;
								DisableTarget();
								loseTarget = false;
								break;
							}
						}
					}
					// Did we not find a valid ammo to eat form this gadget? No worries, our catch-all "lose target" will ensure that we don't try to snack on this inedible ammo. 
					// #badcoding
					if (loseTarget) LoseTarget("no valid ammo in this gadget ");

				}
			} else if (target.type == TargetType.PlayerInventory){
				NumberInfo ni = target.targetObj.GetComponent<NumberInfo>();
				if (ni){
					
					GameObject newTarget = Inventory.inst.DropInventoryItem(target.targetObj);
					if (newTarget){
						canSetTarget = true;

						SetTarget(newTarget.transform,TargetType.Food);
						//						// commented Debug.Log("new target:"+newTarget);
						target.transform.parent = tongueEnd;
						target.transform.localPosition = Vector3.zero;
						DisableTarget();
						loseTarget = false;
					}
				}
			}
			if (loseTarget){

				LoseTarget("losetarget");
			}

		} else {
			DisableTarget();
			//				// commented Debug.Log("new targ;"+target.transform.name);
			if (target != null && target.transform && !target.transform.GetComponent<GadgetThrow>()){
				target.transform.parent = tongueEnd.transform;
				//					Debug.Log("tonguing childed target..");
				target.transform.localPosition = Vector3.zero;
			} else {

				LoseTarget("target null, or target transform null");
			}

		}
	}

	void Tongue(){

		if (tongueState == TongueState.Extending){
			if (!TongueFinishedExtending()){
				ExtendTongueTowardsTarget();
			} else {
				ConvertTargetToEdibleForm();
				tongueState = TongueState.Retracting;
			}
		} else if (tongueState == TongueState.Retracting){
			RetractTongueTowardsMouth();
			if (TongueReachedMouth()){
//				Debug.Log("frog ate finally");
				if (target != null && target.transform != null && target.transform.GetComponent<NumberInfo>() != null && target.transform.parent == tongueEnd){
					EatIt(target.transform.gameObject);
//					canSetTargetTimer = Random.Range(0.2f,0.6f);
				}
				line.SetVertexCount(0);
				LoseTarget("dist from maw");
			
//				Debug.Log("reached mouth, can set target.");
			}
		}
	}


	public  void EatIt(GameObject o){
		base.Eat(o);
		if (targetWasFrog){
			seekTargetTimer = 2;
		} else {
			Debug.Log("o animal:"+o.name); //o.GetComponent<Animal>().AnimalIsFrogType());
		}
	}

	public override void LoseTarget(string source=""){
		base.LoseTarget(source);
		finishedExtending = false;
		tonguing = false;
		canSetTarget = true;
		tongueState = TongueState.Ready;
		if (tongueEnd && tongueAnchor) {
//			Debug.Log("set pos");
			tongueEnd.position = tongueAnchor.position;

			// clear any objects that may linger inside tongueend
			foreach(Transform t in tongueEnd){
				Destroy(t.gameObject);
			}
		}
	}

	bool targetWasFrog = false;
	void DisableTarget(){
		
//		Debug.Log("tonguing disabled its target.");
		target.transform.localScale = target.transform.lossyScale * 0.7f;
//		// commented Debug.Log("Cleaning target;"+target.transform.name);
		Animal an = target.transform.gameObject.GetComponent<Animal>();
		targetWasFrog = false;
		if (an){
			if (an.AnimalIsFrogType()){
				targetWasFrog = true;
			}
			Fraction orig = target.transform.GetComponent<NumberInfo>().fraction; // could be animal, which inherits from numberinfo. We'll swap it for a generic numberinfo so animal props aren't retained. But retain fraction. Ugly code.
			NumberInfo rep = target.transform.gameObject.AddComponent<NumberInfo>();
			rep.fraction = orig;
			Destroy(an);
		}
		CleanComponentsFromDyingNumber(target.transform.gameObject, new System.Type[] {typeof(NumberInfo)} );
//		if (target.transform.GetComponent<Collider>()) Destroy(target.transform.GetComponent<Collider>());
//		if (target.transform.GetComponent<Rigidbody>()) Destroy(target.transform.GetComponent<Rigidbody>());
	}
	bool canSetTarget = true;
	public override void SetTarget(Transform t,TargetType tt){
		// This idffers from the base.set target case in that this is AFTER the target has been tongued and reached my mouth
		if (canSetTarget){
			FrogTarget f = t.gameObject.AddComponent<FrogTarget>();
			f.DestroyAfterSeconds(3f);
			base.SetTarget(t,tt);
		}
	}

	public override bool CheckValidTarget( NumberInfo ni){
//		Debug.Log("Valid?:"+ni);
		bool careAboutRigidbodies = !(target != null && target.transform != null && target.transform.parent == tongueEnd);
//		Debug.Log("target;"+target);
//		Debug.Log("target.tran:"+target.transform);
//		Debug.Log("targ paretn:"+target.transform.parent);
		return AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni,careAboutRigidbodies); // note frog is overriding the rigidbody requiirement only if tonguing.
	}

	public override void SetTarget(Transform t, TargetType tt, GameObject targetObj, Gadget g = null){
//				Debug.Log("frog set target:"+t);
		if (canSetTarget){
			if (targetObj) {
				FrogTarget f = targetObj.AddComponent<FrogTarget>();
				f.DestroyAfterSeconds(3f);
			}
			else {
				FrogTarget f = t.gameObject.AddComponent<FrogTarget>();
				f.DestroyAfterSeconds(3f);
			}
			base.SetTarget(t,tt,targetObj,g);
		}
	}

	float stopRotatingTimer = 0;
	public override void RotateTowardsTarget(){
		return;

	}

	void StopRotatingForSeconds(float s){
		stopRotatingTimer = s;
	}

	public override void CheckCloseToTargetAndEat(){
//		Debug.Log("Frog checking.");
		if (PlayerInRange() && canSetTarget){
			Eat(target.transform.gameObject);
		}
	}


	float eatTimeout = 0;
	public override void Eat(GameObject o){
//		Debug.Log("frog eating with state:"+tongueState.ToString());
		if (tongueState == TongueState.Ready){

			canSetTarget = false;
			tongueState = TongueState.Extending;

			tonguing = true;
		}
	}

	public override void OnDestroy(){
		base.OnDestroy();
		if (!muteDestroySound){
			AudioManager.inst.PlayRandomFrogSound(transform.position, 2);
		}

	}


}
