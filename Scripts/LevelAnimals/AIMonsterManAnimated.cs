using UnityEngine;
using System.Collections;


public enum WalkStyle{
	Big,
	Small
}

[System.Serializable]
public enum MonsterAction {
	Nothing,
	SmashDown,
	SmashAcross,
	Grab,
	Drop
}

[System.Serializable]
public class ActionSet {
	public Transform destination;
	public MonsterAction monsterAction;
	public Transform grabObject=null;

}

public class AIMonsterManAnimated : MonoBehaviour {



	public NumberInfo[] myBlocks;
	public Transform face;
	public GameObject[] criticalBlocks;
	public bool drawGizmos = false;
	public int delayBetweenAttack = 10;
	float delayBetweenRockets = 3.2f;
	float attackRange=75;
	float approachRange=120;
	public int maxRocketsPerAttack = 3;
	public Transform rocketLauncher;
	public Transform[] destinations;
	public Material destinationDrawMaterial;
	int index=0;
	public float speed=25;
//	public Transform hand;
//	Transform grabbedObject;
	public bool loop=false;
	public Fraction frac = new Fraction(1,1);
	public float walkSpeed = .3f;
	float footstepTimer=0;
	public WalkStyle walkStyle = WalkStyle.Big;

	Animator anim;
	float sqrApproachRange;
	float sqrAttackRange;
	void Start(){
		name += Random.Range(0,10);
		sqrApproachRange =  approachRange * approachRange;
		sqrAttackRange = attackRange * attackRange;
		anim = GetComponent<Animator>();
		anim.SetBool("Walking",false);
		InitiateMyBlocks();

	}

	void OnEnable(){
		startTimer = Random.Range(0f,2f);
		checkBlocksTimer =3f;
	}

	void InitiateMyBlocks(){
//		Debug.Log ("init.");
		myBlocks = GetComponentsInChildren<NumberInfo>();
		for (int i=0;i<myBlocks.Length;i++){
			Fraction f = frac;
//			if (i % 2 == 0) {
////				f = Fraction.Multiply(frac,factor); // every other block will be different.
//			}
			NumberInfo ni = myBlocks[i];
			ni.SetNumber(f);
			MonsterAIRevertNumber mr = ni.gameObject.GetComponent<MonsterAIRevertNumber>();

			mr.SetNumber(f);
//			Debug.Log("setting mr;"+f);

		}

	}



	void OnDrawGizmos(){
		if (!drawGizmos) return;
		Gizmos.color=new Color(1,0,1);

		for (int i=0;i<destinations.Length;i++){
			Vector3 a = destinations[i].position;
			Vector3 b = destinations[ (i+1) % destinations.Length].position;
			Gizmos.DrawLine(a,b);
		}

	}


	float rocketTimer1=.5f;
	float rocketCooldown=0f;
	float sequenceCooldown=0f;
	float targetCheckTimer=0;
	int rocketsFired=0;
	int numberOfRocketsToFire=3;
	float blocksCheck=0;
	float targetLockedTimer=0;
	float seekNewTargetAfterSeconds = 5;
	float dontSeekTargetForSeconds = 0;
//	public Transform leftFoot;
//	public Transform rightFoot;
	float startTimer = 1;
	Transform target;

	public bool sleeping = false;
	float sqrDistToSleep = 50000;
	float sleepTimer = 0;

	public void CheckSleep(){
		// Sleep if player is far away.
		sleepTimer -= Time.deltaTime;
		if (sleepTimer < 0){
			sleepTimer = Random.Range(1.5f,2f);
			if (Vector3.SqrMagnitude(Player.inst.transform.position-transform.position) > sqrDistToSleep){
				sleeping = true;
			} else {
				sleeping = false;
			}
		}
	}



	void Update(){
		if (dead) return;
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		if (GameManager.inst.GameFrozen) return;
		CheckSleep();
		if (sleeping){ return; }
				
		checkBlocksTimer -= Time.deltaTime;
		startTimer -= Time.deltaTime;
		if (startTimer > 0) return;
		if (target) {
			targetLockedTimer += Time.deltaTime;
			if (targetLockedTimer > seekNewTargetAfterSeconds){
				if (Random.Range(0,100) > 70f){
					dontSeekTargetForSeconds = Random.Range(8f, 15f);
				}
				seekNewTargetAfterSeconds = Random.Range(3,9f);
				target = null;
			}
		}
		dontSeekTargetForSeconds -= Time.deltaTime;
		rocketTimer1-=Time.deltaTime;
		rocketCooldown-=Time.deltaTime;
//		sequenceCooldown-=Time.deltaTime;

		if (CheckForMissingBlocks()) { Die(); return; }

		if (dontSeekTargetForSeconds < 0) CheckForTarget();


		if (target){
			if (face) face.LookAt(target);
			Vector3 flatdestpos = new Vector3(target.position.x,transform.position.y,target.position.z);
			transform.LookAt(flatdestpos);
			rocketLauncher.LookAt(target);
			anim.SetBool("Walking",false);


			if (rocketCooldown < 0){
				if (TargetInAttackRange()){
					FireRocketAtTarget();
					rocketCooldown = Random.Range(delayBetweenRockets/2f,delayBetweenRockets*2f);
					rocketsFired++;
					if (rocketsFired >= maxRocketsPerAttack) {
						rocketCooldown = Random.Range(5,10f);
						rocketsFired=0;
						//						sequenceCooldown = delayBetweenAttack;
					}
				}
			} 
			// Reset whole sequnce (continue walking) after some time
			//				int delayAfterLastRocket = 2;
//			if (sequenceCooldown < -5){
//				sequenceCooldown = delayBetweenAttack;
//				rocketCooldown = 0;
//				footstepTimer = 0;
//			}
			if (!TargetTooClose(target) && TargetInRange(target.position) && !TargetInAttackRange()){
				
				WalkForwards();

			}
		} else {
//			// commented Debug.Log("clips 1 name;"+clips[0].name);

			WalkForwards();

		}
	}

	bool TargetInAttackRange(){
		if (!target) return false;
		return Vector3.SqrMagnitude(target.transform.position-transform.position) < sqrAttackRange;
	}

	public Fraction GetFraction(){
		return frac;
	}

	public void SetFraction(Fraction f){
		frac = f;
		InitiateMyBlocks();
	}

	void FireRocketAtTarget(){
//		// commented Debug.Log("Firing from:"+name+" to: "+target.name+" at time;"+Time.time);
//		AudioManager.inst.PlayRocketLaunch(transform.position);
		Vector3 dirToTarget = Vector3.Normalize(target.position-rocketLauncher.position);
		GameObject rocket = SMW_GF.inst.FireRocketFromTo(rocketLauncher.position+rocketLauncher.forward*10,dirToTarget,frac,2f,30,440,transform.root);
		rocket.name="rock";
		rocket.AddComponent<AttackPlayerModifyAmmo>();
	}




	void PlayFootstepsAudio(){
//		float frame = anim["walk"].time * anim.clip.frameRate;
//		float playbackTime = anim.GetCurrentAnimatorStateInfo().normalizedTime % 1;
//		float frameRate = anim.GetCurrentAnimatorStateInfo().speed * anim.GetCurrentAnimatorClipInfo().Length;
//		// commented Debug.Log("len:"+anim.GetCurrentAnimatorClipInfo().Length+", playbacktime:"+playbackTime+",framreate;"+frameRate);
//		float frame = playbackTime * frameRate;
		float frame = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
//		// commented Debug.Log("frame;"+frame);
		footstepTimer -= Time.deltaTime;
//		// commented Debug.Log("frame:"+frame);
		if ( Mathf.Abs(frame-0.20f) < 0.03f  && footstepTimer < 0){ // right foot down
//			if (rightFoot){
//				for (int i=0;i<5;i++){
//					float dustSize = Random.Range(10,12f);
//					Vector3 pos = rightFoot.transform.position + Utils.FlattenVector(Random.insideUnitSphere);
//					EffectsManager.inst.MakeDust(pos,dustSize);
//				}
//			}
			footstepTimer = 0.3f;
			AudioManager.inst.PlayBigFootsteps(transform.position);
		} else if (Mathf.Abs(frame-0.7f) < 0.03f && footstepTimer < 0){ // left foot down
//			if (leftFoot){
//				for (int i=0;i<5;i++){
//					float dustSize = Random.Range(10,12f);
//					Vector3 pos = leftFoot.transform.position + Utils.FlattenVector(Random.insideUnitSphere);
//					EffectsManager.inst.MakeDust(pos,dustSize);
//				}
//			}
			footstepTimer = 0.3f;
			AudioManager.inst.PlayBigFootsteps(transform.position);
		}

	}


	void CheckForTarget(){
		targetCheckTimer-=Time.deltaTime;
		if (targetCheckTimer < 0){
			targetCheckTimer=.6f;
			target = GetTarget();
//			if (target) // commented Debug.Log("target;"+target.name);
//			else // commented Debug.Log("target null");
		}
	}

	void WalkForwards(){
		
		PlayFootstepsAudio();
		anim.SetBool("Walking",true);
		Transform dest;
		if (target){
			dest = target;
//			// commented Debug.Log("dest is target:"+target.name);
			if (TargetTooClose(target)){
//				// commented Debug.Log("too close to target, backing off.");
				return;
			}
		} else {
			dest = destinations[index % destinations.Length];
			if (Vector3.Magnitude(Utils.FlattenVector(transform.position)-Utils.FlattenVector(dest.position))<7){
//				// commented Debug.Log("reched dest:"+dest);
				index++;
			}	
		}
		Vector3 flatdestpos = new Vector3(dest.position.x,transform.position.y,dest.position.z);
		float rotSpeed = 2;
		transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(flatdestpos - transform.position),Time.deltaTime * rotSpeed);
//		// commented Debug.Log("rotating towards;"+dest.name);
//		transform.LookAt(flatdestpos);
		float moveforce = 5000;
		GetComponent<Rigidbody>().AddForce(Vector3.Normalize(flatdestpos-transform.position)*speed*Time.deltaTime * moveforce);




	}

	bool TargetTooClose(Transform t){
		float targetBackOffDistance = 10;
		return (Vector3.Magnitude(transform.position-t.position) < targetBackOffDistance);
	}

	float checkBlocksTimer = 0;
	bool CheckForMissingBlocks(){
		if (checkBlocksTimer > 0) return false;
//		// commented Debug.Log("checkin");
		int lostBlocks = 0;

		blocksCheck-=Time.deltaTime;
		if (blocksCheck < 0){
//			// commented Debug.Log("checking missing:");
			blocksCheck=.5f;
			foreach (NumberInfo ni in myBlocks){	
				if (ni == null || ni.fraction.numerator == 0) { // || NumberPool.inst.pooledNumbers.Contains(ni)){ 
					lostBlocks ++;
//					// commented Debug.Log("lostblocks plus because:"+ni+","+ni.fraction.numerator+"."+NumberPool
				}
				else {
//					// commented Debug.Log("block ok:"+ni.name);
				}
			}
			if (lostBlocks > 8 ){
//				// commented Debug.Log ("over 8.");
				return true;
			}
		}
//		if (lostBlocks > 0) // commented Debug.Log ("lostblocks:"+lostBlocks);
//		else // commented Debug.Log("none lost.");
		int lostcrit=0;
		foreach(GameObject o in criticalBlocks){
			if (o == null){
				lostcrit++;
			}
		}
		if (lostcrit > 1){
			return true;
		}
//		// commented Debug.Log("returning lb:"+lostBlocks);
		return false;
	}

	Transform GetTarget(){
		Transform t=target;
		if (t){
			if (!TargetInRange(t.position)){
//				// commented Debug.Log("lost target, too far.");
				t = null;
			}
		}
		if (t == null) {
			foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene()){
				if (ni){
					if (Mathf.Sign(ni.fraction.numerator) != Mathf.Sign(frac.numerator)){
						if (ni.transform.root != transform.root && Vector3.SqrMagnitude(ni.transform.position-transform.position) < sqrApproachRange && !ni.GetComponent<DoesExplodeOnImpact>()) {
							// Note this means he targets EVERYTHING.
							t = ni.transform;
							targetLockedTimer = 0;
							break;
						}
					}
				}
			}

		}

		return t;
	}

	bool TargetInRange(Vector3 target){
		return (Vector3.Magnitude(transform.position-target)<sqrApproachRange);
	}

	bool dead = false;
	void Die(){
		dead = true;
		transform.root.GetComponentInChildren<ResourceDrop>().DropResource();
		// commented Debug.Log("Dying now.");
		foreach(NumberInfo ni in myBlocks){
			if (ni != null){
				if (transform.root.GetComponentInChildren<LevelMonster_Shield>()){
					Destroy (transform.root.GetComponentInChildren<LevelMonster_Shield>().gameObject);
				}
				
				ni.transform.parent=null;
				if (!ni.gameObject.GetComponent<Rigidbody>()){
					ni.gameObject.AddComponent<Rigidbody>();
					ni.GetComponent<Rigidbody>().mass = 4;
					ni.GetComponent<Rigidbody>().drag = .2f;	
				}
				ni.gameObject.GetComponent<Collider>().isTrigger = false;
				if (ni.GetComponent<Rigidbody>()){
					ni.GetComponent<Rigidbody>().isKinematic=false;
					ni.GetComponent<Rigidbody>().useGravity=true;
				}
//				ni.pickupFlag = true; // todo: Make inventory recognize cube numbers as well.
				ni.gameObject.layer = LayerMask.NameToLayer("Default");
				ni.greaterThanCombine = 0;
				ni.lessThanCombine = 1;
//				if (ni.transform.transform.localScale.x > 3) // crit block scale is 4 and other block scale is 2.6

				PickUppableObject pip = ni.gameObject.AddComponent<PickUppableObject>();
				pip.heldScale = 1.5f; //2 / ni.transform.localScale.x;
					
//				}
//				pip.heldScale 
//				rocketLauncher.gameObject.AddComponent<Rigidbody>();
				rocketLauncher.transform.parent = null;
				TimedObjectDestructor tod = rocketLauncher.gameObject.AddComponent<TimedObjectDestructor>();
				
			}
		}
		Destroy(transform.root.gameObject);
	}

	bool needDestroyLr = false;
	void LateUpdate(){
		if (LevelBuilder.inst.levelBuilderIsShowing){
			needDestroyLr = true;
			for(int i=0;i<destinations.Length;i++){

				// Set up line renderers so player can see the path.
				LineRenderer lr = destinations[i].GetComponent<LineRenderer>();
				if (!lr) {
					lr = destinations[i].gameObject.AddComponent<LineRenderer>();
				}
				lr.material = destinationDrawMaterial;
				lr.SetVertexCount(2);
				lr.SetWidth(2,2);
				lr.SetColors(Color.white,Color.red);
				lr.SetPosition(0,destinations[i].position);
				lr.SetPosition(1,destinations[ (i+1) % destinations.Length].position);

				// Make sure float above terrain.
				RaycastHit hit;
				if (Physics.Raycast(destinations[i].position + Vector3.up * 200,Vector3.down,out hit,Mathf.Infinity,~LayerMask.NameToLayer("Terrain"))){
//					// commented Debug.Log("hit:"+hit.collider.name+"at distance;"+hit.distance+"; hit point y plus 10;"+(hit.point.y + 10));
					destinations[i].transform.position = new Vector3(destinations[i].transform.position.x,hit.point.y + 5,destinations[i].transform.position.z);
				} else {
//					// commented Debug.Log("miss:"+hit.collider.name);
					destinations[i].transform.position = new Vector3(destinations[i].transform.position.x,60,destinations[i].transform.position.z);
				}

			}
		} else if (needDestroyLr){
			needDestroyLr = false;
			foreach(Transform t in destinations){
				if (t.GetComponent<LineRenderer>()){
					Destroy(t.GetComponent<LineRenderer>());
				}
			}

		}
	}

	public void Freeze(){
		GetComponent<Rigidbody>().isKinematic = true;
	}

	public void UnFreeze(){
		GetComponent<Rigidbody>().isKinematic = false;
//		anim.SetBool("Walking",t
	}
}
