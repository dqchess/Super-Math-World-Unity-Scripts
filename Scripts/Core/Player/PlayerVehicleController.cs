using UnityEngine;
using System.Collections;

public class PlayerVehicleController : MonoBehaviour {

	public Vehicle currentVehicle = null;
	public float seekRadius = 15f;
	public float sqrSeekRadius{
		get {
			return seekRadius * seekRadius;
		}
	}
	public static PlayerVehicleController inst;

	public void SetInstance(){
		inst = this;
	}

	// Update is called once per frame
	float vehicleBoardTimer = 0;
	void Update () {
		if (Time.timeScale != 0){
			vehicleBoardTimer -= Time.deltaTime;
			if (Input.GetKeyDown(KeyCode.F)){
				
				if (vehicleBoardTimer > 0) return;
				vehicleBoardTimer = 0.5f;
//				// commented Debug.Log("curve:"+currentVehicle);
				if (!currentVehicle){
					Vehicle v = VehicleNearby();
					if (v){
//						// commented Debug.Log("get in");
						PlayerGetInVehicle(v);
						currentVehicle = v;
					}
				} else {
					if (currentVehicle == null){
						// commented Debug.LogError("error about current veh");
					} else {
						PlayerGetOutOfVehicle();
						currentVehicle = null;
					}
				}
			}
		}
	}
	void LateUpdate(){
//		if (currentVehicle){
//			Player.inst.transform.position = currentVehicle.operatingVehicle.position;
//			Quaternion rot = Quaternion.identity;
//			rot.eulerAngles = new Vector3(0,currentVehicle.operatingVehicle.rotation.eulerAngles.y,0); //currentVehicle.operatingVehicle.rotation;
//			Player.inst.transform.rotation = rot;
//			//			Quaternion rot = Player.inst.pivot.rotation;
//			//			rot.eulerAngles = new Vector3(currentVehicle.angleOfCamPivotWhileOperating,rot.eulerAngles.y,rot.eulerAngles.z);
//			//			Player.inst.pivot.rotation = rot;
//		}
	}

	Vehicle VehicleNearby(){
		foreach(Collider col in Physics.OverlapSphere(transform.position,seekRadius)){
//			// commented Debug.Log("col:"+col);
			Vehicle v = col.GetComponent<Vehicle>();
			if (!v) v = col.GetComponentInParent<Vehicle>();
			if (v){
				Vector3 dirToVehicle = col.transform.position - transform.position;
//				float angleInFrontOfPlayer = 45;
//				float angleToVehicle = Mathf.Abs(Vector3.Angle(transform.forward,dirToVehicle));
//				// commented Debug.Log("angle 1,2:"+angleInFrontOfPlayer+","+angleToVehicle);
//				if (angleToVehicle < angleInFrontOfPlayer){
					return v;
//				}
				
			}
		}
		return null;
	}

	void PlayerGetInVehicle(Vehicle v){
		if (Player.inst.gameObject.GetComponent<PlayerUnderwaterController>().playerUnderwater) {
			Player.inst.gameObject.GetComponent<PlayerUnderwaterController>().SetPlayerUnderwater(false);
		}
		PlayerGadgetController.inst.EquipGadget(PlayerGadgetController.inst.gadgetThrow);

		PlayerNowMessageWithBox.inst.Display("Press W to drive. Press F to exit.",v.icon,transform.position);
//		SMW_GF.inst.SetLayerRecursively(v.gameObject,LayerMask.NameToLayer("DontCollideWithPlayer"));
		if (v.gameObject.layer != LayerMask.NameToLayer("DontCollideWithAnything"))v.gameObject.layer = LayerMask.NameToLayer("DontCollideWithPlayer");
		foreach(Collider c in v.GetComponentsInChildren<Collider>()){
			if (c.gameObject.layer != LayerMask.NameToLayer("DontCollideWithAnything")) c.gameObject.layer = LayerMask.NameToLayer("DontCollideWithPlayer");
		}
		if (v.playerInvisibleWhileOperating){
			Player.inst.HidePlayer();
		}
//		// commented Debug.Log("getin");
		v.canControl = true;
		v.PlayerGetInVehicle();
		InGameHUD.inst.crosshairs.SetActive(false);
		StopAllCoroutines();
//		// commented Debug.Log("getting in v;"+v);
		PlayerCamera.inst.SetCameraMode(CameraMode.Vehicle,v);
//		playerOperatingVehicle = true;
		if (v.GetComponentInChildren<MouseLook>()) v.GetComponentInChildren<MouseLook>().enabled = true;
//		BoatController bc = v.GetComponent<BoatController>();

//		Player.inst.transform.position = v.operatingVehicle.position;
//		Player.inst.transform.rotation = v.operatingVehicle.rotation;
		Player.inst.SetPosition(v.operatingVehicle);
//		Player.inst.transform.parent = v.operatingVehicle.transform;
		Player.inst.GetComponent<CharacterController>().enabled = false;

		if (v.position == PlayerPositionWhileOperatingVehicle.Sitting){
			MascotAnimatorController.inst.Driving(true);
		}
		MascotAnimatorController.inst.Swimming(false);
		MascotAnimatorController.inst.OperatingVehicle(true);
//		Player.inst.FreezePlayerFreeLook();
		Player.inst.FreezePlayer();

//		AudioSource aud = v.GetComponent<AudioSource>();
		v.HandleAudio();

		Inventory.inst.InventoryAvailable(false);
		Inventory.inst.HideBelt();
		PlayerCamera.inst.transform.parent = v.transform;
	} 

//	IEnumerator VehicleAudioRunning(AudioSource aud, float delay, AudioClip clip){
//		yield return new WaitForSeconds(delay * .85f);
//		aud.clip = clip;
//		aud.loop = true;
//
//		aud.Play();
//	}
//
//	public void PlayerGetOutOfVehicle(){
////		if (currentVehicle)	PlayerGetOutOfVehicle(currentVehicle);
//	}
//
	public void PlayerGetOutOfVehicle(){
		
		if (currentVehicle == null) return;
		InGameHUD.inst.crosshairs.SetActive(true);
		Inventory.inst.InventoryAvailable(true);
		Inventory.inst.ShowBelt();
		Player.inst.Unparent();
		Player.inst.GetComponent<CharacterController>().enabled = true;
		currentVehicle.PlayerGetOutOfVehicle();
		PlayerCamera.inst.SetCameraMode(CameraMode.Normal,currentVehicle);
		currentVehicle.StopVehicle(); 
//		// commented Debug.Log("Trying to get out of;"+v.name);
		Player.inst.ShowPlayer();
		StopAllCoroutines();
//		PlayerCamera.inst.SetCameraMode(CameraMode.Normal,null);
//		playerOperatingVehicle = false;
		if (currentVehicle.GetComponentInChildren<MouseLook>()) currentVehicle.GetComponentInChildren<MouseLook>().enabled = false;

		currentVehicle.canControl = false;
//		Inventory.inst.UpdateBeltSelection();





		MascotAnimatorController.inst.Driving(false);
		MascotAnimatorController.inst.Swimming(false);

		Player.inst.UnfreezePlayer();
//		Player.inst.UnfreezePlayerFreeLook();

		if (currentVehicle.CanMove()){
//			// commented Debug.Log("stop;"+v);
			currentVehicle.VehicleStopSound();

		}


		StartCoroutine(SetColliderAfterSeconds(currentVehicle,.01f)); // ugh, wait one frame beacuse collider layer changes happen BEFORE the end of the frame but transform position happens AFTER -- so we are colliding with the car before we move outside of it
		currentVehicle = null;
		PlayerCamera.inst.transform.parent = Player.inst.transform;
		PlayerCamera.inst.ResetPositionToStart();

	}

	IEnumerator SetColliderAfterSeconds(Vehicle v, float s){
		yield return new WaitForSeconds(s);
		if (v){
			foreach(Collider c in v.GetComponentsInChildren<Collider>()){ 
				c.gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}
		//		SMW_GF.inst.SetLayerRecursively(v.gameObject,LayerMask.NameToLayer("Default"));

	}

	public void SetPlayerVehicleState(PlayMode mode){
		if (mode == PlayMode.Editor){
			
			PlayerGetOutOfVehicle();
		}
	}
}
