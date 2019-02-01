using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CharacterController))]
public class SMW_FPSWE: MonoBehaviour {

	public bool restrictZaxis = false;
	public static SMW_FPSWE inst;
	// Personalized delegate with personalized parameters
	public delegate void EventHandler(Vector3 moveDirection);

	// Event to be triggered using EventHandler parameters
	public event EventHandler Moved;

	public bool isControllable = false;
	public Transform activePlatform;
	Vector3 activeLocalPlatformPoint;
	Vector3 activeGlobalPlatformPoint;
	Quaternion activeLocalPlatformRotation;
	Quaternion activeGlobalPlatformRotation;
	public float walkSpeed = 6.0f;
	public float runSpeed = 9.0f;
	public bool weightless = false;

	// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
	public bool limitDiagonalSpeed = true;
	MascotAnimatorController ms;
	// If checked, the run key toggles between running and walking. Otherwise player runs if the key is held down and walks otherwise
	// There must be a button set up in the Input Manager called "Run"
	//    public bool toggleRun = false;

	public bool underwater = false;
	float underwaterJumpSpeed = 10f;
	public float jumpSpeed = 25f;
	float jumpSpeedAtStart =25f;
	Vector3 underwaterGravity = new Vector3(0,10,0);
	Vector3 gravity = new Vector3(0,120.0f,0);
	Vector3 gravityAtStart=new Vector3(0,120,0);

	// Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
	public float fallingDamageThreshold = 10.0f;

	// If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
	public bool slideWhenOverSlopeLimit = false;

	// If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
	public bool slideOnTaggedObjects = false;

	public float slideSpeed = 32.0f;

	// If checked, then the player can change direction while in the air
	public bool airControl = false;
	private float airControlFactor = 1f;

	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;

	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 0;

	public Vector3 moveDirection = Vector3.zero;
	public bool grounded = false;
	public CharacterController controller;
	private Transform myTransform;
	public float speed;
	private RaycastHit hit;
	//	private Collider lastColliderHit;
	//	private bool isStandingOnElevator = false;
	private float fallStartLevel;
	private bool falling;
	private float slideLimit;
	private float rayDistance;
	private Vector3 contactPoint;
	private bool playerControl = false;
	private int jumpTimer;

	public bool dying = false;
	bool playerNeedsResetting = false;
	public float dieTimer = 0.0f;

	public float axisHorizontal = 0.0f;
	public float axisVertical = 0.0f;

	public float walkAccel = 8.0f;

	public GameObject characterArt;


	public bool touchAxes = false;
	public float touchAxisHorizontal;
	public float touchAxisVertical;
	public bool touchJump = false;

	// fix tripping / getting stuck issue
	float stepHeight = 1;
	float deltaStep = 0.3f;
	private bool toggle = false;

	void Start() {
		inst = this;
		contactPoint = transform.position; // prevents sliding on game start
		ms = MascotAnimatorController.inst;
		jumpSpeedAtStart = jumpSpeed;
		gravityAtStart = gravity;
		controller = GetComponent<CharacterController>();
		myTransform = transform;
		speed = walkSpeed;
		rayDistance = controller.height * .5f + controller.radius;
		slideLimit = controller.slopeLimit - .1f;
		jumpTimer = antiBunnyHopFactor;
		#if UNITY_IPHONE && !UNITY_EDITOR
		touchAxes = true;
		#else
		touchAxes = false;
		#endif


	}

	public bool PlayerInsideEnemyZone(){
		Collider[] cols = Physics.OverlapSphere(transform.position,.1f);
		foreach (Collider col in cols){
			// ugh horrible code but CheckSphere always returns true like a motherfucker
			if  (col.gameObject.layer == LayerMask.NameToLayer("EnemyZone")){
				//					// commented Debug.Log("inside "+col.name);
				return true;
			}
		} 
		return false;
	}

	// CVN 
	// For returning the player's velocity so that we can match projectile velocity to player velocity.
	public Vector3 deltaPos=Vector3.zero;
	Vector3 lastPos=Vector3.zero;
	float stopSlidingTimer = 0;
	float slideTimer = 0;
	Vector3 lastSlidingPosition;
	//	bool specialSliding = true;
	int helpCount = 0; // sometimes we get stuck on edges of boxy mesh colliders due to weird normals, and cna't move! We try to bounce ourselves out 3 times and if that fails, we "help" player by asking them to restart. #facepalm
	void FixedUpdate() {
		bool allowSliding = true;
		if (Player.frozen) {
			return;
		}
		if(dying) {
			return;	
		}


		float gravityDecayFactor = momentum.magnitude > maxMomentumWhenGrounded ? .3f : grounded && ignoreGroundedForModMomentumForSeconds < 0 ? .3f :  1f;
		Vector3 targetGravity = underwater ? underwaterGravity : gravityAtStart;
		//		if (debug) Debug.Log("gravity decay:"+gravityDecayFactor+", grav:"+gravity.y+", target;"+targetGravity.y);
		gravity = Vector3.Lerp(gravity,targetGravity,Time.deltaTime * gravityDecayFactor);


		deltaPos = transform.position - lastPos;
		lastPos = transform.position; // end CVN


		//float inputX = Input.GetAxis("Horizontal");
		//float inputY = Input.GetAxis("Vertical");

		//		float inputXraw = (Input.GetKey(GlobalInput.bindings["Walk Left"]) ? -1.0f : 0.0f) + 
		//			(Input.GetKey(GlobalInput.bindings["Walk Right"]) ? 1.0f : 0.0f);
		float inputXraw = 0;
		float inputYraw = 0;
		if (restrictZaxis == false){
			inputXraw = Input.GetAxis("Horizontal");
			inputYraw = Input.GetAxis("Vertical");
		} else {
			inputXraw = 0f;
			inputYraw = Input.GetAxis("Horizontal");
		}
		//		// commented Debug.Log("input x raw:"+inputXraw);
		//		float inputYraw = (Input.GetKey(GlobalInput.bindings["Walk Forward"]) ? 1.0f : 0.0f) + 
		//			(Input.GetKey(GlobalInput.bindings["Walk Backwards"]) ? -1.0f : 0.0f);

		if (touchAxes) {
			inputXraw = touchAxisHorizontal;
			inputYraw = touchAxisVertical;
		}		

		if(inputXraw > axisHorizontal) { axisHorizontal = Mathf.Min(axisHorizontal + walkAccel * Time.deltaTime, 1.0f); }
		if(inputXraw < axisHorizontal) { axisHorizontal = Mathf.Max(axisHorizontal - walkAccel * Time.deltaTime, -1.0f); }
		if(Mathf.Abs(axisHorizontal) < walkAccel * Time.deltaTime && inputXraw == 0) { axisHorizontal = 0.0f; }
		float inputX = axisHorizontal;

		if(inputYraw > axisVertical) { axisVertical = Mathf.Min(axisVertical + walkAccel * Time.deltaTime, 1.0f); }
		if(inputYraw < axisVertical) { axisVertical = Mathf.Max(axisVertical - walkAccel * Time.deltaTime, -1.0f); }
		if(Mathf.Abs(axisVertical) < walkAccel * Time.deltaTime && inputYraw == 0) { axisVertical = 0.0f; }
		float inputY = axisVertical;

		// If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;

		bool slidingThisFrame = false;
		if (grounded) {
			if (stopSlidingTimer < 0) {
				SetSliding(false);
			} else {

			}
			// See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
			// because that interferes with step climbing amongst other annoyances
			//			controller.hit
			//			specialSliding = false;
			int hitIndex = -1;
			foreach(RaycastHit hit2 in Physics.RaycastAll(myTransform.position, -Vector3.up, rayDistance)){
				hitIndex ++;



				// This block I think was doing 4 raycasts from corenrs to prevent sliding when player was on the "Edge" of a platform
				//				lastColliderHit = hit.collider;
				//				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
				// Hang on a second. Let's make sure to check for "gaps" in the ground that we shouldn't actually be trying to slide around.
				//				Vector3[] ps2 = new Vector3[4] { 
				//					myTransform.position + Vector3.right * .5f,
				//					myTransform.position + Vector3.forward * -.5f,
				//					myTransform.position + Vector3.forward * .5f,
				//					myTransform.position + Vector3.right * -.5f 
				//				};
				//				//			// commented Debug.Log("mytran;"+myTransform.position+"ps:"+(ps[0])+(ps[1])+(ps[2])+(ps[3]));
				//				float highest = -1000f;
				//				float lowest = 10000f;
				//				string hits = "hit:";
				//				foreach(Vector3 pp2 in ps2){
				//					RaycastHit hitt;
				////					// commented Debug.Log("allow;"+allowSliding);
				//					if (Physics.Raycast(pp2, -Vector3.up, out hitt, rayDistance)) {
				//						hits+=Mathf.Round(hit.point.y*10)/10f+", ";
				//						// if any of the 4 checks around my character fail for sliding, don't slide. We can stand there
				//						//(for example we are standing on a hole, but the hole is small so we can stand on the edge of the hole (or gap between planks). 
				//						// These raycasts check for 4 positions around where we are standing for these type cases.
				////						if (hitt.point.y > highest) highest = hitt.point.y;
				////						if (hitt.point.y < lowest) lowest = hitt.point.y;
				//						if (Vector3.Angle(hitt.normal, Vector3.up) < slideLimit && !hitt.collider.isTrigger){ // || hit.collider.name.ToLower().Contains("pipe")) {
				////							Debug.Log("noslide 1");
				//							allowSliding = false;
				//
				////							GameObject dbs = Utils.DebugSphere(hitt.point);
				////							dbs.transform.localScale = Vector3.one * .3f;
				////							TimedObjectDestructor tod = dbs.AddComponent<TimedObjectDestructor>();
				////							tod.DestroyNow(2);
				//
				//						} else {
				//							EditorTesting.inst.DebugArrow(hit.point,hit.normal,Color.black);
				////							hits += pp2 + ",";
				//						}
				//
				//					}
				//				}
				//				Debug.Log(hits);






				//				Debug.Log("hi:"+Mathf.Round(highest)+", low:"+Mathf.Round(lowest)+"; delta;"+Mathf.Round(highest-lowest));
				//				EditorTesting.inst.DebugArrow(hit2.point,hit2.normal,Color.red);
				Vector3 dirFromHitToPlayer = Player.inst.transform.position - hit2.point;
				float angleBetweenNormalAndPlayerDir = Vector3.Angle(hit2.normal,dirFromHitToPlayer);
				bool hitAngleGood = angleBetweenNormalAndPlayerDir<45;
				float normAngle = Vector3.Angle(hit2.normal, Vector3.up);

				if (
					(normAngle > slideLimit && allowSliding && hitAngleGood)

				) {
					if (hit2.collider.transform.root.name.Contains("pipe")){
						Debug.Log("pipeslide");
	//					Debug.Log("hit2 coll with normang:"+normAngle+", anglebtw norm and playdri:"+angleBetweenNormalAndPlayerDir);
						slidingThisFrame = true;
						Vector3 hitNormal = hit2.normal;

						moveDirection = new Vector3(hitNormal.x, -1 * Mathf.Abs(hitNormal.y), hitNormal.z);
						EditorTesting.inst.DebugArrow(hit2.point,hit2.normal,Color.red);
						//					Debug.Log("movedirection:"+moveDirection);
						//					Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
						Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
					}
				} 

//				break;		
			}
			// However, just raycasting straight down from the center can fail when on steep slopes
			// So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
			if (!slidingThisFrame) {
				float maxDist = 3;
				Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit,maxDist);

				// Hang on a second. Let's make sure to check for "gaps" in the ground that we shouldn't actually be trying to slide around.
				Vector3[] ps = new Vector3[4] { 
					contactPoint + Vector3.right * .5f,
					contactPoint + Vector3.forward * -.5f,
					contactPoint + Vector3.forward * .5f,
					contactPoint + Vector3.right * -.5f 
				};
				//			// commented Debug.Log("mytran;"+myTransform.position+"ps:"+(ps[0])+(ps[1])+(ps[2])+(ps[3]));
				foreach(Vector3 pp in ps){
					RaycastHit hitt;
					//					string hits = "hit:";
					if (Physics.Raycast(pp, -Vector3.up, out hitt,maxDist)) {

						// if any of the 4 checks around my character fail for sliding, don't slide. We can stand there
						//(for example we are standing on a hole, but the hole is small so we can stand on the edge of the hole (or gap between planks). 
						// These raycasts check for 4 positions around where we are standing for these type cases.
						float ang = Vector3.Angle(hitt.normal, Vector3.up);
						//						Debug.Log("ang;"+ang);
						if (ang < slideLimit && !hitt.collider.isTrigger || hitt.collider.gameObject.layer != LayerMask.NameToLayer("Terrain")) {
							//							Debug.Log("noslide 2, ang:"+ang+", slidelim:"+slideLimit);
							allowSliding = false;
						} else{
							//							EditorTesting.inst.DebugArrow(hit.point,hit.normal,Color.white);
							//							Debug.Log("noslide 2, ang:"+ang+", slidelim:"+slideLimit);
							//							hits += pp + ",";
						}
						//						// commented Debug.Log(hits+","+allowSliding);
					}
				}
				Vector3 dirFromHitToPlayer = Player.inst.transform.position - hit.point;
				float angleBetweenNormalAndPlayerDir = Vector3.Angle(hit.normal,dirFromHitToPlayer);
//				Debug.Log("ang :"+angleBetweenNormalAndPlayerDir);
				if (angleBetweenNormalAndPlayerDir < 90){
					
					if (hit.collider && allowSliding && Vector3.Angle(hit.normal, Vector3.up) > slideLimit){
						//				if (allowSliding && Vector3.Angle(hit.normal, Vector3.up) > slideLimit && hitAngleGood && hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")){
						//					// commented Debug.Log ("sliding true 2");
						//					EditorTesting.inst.DebugArrow(hit.point,hit.normal,Color.green);
						//					// commented Debug.Log("slide true 3");
//						Debug.Log("2");
						slidingThisFrame = true;
					} else {
						//					// commented Debug.Log("not sliding");
						
					}
				}
			} 


			SetSliding(slidingThisFrame);


			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling) {
				falling = false;
			}

			PreventStickingDuringSlide();




			// If running isn't on a toggle, then use the appropriate speed depending on whether the run button is down
			//            if (!toggleRun)
			//                speed = Input.GetButton("Run")? runSpeed : walkSpeed;

			// If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
			if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
				//				if (specialSliding){
				if (false) {
					//					Debug.Log("special.");
					moveDirection = Vector3.Normalize(transform.position - contactPoint);
				} else  {

					Vector3 hitNormal = hit.normal;
//					EditorTesting.inst.DebugArrow(hit.point,hit.normal,Color.yellow);
					moveDirection = new Vector3(hitNormal.x, -1 * Mathf.Abs(hitNormal.y), hitNormal.z);
//					EditorTesting.inst.DebugArrow(hit.point,moveDirection,Color.cyan);
//					Debug.Log("movedirection:"+moveDirection);
//					Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
//					Debug.Log("movedirection2:"+moveDirection);

				}
				moveDirection *= slideSpeed;
				playerControl = false;
				PlayerWalkDustParticles.inst.Emit (.5f);

				// Slide dust particles FX:

			}
			// Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
			else {
				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				//				if (toggleRun) speed = runSpeed;
				//				else speed=walkSpeed;
				moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;
			}

			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (!jumpPressed && !touchJump)
				//            if (!Input.GetKey(GlobalInput.bindings["Jump"]) && !touchJump)
				jumpTimer++;
			else if (jumpTimer >= antiBunnyHopFactor) {
				JumpNow();
				//				// commented Debug.Log ("tried to jump. Really.");

			}
		}
		else {
			if (underwater && !jumpingUnderwater){
				// Underwater jumping handled here
				//				moveDirection.x = inputX * speed;
				//				moveDirection = myTransform.TransformDirection(moveDirection);
				//				moveDirection = Camera.main.transform.forward * speed * inputY;
				float yFactor = Camera.main.transform.forward.y;
				moveDirection = new Vector3(inputX * inputModifyFactor, yFactor, inputY * inputModifyFactor);
				//				moveDirection = myTransform.TransformDirection(moveDirection) * speed;

				//				moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);
				//				if (toggleRun) speed = runSpeed;
				//				else speed=walkSpeed;
				moveDirection = myTransform.TransformDirection(moveDirection) * speed;
				playerControl = true;


				if (!jumpPressed && !touchJump) {
					//            if (!Input.GetKey(GlobalInput.bindings["Jump"]) && !touchJump)
					jumpTimer++;
					//					// commented Debug.Log ("jumptimer:"+jumpTimer);
				} else if (jumpTimer >= antiBunnyHopFactor) {
					//					// commented Debug.Log ("underwater jump.");
					JumpNow();
				}

				//				moveDirection -= gravity * Time.deltaTime;
				//				return;
			} else {
				// If we stepped over a cliff or something, set the height at which we started falling
				if (!falling) {
					falling = true;
					fallStartLevel = myTransform.position.y;
				}

				// If air control is allowed, check movement but don't touch the y component
				if ((airControl || SMW_CHEATS.inst.cheatsEnabled) && playerControl) {
					moveDirection.x = inputX * speed * inputModifyFactor * airControlFactor;
					moveDirection.z = inputY * speed * inputModifyFactor * airControlFactor;
					moveDirection = myTransform.TransformDirection(moveDirection);
				}
			}
		}
		if(touchJump) { touchJump = false; }
		// Apply gravity
		ApplyGravity();
		ApplyAnimations();




		// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
		// Do test the code! You usually need to change a few small bits.






		Vector3 newMoveDirection = moveDirection;
		if (activePlatform){
			//			 Debug.Log ("platformdiffy:" +platformDiff.y);
			if (platformDiff.y > 0){
				platformDiff += new Vector3(0,-platformDiff.y,0);
			}
			newMoveDirection = moveDirection + platformDiff/Time.deltaTime;

		}

		// Decay momentum
		float momentumDecayFactor = grounded ? 5f : 2f;
		//		if (debug) Debug.Log("momentumDecayFactor:"+momentumDecayFactor);
		momentum = Vector3.Lerp(momentum,Vector3.zero,Time.deltaTime * momentumDecayFactor);

		newMoveDirection += momentum;


		// Move the controller, and set grounded true or false depending on whether we're standing on something
		CollisionFlags colFlags = controller.Move(newMoveDirection * Time.deltaTime);

		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.worldDistanceMoved,Utils.FlattenVector(newMoveDirection).magnitude);
		//		RaycastHit groundHit;
		//		bool hitGround = false; 
		//		if (Physics.Raycast(transform.position,Vector3.down,out groundHit,1f)){
		//			hitGround =  groundHit.collider ? !groundHit.collider.isTrigger ? true : false : false;
		//		}
		//		if (hitGround) // commented Debug.Log("ground:"+groundHit.collider.name);
		if (isControllable) SetGrounded((colFlags & CollisionFlags.Below) != 0 );
		if((colFlags & CollisionFlags.Above) != 0) {
			moveDirection.y = Mathf.Min(moveDirection.y, 0);
		}

		// if gravity negative, grounded false
		if (gravity.y<0) SetGrounded(false);

		// CVN add grounded for being on the ceiling
		//		if (!grounded) {
		//			if (Physics.Raycast(myTransform.position, Vector3.up, out hit, rayDistance)) {
		//				//grounded=true;
		//			}
		//			// controller.Move(moveDirection * Time.deltaTime); // cvn - if you don't move cc here, jumping slows you down to a stop!
		//		}
		//		Debug.Log("movedir;"+moveDirection);
	}


	void ApplyAnimations(){
		float jumpDuration = 0.5f;
		//		if ((falling || sliding) && (Time.time > lastJumpTime + jumpDuration)) {
		if (sliding){
			ms.Sliding(true);
		} else {
			//			if (Time.time < lastJumpTime + jumpDuration){
			//				float d = Time.time - lastJumpTime;
			//				// commented Debug.Log ("lastjumptime:"+lastJumpTime+", delta;"+d);
			//			}
			ms.Sliding(false);
		}
	}

	void ApplyGravity(){
		if (Player.frozen) return;
		if (weightless) return;
		moveDirection -= gravity * Time.deltaTime;
	}

	public bool sliding=false;
	float retainSlideTimer = 0f;
	void SetSliding(bool flag){
		//		Debug.Log("Set sliding:"+flag);
		if (flag) stopSlidingTimer = 0.2f;
		if (flag) retainSlideTimer = 0.2f; // after beginning a slide don't allow out of the slide for a N seconds
		if (!flag && retainSlideTimer > 0){
			return;
		}
		if (sliding != flag){
			//erm
			//			Debug.Log("sliding not flag:"+sliding+","+flag);
			sliding = flag;

		}
	}
	bool jumpingUnderwater= false;
	float jumpingUnderwaterTimer=0;
	float lastJumpTime=0;
	public void JumpNow(){
		if (sliding && !allowJumpingWhenStuck) return;
		lastJumpTime = Time.time;
		jumpTimer = 0;
		if (underwater) {
			jumpingUnderwater = true;
			jumpingUnderwaterTimer = 0.5f;
			//			if (!Player.inst.gameObject.GetComponent<PlayerUnderwaterController>().PlayerCanSwimUp()) {
			////				MascotAnimatorController.inst.JumpStart();
			////				moveDirection += Vector3.up * .2f + transform.forward * 0.4f; // ;new Vector3(0,jumpSpeed * .2f, jumpSpeed * 7f);
			//				return;
			//			}
		} else {
			activePlatform = null;
			lastHit = null;
			AudioManager.inst.PlayJump();
		}

		MascotAnimatorController.inst.JumpStart();
		moveDirection.y = jumpSpeed;
		//		Debug.Log("movedir y:"+moveDirection.y);
		if (allowJumpingWhenStuck) {
			moveDirection += Player.inst.transform.forward * 2f;
			//			moveDirection.z += 1f;
		}
		//		// commented Debug.Log ("jumpstart");
	}

	float maxMomentumWhenGrounded = 3;
	//	float groundedSoundFxTimer = 0f;
	void SetGrounded(bool flag){
		if (grounded == flag) return;
		grounded = flag;
		if (grounded && MascotAnimatorController.inst.myAnimator.GetFloat("ungroundedtime") > 0.5f) {
			//			Debug.Log("Landed!");
			MascotAnimatorController.inst.DetermineFootstepAudioType();
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));

			//			}
		}
		if (grounded && ignoreGroundedForModMomentumForSeconds < 0) {
			momentum = momentum.normalized * maxMomentumWhenGrounded;
		} 

		PlayerCostumeController.inst.SetGrounded(flag); 

	}



	public void JetpackShift(){

		SetGrounded(false);
		moveDirection.y = 45;
		moveDirection.z = 70;

	}
	public void Jetpack(){

		SetGrounded(false);
		moveDirection.y = 30;
		moveDirection.z = 20;


	}

	bool allowJumpingWhenStuck = false; 

	void PreventStickingDuringSlide(){
		allowJumpingWhenStuck = false;
		if (sliding){
			// prevent sticking in a slide
			stopSlidingTimer = 0.5f; // minimum cooldown for stopping sliding is half a second.
			Vector3 slidingPosition = transform.position;
			if (Vector3.SqrMagnitude(slidingPosition-lastSlidingPosition) < .05f){
				slideTimer += Time.deltaTime;
				//						// commented Debug.Log("slidetimer;"+slideTimer);
				if (slideTimer > 1.3f){
					allowJumpingWhenStuck = true;
					//					transform.position += (Vector3.up) * 3f + Random.onUnitSphere;
					//					slideTimer = 0;
					//					helpCount ++;
					//					if (helpCount > 3){
					//						GameManager.inst.AskRestartLevel("It looks like you're stuck! Restart?","Uh oh..",GameManager.inst.questionMarkIcon);
					//						helpCount = 0;
					//					}
				}
				if (slideTimer > 3f){

					JumpNow();
				}
			}
			lastSlidingPosition = transform.position;
		}

	}

	// Update is called once per frame
	bool jumpPressed = false;
	void Update(){ 
		if (Player.frozen) return;
		retainSlideTimer -= Time.deltaTime;
		stopSlidingTimer -= Time.deltaTime;
		ignoreGroundedForModMomentumForSeconds -= Time.deltaTime; //Mathf.Max(0, ignoreGroundedForModMomentumForSeconds - Time.deltaTime);
		if (!restrictZaxis && Input.GetButton("Jump") || restrictZaxis && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) ){
			jumpPressed = true;
		} else {
			jumpPressed = false;
		}

		if (restrictZaxis){
			if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)){
				characterArt.transform.forward = Vector3.right; 
			}
			if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)){
				characterArt.transform.forward = -Vector3.right; 
			}

		}

		jumpingUnderwaterTimer -= Time.deltaTime;
		if (jumpingUnderwaterTimer < 0){
			jumpingUnderwater = false;
		}

		// fix getting stuck / tripping on edges
		// from http://answers.unity3d.com/questions/143773/why-does-the-character-controller-get-stuck.html
		//		float f = 0;
		//		if (toggle) f = deltaStep;
		//		controller.stepOffset = stepHeight + f;
		//		toggle = !toggle;



		//		if(networkView != null && !networkView.isMine) { return; }

		// If the run button is set to toggle, then switch between walk/run speed. (We use Update for this...
		// FixedUpdate is a poor place to use GetButtonDown, since it doesn't necessarily run every frame and can miss the event)

		#if !UNITY_IPHONE || UNITY_EDITOR
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			speed = runSpeed;
		} else if ((Input.GetKey (KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)) && SMW_CHEATS.inst.cheatsEnabled) {
			speed = runSpeed * 2.5f;

		} else {
			speed = walkSpeed;
		}


		if (PlayerUnderwaterController.inst.playerUnderwater) {
			//			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed *= 0.2f; // slower underwater.
			//			else speed *= 0.3f;
			speed *= 0.62f;
		}


		#endif
		//		} else if (Input.GetKey(KeyCode.LeftControl)){ // TODO: editor / prototype build
		//			speed = runSpeed*3;



		//		if(dying) {
		//			//float mag = characterArt.transform.localScale.x;
		//			//characterArt.transform.localScale = Vector3.one * Mathf.Max(mag - Time.deltaTime, 0);	
		//			dieTimer -= Time.deltaTime;
		//			if (dieTimer <= .4f && playerNeedsResetting){
		////				// commented Debug.Log ("dietime:"+dieTimer+", not spin");
		//				playerNeedsResetting = false;
		////				// commented Debug.Log ("needs restting");
		//				characterArt.transform.localRotation = Quaternion.identity;
		//				Camera.main.GetComponent<GrayscaleEffect>().enabled = false;
		//
		//			} else if (dieTimer > .4f) {
		////				// commented Debug.Log ("dietime:"+dieTimer+", spin");
		//				characterArt.transform.Rotate(Vector3.up, Time.deltaTime * 6.28f * 100);
		//			}
		//			if(dieTimer <= 0) {
		////				// commented Debug.Log ("dietime:"+dieTimer+", reset");
		//				dying = false;
		//				Player.inst.UnfreezePlayer();
		//				Player.inst.UnfreezePlayerFreeLook();
		//
		//			}
		//		}mov

		if (Moved != null) Moved(moveDirection);

	}

	// Store point that we're in contact with for use in FixedUpdate if needed
	public Transform lastHit;
	Vector3 lastHitPos;
	Vector3 platformDiff;
	public float skipPlatformCheckTimer = 0;
	void OnControllerColliderHit (ControllerColliderHit hit) {
		Debug.Log("fpswe hit;"+hit.collider.name);
		skipPlatformCheckTimer -= Time.deltaTime;
		if (skipPlatformCheckTimer > 0) return;
		contactPoint = hit.point;

		activePlatform = null;
		if (lastHit == hit.collider.transform){
			if (lastHitPos != hit.collider.transform.position){
				if (hit.point.y < transform.position.y && !hit.collider.transform.root.GetComponentInChildren<Vehicle>()) { // try to stand on this platform if in the right position and it's not a vehicle and move alonge w the platform
					platformDiff = hit.collider.transform.position - lastHitPos;
					activePlatform = hit.collider.transform;
					//					// commented Debug.Log("activeplat: " +hit.collider.name);
				}

			}
		}

		lastHit = hit.collider.transform;
		lastHitPos = hit.collider.transform.position;
	}

	// If falling damage occured, this is the place to do something about it. You can make the player
	// have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
	void FallingDamageAlert (float fallDistance) {
		//        print ("Ouch! Fell " + fallDistance + " units!");   
	}

	// CVN 
	// For returning the player's velocity so that we can match projectile velocity to player velocity.
	public Vector3 GetPlayerSpeed(){
		return deltaPos*50f;
	}

	public void Kill() {
		if(dying) { return; }
		Player.inst.FreezePlayer();
		//		Player.inst.FreezePlayerFreeLook();
		playerNeedsResetting = true;
		AudioManager.inst.PlayDeathSound(transform.position);
		Camera.main.GetComponent<GrayscaleEffect>().enabled = true;
		dying = true;
		//		// commented Debug.Log ("dying true");
		dieTimer = 1.9f;
		//		GA.API.Design.NewEvent("PlayerDeath", transform.position);

		//		ServerComm.inst.SendDeathEvent(PlayerPrefs.GetString("login_username"),Application.loadedLevelName,transform.position);

	}

	int timesJumped=0;
	Vector2 playerStuckPos=Vector2.zero;
	bool PlayerStuck(){
		playerStuckPos = new Vector2(transform.position.x,transform.position.z);
		if (transform.position.x == playerStuckPos.x && transform.position.z == playerStuckPos.y){
			return true;
		} else {
			return false;
		}
	}

	public void SetPlayerUnderwater(bool u){
		//		Debug.Log ("set player underwater:"+u);
		if (underwater != u){
			underwater = u;
			if (underwater){
				jumpSpeed = underwaterJumpSpeed;
				gravity = underwaterGravity;
				moveDirection = Vector3.zero;
			} else {
				jumpSpeed = jumpSpeedAtStart;
				gravity = gravityAtStart;
				//			// commented Debug.Log ("grav set to:"+gravity);
			}
		}
	}

	//	Vector3 PipeFixedHitNormal(RaycastHit hit){
	//		if (hit.collider.name.ToLower().Contains("pipe")){
	//			return (Player.inst.transform.position - hit.point).normalized;
	//		} else return hit.normal;
	//	}
	//

	// The following functions take care of the case when we want special momentum for player
	// Gravity will be modified because player gravity is 4x regular item gravity (for better control feeling and gameplay mechancis)
	// Momentum will be modified to let the player fly in an arc (for example when shot out of a machine)
	// These values decay over time and especially decay if player is grounded
	Vector3 momentum;
	public void AddMomentum(Vector3 mom){
		momentum += mom;
	}
	public void ModGravity(float newGravity){
		// will settle over time
		gravity = new Vector3(0,newGravity,0);
	}


	float ignoreGroundedForModMomentumForSeconds = 0f;
	public void IgnoreGroundedForModMomentumForSeconds(float s){

		SetGrounded(false);
		ignoreGroundedForModMomentumForSeconds = s;
	}

}