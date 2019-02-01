using UnityEngine;
using System.Collections;


public class CarController2 : Vehicle {




	public override void OnGameStarted(){
		base.OnGameStarted();
		vc.OnGameStarted();
	}

	public float inputForce = 2000;


	float particleRate = 2f;
	float minSpeedForParticles = 9;
	//	float downForce = 120f;


	public vehicleController vc;
	public override void Start(){
		
		base.Start();
	}



	override public void VehicleStopSound(){
		if (!LevelBuilder.inst.levelBuilderIsShowing){
			AudioManager.inst.PlayVehicleCarStop();
		}
	}


	float carRunningTimer = 0;
	bool carRunningSound = false;
	float underwaterTimer = 0;
	float pitch = 0;
	override public void Update () {
		if (GameManager.inst.GameFrozen) return;
		base.Update();

		if (canControl && energyFrac != null && !underwater){

			// handle sound
			if (carRunningSound){
				carRunningTimer -= Time.deltaTime;
				if (carRunningTimer < 0){
					float deltaPos = Vector3.Magnitude(rp.nowPosition - rp.lastPosition);
					pitch = lowestPitch + deltaPos/6f;
//					carRunningTimer = AudioManager.inst.vehicleCarRunning.length / pitch / 1f;
					GetComponent<AudioSource>().pitch = pitch;

					if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Play();
				}
			}


			vc.inputVertical = Input.GetAxis("Vertical");
			vc.inputHorizontal = Input.GetAxis("Horizontal");
			if (vc.inputHorizontal == 0){
				vc.inputHorizontal = Input.GetAxis("Mouse X");
			}
			float speedFactor = 20000f;
			vc.hp = Mathf.Pow(Mathf.Abs(energyFrac.GetAsFloat()),0.4f) * speedFactor * Mathf.Sign(energyFrac.numerator);
//			// commented Debug.Log("energy frac getasfloat:"+energyFrac.GetAsFloat());

//			float forwardForce = Input.GetAxis("Vertical");

//			float boatSpeed = energyFrac.GetAsFloat() * inputForce;
//			Vector3 f = Vector3.Normalize(new Vector3(transform.forward.x,0,transform.forward.z)) * forwardForce * boatSpeed;
			//				f = new Vector3(f.x,0,f.y); // normalized to be flat
//			GetComponent<Rigidbody>().AddForce(f,ForceMode.Force);

			float effectiveSpeed = Vector3.Distance(rp.nowPosition,rp.lastPosition);


		} else {
			vc.hp = 0;
			if (underwater && PlayerVehicleController.inst.currentVehicle == this){
				underwaterTimer += Time.deltaTime;
				if (underwaterTimer > 5){
					PlayerNowMessage.inst.Display("Your car doesn't work underwater! Press F to exit car, or TAB to restart.",transform.position);
					underwaterTimer = -20;
				}
			}
		}


		// We want volume between .3 and 1.
		float minVolume = 0.3f;
		float maxVolume = 1f;
//		aud.volume = minVolume + delta * (maxVolume - minVolume);
		//		// commented Debug.Log("vol:"+aud.volume);



	}

	public override void EnergyNumberDestroyed(GameObject source){
		base.EnergyNumberDestroyed(source);
	}


	override public void PlayerGetInVehicle(){
		base.PlayerGetInVehicle();
		if (vehicleRigidbody) vehicleRigidbody.drag = 0.5f;
	}
	override public void PlayerGetOutOfVehicle(){
		base.PlayerGetOutOfVehicle();
		if (GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
		if (vehicleRigidbody) vehicleRigidbody.drag = 10;
	}
	override public void SetUnderwater(bool f){
		base.SetUnderwater(f);
		if (f == true){
			carRunningSound = false;
			VehicleStopSound();
//			if (aud.isPlaying){
//				VehicleStopSound();
//				aud.Stop();
//			}
		} else {
			
		}
	}

	override public bool CanMove(){
		if (underwater) return false;
		else return base.CanMove();
	}

	override public void HandleAudio(){
		if (CanMove()){
			AudioManager.inst.PlayVehicleCarStart();
			carRunningSound = true;
			carRunningTimer = 0.85f;

		} else {
			carRunningSound = false;
			AudioManager.inst.PlayVehicleCarStop();
		}
	}

}
