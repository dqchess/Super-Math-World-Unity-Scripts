using UnityEngine;
using System.Collections;

public class Animal_Sheep : Animal {

	#region UserEditable 



	public override void OnLevelBuilderObjectPlaced(){
		anim.SetBool("Standing",true);
		GetComponent<Rigidbody>().isKinematic = true;

	}

	public override void OnGameStarted(){
		base.OnGameStarted();
		Init();
		anim.SetBool("Standing",false);
		GetComponent<Rigidbody>().isKinematic = false;
	}
	#endregion
	[Header("Animal - Sheep")]
	public float jumpForce = 100;
	public Animator anim;
	float herdSpacing = 4;
	// Use this for initialization
	float timer2 = 0f;

	public override void Init(){
		base.Init();
		AudioManager.inst.PlayRandomSheepSound(transform.position);
		InitSinGrowAttributes(0.1f,true);

		
		RandomizeSheepAnimationOffset();
	}


	void RandomizeSheepAnimationOffset(){
		timer2 = Random.Range(3f,10f); // for randomization of start time of animation loops.

	}
	// 
	/* **** 
	 * 
	 * Sheep heavior. 
	Mostly Stand around (not moving). 
	Rotate randomly every 4-10 seconds. 
	Move forwards for 2-3 seconds at a time. 
	Search for targets periodically.
	Prefer player over numbers as target.
	Otherwise, prefer the number the player just threw.
	Otherwise, prefer the nearest target.
	If a target found, move towards target slowly until target lost / eaten.
*/


	float canSetTargetTimer = 0f;



	float soundTimer = 0;
	float soundInterval = 10;
	float sheepRotateTimer = 0;
	float rotateInterval = 30;
	float currentRotateDuration = 3;
	float baseRotateDuration = 4.5f;
	int rotateChancePercent = 50;
	int sheepSoundChancePercent = 6;
	float rotateSpeed = 30;
	Vector3 rotateDirection = Vector3.up;
	int sheepSoundChancePercentWithTarget = 15;
	public override void AnimalUpdate(){
		if (animalDisabled) return;
		soundTimer -= Time.deltaTime;
		if (soundTimer < 0){
			soundTimer = Random.Range(soundInterval/2f,soundInterval*2f);
			int newPercent = target == null ? sheepSoundChancePercent : sheepSoundChancePercentWithTarget;
			if (Random.Range(0,100) > (100 - newPercent)){
				AudioManager.inst.PlayRandomSheepSound(transform.position);
			}
		}
		if (!canSetTarget){
			canSetTargetTimer -= Time.deltaTime;
			if (canSetTargetTimer < 0){
				canSetTarget = true;
			}
		}
		timer2 -= Time.deltaTime;
		anim.SetFloat("timer",timer2);
		TryGetTarget();
		TryLoseTarget(); // Lose the target it fits too far, or is no longer pref
		CheckTargetActive();
		if (target != null) {
			if (target.transform != null){
				RotateTowardsTarget();
				MoveTowardsTarget();
				CheckSheepInFrontOfMe();
				CheckCloseToTargetAndEat();
			}
		} else {
			sheepRotateTimer -= Time.deltaTime;
			if (sheepRotateTimer < currentRotateDuration){
				transform.Rotate(rotateDirection,Time.deltaTime * rotateSpeed);
				if (sheepRotateTimer < 0){
					rotateDirection = Random.Range(0,2) == 0 ? Vector3.up : -Vector3.up;
					sheepRotateTimer = Random.Range(rotateInterval/2f, rotateInterval*2f);
					currentRotateDuration = Random.Range(baseRotateDuration /2f, baseRotateDuration * 2f);
				}

			}
		}
		SetAnimationBasedOnSpeed();
		MaintainFixedXZRotation();
		base.AnimalUpdate();
	}

	void SetAnimationBasedOnSpeed(){
		if (target != null){
			if (target.transform){
				float deltaPos = Vector3.Distance(rp.lastPosition,rp.nowPosition);
		//		float deltaRot = GetComponent<Rigidbody>().angularVelocity.magnitude;
				float deltaRot = Quaternion.Angle(lastRotation,nowRotation);
				deltaPos += Mathf.Min(deltaRot,.25f);
		//		// commented Debug.Log("deltrot:"+deltaRot);
				if (deltaPos > .01f) anim.SetBool("Standing",false); 
				else anim.SetBool("Standing",true);
				anim.SetFloat("walkspeed",Mathf.Min(0.06f,deltaPos) * 15f);
				return;
			}
		}
		anim.SetBool("Standing",true);
//		// commented Debug.Log("Dep:"+deltaPos);
	}

	Quaternion lastRotation = new Quaternion();
	Quaternion nowRotation = new Quaternion();

	public override void LateAnimalUpdate () {
		base.LateAnimalUpdate();
		lastRotation = nowRotation;
		nowRotation = transform.rotation;
	}

	void MaintainFixedXZRotation(){
		Quaternion rot = transform.rotation;
		rot.eulerAngles = new Vector3(0,rot.eulerAngles.y,0);
		transform.rotation = rot;
	}
	float moveFreezeTimer = 0;
	float hopTimer = 0f;
	void MoveTowardsTarget(){
		if (moveFreezeTimer > 0){
			moveFreezeTimer -= Time.deltaTime;
			return;
		}

		// don't get too close to player, if player was the target.
		if (target.type == TargetType.Player) {
			if (Vector3.Distance(target.transform.position,transform.position) < playerBackOffDistance){
				return;
			}
		}

		Vector3 dirToTarget = target.transform.position - transform.position;
		float deltaAngleFromTarget = Vector3.Angle(transform.forward,dirToTarget); // should be between 0-180 degrees.
		float angleAtten = Mathf.Min(10f / deltaAngleFromTarget,1f);
		float targetFactor = target.type == TargetType.Player ? 1.5f : 1f;
//		// commented Debug.Log("angle:"+deltaAngleFromTarget+",atten:"+angleAtten);
		GetComponent<Rigidbody>().AddForce(moveForce * angleAtten * transform.forward * targetFactor,ForceMode.Impulse);
		hopTimer -= Time.deltaTime;
		if (hopTimer < 0){
			hopTimer = Random.Range(0.5f,1.1f);
//			// commented Debug.Log("hop");
			GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce); // damn sheep are FAT
		}

	}

	bool canSetTarget = true;
	public override void SetTarget(Transform t,TargetType tt){
		if (canSetTarget){
			
			base.SetTarget(t,tt);
		}
	}

	float stopRotatingTimer = 0;
	public override void RotateTowardsTarget(){
		Quaternion targetRot = Quaternion.LookRotation(target.transform.position - transform.position,Vector3.up);
		float rotSpeed = 3f;
		// randomly stop rotating for a few seconds sometimes
		if (Random.Range(0,201) > 199){
			StopRotatingForSeconds(Random.Range(.2f,0.8f));
		}
		if (stopRotatingTimer > 0){
			stopRotatingTimer -= Time.deltaTime;
			return;
		}
//		// commented Debug.Log("rota?");
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
	}

	void StopRotatingForSeconds(float s){
		stopRotatingTimer = s;
	}


	float eatTimeout = 0;
	public override void Eat(GameObject o){
		canSetTarget = false;
		canSetTargetTimer = Random.Range(5f,6f);
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni)){
			anim.SetTrigger("Eat");
			RandomizeSheepAnimationOffset();
			// anim.clip = squawk;
			// anim.Play();
//			AudioManager.inst.PlaySheepMunch(transform.position);
			base.Eat(o);
//			StartCoroutine("EatAfterSeconds",o);
		} else {
			LoseTarget();
		}

	}
//	GameObject ateNumber = null;
//	IEnumerator EatAfterSeconds(GameObject o){
//		o.GetComponent<Collider>().enabled = false;
//		o.GetComponent<Rigidbody>().isKinematic = true; // freeze the number for eating. Don't allow others to interact with it. It's MINE.
//		ateNumber = o;
//		yield return new WaitForSeconds(0.05f);
//		base.Eat(o);
//	}
//


	public override void LoseTarget(string source=""){
		if (!anim) return;

		base.LoseTarget(source);
	}

	float checkSheepTimer = 0;
	public void CheckSheepInFrontOfMe(){
		checkSheepTimer -= Time.deltaTime;
		if (checkSheepTimer < 0){
			checkSheepTimer = Random.Range(0.5f,1f);
			Ray ray = new Ray(transform.position,transform.forward);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray,out hit,herdSpacing)){
				if (hit.collider.GetComponent<Animal>()){
					StopMovingForSeconds(Random.Range(1,2.5f));
				}
			}
		}
	}

	void StopMovingForSeconds(float s){
//		// commented Debug.Log("sheep stopped");

		moveFreezeTimer = s;
	}


	public override void OnDestroy(){
		base.OnDestroy();
		if (muteDestroySound) return;
		AudioManager.inst.PlayRandomSheepSound(transform.position, 2);
	}

}
