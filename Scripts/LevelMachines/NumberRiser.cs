using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum RiserType {
	Riser,
	Elevator
}

public class NumberRiser : UserEditableObject {

	// Takes a number as it input and rises up to the level according to the numbers' value
	public GameObject firstTimeArrow;
	public AudioSource elevatorSound;
	public RiserType type = RiserType.Riser;
	public GameObject floor;
	public LayerMask ceilingLayerMask;
	#region UserEditable 
	public static string heightScaleKey = "height_scale_key";

	public override SimpleJSON.JSONClass GetProperties(){ 
		//		Dictionary<string,string> properties = new Dictionary<string,string>();
		SimpleJSON.JSONClass N = base.GetProperties();
		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,frac,N);
		N[NumberRiser.heightScaleKey].AsFloat = heightScale;

		return N;

//		return N;
	}


	public override GameObject[] GetUIElementsToShow(){
		//		List<GameObject> elements = new List<GameObject>();
		//		elements.Add(LevelBuilder.inst.POCMFractionButton);
		return new GameObject[] {
			LevelBuilder.inst.POCMcopyButton,
			LevelBuilder.inst.POCMFractionZeroButton,
			LevelBuilder.inst.POCMheightButton,
			LevelBuilder.inst.POCMmodRiser

		};
		//		return elements.ToArray();
	}



	/* footpring was: (){
		return 1.2f;
	 */

	public override void OnGameStarted(){
		base.OnGameStarted();
		StartCoroutine(SetBottomRiserAfterSeconds(1f)); // prevents the object from detecting its replacement as a bottomriser and setting it before this object is destroyed, during a loadlevel() op
//		SetBottomRiser();
		ElevatorMoveTo(frac); // Don't use start function if created while game paused, e.g. level builder drag and drop
	}

	IEnumerator SetBottomRiserAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		SetBottomRiser();
	}

	// upoffset 	}

	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		if (N.GetKeys().Contains(Fraction.fractionKey)){
			frac = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
			ElevatorMoveTo(frac);
		}
		if (N.GetKeys().Contains(NumberRiser.heightScaleKey)){
			SetScale(N[NumberRiser.heightScaleKey].AsFloat);
		}
		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */

		//		SimpleJSON.JSONNode n = SimpleJSON.JSON.Parse(props);


	}

	#endregion
	void SetBottomRiser(){

		NumberRiser highest = null;
		foreach(Collider c in Physics.OverlapBox(transform.position - Vector3.up * 5,Vector3.one*5)){
			NumberRiser nr = c.transform.root.GetComponent<NumberRiser>();
			if (nr && nr != this){
				if ( Vector3.Magnitude(Utils.FlattenVector(nr.transform.position - transform.position)) < 1f && ((highest && nr.transform.position.y > highest.transform.position.y) || !highest)){
					// Iff the hit riser is within 1 worls unit of our xz position, below us, and higher up than any previously found riser
					highest = nr; // set it to the riser we'll reference for sitting atop
				}
			}
		}
		bottomRiser = highest; // this is the riser closest to being directly below us.

		if (bottomRiser == this) { // this somehow happened once
			bottomRiser = null;
		}
		if (bottomRiser) {
			bottomRiser.platformTop.gameObject.layer = LayerMask.NameToLayer("DontCollideWithPlayer"); // prevent player from cheating by using a middle riser to cross when they should obviously be using only the top riser ( if risers are stacked we don't want players walking on risers near the bottom.)
		}
	}


	public float heightScale = 1; // a bigger scale makes the difference between 3 and 4 in successiver risers *smaller*, because if we set the scale to 1/4 we want the risers to move "a lot" versus if we set scale to 100 we watn the risers to move "a little" so that player needs 100, 200, 300 to make stairs.
	float convertHeightScaleToWorldUnits = 4; // makes "1" be a regular step for player to jump.
	public Transform platformTop;
	public Transform platformLowestPosition;
	public Transform platformPole;
	public Transform conePosStart;
	public Transform conePosFinish;
	public Transform numberHeldPosition;

	bool moving = false;
	bool grabbing = false;
	Vector3 targetPosPlatform = Vector3.zero;
//	Vector3 poleStartPosLocal;
//	Vector3 platformStartPosLocal;
	GameObject numberHeld;

	public float scaleOfNumberHeld = 1.8f;
	float platformThickness = 1.2f; // because we want the total length to align to the *top* of this platofrm we must subtract the thickness.
	float platformHeight;
	float targetPlatformHeight;

	public Fraction frac = new Fraction(2,1);
	void Start(){
		ElevatorMoveTo(frac); // Don't use start function if created while game paused, e.g. level builder drag and drop
	}

	bool CanReceiveNumberByTrigger(){

		bool flag = !(moving || grabbing);

		return flag;
	}

	NumberRiser bottomRiser;
	void Update(){
		if (PauseMenu.paused) return;
		if (Utils.IntervalElapsed(4f)){
			// ugh, because sometimes the freakin' number gets destroyed by other means while its being grabbed
			if (!numberHeld){
				grabbing = false;
			}
		}
		float dt = Time.unscaledDeltaTime;
		if (bottomRiser){
			transform.position = new Vector3(transform.position.x,(bottomRiser.platformTop.position + Vector3.one * platformThickness).y,transform.position.z);
		}
		if (moving) {
			if (type==RiserType.Riser) {
				// for risers (staircases), disable the platform collider while moving which prevents player from being able to "ride" the riser up as it goes
				// we prevent this because we don't want player to "ride" up we want them to make a staircase so they learn about number lines
				platformTop.GetComponent<Collider>().enabled = false;
			}
			float moveSpeed = 4f;
			if (LevelBuilder.inst.levelBuilderIsShowing) moveSpeed = 40f;
			if (type == RiserType.Riser) {
				platformHeight = Mathf.Lerp(platformHeight,targetPlatformHeight,dt * moveSpeed);
			} else if (type == RiserType.Elevator) {
				// Elevators start and end slow but move fast in between.
				// We approximate a bell curve with:
				// (1+cos(2pi * x - pi)) / 2
				// which creates a bell curve of max y value 1 between interval 0, 1 on x axis.
				float distMoved = Mathf.Abs(previousTargetHeight-platformHeight);
				float totalDistToMove = Mathf.Abs(targetPlatformHeight - previousTargetHeight); // the value between 0 and 1 which represents how far lift has moved during this move action
				float percentMoveComplete = Utils.UnNan(distMoved / totalDistToMove);

				float bellCurveFactor = Mathf.Min(40f,totalDistToMove/15f); // how *much* faster than the typical speed should it move during the middle of its journey?
				float baseElevatorSpeed = 6f;


				moveSpeed = baseElevatorSpeed + Utils.CubicInOut(percentMoveComplete) * bellCurveFactor;
				if (height == 0) moveSpeed *= 2f; // move faster for zero.
				if (LevelBuilder.inst.levelBuilderIsShowing) moveSpeed *= 5f;


				platformHeight = Mathf.MoveTowards(platformHeight,targetPlatformHeight,moveSpeed * dt);
//				Debug.Log("targh:"+targetPlatformHeight+", movesp:"+moveSpeed+", dt:"+dt);
//				Debug.Log("movetw:"+platformHeight);
				if (!LevelBuilder.inst.levelBuilderIsShowing && !elevatorSound.isPlaying){
					 elevatorSound.Play();
				}
			}


			float finishMoveThreshold = .1f;
			if (Mathf.Abs(platformHeight - targetPlatformHeight) < finishMoveThreshold){
				platformHeight = targetPlatformHeight;

				StopElevator();
				if (type == RiserType.Elevator){
					if (!LevelBuilder.inst.levelBuilderIsShowing)  AudioManager.inst.PlayElevatorArrived(elevatorSound.transform.position);
				} if (type == RiserType.Riser) {
					if (!LevelBuilder.inst.levelBuilderIsShowing)  AudioManager.inst.PlayRiserArrived(platformTop.position);
					platformTop.GetComponent<Collider>().enabled = true; // re-enable the collider, player is now allowed to stand here.
				}
			}
			platformTop.transform.position = platformLowestPosition.position + Vector3.up * (platformHeight - platformThickness);

			platformPole.transform.localScale = new Vector3(1,1,platformHeight/2f*21f/22f);

			if (type==RiserType.Elevator && height >= 0 && platformHeight >= 0 && !floor.activeSelf) {
				floor.SetActive(true);
			}
				

			if (type == RiserType.Elevator){
				// For elevators, dont let them go through ceilings. or floors

				if (!LevelBuilder.inst.levelBuilderIsShowing){
					float stopIfThisCloseToCeiling = 6f;
//					Debug.Log("targh:"+targetPlatformHeight);
					if (targetPlatformHeight > 0){
						foreach(RaycastHit hit in Physics.RaycastAll(platformTop.position+Vector3.up*2,Vector3.up,stopIfThisCloseToCeiling,ceilingLayerMask)){
							if (!hit.collider.transform.root.GetComponent<NumberRiser>() && !hit.collider.isTrigger && hit.distance < stopIfThisCloseToCeiling){
								if (hit.collider.GetComponent<Rigidbody>() && !hit.collider.GetComponent<Rigidbody>().isKinematic) continue;
		//						WebGLComm.inst.Debug("Eelevator hit:"+hit.collider.name);
								StopElevator();
								PlayerNowMessageWithBox.inst.Display("The elevator got too close to the ceiling and was reset.",icon,Player.inst.transform.position);
								AudioManager.inst.PlayWrongAnswer(platformTop.position);
								AudioManager.inst.PlayExhaustWhoosh(platformTop.position);
								ElevatorMoveTo(new Fraction(0,1));
								return;
							}
						}
					}

					if (targetPlatformHeight < 0){
						foreach(RaycastHit hit in Physics.RaycastAll(platformTop.position+Vector3.up*2f,Vector3.down,stopIfThisCloseToCeiling,ceilingLayerMask)){
							if (!hit.collider.transform.root.GetComponent<NumberRiser>() && !hit.collider.isTrigger && hit.distance < stopIfThisCloseToCeiling){
								if ((hit.collider.GetComponent<Rigidbody>() && !hit.collider.GetComponent<Rigidbody>().isKinematic) || hit.collider.transform.root == transform.root){
//									Debug.Log("cont on "+hit.collider.name);
									continue;
								}
								//						WebGLComm.inst.Debug("Eelevator hit:"+hit.collider.name);
								StopElevator();
								PlayerNowMessageWithBox.inst.Display("The elevator got too close to the ground and was reset.",icon,Player.inst.transform.position);
								AudioManager.inst.PlayWrongAnswer(platformTop.position);
								AudioManager.inst.PlayExhaustWhoosh(platformTop.position);
								ElevatorMoveTo(new Fraction(0,1));
								return;
							} else {
//								Debug.Log("but.."+hit.collider+name);
							}
						}
					}
				}
			}

		}

		if (grabbing) {
			float moveSpeed = 1.5f;
			if (LevelBuilder.inst.levelBuilderIsShowing) moveSpeed *= 10f; // LOL but yeah this is needed for level builder placement, so that the change is seen instantly.
			if (!numberHeld) {
				numberHeld = null;
				return;
			}
			numberHeld.transform.position = Vector3.Lerp(numberHeld.transform.position,conePosFinish.position,dt * moveSpeed);
			numberHeld.transform.localScale *= 0.97f;
			float finishGrabThreshold = .4f;
			if (Vector3.Distance(numberHeld.transform.position,conePosFinish.position) < finishGrabThreshold){
				grabbing = false;
				if (!LevelBuilder.inst.levelBuilderIsShowing) AudioManager.inst.PlayPlungerSuck(transform.position,1);
				moving = true;
				numberHeld.transform.position = numberHeldPosition.position;
				numberHeld.transform.localScale = Vector3.one * scaleOfNumberHeld;

			}
		} 

	}


	void StopElevator(){
		
		if (elevatorSound && elevatorSound.isPlaying){
			elevatorSound.Stop();
		}
		moving = false;
	}

	void OnTriggerEnter(Collider other){
		Trigger(other.gameObject);	
	}

	public void Trigger(GameObject other){

		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (CanReceiveNumberByTrigger() && ni && ni.myShape == NumberShape.Sphere && !ni.usedThisFrame && IsValidInput(ni)){

			ni.usedThisFrame = true;
			GrabNewNumber(ni);
			if (!LevelBuilder.inst.levelBuilderIsShowing && firstTimeArrow != null){
				Destroy(firstTimeArrow);
			}
		} 
	}

	bool IsValidInput(NumberInfo ni){
		return !ni.GetComponent<DoesExplodeOnImpact>() && !ni.GetComponent<ShrinkAndDisappear>();
	}

	void GrabNewNumber(NumberInfo ni){
		
		if (!ni) {
//			WebGLComm.inst.Debug("no ni for riser!");
		}
		if (!LevelBuilder.inst.levelBuilderIsShowing) AudioManager.inst.PlayCartoonEat(transform.position,1);

		if (numberHeld) {

			Destroy(numberHeld);
		} else {

		}

		numberHeld = ni.gameObject;
		Utils.RemoveDeathComponents(numberHeld);

		if (numberHeld.GetComponent<Rigidbody>()){
			Destroy(numberHeld.GetComponent<Rigidbody>());
		}
		if (numberHeld.GetComponent<Collider>()){
			Destroy(numberHeld.GetComponent<Collider>());
		}
		grabbing = true;
		if (type == RiserType.Elevator){
			numberHeld.transform.parent = platformTop;
			numberHeld.transform.localScale = Vector3.one;
		} else if (type == RiserType.Riser){
			numberHeld.transform.parent = transform;
			
		}
		numberHeld.transform.position = conePosStart.position;
		SetHeight(ni.fraction.GetAsFloat());

		if (type==RiserType.Elevator){
			if (height < 0) {
				floor.SetActive(false);

			} 
		}





	}

	float previousTargetHeight = 0f;
	void SetHeight(float h){
		previousTargetHeight = targetPlatformHeight;
		height = h;
		targetPlatformHeight = height * convertHeightScaleToWorldUnits / heightScale;


	}

	float height = 1;
	public void ElevatorMoveTo(Fraction f){ // For external (override) elevator set
//		if (!LevelBuilder.inst.levelBuilderIsShowing) AudioManager.inst.PlayPlungerSuck(transform.position,1);
		if (numberHeld) {

			Destroy(numberHeld);
		} else {

		}
		bool destroyIfZero = false;
		GameObject newNumberHeld = NumberManager.inst.CreateNumber(f,numberHeldPosition.position,NumberShape.Sphere,destroyIfZero);
		NumberInfo ni = newNumberHeld.GetComponent<NumberInfo>();
		GrabNewNumber(ni);

	}

	public void SetScale(float f){

		heightScale = f;
		platformHeight = height * convertHeightScaleToWorldUnits / heightScale;
		platformTop.transform.position = platformLowestPosition.position + Vector3.up * platformHeight;

		platformPole.transform.localScale = new Vector3(1,1,platformHeight/2f);
	}

	public float GetScale(){
		return heightScale;
	}

	public void ResetElevator () {
		ElevatorMoveTo(new Fraction(0,1));
	}

	public void PlatformTriggerEnter(GameObject o){
		if (o.GetComponent<Player>()){
			Player.inst.transform.parent = platformTop;
		}
	}
	public void PlatformTriggerExit(GameObject o){
		if (o.GetComponent<Player>()){
			Player.inst.Unparent();
		}
	}

}
