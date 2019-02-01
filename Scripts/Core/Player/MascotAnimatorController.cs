/* 
	 *   YOU CAN FIND AN IMAGE VERSION OF THIS TUTORIAL AND MORE DOCUMENTATION ON THE MIXAMO COMMUNITY
	 *                                
	 *											 COMMUNITY.MIXAMO.COM
	 * 
	 * 
	 * 																Happy creating!
	 * 
	 *																 - The Mixamo Team
	 * 
	 */


/*Intro to Animators in Unity
 * 
 * Animator controllers and control scripts are designed to work together.
 * Take a look at the "DemoAnimatorController" in the Mixamo > Demo folder.  To view the animator information open the
 * Window > Animator panel with the asset selected in the Project view.
 * 
 * Looking at the window you'll see several 'boxes' with different names, colors and lines connecting them.
 * 
 * Each box is a state, usually with one animation, but sometimes it could have multiples.  The orange box is the default
 * or starting state.  Blue hexagonal rectangles are sub-state machines, essentially logical layers that have their own states.
 * Each white line is a transition with rules about when to transition between states.
 * 
 * You can click on a state to see the animation that is played when the animator is in that state.  For example, clicking
 * on the "jump" state the inspector will update to show that a .anim called jump is assigned to the motion field.  You can
 * also see the transitions available to and from a state.  In the jump example, there is one transition listed as
 * "Jump -> IdleWalkingBlend".
 * 
 * To see the rules of a transition click on the arrow between states.  Click on the arrow between "Jump" and "IdleWalkingBlend".
 * You can see in the inspector that the "Conditions" for that transition is Exit Time of 0.88.  This means that it will play the
 * animation to 0.88 (or 88% complete) and then transition into the next animation.
 * 
 * You can also give custom conditions based on integer, float and boolean inputs.  On the bottom left of the Animator window
 * you'll see a Parameters list.  These are custom parameters we've added to control the animations in our Animator.  Jumping is a
 * boolean (true or false), while VSpeed is a float (decimal)
 * 
 * Another useful feature is the ability to watch your animation logic in the Animator window in real time.  If you have your
 * character selected in the hierarchy and press play the animator will show your current animation and transition information with a blue
 * playing line across the bottom of each state. 
 * 
 */





using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum FootstepSoundType {
	Grass,
	Wood,
	Concrete,
	Glass,
	Sponge
}
public class MascotAnimatorController : MonoBehaviour {
	
	
	//First, we will create a reference called myAnimator so we can talk to the Animator component on the game object.
	//The Animator is what listens to our instructions and tells the mesh which animation to use.
	public Animator myAnimator;

	public static MascotAnimatorController inst;

	public void SetInstance(){
		inst = this;
	}
	
	// The start method is called when the script is initalized, before other stuff in the scripts start happening.
	void Start () {
		//We have a reference called myAnimator but we need to fill that reference with an Animator component.
		//We can do that by 'getting' the animator that is on the same game object this script is appleid to.
//		myAnimator = GetComponentInChildren<Animator>();
	}

	public void Init(Animator a){
		myAnimator = a;
		myAnimator.SetFloat("MotionZ", 0);

//		SMW_FPSWE.inst.Moved += Moved;
		myAnimator.SetFloat("ungroundedtime",-1);

	}

	float mz = 0f;
	public virtual void Moved(Vector3 moveDirection){
		if (jumping) return;
		if (Player.frozen) return;
		moveDirection = Utils.FlattenVector(moveDirection);// / totalSpeedRange;
		mz = moveDirection.magnitude;
//		Debug.Log("movedir mag;"+moveDirection.magnitude);
//		if (Input.GetKey(KeyCode.S)) mz *= -1;
		myAnimator.SetFloat("MotionZ", moveDirection.magnitude);
//		Debug.Log("mz:"+mz);
	}

	// Update is called once per frame so this is a great place to listen for input from the player to see if they have
	//interacted with the game since the LAST frame (such as a button press or mouse click).

	void Update () {
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.W)){
			mz = 2;
		}
		#endif
		if (Player.frozen){
			myAnimator.SetFloat("MotionZ",0);
//			myAnimator.SetFloat("MouseMotionX",1);
			return;
		}

//		// commented Debug.Log("frame;"+myAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash+","+myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
		RaycastHit hit;
		float groundDist = 10f;
		if (Physics.Raycast(new Ray(Player.inst.transform.position,Vector3.down),out hit,10f)){
			groundDist = hit.distance;
		}
//		Debug.Log("grounddist;"+groundDist);
		myAnimator.SetBool("CloseToGround",groundDist < 1f);


		if (!grounded){
			myAnimator.SetFloat("ungroundedtime",myAnimator.GetFloat("ungroundedtime") + Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.W)){
			myAnimator.SetBool("Holding_W",true);
		} else {
			myAnimator.SetBool("Holding_W",false);
		}
		if (Input.GetKey(KeyCode.S)){
			myAnimator.SetBool("Holding_S",true);
		} else {
			myAnimator.SetBool("Holding_S",false);
		}

		float motionX = Input.GetAxis("Mouse X");
		motionX = Mathf.Clamp(motionX,-5,5);
//		motionX = 1;
//		// commented Debug.Log("mouse x:"+motionX); //Input.GetAxis("Mouse X"));

		myAnimator.SetFloat("MouseMotionX", motionX);

		if (Input.GetKeyDown (KeyCode.A)){
			myAnimator.SetBool("strafeLeft",true);
		} 
		if (Input.GetKeyDown(KeyCode.D)){
			myAnimator.SetBool("strafeRight",true);
		}
		if (Input.GetKeyUp (KeyCode.A)){
			myAnimator.SetBool("strafeLeft",false);
		}
		if (Input.GetKeyUp (KeyCode.D)){
			myAnimator.SetBool("strafeRight",false);
		}

//		if (!swimming){
//
//		}
//		AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
//		// commented Debug.Log ("info:"+info.tagHash);
//		myAnimator.Play("Jumping");


		//Set Jump Boolean to true to trigger jump animation, then wait a small time and set to false so we don't jump agani.
//		if(Input.GetButtonDown ("Jump")){
//			// commented Debug.Log ("jump button detected.");
//			myAnimator.SetBool ("Jumping", true);
//			myAnimator.SetTrigger("Jump");
////			Invoke ("StopJumping", 0.1f);
//		}
		
//		Debug.Log("motionz;"+mz);
		if (grounded && Mathf.Abs(mz) > 1.05f) { // wht its 1? 1 is idle apparently, motionz is never <1
			PlayFootstepsAudio();
		}

	}

	float footstepTimer = 0f;

	AnimationClip GetClip(string clipName){
		foreach (AnimationClip clip in myAnimator.runtimeAnimatorController.animationClips) {
			if (clip.name == clipName) {
				return clip;
			}
		}
		return null; 
		
	}

	FootstepSoundType soundType = FootstepSoundType.Grass;

	void PlayFootstepsAudio(){
//		Debug.Log("Try");
		if (PlayerUnderwaterController.inst.playerUnderwater) return;
		List<AnimationClip> animList = new List<AnimationClip>();
//		AnimationClip clip = GetClip("running_inPlace");
//		animList.Add(clip);

		//		float frame = anim["walk"].time * anim.clip.frameRate;
		//		float playbackTime = anim.GetCurrentAnimatorStateInfo().normalizedTime % 1;
		//		float frameRate = anim.GetCurrentAnimatorStateInfo().speed * anim.GetCurrentAnimatorClipInfo().Length;
		//		// commented Debug.Log("len:"+anim.GetCurrentAnimatorClipInfo().Length+", playbacktime:"+playbackTime+",framreate;"+frameRate);
		//		float frame = playbackTime * frameRate;
//		float frame = myAnimator.GetCurrentAnimatorClipInfo(0,animList).clip(normalizedTime % 1;
		//		// commented Debug.Log("frame;"+frame);
		float frame = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
		footstepTimer -= Time.deltaTime;
		if (footstepTimer < 0){
			DetermineFootstepAudioType();

		}
		float tolerance = 0.05f; // rangeOfAnimationInWhichToPlayFootstepSound
		//		// commented Debug.Log("frame:"+frame);
		if ( Mathf.Abs(frame-0.20f) < tolerance  && footstepTimer < 0){ // right foot down
			//			if (rightFoot){
			//				for (int i=0;i<5;i++){
			//					float dustSize = Random.Range(10,12f);
			//					Vector3 pos = rightFoot.transform.position + Utils.FlattenVector(Random.insideUnitSphere);
			//					EffectsManager.inst.MakeDust(pos,dustSize);
			//				}
			//			}
			footstepTimer = 0.08f;
//			Debug.Log("left");
			PlayFootstep();

		} else if (Mathf.Abs(frame-0.7f) < tolerance && footstepTimer < 0){ // left foot down
			//			if (leftFoot){
			//				for (int i=0;i<5;i++){
			//					float dustSize = Random.Range(10,12f);
			//					Vector3 pos = leftFoot.transform.position + Utils.FlattenVector(Random.insideUnitSphere);
			//					EffectsManager.inst.MakeDust(pos,dustSize);
			//				}
			//			}
			footstepTimer = 0.08f;
//			Debug.Log("right");
			PlayFootstep();
		}

	}

	public void DetermineFootstepAudioType(){
		soundType = FootstepSoundType.Grass; // the default
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(Player.inst.transform.position,Vector3.down,out hit,1f)){
			Renderer r = hit.collider.GetComponent<Renderer>();
			if (r && r.material && r.material){
				string ln = r.material.name.ToLower();
				if (ln.Contains("moss") || ln.Contains("dirt") ){
					soundType = FootstepSoundType.Grass;
				} else if (ln.Contains("wood")){
					soundType = FootstepSoundType.Wood;
				} else if (ln.Contains("stone") || ln.Contains("concrete")){
					soundType = FootstepSoundType.Concrete;
				} else if (ln.Contains("glass")) {
					soundType = FootstepSoundType.Glass;
				} else if (ln.Contains("sponge")){
					soundType = FootstepSoundType.Sponge;
				}
				// Here we do a (crappy) relation hash for EffectsManager colors. If we are hitting one of these colors, make the corresponding sound. Unforuntaely


				//					if (r.material.mainTexture.name.Contains
				//					Debug.Log("material texture;"+r.material.mainTexture);
				// use a texture matrix to decide what type of footstep we have
			}
		}
	}

	public IEnumerator PlayFootstepAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		PlayFootstep();
	}

	public void PlayFootstep(float vol = 0.25f){
		AudioManager.inst.PlayPlayerFootsteps(soundType,vol);
	}

	public bool jumping = false;
	public void JumpStart(){
		jumping = true;
//		// commented Debug.Log ("jump button detected.");
		StartCoroutine("JumpEnd");
		myAnimator.SetBool ("Jumping", true);
		myAnimator.SetTrigger ("Jump");
	}

	public IEnumerator JumpEnd(){
		yield return new WaitForSeconds(.4f);
		jumping = false;

//		// commented Debug.Log ("Jump over!");
		myAnimator.SetBool("Jumping",false);
		yield break;	
	}
	
	//This method is called after jumping is started to stop the jumping!
	void StopJumping(){
//		myAnimator.SetBool ("Jumping", false);
//		// commented Debug.Log ("jumping:"+myAnimator.GetBool("Jumping"));
	}
	
	//We've added some simple GUI labels for our controls to make it easier for you to test out.
	public void FireBow(){
		myAnimator.SetTrigger("FireBow");
	}


	public void HoldRightArmPalmUp(bool flag){
		myAnimator.SetBool ("HoldRightArmPalmUp",flag);
		if (flag){
			myAnimator.SetLayerWeight (2, 1f);
			myAnimator.SetLayerWeight (0, 0.5f);
		} else {
			myAnimator.SetLayerWeight (2, 0f);
			myAnimator.SetLayerWeight (0, 0f);
		}
	}
	public void HoldRightArm(bool flag){
//		Debug.Log ("hold:"+flag);
		myAnimator.SetBool ("HoldRightArm",flag);
		if (flag){
			myAnimator.SetLayerWeight (2, 1f);
			myAnimator.SetLayerWeight (0, 0.5f);
		} else {
			myAnimator.SetLayerWeight (2, 0f);
			myAnimator.SetLayerWeight (0, 0f);
		}
//		// commented Debug.Log ("myanimator layer weight 1:"+myAnimator.GetLayerWeight(1)+"; layer weight 0 was: "+myAnimator.GetLayerWeight(0));
	}

	bool swimming = false;
	public void Swimming(bool flag){
		if (swimming != flag){
			swimming = flag;
			if (flag){
	//			myAnimator.SetLayerWeight(2,0); // no falling
			 	myAnimator.SetLayerWeight(1,1);
				myAnimator.SetLayerWeight(0,0.5f);
			} else {
				myAnimator.SetLayerWeight(1,0f);
			}
		}
//		Debug.Log("layer 1 weight:"+myAnimator.GetLayerWeight(1));
	}

//	bool operatingVehicle = false;
	public void OperatingVehicle(bool flag){
		myAnimator.SetBool("OperatingVehicle",flag);
	}

	public void Sliding(bool flag){
		if (Player.frozen) return;
		myAnimator.SetBool ("is_sliding", flag);
//		// commented Debug.Log ("sliding:"+flag);
//		if (swimming) return;
//		myAnimator.SetBool("grounded", !flag);
//		if (flag){
//			myAnimator.SetLayerWeight(2,1);
//		} else {
//			myAnimator.SetLayerWeight(2,0);
//		}
	}


	public void SwingSword(){

		myAnimator.SetTrigger("swingSword");
	}

	public void PointForwards(){
//		// commented Debug.Log ("hi");
		myAnimator.SetTrigger("pointForward");
	}

	public void SetHoldingBoolsFalse(){
		myAnimator.SetBool("holdingRightArmPalmUp",false);
		myAnimator.SetBool("holdingRightArmChop",false);
		myAnimator.SetBool("holdingRightArm",false);
//		// commented Debug.Log ("settin em false");

	}

	public void HoldRightArmChop(bool flag){
//		// commented Debug.Log ("settin holdchop "+flag);
		myAnimator.SetBool("holdingRightArmChop",flag);
		if (flag){
			myAnimator.SetLayerWeight (2, 1f);
			myAnimator.SetLayerWeight (0, 0.5f);
		} else {
			myAnimator.SetLayerWeight (2, 0f);
			myAnimator.SetLayerWeight (0, 0f);
		}
		//		
	}

	public void Driving(bool flag){
		if (flag){
			myAnimator.SetLayerWeight (3, 1);
			myAnimator.SetLayerWeight (0, 0.5f);
		} else {
			myAnimator.SetLayerWeight (3, 0);
			myAnimator.SetLayerWeight (0, 0.5f);
		}
	}

	bool grounded = true;
	public void SetGrounded(bool flag){
		if (!myAnimator.GetBool("grounded") && flag && !swimming){
			// we went from an ungrounded to a grounded state, so play a double footstep.
			DetermineFootstepAudioType();
			PlayFootstep(.7f);
		}
		myAnimator.SetBool("grounded",flag);
		grounded = flag;
		if (grounded) myAnimator.SetFloat("ungroundedtime",0);
	}

	public void JumpUnderwater(){
		myAnimator.SetTrigger("JumpUnderwater");
	}

//	IEnumerator SetGroundedFalseAfterSeconds(float s){
//		yield return new WaitForSeconds(s);
//		myAnimator.SetBool("grounded",false);
//	}


}

/*
 * We hope that this has been helpful to get you started with your own custom game logic and animation setup.  For any questions don't hesitate to reach us
 * at the Mixamo community!  community.mixamo.com
 */
