using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum TargetType {
	Food,
	Player,
	PlayerInventory,
	Home
}

[System.Serializable]
public class AnimalTarget {
	public Transform transform;
	public GameObject targetObj; // used only for inventory objects now, because we cannot use "transform" of those objects as the actual object is disabled and not located in the right place (the player)
	// targetobj is needed because we need to still have the functions like "Trylosetarget"  to work for PLAYER's distance, although player is not the actual target, a player inventory item is target
	public Quaternion rotation;
	public TargetType type = TargetType.Food;
	public Gadget gadget;
}

public abstract class Animal : NumberInfo, IMuteDestroySound, IDestroyedByPlayer {

	#region UserEditable
	// upoffset 		return 2;

	/* footpring was: (){
		return 3;
	 */
//	public override bool Exclude(){ return false; }

	public override void OnGameStarted(){
//		name += Random.Range(0,1000);
		base.OnGameStarted();
		Init();
	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> showThese = new List<GameObject>();
		showThese.AddRange(base.GetUIElementsToShow());
		showThese.Add(LevelBuilder.inst.POCMModifyAnimalRulesButton);
		return showThese.ToArray();
	}

	#endregion

	[Header("Animal")]
	public AnimalType type;
	public Transform head;
	// Sighhhh.. needs a refactor.




	public bool debug=false;
	public Transform home;
	// Direction & Interaction
	[SerializeField] public AnimalTarget target = new AnimalTarget();
	public float moveForce = 30;
	float rotSpeed = 2.5f;
	float rotSpeedFactor  = 1;
//	NumberInfo ni;

	public float targetEatDistance = 5f;

	// Pack behavior
	List<Animal> friends = new List<Animal>();
	List<Animal> nearFriends = new List<Animal>();
	List<Animal> friendsToRemove = new List<Animal>();

	public Vector3 forceThisFrame;

	public bool stayNearHome = true; //debug // moved to globalvars
	public float playerBackOffDistance = 50;
	float homeDistance = 15.0f;
	public float targetSeekRadius = 140;
	public float loseTargetDist = 250;
	public float stayNearHomeDistance = 15f;
	float homeBackOffDistance = 5f;
	float homeForce = 5.0f;
	float maxHomeForceCo = 4.0f;
	bool stayNearFriends = true;
	float idealFriendDistance = 10.0f;
	float careRange = 100.0f;

	

	public override void Start(){
		base.Start();
		Init();
	}
	// Line FX
	public LineRenderer line;
	public Material lineMaterial;


	// Efficiency
//	public int skipFrames = 2;
//	public int skip = 0;


	// Use this for initialization
	[System.NonSerialized] public RecordPosition rp;
	bool initialized = false;
	public virtual void Init () {
		if (initialized) return;
		initialized = true;
		rp = GetComponent<RecordPosition>(); 

		SetUpLineMaterial();
		SetUpFriends();

		SetNumber(fraction);
//		// commented Debug.Log("init;"+name);
		PlayerGadgetController.inst.gadgetThrow.onPlayerThrow += OnPlayerThrow;
	}

	void OnPlayerThrow(GameObject thrownObject){
//		// commented Debug.Log("player threw.");
		LoseTarget ();
		NumberInfo ni = thrownObject.GetComponent<NumberInfo>();
		if (ni){
			if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni)) {
				AttemptToSetTarget(thrownObject.transform);
			}
		}

	}


	void SetUpFriends(){
		// NOTE: This depends on all friendly animals being parented to the same object together on scene start.
		if (transform.parent){
			foreach(Transform t in transform.parent){
				if (t == gameObject.transform) continue;
				if (t.GetComponent(this.GetType())) friends.Add (t.GetComponent<Animal>());
			}
		}

		// This method doens't depend on parent..
		float maxFriendDistance = 30;
		foreach(Animal a in FindObjectsOfType<Animal>()){
			Transform t = a.transform;
			if (t == gameObject.transform) continue;
			if (Vector3.Distance(t.position,transform.position) > maxFriendDistance) continue;
			if (t.GetComponent(this.GetType()) && Vector3.SqrMagnitude(t.position-transform.position) < careRange * careRange * 3f) {
//				// commented Debug.Log("add;"+t.name);
				friends.Add (t.GetComponent<Animal>());
			}
		}
	}

	virtual public void SetUpLineMaterial(){
		line = gameObject.GetComponent<LineRenderer>();
		if (!line) line = gameObject.AddComponent<LineRenderer>();
		if (line == null) line = gameObject.AddComponent<LineRenderer>();
		line.material = lineMaterial;
	}

	public float seekTargetInterval = .2f;
	[System.NonSerialized] public float seekTargetTimer = 0;

	bool TargetStillValid(){
		if (target == null) return false;
		if (!target.transform)return false;
		return true;
	}


	public virtual bool animalIsAlive(){
		return GetComponent<Rigidbody>() || GetComponent<Animal_Frog>();
	}
//	public bool sleeping = false;
//	float sqrDistToSleep = 5000;
//	float sleepTimer = 0;
//	public void CheckSleep(){
//		// Sleep if player is far away.
//		sleepTimer -= Time.deltaTime;
//		if (sleepTimer < 0){
//			sleepTimer = Random.Range(1.5f,2f);
//			if (GetComponent<Rigidbody>() && (Vector3.SqrMagnitude(Player.inst.transform.position-transform.position) > sqrDistToSleep && target == null && (GetComponent<Rigidbody>().velocity == Vector3.zero || type == AnimalType.Bee))){
//				
//				sleeping = true;
//			} else {
//				sleeping = false;
//			}
//		}
//	}

	public bool animalDisabled = false; // if it's eatn by a snail, e.g.
	public virtual void AnimalUpdate () {
		if (animalDisabled) return;
		Debug.Log("updating base;");
		if (GameManager.inst.GameFrozen) { return; }
//		CheckSleep();
//		if (sleeping){
//			GetComponent<Rigidbody>().Sleep();
//			return;
//		}


//		if (type != AnimalType.Frog) {
//				GetComponent<Rigidbody>().isKinematic = true;
//				return;
//			} else {
//				GetComponent<Rigidbody>().isKinematic = false;
//			}
//		}

		if (!animalIsAlive()) return;
		if ((type != AnimalType.Bird && type != AnimalType.Bee) || (type == AnimalType.Fish && !underwater)) return;
//		if (type == AnimalType.Sheep) return;
//		if (type == AnimalType.Frog) return; 
//		if (type == AnimalType.Toad) return; 
//		if (type == AnimalType.Fish && !underwater) return;
		//lol.. well apparently i still need to pass through to the base update in some cases

		TryLoseTarget();
		if (target != null) {
			RotateTowardsTarget();
//			// commented Debug.Log("rotating towards target:" +name);
		} else  {
			IdleRotate();
		}
		MoveForwards();
		TryGetTarget();

		if (target == null){
			if (stayNearHome){
//				// commented Debug.Log("staying.");
				StayNearHome();
			}
			if (stayNearFriends){
				StayNearFriends();

			}
		}
		if (GetComponent<Rigidbody>()){
//			// commented Debug.Log ("force this frame:"+forceThisFrame);
			GetComponent<Rigidbody>().AddForce(forceThisFrame,ForceMode.Acceleration);
		}
		forceThisFrame = Vector3.zero;
//		// commented Debug.Log(

		CheckTargetActive();
		if (target == null) return;
		CheckCloseToTargetAndEat();
	}


	virtual public void StayNearFriends(){
//		Debug.Log("Friends");
		GetNearFriends();
		RemoveDestroyedFriends();
		AvoidHitting();
		MaintainProximity();
		MaintainDirection();
	}

	float targetActiveTimer = 0;
	virtual public void CheckTargetActive(){
		if (target == null) return;
		if (target.transform == null) return;
		targetActiveTimer -= Time.deltaTime;
		if (targetActiveTimer < 0){
			targetActiveTimer = 1;
//			// commented Debug.Log("targ;"+target.transform.name);
			if (target.transform.gameObject.activeSelf == false && target.type != TargetType.Player) LoseTarget("nacve");
		}
		if (target.transform == PlayerGadgetController.inst.gadgetThrow.transform){
			List<Ammo> ammos = PlayerGadgetController.inst.GetCurrentGadget().GetAmmoInfo();
//			if (ammos.Count == 0) LoseTarget("no ammo to target");
			// If target was player hand, which is no longer equipped, lose target
//			if (PlayerGadgetController.inst.GetCurrentGadget() != PlayerGadgetController.inst.gadgetThrow) { LoseTarget("nt throw"); }
//			 Target was player hand and still equipped but the number was lost
//			else if (PlayerGadgetController.inst.GetCurrentGadget().GetAmmoInfo().Count == 0) { LoseTarget("no held num"); }
			bool loseTarget = true; // assume no ammo will match.
			foreach(Ammo a in ammos){
				if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,a.ammoValue)){
					loseTarget = false; // found a valid ammo, don't lose this target.
				}
			}
			if (loseTarget) LoseTarget("failed ammo check");
			// Target was player hand and still equipped but the number changed to invalid target
//			else if (!AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni)) { LoseTarget("no vald held num"); }
		}
	}

	virtual public void TryGetTarget(){
		if (this.gameObject.activeSelf){
			seekTargetTimer -= Time.deltaTime;
			if (seekTargetTimer < 0){
				seekTargetTimer = Random.Range(seekTargetInterval/2f,seekTargetInterval*2f);
				if (target == null) SeekTarget();
				else if (target.transform == null) SeekTarget();
				else if (target.type == TargetType.Home) SeekTarget();
			}
		}
	}

	void RemoveDestroyedFriends(){
		foreach(Animal friend in nearFriends) {
			if (!friend || !friend.GetComponent<Rigidbody>()) {
				friendsToRemove.Add(friend);
			}
		}
		foreach(Animal p in friendsToRemove){
			if (nearFriends.Contains(p)) nearFriends.Remove(p);
			if (friends.Contains(p)) friends.Remove (p);
		}
		friendsToRemove.Clear();
	}
	
	virtual public void StayNearHome() {
		// if (!home) home = PlayerGadgetController.inst.magnetGraphics.transform; // assume player should be home if we lost it
		if (home){
			Vector3 offset = home.position - transform.position;
			if (offset.magnitude > stayNearHomeDistance){
	//			float atten = Mathf.Min(offset.magnitude / homeDistance, maxHomeForceCo);
//				// commented Debug.Log("set target home.");
				SetTarget(home,TargetType.Home);
				RotateTowardsTarget();
			}
		}
	}

	virtual public void LateAnimalUpdate(){

		DrawTargetLine();

	}

	public virtual void DrawTargetLine(){
		if (!this) return;
		if (!line) return;
		if (target != null && target.transform != null && target.type != TargetType.Home){
//			// commented Debug.Log ("Still target;"+target.transform);
			//			if (playerHasTarget) line.SetColors(Color.red,Color.white);
			//			else line.SetColors(Color.blue,Color.white);
			line.SetVertexCount(2);
			line.SetWidth(.1f,.05f);
			line.SetPosition(0,transform.position);
			Vector3 targetPos = target.transform.position;
			if (target.type == TargetType.Player) {
				targetPos += Vector3.up * 0.5f;
			}
			line.SetPosition(1,targetPos);
			//			float startWidth = 1f;
			//			float endWidth = 1f;
		} else {
			//			if (debug) // commented Debug.Log ("line dispapears now.");
			line.SetVertexCount(0);
		}
//		KeepRotationReasonable();

	}

	public float reasonableAngleRange = .1f;
	void KeepRotationReasonable(){
		// commented Debug.Log("keeping rotation reasonable");
		Quaternion rot = transform.rotation;
		rot = new Quaternion(Mathf.Clamp (rot.x,-reasonableAngleRange,reasonableAngleRange),rot.y,Mathf.Clamp (rot.z,-reasonableAngleRange,reasonableAngleRange),rot.w);
//		transform.rotation = Quaternion.Slerp (transform.rotation,rot,Time.deltaTime * rotSpeed);
		transform.rotation = rot;
		Debug.Log("got rot:"+rot);
		//		float x = Mathf.Clamp (rot.eulerAngles.x % 360,-25,25);
		//		float z = Mathf.Clamp (rot.eulerAngles.z % 360,-15,15);
		//		rot.eulerAngles = new Vector3(x,rot.eulerAngles.y,z);
		//		transform.rotation = rot;
	}
//
//	public void PopulateNearFriends(){
//		nearFriends = new List<Animal>();
//		foreach(Animal friend in friends) {
//			if(!friend || !friend.gameObject) { continue; }
//			if((transform.position - friend.transform.position).sqrMagnitude < careRange*careRange) {
//				nearFriends.Add(friend);
//			}
//		}
//	}


	virtual public void MoveForwards() {
		//		if (!underwater) return; // flop?
		if (!GetComponent<Rigidbody>()) return; // player holding should be an easier check..
		float atten = 1;
		float targetFactor = 1;

		if (target != null && target.transform != null) {
			Vector3 targetDir = target.transform.position - transform.position;
			float angle = Vector3.Angle(transform.forward,targetDir);
			atten = Mathf.Min (1,Mathf.Max (.1f,15f/angle));
			float playerFactor = 1f;
			if (target.type == TargetType.Player) {
				if (targetDir.sqrMagnitude < playerBackOffDistance * playerBackOffDistance){
					atten = 0;
//					atten *= Mathf.Clamp (targetDir.sqrMagnitude / 100 * playerFactor,0.5f,1f); //Mathf.Min(1,Mathf.Max (0.3f,targetDir.sqrMagnitude / 100));
				}
			}
		}
//			if (Vector3.SqrMagnitude(target.t
		forceThisFrame += (moveForce * atten * targetFactor * transform.forward);
		if (this.GetType() == typeof(Animal_Fish)){
//			Debug.Log("fishing f:"+forceThisFrame+","+moveForce+","+targetFactor+","+atten);
		}

	}

	float rotateTimer = 0;
	public Quaternion targetRot = Quaternion.identity;
	public void SetTargetRot(Quaternion q){
		targetRot = q;
	}


	virtual public void IdleRotate(){
//		// commented Debug.Log ("idle rotate.");
		rotateTimer -= Time.deltaTime;
		if (rotateTimer < 0){
//			// commented Debug.Log ("idling rotating to new coords.");
			rotateTimer = Random.Range (7,17f);
			Vector3 rand = new Vector3(Random.Range (-1f,1f),Random.Range (-.15f,.15f),Random.Range (-1f,1f)); // flat to within 15%
			targetRot = Quaternion.LookRotation(rand);
		}
//		// commented Debug.Log ("idle rotate:"+targetRot.eulerAngles);
		transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime * rotSpeed);
		Debug.Log("idle rotate");
		// Rotate to within 45 degrees of a cone looking forwards.
		
	}

	AnimalType[] frogTypes; 
	bool establishedType = false;
	bool isFrogType = false;
	public bool AnimalIsFrogType(){
		if (establishedType) return isFrogType;
		else {
			frogTypes = new AnimalType[5];
			frogTypes[0] = AnimalType.Frog;
			frogTypes[1] = AnimalType.Toad;
			frogTypes[2] =  AnimalType.Croaker;
			frogTypes[3] = AnimalType.Pollywog;
			frogTypes[4] = AnimalType.FrogQueen;;
			isFrogType = frogTypes.ToList().Contains(type);
			establishedType = true;
			// why did I make this so complicated ..? Couldn't instantiate array of enums like: AnimalType[] frogTypes = new AnimalType[] { frogtype1, frogtype2, .. };
			return isFrogType;
		}
	}
	public virtual void TryLoseTarget(){
//		// commented Debug.Log("lose target?");

		if (target == null){
			LoseTarget("target was null");
		} else if (target.transform == null)	{
			LoseTarget("target trnfsrm was null");
		} else {
//			if (target.transform == null)	 LoseTarget("transform was null");
			NumberInfo ni = target.transform.GetComponent<NumberInfo>();

			if (target.transform.gameObject.activeSelf == false && target.targetObj == null) 	LoseTarget("was inactive and null obj");
			else if (TargetTooFar()) 	LoseTarget("toofar");
			else if (TargetHomeTooClose()) LoseTarget("target was home (we are returning home) and we got close enough");
			else if (ni && !CheckValidTarget(ni)) LoseTarget("no vald"); 
			else if (target.type == TargetType.Player && (!AnimalIsFrogType())){ // only frogs will try to lose the target if player isn't HOLDING the number
				if (!PlayerGadgetController.inst.ThrowGadgetEquipped()) {
	//				// commented Debug.Log(name + " lost player as target  beacuse player not holding anything!");
					LoseTarget("playerhold");
				} else {
					// Player still holding a number but is it a valid target?
					List<Ammo> ammos = PlayerGadgetController.inst.GetCurrentGadget().GetAmmoInfo();
					foreach(Ammo a in ammos){
						if (!AnimalBehaviorManager.inst.CheckValidTarget(tpr,this,a.ammoValue)) {
		//					// commented Debug.Log(name + " lost player as target  beacuse player holding wrong number!");
							// if any ammo was invalid..
							LoseTarget("newly invalid playerheld");
						}
					}
				}
			}
		}
	}

	virtual public bool CheckValidTarget(NumberInfo ni){
		return AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni);
	}

	bool TargetHomeTooClose(){
//		if (target != null){
		if (target.type == TargetType.Home){
			if (Vector3.Distance(target.transform.position,transform.position) < homeBackOffDistance){
				return true;
			}
		}
//		}
		return false;
	}

	bool TargetTooFar(){
//		Debug.Log("Target too far?");
		Vector3 tp = target.type == TargetType.PlayerInventory ? Player.inst.transform.position : target.transform.position; // checking distance to player if the target type was inventory
		if (Vector3.Magnitude(transform.position-tp) > loseTargetDist){
			//			// commented Debug.Log ("Target, " +target.name+", was too far away. Lost.");

			//			// commented Debug.Log ("player has: false");
			return true;
		}
		return false;
	}

	public virtual void LoseTarget(string source=""){
//		Debug.Log("lost targ:"+source);
		if (target == null) {
			return;
		}
//		Debug.Log("lost target:"+target.transform+" because "+source);
		if (!line){
//			// commented Debug.Log("error no line on.. this obj");
		} else{
			line.SetVertexCount(0);
		}
//		// commented Debug.Log("target lost.");
		target = null;
	}

	AnimalTargetPreferenceRel tpr;
	public AnimalTargetPreferenceRel GetAnimalTargetPreference(){
		if (tpr == null) tpr = AnimalBehaviorManager.inst.GetAnimalTargetPreferenceRelFromAnimalType(type);
		return tpr;
	}


	List<NumberInfo> cachedNumbers = new List<NumberInfo>();
	float checkNumbersTimer = 0f;
	List<NumberInfo> GetNumbersInRange(){
		checkNumbersTimer -= Time.deltaTime;
		if (checkNumbersTimer < 0){
			checkNumbersTimer = Random.Range(0.5f,1f);
			float tt = targetSeekRadius * targetSeekRadius;
			List<NumberInfo> ret = new List<NumberInfo>();
			foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene()){
				if (ni){
					if (Vector3.SqrMagnitude(ni.transform.position - transform.position) < tt){
						ret.Add(ni);
					}
				}
			}
			cachedNumbers = ret;
		} else {
		}
		return cachedNumbers;
	
	}
	public bool PlayerInRange(){
//		Debug.Log("player range? " + (Vector3.Magnitude(Player.inst.transform.position-transform.position) < targetSeekRadius) + " , " +GetAnimalTargetPreference().cannibalize);
		return Vector3.Magnitude(Player.inst.transform.position-transform.position) < targetSeekRadius || GetAnimalTargetPreference().cannibalize;
	}

	public virtual void SeekTarget(){
		
//		Collider[] cols = Physics.OverlapSphere(transform.position,targetSeekRadius);
		float dist = Mathf.Infinity;
		Transform closest = null;
		if (!this) return;

		if (PlayerInRange()){
			List<Ammo> ammos = new List<Ammo>();
			if (AnimalIsFrogType()){
//				Debug.Log("seek frog.");
				// Frogs and toads search your whole inventory.
				foreach(Gadget g in PlayerGadgetController.inst.GetAllGadgetInInventory()){
//					Debug.Log("gadget."+g+".  its ammoinfo count:"+g.GetAmmoInfo().Count);
					foreach(Ammo a in g.GetAmmoInfo()){
						// Try to set the target to the number held by the player so you can bait the animal to follow you
//						Debug.Log("A:"+a.ammoValue);
						if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,a.ammoValue)){
//							Debug.Log("try to set target to player gadg:"+g+", ammo;"+a);
							AttemptToSetTarget(Player.inst.transform,false,g);
							// Note that if a gadget has TWO ammos loaded only ONE of which is valid, we only pass the GADGET as the target. Later we'll need to sort whether this gadget really has edible ammo.
							return; // prefer player.
						} else {
//							Debug.Log("A wasn't valid:"+a);
						}
					}		
				}
//				Debug.Log("got ammos;"+ammos.Count);
			} else {
				// Not a frog, search current gadget only.
				foreach(Ammo a in PlayerGadgetController.inst.GetCurrentGadget().GetAmmoInfo()){
					// Try to set the target to the number held by the player so you can bait the animal to follow you
					if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,a.ammoValue)){
	//					Debug.Log("try to set target to playER");
						AttemptToSetTarget(Player.inst.transform);
						return; // prefer player.
					}
				}	
			}
		
			if (AnimalIsFrogType()){
				// If player not holding number in hand, and animal was frog, also check player inventory.
				// This is because we want frogs to act as restrictors and they can eat all numbers from your inventory if the rules match (e.g. all whole numbers)
				foreach(NumberInfo ni2 in Inventory.inst.GetNumbersInInventory()){
					if (ni2.GetComponent<FrogTarget>()) continue;
					if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni2)){
						
						AttemptToSetTarget(ni2.transform,true);
						return; // prefer player.
					}
				}

				// only frogs target vehicle numbers. For now.
				foreach(Vehicle v in VehicleManager.inst.vehicles){
					
					EnergyBall eb = v.GetComponentInChildren<EnergyBall>();
					if (eb) {
						
						NumberInfo ni2 = eb.GetComponent<NumberInfo>();
//						Debug.Log("eb:"+ni2.name);
						if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(),this,ni2,false)){
//							Debug.Log("attempt:"+ni2.transform.position+","+transform.position+", delta:"+Vector3.Distance(ni2.transform.position,transform.position));
							if (AttemptToSetTarget(ni2.transform)){
								return;
							}
//							return;
						} else {
//							Debug.Log("invalid");
						}
					}
				}
			}



		}

//		Debug.Log(name + " is looping 1. Numbers in range:"+GetNumbersInRange().Count);
//		NumberManager.inst.GetAllNumbersInScene(); // ?? This is really the most efficient? hm
		foreach(NumberInfo ni in GetNumbersInRange()){
			if (!ni) continue;
			if (ni.GetComponent<FrogTarget>() && !ni.GetComponent<Animal_Frog>()) continue;
//			if ((AnimalIsFrogType()) && ni.GetComponent<FrogTarget>()) continue;
			float diff = Vector3.Distance(ni.transform.position,transform.position);
			if (diff < dist){
				if (AnimalBehaviorManager.inst.CheckValidTarget(GetAnimalTargetPreference(), this,ni)){
					dist = diff;
					closest = ni.transform;
				} else {
//					Debug.Log("invalid:"+ni);
				}
			}
		}
		if (closest) {
//			Debug.Log(name + " tried to set target to "+closest);
			AttemptToSetTarget(closest);
		}
	}


	


	virtual public void RotateTowardsTarget(){
		Vector3 dirToTarget = Vector3.Normalize(target.transform.position - transform.position);
		RotateTowardsTarget(dirToTarget);
	}

	virtual public void RotateTowardsTarget(Vector3 dirToTarget) {
		if (dirToTarget == Vector3.zero) return;
		Quaternion targetRott = Quaternion.LookRotation(dirToTarget, Vector3.up);
//		targetRott.eulerAngles = new Vector3(targetRott.eulerAngles.x,
		transform.rotation = Quaternion.Slerp (transform.rotation,targetRott,Time.deltaTime * rotSpeed * rotSpeedFactor);
		Debug.Log("rotate target");
	}

	bool TargetIsSelf(Transform t){
		return t == transform;
	}

	public virtual bool AttemptToSetTarget(Transform t){
		return AttemptToSetTarget(t, false);
	}

	public virtual bool AttemptToSetTarget(Transform t, bool inv, Gadget g = null){

		if (t == null) {
			return false;
		}
//		Debug.Log("setting tareget:"+t.name);
//		if (!IsValidTarget(t))	return false;
		if (inv){ 
			
			// special case for inventory since the "transform" of the target will actually tbe the inventory's item3d which is a disabled object not located physically on the player
			// so instead we check distance to player rather than the target transofrm
//			// commented Debug.Log("inv!");
			if (Vector3.Distance(transform.position,Player.inst.transform.position) < targetSeekRadius) {

				SetTarget(PlayerGadgetController.inst.gadgetThrow.transform,TargetType.PlayerInventory, t.gameObject,g);
			}

		}
		if (Vector3.Distance(t.position,transform.position) > targetSeekRadius) {  return false;}
		if (t.gameObject.tag == "Player"){
//			Debug.Log("player");

			SetTarget (PlayerGadgetController.inst.gadgetThrow.transform,TargetType.Player,null,g);
		} else {
			
//			Debug.Log("succesS");
			SetTarget (t,TargetType.Food);
		}

		return true;
	}

	public virtual void SetTarget(Transform t, TargetType tt){
		SetTarget(t,tt,null);
	}

	public virtual void SetTarget(Transform t,TargetType tt, GameObject targetObj, Gadget g = null){

		target = new AnimalTarget();
		target.transform = t;
		target.type = tt;
		if (g) {
//			Debug.Log("set targ with g;"+g);
			target.gadget = g; // sooo many threads that this is woven into how annoying, but we need to keep a ref to the gadget from within which we found a tasty target ammo
		} else {
//			Debug.Log("set targ:"+t.name+", no gadget");
		}
//		Debug.Log("set target. t:"+t);
		if (targetObj) target.targetObj = targetObj; // should only happen with frogs.. This is code debt. Scrap this script and start over!!
//		// commented Debug.Log("set target:"+t.name+", target type:"+tt);
	}

	
	virtual public void Eat(GameObject o){
		if (!o){
			LoseTarget("no obj");
			return;
		}

		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (!ni) {
			LoseTarget("no ni");
			return;
		}
		if (ni.usedThisFrame) {
			LoseTarget("ni used");
			return;
		}
		ni.usedThisFrame = true;
		ni.NotifyDestroyer(this.gameObject); // let numberinfo know who is eating it.
		Fraction result = Fraction.Add(this.fraction,ni.fraction);
		if (result.numerator == 0){
			Debug.Log("Result of  animal "+this.fraction+" and ate:"+ni.fraction + " was zero!");
			foreach(IDestroyedByPlayer dp in gameObject.GetComponents(typeof(IDestroyedByPlayer))){
				dp.DestroyedByPlayer();
			}
		}
		base.Eat(ni);
//		Sprite icon = ni.GetComponent<UserEditableObject>().icon;

//		justAteFX.transform.parent = transform;

		if (AnimalIsFrogType()){
			float p = Random.Range(0.75f,1.1f);
			AudioManager.inst.PlayMunch(transform.position,1,p); // frogs have a quieter munch
		} else {
			AudioManager.inst.PlaySheepMunch(transform.position);
		}
		LoseTarget("ate");
	}



	float checkEatTimer = 0;
	public virtual void CheckCloseToTargetAndEat(){
		if (target == null) return;
		if (target.transform == null || target.type != TargetType.Food) return;
//		Debug.Log("Check close to targ");
		checkEatTimer -= Time.deltaTime;
		if (checkEatTimer < 0){
			checkEatTimer = 0.5f;
			float dist = Vector3.Distance(target.transform.position,transform.position);
			if (dist < targetEatDistance){
//				Debug.Log("sheep ate at distance:"+ dist);
				Eat (target.transform.gameObject);

				LoseTarget("aed");
			} 
		}
	}



	public bool underwater = true;
	[System.NonSerialized] public int underwaterCount = 0;
	public virtual void SetUnderwater(bool flag){
		if (flag) underwaterCount ++;
		else underwaterCount --;
		if (debug) // commented Debug.Log ("set underwater:"+flag+", under count;"+underwaterCount);
		underwater = underwaterCount > 0 ? true : false;

	}

	float friendsTimeout = 0;
	void GetNearFriends(){
		friendsTimeout -= Time.deltaTime;
		if (friendsTimeout < 0){
			friendsTimeout = Random.Range(2,4f);
			nearFriends = new List<Animal>();
			int i=0;
			foreach(Animal friend in friends) {
				if (i > 4) continue;
				if(!friend || !friend.gameObject) { continue; }
				if((transform.position - friend.transform.position).sqrMagnitude < careRange*careRange) {
					nearFriends.Add(friend);
					i++;
				}
			}
		}
	}


	// Stay near friends but not TOO near
	public float fleeDistance = 24.0f;
	public float fleeForce = 10.0f;
	
	// The maximum distance to start returning to the swarm
	float tooFarDistance = 10.0f;
	float tooFarForce = 30.0f;
	void AvoidHitting() {
		foreach(Animal friend in nearFriends) {
			

			Vector3 offset = transform.position - friend.transform.position;
			if(offset.sqrMagnitude < fleeDistance*fleeDistance) {
				float atten = (fleeDistance - offset.magnitude) / fleeDistance;
				float targetAtten = 0;
				if(target != null) {
					targetAtten = Mathf.Min((target.transform.position - transform.position).magnitude / targetSeekRadius, 1.0f);
					targetAtten = (1 - targetAtten) * targetedFleeAttenuation;
				}
				forceThisFrame += offset.normalized * fleeForce * atten * (1-targetAtten);
			}
		}

	}


	float loseTargetTimer = 0.0f;
	float loseTargetInterval = 7.0f;	
	float targetLostCooldownTimer = 0.0f;
	float targetLostCooldown = 1.0f;// was 2
	
	// Coefficient for the flee force as I approach my target
	float targetedFleeAttenuation = 0.3f; // was .7
	
	void MaintainProximity() {
		Vector3 average = Vector3.zero;
		foreach(Animal friend in nearFriends) {
			average += friend.transform.position;
		}
		average /= nearFriends.Count;
		Vector3 offset = average - transform.position;
		if(offset.sqrMagnitude > tooFarDistance * tooFarDistance) {
//			// commented Debug.Log ("proximitagin");
			forceThisFrame += (offset.normalized * tooFarForce);
		}
	}

	void MaintainDirection() {
		Vector3 average = Vector3.zero;
		foreach(Animal friend in nearFriends) {
//			// commented Debug.Log("friends;"+friend.name);
			average += friend.GetComponent<Rigidbody>().velocity;
		}
		float orientForce = 3.0f;
		RotateTowardsTarget(average);
	}

	public void MuteDestroy(){
		muteDestroySound = true;
	}
	public bool muteDestroySound = false; // sometimes, e.g. when we're multiply stack fx, we don't want to make a peep when we die.

	void Awake(){
		UpdateManager.inst.animals.Add(this);
	}
	public override void OnDestroy(){
		base.OnDestroy();
		UpdateManager.inst.RemoveAnimal(this);
	}

	protected void OnDisable(){
		LoseTarget("Disabled animal");
	}

	public void DestroyedByPlayer(){
		ResourceDrop rd = GetComponentInChildren<ResourceDrop>();
		if (rd){
			rd.DropResource();
		}
	}
}
