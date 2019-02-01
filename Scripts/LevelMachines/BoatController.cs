using UnityEngine;
using System.Collections;

public class BoatController : Vehicle {


	float rotationSpeed = 1.0f;
	public float inputForce = 6000;
	public ParticleSystem wake;
	float particleRate = 2f;
	public Transform boatGraphics;
	public Vector3 boatGraphicsStartLocalRot;
	float minSpeedForParticles = 4;
	public AnimationCurve speedCurve;

//	float downForce = 120f;

	// Update is called once per frame

	public float downforce = 5000f;


//		return "Vehicle - Boat";

	override public void VehicleStopSound(){
		if (!LevelBuilder.inst.levelBuilderIsShowing){
			AudioManager.inst.PlayVehicleBoatStop();
		}
	}

	public override void Start(){
		base.Start();

		boatGraphicsStartLocalRot = boatGraphics.transform.localRotation.eulerAngles;

	}

	float outOfWaterTimer = 0;
	float slideBackToWaterAfterSeconds =3;

	Vector3 a = Vector3.zero;
	Vector3 b = Vector3.zero;
	void OnDrawGizmos(){
//		Gizmos.DrawLine(a,b);
	}

	float delta = 0; // inverted height above water clamped from 0-1. used for controlling wake size and aud volume.
	float outOfWaterTimerInterval =0f;
	float boatRunningTimer = 0;
	bool boatRunningSound = false;
	float pitch = 0;
	override public void Update () {
		if (GameManager.inst.GameFrozen) return;
		base.Update();
		if (canControl && energyNumber != null){

			// handle sound
			if (boatRunningSound){
				boatRunningTimer -= Time.deltaTime;
				if (boatRunningTimer < 0){
					float deltaPos = Vector3.Magnitude(rp.nowPosition - rp.lastPosition);
					pitch = lowestPitch + deltaPos/4f;
					boatRunningTimer = AudioManager.inst.vehicleBoatRunning.length / pitch / 1.5f;
					//					// commented Debug.Log("pitch:"+pitch);
					AudioManager.inst.PlayVehicleBoatRunning(pitch);
				}
			}
		}


//		if (transform.position.y > utc.getHeightByPos(transform.position)){
//			float distanceAboveWaterSurface = transform.position.y - utc.getHeightByPos(transform.position);
////			// commented Debug.Log("dist;"+distanceAboveWaterSurface);
//			RaycastHit hit;
//			Ray ray = new Ray(transform.position+Vector3.up*5,Vector3.down);
//			if (Physics.Raycast(ray,out hit,distanceAboveWaterSurface,~LayerMask.NameToLayer("Terrain"))){
////				// commented Debug.Log("hit:"+hit.collider.name);
////				if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")
//				// We hit the terrain at a distance SHORTER than the distance between boat and water surface, meaning we're on dry land. Don't fall.
//			} else {
////				// commented Debug.Log("fall");
//				// fall.
//
//				float fallSpeed = 15;
//				GetComponent<Rigidbody>().AddForce(Vector3.down * fallSpeed,ForceMode.Force); // transform.position -= Vector3.up * Time.deltaTime * fallSpeed;
//				wake.emissionRate = 0; 	
////				// commented Debug.Log("falling!");
////				return;
//			}
//		}

	}
	float boatSpeed = 1000f;
	void FixedUpdate(){
		if (canControl && energyNumber != null){
			if (InWater() || outOfWaterTimer < 1f){
				/*
				 * boat speed
				 * x to what power yields 200 if x=10 and 4000 if x =10,000
				 * or, 10^x = 200 and 10,000^x = 4000
					200 should be with 10
					500 with 100
					1500 with 500
					4000 with 10,000
				*/
				float xAxisScale = 500f; // the max number you should reach
				float yAxisScale = 3000f;
				float boatSpeed = speedCurve.Evaluate(energyFrac.GetAsFloat()/xAxisScale)* yAxisScale; 
				// animation curve is defined in the inspector.
//				Debug.Log("boatpseed:"+boatSpeed+" with enfracflot;"+energyFrac.GetAsFloat());
				// the x axis, or animation time, is where the number value will fall, for example if boat's number value is 5000, it falls at 0.5
				// the y axis is how fast we want the boat to go for that x value.

				float forwardForce = Input.GetAxis("Vertical");
//				// commented Debug.Log("?");

//				// commented Debug.Log("boatspeed = "+boatSpeed);

				Vector3 f = Vector3.Normalize(new Vector3(transform.forward.x,0,transform.forward.z)) * forwardForce * boatSpeed;
//				f = new Vector3(f.x,0,f.y); // normalized to be flat
//				Debug.Log("moving!");
				GetComponent<Rigidbody>().AddForce(f,ForceMode.Force);


				float mag = GetComponent<Rigidbody>().velocity.magnitude;
				if (wake){
					// Wake FX including emission rate, size, and volume of motor

					float emRate = mag * particleRate;
					if (GetComponent<Rigidbody>().velocity.magnitude > minSpeedForParticles){
						wake.emissionRate = emRate;
						delta = transform.position.y - UltimateToonWaterC.inst.getHeightByPos(new Vector2(transform.position.x,transform.position.z));
						// delta oscilates between -1 and 1, approx. To normalize add 1.
						delta += 1;
						delta = Mathf.Clamp(delta,0,2f);
						delta /= 2f; // gives a value between 0-1
						delta = 1 - delta; // flip, so that 1 yields biggest wake
						wake.startSize = 20 + Mathf.Pow(delta,10) * 40;
					} else {
						wake.emissionRate = 0; 	
					}
				}
			} else {
				if (wake) wake.emissionRate = 0;

			}

			if (outOfWaterTimer > 0){
//				Debug.Log("adding ");
				GetComponent<Rigidbody>().AddForce(Vector3.down * downforce * Time.deltaTime);
			}
			if (outOfWaterTimer > 20){
				
				PlayerNowMessageWithBox.inst.Display("Your boat doesn't work out of water! Press F to exit boat, or TAB to restart.",icon,transform.position);
					
			}



		}


		// We want volume between .3 and 1.
		float minVolume = 0.3f;
		float maxVolume = 1f;
		aud.volume = minVolume + delta * (maxVolume - minVolume);
//		// commented Debug.Log("vol:"+aud.volume);


		// Rotation fx with accel
		float effectiveSpeed = Vector3.Distance(rp.nowPosition,rp.lastPosition);
		float rotForce = effectiveSpeed * -1000f;
		GetComponent<Rigidbody>().AddTorque(transform.right * rotForce);


		if (!InWater() && !LevelBuilder.inst.levelBuilderIsShowing){
			outOfWaterTimer += Time.deltaTime;
		} else {
			outOfWaterTimer = 0;
		}

		if (outOfWaterTimer > slideBackToWaterAfterSeconds){
			outOfWaterTimerInterval -= Time.deltaTime;
			if (outOfWaterTimerInterval < 0){
				int frameSkip = 10;
				outOfWaterTimerInterval = Time.deltaTime * frameSkip;
				RaycastHit hit;
				Ray ray = new Ray(transform.position + Vector3.up * 10,Vector3.down);
				if (Physics.Raycast(ray,out hit,Mathf.Infinity,1 << 9 )){ //SceneLayerMasks.inst.terrainOnly)){
	//				// commented Debug.Log("hit layer:"+LayerMask.LayerToName(hit.collider.gameObject.layer));
					Vector3 moveDir = hit.normal.normalized;
	//				Gizmos.DrawLine(hit.point,moveDir * 10);
	//				a = hit.point;
	//				b = hit.point + moveDir * 10;
					moveDir = new Vector3(moveDir.x,moveDir.y/10f,moveDir.z);
					float backToWaterSpeed = 750;
					moveDir *= backToWaterSpeed;
//					Debug.Log("moving!");
					GetComponent<Rigidbody>().AddForce(moveDir * frameSkip);
	//				// commented Debug.Log("slide to water:"+moveDir + " while hit;"+hit.collider.name);
				}
			}
		}

//		em.rate = new ParticleSystem.MinMaxCurve();
//		wake.emission.rate = em.rate;

		Quaternion rot = transform.rotation;
		rot.eulerAngles = new Vector3(0,rot.eulerAngles.y,0);
		float fixSpeed = 1;
		transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * fixSpeed);
	}



	bool InWater(){
		bool flag = (transform.position.y < 42 && transform.position.y > 0);
//		Debug.Log("in water?"+flag+", posy:"+transform.position.y);
		return flag;

	}


	void LateUpdate(){
		if (canControl){
			if (Input.GetAxis("Horizontal") != 0) {
				transform.Rotate(Vector3.up,Input.GetAxis("Horizontal") * rotationSpeed * 1);
			} 
			float sensitivityX = 0.5f;
			if (Input.GetAxis("Mouse X") != 0){
				transform.Rotate(Vector3.up,Input.GetAxis("Mouse X") * rotationSpeed * sensitivityX);
			}
		}
	}

	public override void EnergyNumberDestroyed(GameObject source){
		base.EnergyNumberDestroyed(source);
		wake.emissionRate = 0;
	}

	override public void HandleAudio(){
		if (CanMove()){
			AudioManager.inst.PlayVehicleBoatStart();
			boatRunningSound = true;
			boatRunningTimer = 0.85f;

		} else {
			boatRunningSound = false;
			AudioManager.inst.PlayVehicleBoatStop();
		}
	}
	public override void PlayerGetOutOfVehicle(){
		base.PlayerGetOutOfVehicle();
		// If player gets out of a boat near a dock, place player on the dock.
		float maxDistToTeleportToDock = 50f; 
		float d1 = Mathf.Infinity;
		BoatDockLocation locationToTeleport = null;
		if (!LevelBuilder.inst.levelBuilderIsShowing){
			foreach(BoatDockLocation bdl in FindObjectsOfType<BoatDockLocation>()){
				float d2 = Vector3.Magnitude(transform.position - bdl.transform.position);
				Vector3 dockDirFromPlayer = bdl.transform.position - Player.inst.transform.position;
				if (d2 < maxDistToTeleportToDock) { // && Vector3.Angle(Player.inst.transform.forward,dockDirFromPlayer) < 50){
					if (d2 < d1){
						locationToTeleport = bdl;	
						d1 = d2;
					}
				}
			}
//			foreach(Boat
		}
		if (locationToTeleport != null) {
//			// commented Debug.Log("teleporting to:"+locationToTeleport);
			Player.inst.SetPosition(locationToTeleport.transform,true);// Player.inst.transform.position = locationToTeleport.transform.position;

			FPSInputController.inst.motor.inputMoveDirection = Vector3.zero; // kill momentum.
		} else {
			MovePlayerToGetOutOfVehiclePosition();
		}
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		wake.emissionRate = 0;

	}

}
