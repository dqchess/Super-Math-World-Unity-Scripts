using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAIBase : NumberInfo, IMuteDestroySound
{
	public Transform myZone;
	bool allowSleep=true;
	public Vector3 initialPosition;

	
	public float playerVisionRange = 40.0f;
//	public float playerLoseVisionRange = 65.0f;
	public float leashLength = 15f;
	public float idleSpeed = 1.0f;
	public float engageSpeed = 5f;
	public float returnToHomeSpeed = 5f;

	protected float sqrDistanceToPlayer = 0;


	float birthDelayBeforeChangingVelocity=2;
	float birthTime=0;
	
	public float distanceFromCameraToSleep = 150;
	
	protected int framesToSkip = 0;
	int frameSkipT = 0;	
	
	public bool stopped = false;
	public bool insideZone = false;
//	public bool monsterSupplement=false;



	public override void Start ()
	{
		base.Start();
		frameSkipT = (int)(Random.value * framesToSkip);
		
		birthTime = Time.time;
		
//		statusTextObject = GlobalVars.inst.gf.SetUpCenteredCCTextObject(true);
//		statusTextObject.transform.localScale *= 2;
//		statusTextObject.transform.position= transform.position + new Vector3(0,transform.localScale.x/2*1.35f,0);
//		SmoothFollow sf = statusTextObject.AddComponent<SmoothFollow>();
//		sf.objectToFollow = transform;
//		statusTextObject.AddComponent<AlwaysFacePlayer>();
//		
//		statusText = statusTextObject.transform.Find("StatusTextObjectChild").gameObject.GetComponent<CCText>(); // bad way to reference. baaad
//		statusText.Text = "";
		

		
//		int layerMask = 1 << LayerMask.NameToLayer("EnemyZone");
		
		
		
		

		initialPosition = myZone.position; 

	}

	float outOfzoneTime = 0f;
	public bool InsideMyZone(){
		if (!myZone) return true;
		return Vector3.Distance(transform.position,myZone.transform.position) < leashLength;
//		bool flag = myZone.GetComponent<Collider>().bounds.Contains(transform.position);
//		if (!flag){
//			outOfzoneTime -= Time.deltaTime;
//			if (outOfzoneTime < 0){
//				transform.position = myZone.transform.position + Utils.FlattenVector(Random.insideUnitSphere);
//			}
//		} else {
//			outOfzoneTime = Random.Range(10,20f);
//		}
//		return flag;
	}



	
	public bool returnToHome=false;
	public virtual void SetReturnToHome(bool flag){
		returnToHome=flag;
	}
	
	public List<MonsterAIBase> GetAllies() {
		if (!transform.parent) return null;
		MonsterAIBase[] comps = transform.parent.GetComponentsInChildren<MonsterAIBase>();
		List<MonsterAIBase> allies = new List<MonsterAIBase>();
		if (null != comps) {
			allies = new List<MonsterAIBase>(comps);
			allies.Remove(this);
		}
		return allies;
	}
	
	public void PushAwayFromAllies() {
		float closed = 7;
		float closeforce = 1.0f;
		if (null != GetAllies ()){
			foreach (MonsterAIBase ally in GetAllies()) {
				Vector3 off = ally.transform.position - transform.position;
				if(off.sqrMagnitude < closed * closed) {
					Vector3 push = off.normalized * -1;
					GetComponent<Rigidbody>().AddForce(push * (closed - off.magnitude) * closeforce, ForceMode.Acceleration);
					
				}
			}
		}
	}
	float idleTimer = 0;
	float returningTimer=0;
	Vector3 randomIdleDir = Vector3.zero;
	public virtual void MonsterUpdate () {
//		Debug.Log("monsterupdate:"+name);
		if (destroyed) { Destroy(this); return; }
		if(stopped) { GetComponent<Rigidbody>().velocity = Vector3.zero; }
		//if(frameSkipT < framesToSkip) { frameSkipT++; }
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		frameSkipT = 0; 
		if (this == null && transform == null) { /// oh god why
			// Seems like UpdateManager still has a ref to this spikey and is calling it after its destroyed. Should do a check there I SUPPOSE
			return;
		}
		Vector3 initialOffset = initialPosition - transform.position;
		if (!InsideMyZone() && returnToHome) {
			returningTimer += Time.deltaTime;
			if (returningTimer > 1){
				Vector3 dir = myZone.position - transform.position;
				float force = returnToHomeSpeed;
				GetComponent<Rigidbody>().AddForce(force*dir,ForceMode.Impulse);

//				// commented Debug.Log("toward zone");
			}
		} else {
		
			sqrDistanceToPlayer = (Player.inst.transform.position - transform.position).sqrMagnitude;
			if(PlayerInRange()){
				EngagePlayer();
			} else {
				IdleRoll();

			}
		}

//		PushAwayFromAllies();
//		StayNearGround();

		StayOnLeash();
	}

	void StayOnLeash(){
		if (Vector3.Distance(transform.position,myZone.position) > leashLength){
//			Debug.Log("leashing:"+name);
			Vector3 dir = (transform.position - myZone.position).normalized;
			transform.position = myZone.transform.position + dir * leashLength;
			SetReturnToHome(true);
		}
	}

	void EngagePlayer(){
		
		// Get friends to engage player, too
		if (transform.parent){
			foreach (MonsterAIBase maib in transform.parent.GetComponentsInChildren<MonsterAIBase>()){
				if (maib != this && !maib.InsideMyZone()) maib.SetReturnToHome(true);
			}
		}

		Vector3 offset = Player.inst.transform.position - transform.position;
		Vector3 dir = offset.normalized;

		float force = engageSpeed;

		float chokePointRadius = 15;
		float playerDistToChokePoint = Vector3.Distance(Player.inst.transform.position,myZone.transform.position);
		float distToPlayer = Vector3.Distance(transform.position,Player.inst.transform.position);
		if (distToPlayer < 5){
			SpikeyGroup sg = GetComponentInParent<SpikeyGroup>();
			if (sg) sg.MovePlayerBack();
		}
		if (playerDistToChokePoint < chokePointRadius){
			if (distToPlayer > 1f) transform.position += dir * 1f;
//			float chokePointFactor = Mathf.Min(2,2/playerDistToChokePoint);
//			force *= chokePointFactor;
		}

		GetComponent<Rigidbody>().AddForce(force*dir,ForceMode.Acceleration);

	}

	void IdleRoll(){
		if(idleTimer < 0) {
			idleTimer = Random.value * 8;
			randomIdleDir = Random.insideUnitSphere;
		}
		idleTimer -= Time.deltaTime;
		GetComponent<Rigidbody>().AddForce(randomIdleDir * idleSpeed, ForceMode.Acceleration);
	}


	void StayNearGround(){
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(transform.position + Vector3.up * -1,Vector3.down,out hit)){
			if (hit.distance > 3) {
//				// commented Debug.Log("Down boy!");
				float downForce = 5000f;
				GetComponent<Rigidbody>().AddForce(Vector3.down * downForce);
			}
		}
	}

	public void OnReceiveExplosion(){
		frameSkipT = -40;
	}

	public Transform GetRandomSibling(){
		return transform.parent.GetChild(Random.Range(0,transform.parent.childCount));
	}
	


	[System.NonSerialized] public bool ignoringPlayer = false;
	bool PlayerInRange(){
		if (ignoringPlayer) return false;
		bool flag = sqrDistanceToPlayer < playerVisionRange * playerVisionRange && Vector3.Distance(transform.position,myZone.position) < leashLength;
//		Debug.Log("Player range?"+flag);
		return flag;
	}


	



	float lastResetTime = 0;
	float resetDelay = 1;


	void ReturningToPool() {
		Destroy (this);
	}

	public bool muteDestroy = false;
	public void MuteDestroy(){
		muteDestroy = true;
	}

	void Awake(){
		if (UpdateManager.inst) UpdateManager.inst.monsters.Add(this);
	}

	bool destroyed = false;
	public override void OnDestroy(){
//		Debug.Log("destroyed:"+name);
		destroyed = true;
		UpdateManager.inst.RemoveMonster(this);
		base.OnDestroy();
	}

}

