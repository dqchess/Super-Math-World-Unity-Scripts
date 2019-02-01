using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PlayerPositionWhileOperatingVehicle {
	Sitting,
	Standing
}

public class Vehicle : UserEditableObject {

	public NumberInfo energyNumber;
	#region UserEditable 



	public override SimpleJSON.JSONClass GetProperties(){ 

		SimpleJSON.JSONClass N = base.GetProperties();

		N = JsonUtil.ConvertFractionToJson(Fraction.fractionKey,energyFrac,N);
//			WebGLComm.inst.Debug("get prop vehicle. energyfrac;"+energyFrac);

		//		new SimpleJSON.JSONClass();
		//		N[GameManager.inst.numeratorKey] = fraction.numerator.ToString();
		//		N[denominatorKey] = fraction.denominator.ToString();
//		WebGLComm.inst.Debug("getting prop on vehicl:"+myName+",n:"+N.ToString());
		return N;
	}

	public override GameObject[] GetUIElementsToShow(){
		List<GameObject> elements = new List<GameObject>();
		elements.Add(LevelBuilder.inst.POCMFractionButton);
		return elements.ToArray();
	}





	/* footpring was: (){
		return 1.6f;
	 */

	public override void OnGameStarted(){
		if (destroyedThisFrame) return; 
		// This is bad.. basically, we are destroying and re-instantiating objects all in the same frame, then 
		// calling methods on each newly instantiated objects by iterating over all the objects,
		// which unfortunately stil iterates over objects that were just destroyed because the GC doesnt happen till
		// the end of the frame
		// TODO don't iterate over all the objects! Instead, keep track of them!
		// But we don't keep track of each object if a WALL is placed, IT separately creates untracked children! Hmm. Need to track the children always? Sounds messy but refactorable
		base.OnGameStarted();
		VehicleManager.inst.vehicles.Add(this);
		if (vehicleRigidbody) vehicleRigidbody.isKinematic = false;
		
	}

	public override void OnLevelBuilderObjectPlaced (){
		base.OnLevelBuilderObjectPlaced();
		if (vehicleRigidbody) vehicleRigidbody.isKinematic = true;
	}

	// upoffset 	}


	public override void SetProperties(SimpleJSON.JSONClass N){
		base.SetProperties(N);
		Fraction f = JsonUtil.ConvertJsonToFraction(Fraction.fractionKey,N);
		SetEnergyFraction(f);

		/*
		 * Format of properties for the NumberModifiers may differ, look in NumberHoop for the actual implementation of this method.
		 * */
		//		// commented Debug.Log("props:"+props);


	}
	#endregion

	public Rigidbody vehicleRigidbody;
	public bool canControl = false;
	public bool playerInvisibleWhileOperating = false;
	public float lowestPitch = 0.45f;

	public Transform operatingVehicle;
	public Transform getOutOfVehicle;
	public Transform camTransform;
	public GameObject vehicleCamera;
	public RecordPosition rp;
	public float angleOfCamPivotWhileOperating = 45;
	public PlayerPositionWhileOperatingVehicle position = PlayerPositionWhileOperatingVehicle.Standing;
	public AudioSource aud;



	virtual public void Start(){
		// todos possible memory leak
		energyNumber.energyNumberDestroyedDelegate += EnergyNumberDestroyed;
		if (vehicleRigidbody) vehicleRigidbody.isKinematic = true;
		if (!rp) rp = vehicleRigidbody.GetComponent<RecordPosition>();
		if (GetComponentInChildren<MouseLook>()) GetComponentInChildren<MouseLook>().enabled = false;
		if (!aud) aud = GetComponent<AudioSource>();
	}


	public Fraction energyFrac {
		get {
			return energyNumber ? energyNumber.fraction : new Fraction(-1,1);
		}
	}
//	public void SetEnergyFraction(Fraction f){
//		energyFrac = f;
//	}

	// This was for when we could toss a blue energy number into the vehicle, giving it power. Depracted. Vehicles always start with an energy number and once lost the vehicle is dead
//	public void EnergyNumberCollected(NumberInfo ni){
//		SetEnergyFraction(ni.fraction);
//		SetDestroyListenerDelegate(ni);
//		SetVehicleSound(CanMove());
//	}

	public void SetVehicleSound(bool hasEnergy){
		if (!hasEnergy){
			// commented Debug.Log("setsound");
			VehicleStopSound();
		}
	}

	public virtual bool CanMove(){
		return energyNumber != null && (energyFrac.numerator > 0 && ZoneCompatible());
	}

	bool zoneCompatible = true;
	bool ZoneCompatible(){
		return zoneCompatible;
	}

//	public void LeaveZone(){
//		zoneCompatible = true;
//		PlayerNowMessage.inst.Display("Leaving zone");
//	}
//
//	public void SetZoneCompatibility(bool flag, string comp){
//		zoneCompatible = flag;
//		PlayerNowMessage.inst.Display("ZONE SUCCESS! "+energyFrac.numerator+" matches: "+comp);
//		if (!flag){
//			PlayerNowMessage.inst.Display("!ZONE FAILED! "+energyFrac.numerator+" does not match: "+comp+" !",Color.red);
//			GetComponent<Rigidbody>().velocity = Vector3.zero;
//		}
//	}

	virtual public void ModulatePitchBasedOnSpeed(){
		float deltaPos = Vector3.Magnitude(rp.nowPosition - rp.lastPosition);
		float pitchLerpSpeed = 1;
		aud.pitch = Mathf.Lerp(aud.pitch,lowestPitch + deltaPos/2f,Time.deltaTime * pitchLerpSpeed);
	}



	float checkPlayerNearTimer = 0;
	float messageTimer = 0;
	virtual public void Update(){
		if (GameManager.inst.GameFrozen) return;

		checkPlayerNearTimer -= Time.deltaTime;
		if (checkPlayerNearTimer < 0){
			checkPlayerNearTimer = Random.Range(0.5f,1f);
//			// commented Debug.Log("checking veh");
			if (PlayerVehicleController.inst.currentVehicle == null){
				Vector3 pos = transform.position;
				if (vehicleRigidbody) pos = vehicleRigidbody.transform.position;
				float dist = Vector3.SqrMagnitude(pos-Player.inst.transform.position);
//				// commented Debug.Log("realdist:"+Vector3.Distance(transform.position,Player.inst.transform.position));
//				// commented Debug.Log("meh? dist:"+dist);
				if (dist < PlayerVehicleController.inst.sqrSeekRadius){
//					// commented Debug.Log("k");
					if (messageTimer < 0){
						messageTimer = 10f;
						// doesn't currently order closest vehicle.
						PlayerNowMessageWithBox.inst.Display(vehicleMessage,icon,transform.position);
					}
				} else {
//					// commented Debug.Log("uh:"+dist + ","+PlayerVehicleController.inst.sqrSeekRadius);
				}
			}
		}
		messageTimer -= Time.deltaTime;

	}

	string vehicleMessage {
		get {
			if (energyNumber){
				return "Press F to board vehicle. Press WASD to drive.";
			} else {
				return "This vehicle has no energy number and is disabled. Press F to exit";
			}
		}
	}

	public void SetEnergyFraction(Fraction f){
		if (!energyNumber) {
			Debug.LogError("Energy number null on :"+name+" during set fraction.");
			return;
		}

		energyNumber.SetNumber(f);

	}


	public virtual void EnergyNumberDestroyed(GameObject source){
		energyNumber.energyNumberDestroyedDelegate -= EnergyNumberDestroyed;
		StartCoroutine(AskReloadAfterSeconds(5f,source));
	}
	IEnumerator AskReloadAfterSeconds(float s, GameObject source){
		yield return new WaitForSeconds(s);
		PlayerDialogue.inst.playerPressedOKDelegate += PlayerChoseToReload;
		PlayerDialogue.inst.playerPressedCancelDelegate += PlayerChoseToContinue;
		if (source != null && source.GetComponent<Animal>()){
			PlayerDialogue.inst.ShowPlayerDialogue("Do you want to restart the level?","Vehicle number eaten!",source.GetComponent<Animal>().icon);
		} else {
			PlayerDialogue.inst.ShowPlayerDialogue("Do you want to restart the level?","Vehicle number destroyed!",LevelBuilder.inst.questionIcon);
		}
		VehicleStopSound();
		VehicleDisabledFX();
	}

	void VehicleDisabledFX(){
		foreach(Renderer r in GetComponentsInChildren<Renderer>()){
			Material[] mats = r.materials;
			foreach(Material m in mats){
				m.color = Color.gray;
			}
			r.materials = mats;
		}
	}

	void PlayerChoseToReload(){
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerChoseToReload;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerChoseToContinue;
		GameManager.inst.ReloadLevel();
	}

	void PlayerChoseToContinue(){
		PlayerDialogue.inst.playerPressedOKDelegate -= PlayerChoseToReload;
		PlayerDialogue.inst.playerPressedCancelDelegate -= PlayerChoseToContinue;
	}

	public void VehicleRunningSound(){
//		SetVehicleSoundClip(vehicleRunning,true);
	}

	virtual public void VehicleStopSound(){
		
//		SetVehicleSoundClip(vehicleStop,false);

	}

	public void SetVehicleSoundClip(AudioClip clip, bool loop){
//		if (!aud) aud = GetComponentInChildren<AudioSource>();
//		aud.pitch = 1;
//		aud.clip = clip;
//		aud.loop = loop;
//		aud.Play();
	}

	public bool underwater = false;
	public virtual void SetUnderwater(bool f){
		underwater = f;
	}

	virtual public void PlayerGetInVehicle(){
		if (vehicleRigidbody) vehicleRigidbody.isKinematic = false;

		
	}
	virtual public void PlayerGetOutOfVehicle(){
		MovePlayerToGetOutOfVehiclePosition();
		PlayerGadgetController.inst.ReEquipCurrentGadget();
	}

	virtual public void StopVehicle(){
		if (vehicleRigidbody) vehicleRigidbody.velocity = Vector3.zero;
	}

	virtual public void HandleAudio(){}

	virtual public void MovePlayerToGetOutOfVehiclePosition(){
//		Debug.Log("get out veh rot euler:"+getOutOfVehicle.rotation.eulerAngles);

		Player.inst.SetPosition(getOutOfVehicle);
	}

}
