using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]
public class CharacterMotor : MonoBehaviour {
	// Does this script currently respond to input?
	bool canControl = true;
	public bool debug = false;
	bool useFixedUpdate = true;

	// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
	// Very handy for organization!

	// The current global direction we want the character to move in.
	[System.NonSerialized]
	public Vector3 inputMoveDirection = Vector3.zero;



	public class UnderwaterMovement {
		public float underwaterJumpSpeed;
		public float timeBetweenUnderwaterJumps = .05f;
		public float jumpSpeedUnderwater = 20;
		public float jumpUnderwaterDuration = 0.25f;
		public float underwaterUpDownSpeed = 7.4f;
		public float gravity = 0.1f;
	}

	public UnderwaterMovement underwaterMovement = new UnderwaterMovement();

	// Is the jump button held down? We use this interface instead of checking
	// for the jump button directly so this script can also be used by AIs.
	[System.NonSerialized]
	public bool inputJump = false;
	[System.Serializable]
	public class CharacterMotorMovement {
		// The maximum horizontal speed when moving
		public float maxForwardSpeed = 10.0f;
		public float maxSidewaysSpeed = 10.0f;
		public float maxBackwardsSpeed = 10.0f;
		public float waterSlowerMultiplier = 0.6f;
		public float runMultiplier = 1.6f;
		public float cheatMultiplier = 7f;
		
		// Curve for multiplying speed based on slope (negative = downwards)
		public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
		
		// How fast does the character change speeds?  Higher is faster.
		public float maxGroundAcceleration = 30.0f;
		public float maxAirAcceleration = 20.0f;

		// The gravity for the character
		public float gravity = 10.0f;
		public float maxFallSpeed = 20.0f;
		
		// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
		// Very handy for organization!

		// The last collision flags returned from MoveController
		[System.NonSerialized]
		public CollisionFlags collisionFlags;

		// We will keep track of the character's current velocity,
		[System.NonSerialized]
		public Vector3 velocity;
		
		// This keeps track of our current velocity while we're not grounded
		[System.NonSerialized]
		public Vector3 frameVelocity = Vector3.zero;
		
		[System.NonSerialized]
		public Vector3 hitPoint = Vector3.zero;
		
		[System.NonSerialized]
		public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
	}

	[SerializeField] public CharacterMotorMovement movement = new CharacterMotorMovement();

	public enum MovementTransferOnJump {
		None, // The jump is not affected by velocity of floor at all.
		InitTransfer, // Jump gets its initial velocity from the floor, then gradualy comes to a stop.
		PermaTransfer, // Jump gets its initial velocity from the floor, and keeps that velocity until landing.
		PermaLocked // Jump is relative to the movement of the last touched floor and will move together with that floor.
	}

	// We will contain all the jumping related variables in one helper class for clarity.
	[System.Serializable] public class CharacterMotorJumping {
		// Can the character jump?
		public bool enabled = true;

		// How high do we jump when pressing jump and letting go immediately
		public float baseHeight = 1.0f;
		
		// We add extraHeight units (meters) on top when holding the button down longer while jumping
		public float extraHeight = 4.1f;
		
		// How much does the character jump out perpendicular to the surface on walkable surfaces?
		// 0 means a fully vertical jump and 1 means fully perpendicular.
		public float perpAmount = 0.0f;
		
		// How much does the character jump out perpendicular to the surface on too steep surfaces?
		// 0 means a fully vertical jump and 1 means fully perpendicular.
		public float steepPerpAmount = 0.5f;
		
		// For the next variables, [System.NonSerialized] tells Unity to not serialize the variable or show it in the inspector view.
		// Very handy for organization!

		// Are we jumping? (Initiated with jump button and not grounded yet)
		// To see if we are just in the air (initiated by jumping OR falling) see the grounded variable.
		[System.NonSerialized]
		public bool jumping = false;
		
		[System.NonSerialized]
		public bool holdingJumpButton = false;

		// the time we jumped at (Used to determine for how long to apply extra jump power after jumping.)
		[System.NonSerialized]
		public float lastStartTime = 0.0f;
		
		[System.NonSerialized]
		public float lastButtonDownTime = -100;

		[System.NonSerialized]
		public float lastSuccessfulJumpTime = 0f;
		
		[System.NonSerialized]
		public Vector3 jumpDir = Vector3.up;
	}

	public CharacterMotorJumping jumping = new CharacterMotorJumping();

	[System.Serializable]	
	public class CharacterMotorMovingPlatform {
		public bool enabled = true;
		
		public MovementTransferOnJump movementTransfer = MovementTransferOnJump.PermaTransfer;
		
		[System.NonSerialized]
		public Transform hitPlatform;
		
		[System.NonSerialized]
		public Transform activePlatform;
		
		[System.NonSerialized]
		public Vector3 activeLocalPoint;
		
		[System.NonSerialized]
		public Vector3 activeGlobalPoint;
		
		[System.NonSerialized]
		public Quaternion activeLocalRotation;
		
		[System.NonSerialized]
		public Quaternion activeGlobalRotation;
		
		[System.NonSerialized]
		public Matrix4x4 lastMatrix4x4;
		
		[System.NonSerialized]
		public Vector3 platformVelocity;
		
		[System.NonSerialized]
		public bool newPlatform;
	}

	[SerializeField] public CharacterMotorMovingPlatform movingPlatform = new CharacterMotorMovingPlatform();

	class CharacterMotorSliding {
		// Does the character slide on too steep surfaces?
		public bool enabled = true;
		
		// How fast does the character slide on steep surfaces?
		public float slidingSpeed = 15;
		
		// How much can the player control the sliding direction?
		// If the value is 0.5 the player can slide sideways with half the speed of the downwards sliding speed.
		public float sidewaysControl = 1.0f;
		
		// How much can the player influence the sliding speed?
		// If the value is 0.5 the player can speed the sliding up to 150% or slow it down to 50%.
		public float speedControl = 0.2f;
	}

	CharacterMotorSliding sliding = new CharacterMotorSliding();

	[System.NonSerialized]
	public bool grounded = true;

	[System.NonSerialized]
	public Vector3 groundNormal = Vector3.zero;

	private Vector3 lastGroundNormal = Vector3.zero;

	private Transform tr;

	private CharacterController controller;

	float originalGravity = 0f;
	void Awake () {
		controller = GetComponent<CharacterController>();
		tr = transform;
		originalGravity = movement.gravity;
//		SetGrounded(true,0);
	}

	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing || Player.frozen) return;
		if (movement.velocity.ToString().Contains("NaN")){
			// happens when rotation is set by levelbuilder?
			movement.velocity = Vector3.zero;
//			return;
		}


		// We copy the actual velocity into a temporary variable that we can manipulate.
		Vector3 velocity = movement.velocity;
		
		// Update velocity based on input
		velocity = ApplyInputVelocityChange(velocity);

		// Apply momentum
//		velocity = ApplyMomentum(velocity);
		
		// Apply gravity and jumping force
		velocity = ApplyGravityAndJumping (velocity);


		// Moving platform support
		Vector3 moveDistance = Vector3.zero;
		if (MoveWithPlatform()) {
			Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
			moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
			if (moveDistance != Vector3.zero )
				MoveController(moveDistance);
			
			
			// Support moving platform rotation as well:
	        Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
	        Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
	        
	        var yRotation = rotationDiff.eulerAngles.y;
	        if (yRotation != 0) {
		        // Prevent rotation of the local up vector
		        tr.Rotate(0, yRotation, 0);
	        }
		}
		
		// Save lastPosition for velocity calculation.
		Vector3 lastPosition = tr.position;
		
		// We always want the movement to be framerate independent.  Multiplying by Time.deltaTime does this.
		Vector3 currentMovementOffset = velocity * Time.deltaTime;
//		AddVelocity(Vector3.up * 50);
		currentMovementOffset += additionalVelocity;
		
		// Find out how much we need to push towards the ground to avoid loosing grouning
		// when walking down a step or over a sharp change in slope.
		float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if (grounded)
			currentMovementOffset -= pushDownOffset * Vector3.up;
		

		// Reset variables that will be set by collision void
		movingPlatform.hitPlatform = null;
		groundNormal = Vector3.zero;
		
	   	// Move our character!
//		Debug.Log("cur move off;"+currentMovementOffset);
		movement.collisionFlags = MoveController (currentMovementOffset);

		
		movement.lastHitPoint = movement.hitPoint;
		lastGroundNormal = groundNormal;
		
		if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform) {
			if (movingPlatform.hitPlatform != null) {
				movingPlatform.activePlatform = movingPlatform.hitPlatform;
				movingPlatform.lastMatrix4x4 = movingPlatform.hitPlatform.localToWorldMatrix;
				movingPlatform.newPlatform = true;
			}
		}
		
		// Calculate the velocity based on the current and previous position.  
		// This means our velocity will only be the amount the character actually moved as a result of collisions.
		Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
		movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
		Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);
		
		// The CharacterController can be moved in unwanted directions when colliding with things.
		// We want to prevent this from influencing the recorded velocity.
		if (oldHVelocity == Vector3.zero) {
			movement.velocity = new Vector3(0, movement.velocity.y, 0);
		}
		else {
			float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
			movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
		}
		
		if (movement.velocity.y < velocity.y - 0.001f) {
			if (movement.velocity.y < 0) {
				// Something is forcing the CharacterController down faster than it should.
				// Ignore this
				movement.velocity.y = velocity.y;
			}
			else {
				// The upwards movement of the CharacterController has been blocked.
				// This is treated like a ceiling collision - stop further jumping here.
				jumping.holdingJumpButton = false;
			}
		}
			
		// We were grounded but just loosed grounding
//		Debug.Log("grounded:"+grounded+", isgroundedtest;"+IsGroundedTest());
		if (grounded && !IsGroundedTest()) {
			SetGrounded(false,pushDownOffset);
		}
		// We were not grounded but just landed on something
		else if (!grounded && IsGroundedTest()) {

			SetGrounded(true,pushDownOffset);
		}
		
		// Moving platforms support
		if (MoveWithPlatform()) {
			// Use the center of the lower half sphere of the capsule as reference point.
			// This works best when the character is standing on moving tilting platforms. 
			movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height*0.5f + controller.radius);
			movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
			
			// Support moving platform rotation as well:
	        movingPlatform.activeGlobalRotation = tr.rotation;
	        movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
		}


		// Apply momentum to movement.
//		Debug.Log("[------- move:"+movement.velocity+", mmt:"+momentum+"---------");
//		movement.velocity += momentum;
//		Debug.Log("--------move:"+movement.velocity+", mmt:"+momentum+"----------]");
		// We add momentum for things like conveyers and shooters so the player feels they are "flying". This allows the player controller to "carry" the foce with it that was given at a single point like being fired form a gun.
		// Without this the character motor would return to default speeds instantly after leaving the FX of the gun
		DecayMomentum();
		GravityFXDecay();

	}

	void LateUpdate(){
//		Debug.Log("vel y:"+movement.velocity.y);
		additionalVelocity = Vector3.zero; // resets each frame.
	}

	public float gravityDecayFactor = .5f;
	void GravityFXDecay(){
		float groundedFactor = grounded ? 5f : .5f;
		movement.gravity = Mathf.Lerp(movement.gravity,originalGravity,Time.deltaTime * gravityDecayFactor * groundedFactor);
	}


	void FixedUpdate () {
		if (LevelBuilder.inst.levelBuilderIsShowing  || Player.frozen) return;
		if (movingPlatform.enabled) {
			if (movingPlatform.activePlatform != null) {
				if (!movingPlatform.newPlatform) {
					Vector3 lastVelocity = movingPlatform.platformVelocity;
					
					movingPlatform.platformVelocity = (
						movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
						- movingPlatform.lastMatrix4x4.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
					) / Time.deltaTime;
				}
				movingPlatform.lastMatrix4x4 = movingPlatform.activePlatform.localToWorldMatrix;
				movingPlatform.newPlatform = false;
			}
			else {
				movingPlatform.platformVelocity = Vector3.zero;	
			}
		}
	}


	public bool slidingNow = false;
	private Vector3 ApplyInputVelocityChange (Vector3 velocity) {	
		if (!canControl)
			inputMoveDirection = Vector3.zero;
		
		// Find desired velocity
		Vector3 desiredVelocity;
		if (grounded && TooSteep()) {
			slidingNow = true;
//			Debug.Log("grounded;"+grounded+", test:"+IsGroundedTest());
			// The direction we're sliding in
			desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
			// Find the input movement direction projected onto the sliding direction
			var projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
			// Add the sliding direction, the spped control, and the sideways control vectors
			desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
			// Multiply with the sliding speed
			desiredVelocity *= sliding.slidingSpeed;

			// we are sliding
			MascotAnimatorController.inst.Sliding(true);
			PlayerWalkDustParticles.inst.Emit (.5f);

		}
		else {
			slidingNow = false;
			MascotAnimatorController.inst.Sliding(false);
			desiredVelocity = GetDesiredHorizontalVelocity();
			MascotAnimatorController.inst.Moved(desiredVelocity);
			desiredVelocity += momentum;
		}
		
		if (movingPlatform.enabled && movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer) {
			desiredVelocity += movement.frameVelocity;
			desiredVelocity.y = 0;
		}
		
		if (grounded){
			
			desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);

		}
		else{
			velocity.y = 0;
		}
		
		
		// Enforce max velocity change
		float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
		Vector3 velocityChangeVector = (desiredVelocity - velocity);
		if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		// If we're in the air and don't have control, don't apply any velocity change at all.
		// If we're on the ground and don't have control we do apply it - it will correspond to friction.
		if (grounded || canControl) {
			velocity += velocityChangeVector;
		}
		
		if (grounded) {
			// When going uphill, the CharacterController will automatically move up by the needed amount.
			// Not moving it upwards manually prevent risk of lifting off from the ground.
			// When going downhill, DO move down manually, as gravity is not enough on steep hills.
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		
		return velocity;
	}

	float gravitySuspended = 0f;
	public void SuspendGravityForSeconds(float s){
		movement.gravity = 0f;
//		gravitySuspended = s;
	}
	private Vector3 ApplyGravityAndJumping (Vector3 velocity) {
		gravitySuspended -= Time.deltaTime;
		if (!inputJump || !canControl) {
			jumping.holdingJumpButton = false;
			jumping.lastButtonDownTime = -100;
		}
		
		if (inputJump && jumping.lastButtonDownTime < 0 && canControl) {
			jumping.lastButtonDownTime = Time.time;

		}
		if (PlayerUnderwaterController.inst.playerUnderwater){

			//Handle under water jumping

			if (inputJump && Time.time - jumping.lastSuccessfulJumpTime > underwaterMovement.timeBetweenUnderwaterJumps){
				jumping.lastSuccessfulJumpTime = Time.time;
				MascotAnimatorController.inst.JumpUnderwater();
//				Debug.Log("last successful jump:"+jumping.lastSuccessfulJumpTime);
			}
			float dt = Time.time - jumping.lastSuccessfulJumpTime;

			if (dt > underwaterMovement.jumpUnderwaterDuration) velocity.y = 0;
			else velocity.y = (1 - (dt/underwaterMovement.jumpUnderwaterDuration)) * underwaterMovement.jumpSpeedUnderwater;
//			if (jumping.lastButtonDownTime > 200) {
//				velocity.y = 0;
//			}

			// Handle under water movement up/down based on camera forward vector looking up or down

			float zvel = Input.GetAxis("Vertical");
//			int sign = Input.GetAxis("Vertical") > 0 ? 1 : -1;
			float yvel = Camera.main.transform.forward.y * zvel * underwaterMovement.underwaterUpDownSpeed;
//			Debug.Log("yvel:"+yvel);
			velocity.y += yvel;
			velocity.y -= movement.gravity * Time.deltaTime * underwaterMovement.gravity;

		} else if (grounded) {
			velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
		} else {

			velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime; // only apply gravity if not underwater.

//
//			} else {
//				velocity.y = 0; //movement.velocity.y - movement.gravity * Time.deltaTime; // only apply gravity if not underwater.
//			}
			
			// When jumping up we don't apply gravity for some time when the user is holding the jump button.
			// This gives more control over jump height by pressing the button longer.
			if (jumping.jumping && jumping.holdingJumpButton) {
				// Calculate the duration that the extra jump force should have effect.
				// If we're still less than that duration after the jumping time, apply the force.
				if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
					// Negate the gravity we just applied, except we push in jumpDir rather than jump upwards.
					// JumpNow();
					AudioManager.inst.PlayJump();
					MascotAnimatorController.inst.JumpStart();
//					Debug.Log("jump5");
					velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;

				}
			}
			
			// Make sure we don't fall any faster than maxFallSpeed. This gives our character a terminal velocity.
			velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
//			Debug.Log("Y fal:"+velocity.y);
		}
			
		if (grounded) {
			// Jump only if the jump button was pressed down in the last 0.2 seconds.
			// We use this check instead of checking if it's pressed down right now
			// because players will often try to jump in the exact moment when hitting the ground after a jump
			// and if they hit the button a fraction of a second too soon and no new jump happens as a consequence,
			// it's confusing and it feels like the game is buggy.
			if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2)) {
//				float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
				SetGrounded(false);
				jumping.jumping = true;
				jumping.lastStartTime = Time.time;
				jumping.lastButtonDownTime = -100;
				jumping.holdingJumpButton = true;
				
				// Calculate the jumping direction
				if (TooSteep())
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
				else
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
				
				// Apply the jumping force to the velocity. Cancel any vertical velocity first.
				velocity.y = 0;
				velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
				
				// Apply inertia from platform
				if (movingPlatform.enabled &&
					(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
					movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
				) {
					movement.frameVelocity = movingPlatform.platformVelocity;
					velocity += movingPlatform.platformVelocity;
				}
				
				SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else {
				jumping.holdingJumpButton = false;
			}
		}

		
		return velocity;
	}

	void OnControllerColliderHit (ControllerColliderHit hit) {
//		Debug.Log("cm hit;"+hit.collider.name);
		if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
			if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001 || lastGroundNormal == Vector3.zero)
				groundNormal = hit.normal;
			else
				groundNormal = lastGroundNormal;
			
			movingPlatform.hitPlatform = hit.collider.transform;
			movement.hitPoint = hit.point;
			movement.frameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity () {
		// When landing, subtract the velocity of the new ground from the character's velocity
		// since movement in ground is relative to the movement of the ground.
		if (movingPlatform.enabled &&
			(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
			movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		) {
			// If we landed on a new platform, we have to wait for two FixedUpdates
			// before we know the velocity of the platform under the character
			if (movingPlatform.newPlatform) {
				Transform platform = movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if (grounded && platform == movingPlatform.activePlatform)
					yield break;
			}
			Debug.Log("move vel:"+movement.velocity+", movplatfvel:"+movingPlatform.platformVelocity);
			movement.velocity -= movingPlatform.platformVelocity;
		}
	}

	private bool MoveWithPlatform () {
		return (
			movingPlatform.enabled
			&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
			&& movingPlatform.activePlatform != null
		);
	}

	private Vector3 GetDesiredHorizontalVelocity () {
		// Find desired velocity
		Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
		float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
		if (grounded) {
			// Modify max speed on slopes based on slope speed multiplier curve
			if (movement.velocity.ToString().Contains("NaN")){
				movement.velocity = Vector3.zero;
			}
			var movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
//			Debug.Log("moveslope angle:"+movementSlopeAngle);
			if (!float.IsNaN(movementSlopeAngle)){
				maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
			}
		}
		return tr.TransformDirection(desiredLocalDirection * maxSpeed);
	}

	private Vector3 AdjustGroundVelocityToNormal (Vector3 velocity, Vector3 groundNormal) {
		Vector3 sideways = Vector3.Cross(Vector3.up, velocity);

		return Vector3.Cross(sideways, groundNormal).normalized * velocity.magnitude;
	}

	private bool IsGroundedTest () {
		
		return (groundNormal.y > 0.01f);// && CloseToGround();
	}

//	bool CloseToGround(){
//		RaycastHit hit = new RaycastHit();
//		float maxDistAllowedFromGround = 0.3f; // how far above ground can we be and still be grounded
//		if (Physics.Raycast(new Ray(transform.position+Vector3.up*.05f,Vector3.down),out hit, maxDistAllowedFromGround)){
////			Debug.Log("hit:"+hit.distance);
//			return true;
//		}
//		Debug.Log("not close.");
//		return false;
//	}
	float GetMaxAcceleration (bool grounded) {
		// Maximum acceleration on ground and in air
		if (grounded)
			return movement.maxGroundAcceleration;
		else
			return movement.maxAirAcceleration;
	}

	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
	}

	bool IsJumping () {
		return jumping.jumping;
	}

	bool IsSliding () {
		return (grounded && sliding.enabled && TooSteep());
	}

	bool IsTouchingCeiling () {
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}

	bool IsGrounded () {
		return grounded;
	}

	bool TooSteep () {
		return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
	}

	Vector3 GetDirection () {
		return inputMoveDirection;
	}

	void SetControllable (bool controllable) {
		canControl = controllable;
	}

	// Project a direction onto elliptical quater segments based on forward, sideways, and backwards speed.
	// The function returns the length of the resulting vector.
	float MaxSpeedInDirection (Vector3 desiredMovementDirection)  {
		if (desiredMovementDirection == Vector3.zero)
			return 0;
		else {
			float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
			Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
			float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
			// underwater slower
			length *= PlayerUnderwaterController.inst.playerUnderwater ? movement.waterSlowerMultiplier : 1;
			length *= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? movement.runMultiplier : 1;
			length *= SMW_CHEATS.inst.cheatsEnabled && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)) ? movement.cheatMultiplier : 1;
			return length;
		}
	}

	Vector3 additionalVelocity = Vector3.zero;
	public void AddVelocity (Vector3 velocity) {
//		Debug.Log("addvel:"+velocity.magnitude);
		additionalVelocity += velocity;
	}
	public void SetVelocity (Vector3 velocity) {
		grounded = false;
		movement.velocity = velocity;
		movement.frameVelocity = Vector3.zero;
//		SendMessage("OnExternalVelocity");
	}


	public Vector3 GetVelocity(){
		return movement.velocity;
	}

	public CollisionFlags MoveController(Vector3 dir){
		
		AnalyticsManager.inst.RecordEvent(AnalyticsManager.Keys.worldDistanceMoved,Utils.FlattenVector(dir).magnitude);

		if (debug){
			Debug.Log("X:"+dir.x+" ; "+float.IsNaN(dir.x));
			Debug.Log("Y:"+dir.y+" ; "+float.IsNaN(dir.y));
			Debug.Log("Z:"+dir.z+" ; "+float.IsNaN(dir.z));
		}
		if (Utils.IsNan(dir)){
//			dir = Vector3.zero;

		} else {
//			Debug.Log("moved:"+dir);
		}
		if (float.IsNaN(dir.x) || float.IsNaN(dir.y) || float.IsNaN(dir.z)){
			dir = Vector3.up * -1;
		}
//		controller.Move(Vector3.zero);
		return controller.Move(dir);
	}


	public Vector3 momentum;

	public float momentumDecayFactor = 2f;
	void DecayMomentum(){
		float groundedFactor = grounded ? 5f : 1f; //grounded ? 2f : 5f;
		momentum = Vector3.Lerp(momentum,Vector3.zero,Time.deltaTime * momentumDecayFactor * groundedFactor);

	}

	void SetGrounded(bool flag,float pushDownOffset=0f){
		if (PlayerUnderwaterController.inst.playerUnderwater) {
			grounded = false;
			return;
		}
		grounded = flag;
//		Debug.Log("Grounded:"+grounded);
		MascotAnimatorController.inst.SetGrounded(flag);
		if (grounded && MascotAnimatorController.inst.myAnimator.GetFloat("ungroundedtime") > 0.5f) {
			//			Debug.Log("Landed!");
			MascotAnimatorController.inst.DetermineFootstepAudioType();
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));
			StartCoroutine(MascotAnimatorController.inst.PlayFootstepAfterSeconds(Random.Range(0f,0.05f)));

		}
		if (flag == false){

			// Apply inertia from platform
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
					movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
			) {
				movement.frameVelocity = movingPlatform.platformVelocity;
				movement.velocity += movingPlatform.platformVelocity;
			}

			SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			// We pushed the character down to ensure it would stay on the ground if there was any.
			// But there wasn't so now we cancel the downwards offset to make the fall smoother.

			tr.position += pushDownOffset * Vector3.up;
		} else if (flag == true){
			jumping.jumping = false;
			SubtractNewPlatformVelocity();
			SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}


	}

	public void SetMomentum(Vector3 m){
		momentum = Utils.FlattenVector(m); // only the XZ movement needs momentum
		float vertVelocityExchangeXZfactor = .0005f;
		Vector3 velChange = new Vector3(0,m.y,0) * vertVelocityExchangeXZfactor; // velocity chagne instantly for y value.This controller seems to preserve momentum for Y by default but not XZ 
		AddVelocity(velChange);
//		Debug.Log("add vel:"+velChange);
	}


}