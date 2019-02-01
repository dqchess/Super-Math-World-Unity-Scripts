using UnityEngine;
using System.Collections;

public class Animal_Fish : Animal {



	#region UserEditable 


	// upoffset 	}


	/* footpring was: (){
		return 4;
	 */
	public override void OnLevelBuilderObjectPlaced(){
		GetComponent<Rigidbody>().isKinematic = true;
	}
		
	public override void OnGameStarted(){
		base.OnGameStarted();
		GetComponent<Rigidbody>().isKinematic = false;
		Init();

	}



	#endregion

	public Transform tailPivot;
	public Transform bubblesT;


	// Use this for initialization


	void Init(){
		base.Init();
		SetNumber(GetComponent<NumberInfo>().fraction);
	}





	// Update is called once per frame
	public override void AnimalUpdate () {
		
		if (animalDisabled) {
//			Debug.Log("i'm disabled!");
			return;
		}
//		if (sleeping) return;
		CheckUnderwater();
		if (!underwater) {
//			TryFaceWater();
//			// commented Debug.Log ("fish out of water!");
			FlipAndFlop();
			return;
		}
		TailSwim ();
		Bubbles();
		MoveForwards();
		if (GetComponent<Rigidbody>()){
			//			// commented Debug.Log ("force this frame:"+forceThisFrame);
			GetComponent<Rigidbody>().AddForce(forceThisFrame,ForceMode.Acceleration);
		}
		forceThisFrame = Vector3.zero;
//		base.AnimalUpdate();
//		CheckCloseToTargetAndEat();
	}

	float flopTimer = 3f;
	void FlipAndFlop(){
		flopTimer -= Time.deltaTime;
		if (flopTimer < 0){
			Transform t  = Utils.GetClosestObjectOfType<WaterCube>(transform);
			Vector3 helperDir = t != null ? t.position - transform.position : Vector3.zero;
			flopTimer = Random.Range(0.6f,0.7f);
			float flopForce = Random.Range(2500f,5000f);
			float rotFlop = Random.Range(-3f,3f);
			if (GetComponent<Rigidbody>()){
				GetComponent<Rigidbody>().AddForce((Vector3.up + helperDir.normalized/9f) * flopForce );
				GetComponent<Rigidbody>().AddRelativeTorque(transform.right * rotFlop,ForceMode.Impulse);
			}
		}
	}


	public override void IdleRotate(){
		Debug.Log("idly");
		Quaternion rot = new Quaternion();
		rot.eulerAngles = new Vector3(Random.Range(-15f,15f),Random.Range(0,360),0);
		targetRot = rot;
	}

//	public override void Update(){
//		base.Update();
//	}


	void FishGotInTheWater(Transform t){
		transform.LookAt(t);
		if (GetComponent<Rigidbody>()){
			GetComponent<Rigidbody>().velocity = Vector3.zero; // don't fall too fast into water..
		}
	}
	void FishOutOfWater(){
		// The fish just transferred from an in water to an out of water state.
		Transform t = Utils.GetClosestObjectOfType<WaterCube>(transform);
		if (t) {
			flopTimer = 1.5f;
			// first, kick *away* from the water cube.
			if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().AddForce((transform.position-t.position).normalized * 5000f);
			// then LOOK at the cube
//			Debug.Log("rot old:"+transform.rotation.eulerAngles+" to look at:"+t);
			transform.RotateAround(transform.position,transform.forward,90 * Random.value > 0.5f ? 1 : -1);
//			transform.right = Vector3.up;
//			transform.LookAt(t);
//			transform.rotation = Quaternion.LookRotation(t.position-transform.position);
//			transform.LookAt(t.position);
//			Debug.Log("rot:"+transform.rotation.eulerAngles+" to look at:"+t);

			// now lie sideways
		}
	}

	void CheckUnderwater(){
		if (Utils.IntervalElapsed(1f)){
			WaterCube wc = Utils.IsInsideWaterCube(this.transform);
			bool newUnderwater = (transform.position.y < GameConfig.globalWaterHeight) || wc; 
			if (underwater == true && newUnderwater == false){
				FishOutOfWater();
			} else if (underwater == false && newUnderwater == true){
				FishGotInTheWater(wc.transform);
			}
			underwater = newUnderwater;
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb) {
				rb.useGravity = !underwater;
			}
//			if (underwater) {Debug.Log("underwater");} else { Debug.Log("not");}
		}
	

	}

	float tryWaterTime = 0;
	void TryFaceWater(){
		tryWaterTime -= Time.deltaTime;
		if (tryWaterTime < 0){
			tryWaterTime = 1;
			Quaternion rot = transform.rotation;
			rot.eulerAngles += new Vector3(65,0,0);
			targetRot = rot;
		}
	}
//			RaycastHit hit;
//			if (Physics.Raycast(transform.position,transform.forward,out hit,30)){
////			if (Physics.Raycast(transform.position,transform.forward,out hit,20,LayerMask.NameToLayer("Terrain"))){
//				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
//					AboutFace(); 
//				}
//				else // commented Debug.Log ("hit insteadl"+hit.collider.name);
//
////				transform.rotation *= Vector3.up * 0.5f; // spin around
//			} else {
//				// commented Debug.Log ("hit nothing 30 away");
//			}
//			transform.rotation = rot;

	void AboutFace(){
		Debug.Log("about!");
		Quaternion rot = transform.rotation;
		rot.eulerAngles += new Vector3(0,180,0);
		rot.eulerAngles = new Vector3(0,rot.eulerAngles.y,rot.eulerAngles.z);
		transform.rotation = rot;
		targetRot = rot;
	}

	void TailSwim(){
//		// commented Debug.Log("tail swim:");
//		float speed = Vector3.Magnitude(rp.lastPosition - rp.nowPosition);
		float speed = 5;
		float amplitude = 5;
		float r =  Mathf.Sin (Time.time * speed) * amplitude;

		Quaternion rot = tailPivot.localRotation;
		rot.eulerAngles = new Vector3(0,r - 2.5f,0);
		tailPivot.localRotation = rot;
	}

	float bubblesInterval = 3;
	float bubblesTimer = 0;
	void Bubbles(){
		bubblesTimer -= Time.deltaTime;
		if (bubblesTimer < 0){
			EffectsManager.inst.Bubbles(bubblesT.position);
		}

	}

	public override void SetUnderwater(bool b){
		base.SetUnderwater(b);
		if (underwater) GetComponent<Rigidbody>().useGravity = false;
		else GetComponent<Rigidbody>().useGravity = true;
	}


	public override bool AttemptToSetTarget(Transform t){
		if (!underwater) return false;
		return base.AttemptToSetTarget(t);
	}

	void OnCollisionEnter(Collision hit){
//		Debug.Log("fish hit terrain");
		if (hit.gameObject.layer == LayerMask.NameToLayer("Terrain")){
			// If we hit terrain and it wasn't directly below us, about face.
			Vector3 dirToHit = hit.contacts[0].point - transform.position;
			if (Vector3.Angle(dirToHit.normalized,transform.forward) < 30){
//				AboutFace();
			}
		}
	}

}
