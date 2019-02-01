using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SnailType {
	Factors,
	Multiples
}

public class MonsterSnail : UserEditableObject {

	public Transform[] destinations;
	int index=0;
	public Material destinationDrawMaterial;

	#region UserEditable

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (GetComponentInChildren<NumberInfo>()){
			if (N.GetKeys().Contains(Fraction.fractionKey)){
				Fraction newFrac = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
				GetComponentInChildren<NumberInfo>().SetNumber(newFrac); // should only be one (for the starting case...)
			}
//			if (N.GetKeys().Contains(typeKey)){
			// EDIT we're going to just have 2 prefabs, its easier
//				if (N[typeKey] == SnailType.Factors.ToString()){
//					type = SnailType.Factors;
//				} else if (N[typeKey] == SnailType.Multiples.ToString()){
//					type = SnailType.Multiples;
//				}
//			}
//			if (N.GetKeys()
		}  else {
			Debug.LogWarning("Monster blob had no number to set props to!");
		}
	}
	public override SimpleJSON.JSONClass GetProperties(){
		SimpleJSON.JSONClass N = base.GetProperties();
		if (GetComponentInChildren<NumberInfo>()){
			N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,GetComponentInChildren<NumberInfo>().fraction,N);
		} else {
			Debug.LogWarning("Monster blob had no number to get props from!");
//			return new SimpleJSON.JSONClass(); // oops, no number inside!
		}
//		N[typeKey] = type.ToString();
		return N;
	}

	public override void OnGameStarted(){
		base.OnGameStarted();
		GetComponent<Rigidbody>().isKinematic = false;
	}

	#endregion

	void Start(){
		GadgetThrow.inst.onPlayerThrow += PlayerThrew;
		foreach(NumberInfo ni in GetComponentsInChildren<NumberInfo>()){
			ni.numberChangedDelegate += HandleNumberChanged;
			ni.numberDestroyedDelegate += NumberDestroyed;
//			Debug.Log("added num changed delegate to:"+ni);
		}
	}

	void PlayerThrew(GameObject o){
		if (this){ // in case delegate still attached  w/ gameobj null?
			NumberInfo ni = o.GetComponent<NumberInfo>();
			if (IsValidTarget(ni)){
				SetTarget(ni);
			}
		}
	}
	bool targetIsCircleRoute = false;
	void SetTarget(NumberInfo ni){
		targetIsCircleRoute = false;
		//					Debug.Log("YES");
		if (!moving) snailAnim.SetTrigger("Emotion");
		snailAnim.SetBool("Move",true);
		moving = true;
		target = ni.transform;
		targetCheckTimer = 5f;
	}

	public List<NumberInfo> blobNumbers = new List<NumberInfo>();
	public SnailType type = SnailType.Factors;
	public Animator snailAnim;

	void OnTriggerEnter(Collider other){
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni && ni.fraction.denominator == 1 && !ni.GetComponent<BlobNumber>()){
			if (blobNumbers.Contains(ni)) return;
			else {
				
				List<NumberInfo> numsToAdd = new List<NumberInfo>();
				bool allowedToAddThisNumber = false;

				foreach(NumberInfo n2 in blobNumbers){
					Fraction result = Fraction.Add(ni.fraction,n2.fraction);

						if (type == SnailType.Factors && n2.fraction.IsMultipleOf(ni.fraction) && !allowedToAddThisNumber){
							allowedToAddThisNumber = true;

//							break;
						} else if (type == SnailType.Multiples && ni.fraction.IsMultipleOf(n2.fraction) && !allowedToAddThisNumber) {
							allowedToAddThisNumber = true;

//							break;
						}
					
						
//					}
				}
				if (allowedToAddThisNumber){
					numsToAdd.Add(ni);
				}
				foreach(NumberInfo numToAdd in numsToAdd){
					AddNumberToBlob(numToAdd);

				}
			}
		}
	}

//	void HandleNumberChanged (NumberInfo ni)
//	{
//		
//	}

	void EatAndBreakNumbers(NumberInfo n1, NumberInfo n2){
		eatingNumbers.Add(n1);
	}

	float targetCheckTimer = 0f;
	bool moving = false;
	Transform target;

	bool IsValidTarget(NumberInfo ni){
		if (ni.GetComponent<BlobNumber>()) return false;
		if (ni.GetComponent<MonsterSnail>()) return false;
		float attackRange = 35f;
		if (Vector3.Distance(ni.transform.position,transform.position) < attackRange){
//			Debug.Log("Is valid:"+ni+","+ni.myName+",val;"+ni.fraction+"?");
//			Debug.Log("range");
			if (type == SnailType.Multiples){
				foreach(NumberInfo n2 in blobNumbers){
					if (ni.fraction.IsMultipleOf(n2.fraction)){
						return true;
					}
				}
			} else if (type == SnailType.Factors){
				foreach(NumberInfo n2 in blobNumbers){
					if (n2.fraction.IsMultipleOf(ni.fraction)){
						return true;
					}
				}
			}
		}
		return false;
	}
	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing){
			return;
		}

		List<NumberInfo> ate = new List<NumberInfo>();
		List<NumberInfo> toRemove = new List<NumberInfo>(); // if you ate the inverse of a prime you contain (e.g. you ate -5 while containing {1,-5,10}), then break both numbers after eating finished.
		foreach(NumberInfo ni in eatingNumbers){
			if (!ni) {
				ate.Add(ni); 
				continue;
			}
			Rigidbody rb = ni.GetComponent<Rigidbody>();
			float fudge = 1f;
			Vector3 centerOffset = Vector3.up * 4f;
			Vector3 targetPos = Vector3.Normalize(transform.position+centerOffset-ni.transform.position) + Random.insideUnitSphere * fudge;
			rb.AddForce(targetPos*400);

			float distToMaw = Vector3.Distance(ni.transform.position,transform.position+Vector3.up * 4);
			float distToFx = 10.5f;
//			Debug.Log("dist to maw;"+distToMaw);
			if (distToMaw < distToFx){ // when number gets inisde our stomach
				EatFX(ni);
				ate.Add(ni);
				if (!blobNumbers.Contains(ni)) {
					blobNumbers.Add(ni);
					// todos possible memory leak
					ni.numberChangedDelegate += HandleNumberChanged;;
					ni.numberDestroyedDelegate += NumberDestroyed;
				}
//				Debug.Log("ate;"+ni);
			} 
		}
		foreach(NumberInfo a in ate){
			if (a) a.GetComponent<Collider>().enabled = true;
			if (eatingNumbers.Contains(a)){
				eatingNumbers.Remove(a);
			}

		}


		targetCheckTimer -= Time.deltaTime;
		if (targetCheckTimer < 0){
			float playerCareRange = 150f;
			if (Vector3.Distance(Player.inst.transform.position,transform.position) < playerCareRange){
				snailAnim.SetBool("Move",false);
				targetCheckTimer = Random.Range(1f,2f);
				foreach(NumberInfo ni in NumberManager.inst.GetAllNumbersInScene()){
					if (!ni) continue;
					if (IsValidTarget(ni)){
						SetTarget(ni);
	

						break;
					} else {
	
					}
				}
				if (target == null || targetIsCircleRoute) { // didn't get a number target so follow our algo
//					target = destinations[0];
					targetIsCircleRoute = true;
					if (destinations[0].transform.parent != null){
						destinations[0].transform.parent.parent = null;
					}
				

					target = destinations[index % destinations.Length];
					float dist = Vector3.Distance(transform.position,target.position);
//					Debug.Log("dist:"+dist);
					if (dist < 7) {
						//				// commented Debug.Log("reched dest:"+dest);
						index++;
//						Debug.Log("index:"+index);
					}
					moving = true;
				}

			}
		}
		if (moving) {
//			Debug.Log("mov.");
//			float moveForce = 5000f;
			float moveSpeed = 3f;
			if (target != null && target.gameObject.activeSelf && !target.GetComponent<BlobNumber>()) {
				transform.position = Vector3.MoveTowards(transform.position,target.position,Time.deltaTime * moveSpeed);
				float degPerSecond = 55f;
				transform.rotation = Quaternion.RotateTowards(transform.rotation,Utils.FlattenRotation(Quaternion.LookRotation(target.position-transform.position,Vector3.up)),degPerSecond*Time.deltaTime);
			} else {
				targetCheckTimer = 2f;
				moving = false;
				snailAnim.SetBool("Move",false);
			}
//			Vector3 moveDir = Vector3.Normalize(moveDestination - transform.position);
//			GetComponent<Rigidbody>().AddForce(moveForce * moveDir);
		}
		if (blobNumbers.Count == 0) {
			GetComponentInChildren<ResourceDrop>().DropResource();				
			for (int i=0;i<6;i++){
				EffectsManager.inst.CreateShards(transform.position + (Random.insideUnitSphere + Vector3.up) * 5);
			}
			AudioManager.inst.PlayDeathSound(transform.position);

			foreach(IDestroyedByPlayer dp in gameObject.GetComponents(typeof(IDestroyedByPlayer))){
				dp.DestroyedByPlayer();
			}
			Destroy(gameObject);

		}

		foreach(NumberInfo ni in blobNumbers){
//			ni.GetComponent<Rigidbody>().AddForce();
			if (!ni) toRemove.Add(ni);
			else {
				Rigidbody rb = ni.GetComponent<Rigidbody>();
				float fudge = 1f;
				Vector3 centerOffset = Vector3.up * 4f;
				Vector3 targetPos = Vector3.Normalize(transform.position+centerOffset-ni.transform.position) + Random.insideUnitSphere * fudge;
				rb.AddForce(targetPos*400);
				float torque = 400f;
				rb.AddTorque(Vector3.right * torque * Time.deltaTime);
			}
		}
		RemoveSnailNumbers(toRemove);
	}

	void HandleNumberChanged (NumberInfo ni) {
//		Debug.Log("numchangd;"+ni);
		CheckZeros();
	}


	bool TargetTooClose(){
		
		return Vector3.Distance(target.position,transform.position) < 3f;
	}
	List<NumberInfo> eatingNumbers = new List<NumberInfo>();
	void AddNumberToBlob(NumberInfo ni){
		ni.greaterThanCombine=0;
		ni.lessThanCombine = 1; // re-allow combos
		Animal a = ni.GetComponent<Animal>();
		if (a) a.animalDisabled = true; // disable animal for consumption, but don't "destroy" or change it.
		targetCheckTimer = Random.Range(2f,4f);
		if (type == SnailType.Multiples){
			bool proceed = false; // only proceed if we matched a number. Other numbers will be deconstructed.
			foreach(NumberInfo n2 in blobNumbers){
				if (Fraction.Equals(Fraction.GetAbsoluteValue(n2.fraction),Fraction.GetAbsoluteValue(ni.fraction))){
					proceed = true;
					break;
				}
			}
			if (!proceed){ // This happens when a larger multiple was broken down into smaller ones. We stop the checking process, break it up, and eat each piece.
//				Debug.Log("donot");
				List<NumberInfo> numsToAdd = new List<NumberInfo>();
				foreach(NumberInfo n2 in blobNumbers){
					if (ni.fraction.IsMultipleOf(n2.fraction)){
						int count = Mathf.Abs(Fraction.Divide(ni.fraction,n2.fraction).numerator);
						ni.SetNumber(n2.fraction); // important to set the number before instantiation because cctext will lose mesh / mesh 0 vertices if you reverse this
//						Debug.Log("set");
						for(int i=0;i<count;i++){
							// if an 8 is eaten by a Multiple Snal, it makes 4 2's in front of it and eats them one by one.
							GameObject newNum = (GameObject)Instantiate(ni.gameObject,ni.transform.position+transform.forward*i*3f,Quaternion.identity);
							NumberInfo ni3 = newNum.GetComponent<NumberInfo>();
							numsToAdd.Add(ni3);
							newNum.GetComponent<Rigidbody>().useGravity = false;
							ni3.greaterThanCombine = 1; // prevent combos.
							ni3.lessThanCombine = 0;
//							eatingNumbers.Add(ni);
//							ni.GetComponent<Collider>().enabled = false;
//							Debug.Log("add:"+newNum);
						}
						NumberManager.inst.DestroyOrPool(ni);
						break;
					}
				}
				foreach(NumberInfo n3 in numsToAdd){
					AddNumberToBlob(n3);
//					Debug.Log("adding;"+n3);
				}
				return; // after a dconstruction don't add anything to blob, add all the deconstructed pieces individualy in the for loop above.
			}
		}


		eatingNumbers.Add(ni);
		ni.GetComponent<Collider>().enabled = false;



		ni.transform.parent = transform;
		ni.InitSinGrowAttributes(0.5f,true);
		Rigidbody rb = ni.GetComponent<Rigidbody>();
		if (ni.GetComponent<PickUppableObject>()){
			Destroy(ni.gameObject.GetComponent<PickUppableObject>());
		}
		if (!rb) rb = ni.gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.drag = 3f;
		ni.gameObject.AddComponent<BlobNumber>();
		rb.useGravity = false;




//		if ( blobNumbers.Add(ni);
	}

	void EatFX(NumberInfo ni){
		AudioManager.inst.PlayCartoonEatLow(ni.transform.position,1,1);
		StartCoroutine(EatSoundAfterSeconds(ni,.3f));
	}
	IEnumerator EatSoundAfterSeconds( NumberInfo ni, float s){
		yield return new WaitForSeconds(s);
		if (ni){
			AudioManager.inst.PlayPlungerSuck(transform.position,0.5f,1,.7f);
			StartCoroutine(FxAfterSeconds(ni,0.35f));

		}
	}

	IEnumerator FxAfterSeconds(NumberInfo ni, float s){
		yield return new WaitForSeconds(s);
		if (ni){
			EffectsManager.inst.CreateSmallPurpleExplosion(ni.transform.position,1,1);
		}
		CheckZeros(); //Now that we finished eating, do any of our (primes only) belly contents sum to zero?
	}
	void CheckZeros(){
		List<NumberInfo> checks = new List<NumberInfo>();
		List<NumberInfo> toRemove = new List<NumberInfo>();
		bool brk = false; // break the cycle?
		foreach(NumberInfo ni in blobNumbers){
			if (brk) break;
			//				Debug.Log("ate;"+a);
			foreach(NumberInfo a in checks){
				if (brk) break;
				if (ni != a){
//					Debug.Log("Checking :"+ni.fraction+" against "+a.fraction);
					int factorsCount = Fraction.GetAbsoluteValue(ni.fraction).GetFactors().Count;
					Fraction result = Fraction.Add(ni.fraction,a.fraction);
					if (result.numerator == 0 && factorsCount <= 2){
//						Debug.Log("result zero, "+ni.fraction+","+a.fraction);
						ni.ZeroFX(ni.transform.position);
						a.ZeroFX(a.transform.position);
						NumberManager.inst.DestroyOrPool(ni);
						Destroy(a.gameObject);
						toRemove.Add(ni);
						toRemove.Add(a);
						brk = true;
					} else {
						//						Debug.Log("fail zero, "+ni.fraction+","+a.fraction+", result;"+result.numerator+"/"+result.denominator);
					}
				}

			}
			checks.Add(ni);
		}
		RemoveSnailNumbers(toRemove);
	}

	void RemoveSnailNumbers(List<NumberInfo> toRemove){
		foreach(NumberInfo ni in toRemove){
			if (blobNumbers.Contains(ni)){
				blobNumbers.Remove(ni);
			}
		}
	}


	void NumberDestroyed(NumberInfo ni){
		ni.numberDestroyedDelegate -= NumberDestroyed;
		ni.numberChangedDelegate -= HandleNumberChanged;
		if (blobNumbers.Contains(ni)) {
			blobNumbers.Remove(ni);
		}
	}

	public override void OnDestroy(){
		base.OnDestroy();
		GadgetThrow.inst.onPlayerThrow -= PlayerThrew;
	}


	bool needDestroyLr = false;
	void LateUpdate(){
		if (LevelBuilder.inst.levelBuilderIsShowing && destinations.Length > 0 && destinations[0] != null){
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
}
