using UnityEngine;
using System.Collections;
//using UnityEngine.Ass

public class PlayerUnderwaterController : MonoBehaviour {

//	public GameObject underwaterCam;
	public VortexEffect cameraVortexEffect;
	public bool playerUnderwater = false;
	public LayerMask waterLayerMask;
	public GameObject underwaterBubbls;
	public ParticleSystem breatheBubblesParticles;
	bool enableVortex = false;
	bool camUnderwater = false;

	public static PlayerUnderwaterController inst;

	public void SetInstance(){
		inst = this;
	}

	void Start(){
		
		SetPlayerUnderwater(false);
		underwaterBubbls.SetActive(false);

	}

//	Bounds waterBounds;
//	int underwaterCount = 1;

	void SetUnderwaterVisualFX(bool underwater){
		camUnderwater = underwater;
		if (enableVortex) cameraVortexEffect.enabled = underwater;
//		Camera.main.GetComponent<UnityStandardAssets.ImageEffects.ScreenOverlay>().enabled = underwater;
		underwaterBubbls.SetActive(underwater);
	}

	void SetUnderwaterAudioFX(bool underwater){
//		Debug.Log("audi");

		if (underwater){			
			BackgroundAudioManager.inst.EnableUnderwaterSound(); // SetAudioAmbiance(BackgroundAudioManager.inst.underwater);
		} else {		
			BackgroundAudioManager.inst.DisableUnderwaterSound();
		}
	}

	float underwaterTimer = 0;
	public void SetPlayerUnderwater(bool underwater, bool force = false){ // use "force" for when player clicked "restart" .. because otherwise would fail on checking player frozen
		if (underwater == playerUnderwater) return;
		if (underwater) {
//			Debug.Log("<color=#00f>freshly underwater!</color>");
			FPSInputController.inst.motor.SetMomentum(Vector3.zero);
			FPSInputController.inst.motor.SetVelocity(Vector3.zero);
		}
		if ( ((underwaterTimer > 0 || Time.timeScale == 0) && !LevelBuilder.inst.levelBuilderIsShowing) || !GameManager.inst.gameStarted) {
//			WebGLComm.inst.Debug("Setplayerunderwater failed, gametsrte:"+GameManager.inst.gameStarted);
			return;
		} 
//		WebGLComm.inst.Debug("Setplayerunderwater:"+underwater+", gametsrte:"+GameManager.inst.gameStarted);
		if (!underwater) {
			SetUnderwaterAudioFX(false);
//			Inventory.inst.UpdateBeltSelection(true);
		}
//		// commented Debug.Log("set under:"+underwater);
		underwaterTimer = .5f;
		playerUnderwater = underwater;
//		// commented Debug.Log("SET underwater:"+playerUnderwater+"");
		PlayerWalkDustParticles.inst.enabled = !underwater;

		if (!Player.frozen || LevelBuilder.inst.levelBuilderIsShowing || force){ // using force here to ensure player underwater set false in fpswe when restart pushed.
			
//			SMW_FPSWE.inst.SetPlayerUnderwater(underwater);
			MascotAnimatorController.inst.Swimming(underwater);
			if (underwater) FPSInputController.inst.motor.inputMoveDirection = Vector3.zero;
		} else {
//			// commented Debug.Log("not set. froze:"+Player.frozen+", leveshow:"+LevelBuilder.inst.levelBuilderIsShowing);
		}
//		// commented Debug.Log("setting aud.."+underwater);

	}

	float waterHeight = 0;
	float camTimer = 0;
	float deltaSurfaceY = 0;
	float camHeight = -0.2f;
	float playerHeight = 1.7f;
	float underwaterChangeTimer = 0;
	void Update(){
		if (!GameManager.inst.gameStarted) return;
//		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		underwaterTimer -= Time.deltaTime;

		// Cam FX occur instantly if above/below UTWC
		deltaSurfaceY = Camera.main.transform.position.y - UltimateToonWaterC.inst.getHeightByPos(new Vector2(Camera.main.transform.position.x,Camera.main.transform.position.z)); // Negative values mean underwater
		if (deltaSurfaceY < camHeight && !camUnderwater){
			SetUnderwaterVisualFX(true);
		} else if (deltaSurfaceY >= camHeight && camUnderwater){
			SetUnderwaterVisualFX(false);
		}

		if (Player.frozen) return;
		// Player FX occur but there is a "Dead zone" for not switching from one medium to the other.
		// If player is fully underwater there is no chance of change..
		// If player is near the surface, gradually lerp player position towards the surface to prevent in/out popping.

		float deadZone = 1;
		waterHeight = UltimateToonWaterC.inst.getHeightByPos(new Vector2(Player.inst.transform.position.x,Player.inst.transform.position.z));
		deltaSurfaceY = (Player.inst.transform.position.y + playerHeight) - waterHeight; // Negative values mean underwater
		float lerpZoneHeight = 3f;
		bool lerping = false;
		if (deltaSurfaceY < lerpZoneHeight && deltaSurfaceY > 0){
//			// commented Debug.Log("deltasurfy:"+deltaSurfaceY);
			float lerpSpeed = 2;
			lerping = true;
			Vector3 targetPos  = new Vector3(Player.inst.transform.position.x,waterHeight - playerHeight*2f,Player.inst.transform.position.z);
			if (playerUnderwater) {
//				Player.inst.transform.position = Vector3.Lerp(Player.inst.transform.position,targetPos,Time.deltaTime*lerpSpeed);
//				Debug.Log("lerp to:"+targetPos.y);
			}
		}

		underwaterChangeTimer -= Time.deltaTime;
		if (underwaterChangeTimer < 0){
			underwaterChangeTimer = 0.06f; //Random.Range(0.3f,0.7f);
//			bool standingNearTerrain = false;
//			RaycastHit hit = new RaycastHit();
//			if (Physics.Raycast(Player.inst.transform.position,Vector3.down,out hit,10,SceneLayerMasks.inst.terrainOnly)){
//				if (hit.distance < 1){
//					standingNearTerrain = true;
//				}
//			}
			if (Utils.IsInsideWaterCube(Player.inst.transform)){
				SetPlayerUnderwater(true);
			} else if ( deltaSurfaceY < -deadZone){
				SetPlayerUnderwater(true);
				SetUnderwaterAudioFX(true);
			} else if (deltaSurfaceY > deadZone){
				SetPlayerUnderwater(false);
				SetUnderwaterAudioFX(false);
			}
		}

//		bubbleSoundTimer -= Time.deltaTime;
//		timer -= Time.deltaTime;
		camTimer -= Time.deltaTime;
		if (camUnderwater){
			float warbleSpeed = 0.3f;
			float warbleAngle = 10f;
			if (enableVortex) cameraVortexEffect.angle = Mathf.Sin(Time.time * warbleSpeed) * warbleAngle;
			BubblesFX();
		}



//		ToggleFog();
	}

	float bubblesTimer = 0;
	void BubblesFX(){
		bubblesTimer -= Time.deltaTime;
		float thresh = 3;
		if (bubblesTimer < 0){
			if (FPSInputController.inst.motor.inputMoveDirection.magnitude > thresh){
				bubblesTimer = Random.Range(1.5f,4.5f);
				int numbubs = Random.Range (10,50);
				breatheBubblesParticles.Emit(numbubs);
				AudioManager.inst.PlayBubbles(transform.position);
//				for (int i=0; i<numbubs; i++){
//				}
			}

		}
	}

//	public bool PlayerCanSwimUp(){
//		float playerMustBeThisFarUnderwaterToJump = -0.5f;
//		return deltaSurfaceY < playerMustBeThisFarUnderwaterToJump || Utils.IsInsideWaterCube(Player.inst.transform);
//	}

//	public float GetPlayerUnderwaterJumpSpeed(){
//		
//	}

}
