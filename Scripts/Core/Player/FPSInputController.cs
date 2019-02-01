using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterMotor))]
//[RequireComponent (typeof (CharacterController))]

public class FPSInputController : MonoBehaviour {

	public CharacterMotor motor;
	public static FPSInputController inst;

	public void SetInstance(){
		inst = this;
	}

	// Use this for initialization
	void Awake () {
		motor = GetComponent<CharacterMotor>();
	}



	// Update is called once per frame
	void Update () {
		if (LevelBuilder.inst.levelBuilderIsShowing || Player.frozen) return;
		// Get the input vector from kayboard or analog stick
		Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		
		if (directionVector != Vector3.zero) {
			// Get the length of the directon vector and then normalize it
			// Dividing by the length is cheaper than normalizing when we already have the length anyway
			float directionLength = directionVector.magnitude;
			directionVector = directionVector / directionLength;
			
			// Make sure the length is no bigger than 1
			directionLength = Mathf.Min(1, directionLength);
			
			// Make the input vector more sensitive towards the extremes and less sensitive in the middle
			// This makes it easier to control slow speeds when using analog sticks
			directionLength = directionLength * directionLength;
			
			// Multiply the normalized direction vector by the modified length
			directionVector = directionVector * directionLength;
		}
		
		// Apply the direction to the CharacterMotor
		Vector3 moveDirection = transform.rotation * directionVector;

			
		motor.inputMoveDirection = moveDirection;
		motor.inputJump = Input.GetButton("Jump"); //&& !PlayerUnderwaterController.inst.playerUnderwater;
	}


	public void ModGravity(float newGravity){
		// will settle over time -- used to keep player in air after a cannon fire
		motor.movement.gravity = newGravity;
	}
}


//// Require a character controller to be attached to the same game object
//@script RequireComponent (CharacterMotor)
//@script AddComponentMenu ("Character/FPS Input Controller")
