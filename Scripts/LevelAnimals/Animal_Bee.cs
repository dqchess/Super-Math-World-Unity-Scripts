using UnityEngine;
using System.Collections;

public class Animal_Bee : Animal {
	
//	
//	public AnimationClip fly;
//	public AnimationClip soar;
//	public AnimationClip squawk;
	public Sprite beeIcon;

	#region UserEditableObject


	// upoffset 	}

	public override void OnLevelBuilderObjectPlaced(){ 
		GetComponent<Rigidbody>().isKinematic = true;
	}




	public override void OnGameStarted(){
		base.OnGameStarted();
		Init();
		GetComponent<Rigidbody>().isKinematic = false;
	}



	#endregion

	void Init(){
		if (home) Destroy(home.gameObject);
		GameObject homeO = new GameObject("bee home");
		home = homeO.transform;
		home.position = transform.position;

	}

	// Use this for initialization
//	public override void Start () {
//		Init();
//		base.Start();
//		SetUpAnimations();
		
//	}
	

	
	// Update is called once per frame
	float soarTimer = 0;
	public override void AnimalUpdate () {
		
		if (animalDisabled) return;
		if (Player.frozen) return;
		float sqrMagToPlayer = Vector3.SqrMagnitude(Player.inst.transform.position - transform.position);
		float sqrMagAttackRange = 420f;
		float sqrMagKillRange = 16f;
		if (sqrMagToPlayer < sqrMagAttackRange){
			transform.LookAt(Player.inst.transform);
			float attackSpeed = 30f;
			transform.position = Vector3.MoveTowards(transform.position,Player.inst.transform.position + Vector3.up * 2.5f,Time.deltaTime*attackSpeed);
			if (sqrMagToPlayer < sqrMagKillRange){
				GameManager.inst.ForceRestartLevel("A bee got you! Try to keep away from those.. press Yes to try again.","Bzzz!",beeIcon);
			}
			return;
		}
		
		eatTimeout -= Time.deltaTime;
		if (eatTimeout > 0) return;
		CheckCloseToTargetAndEat();
		PushUpwardsIfUnderwater();
		base.AnimalUpdate();

	}
	
	void PushUpwardsIfUnderwater(){
		float buoyancyForce = 100;
		if (underwater) GetComponent<Rigidbody>().AddForce(Vector3.up * buoyancyForce);
	}

	override public void MoveForwards() {  // Bees should be higher off the ground than the average unit.
		BeeStayAboveGround();
		base.MoveForwards();
	}

	float raycastDownCheckTimer = 0;
	bool closeToGround = false;
	public void BeeStayAboveGround(){
//		if (sleeping) return;
		raycastDownCheckTimer -= Time.deltaTime;
		float distToStayAboveGround = 7.5f;
		if (raycastDownCheckTimer < 0){
			raycastDownCheckTimer = Random.Range(1,3f);
			RaycastHit hit;
			if (Physics.Raycast(new Ray(transform.position,Vector3.down),out hit, distToStayAboveGround)){
//				// commented Debug.Log("hit:"+hit.collider.gameObject.name);
				if (!hit.collider.gameObject.GetComponent<Animal>()){
					closeToGround = true;
				}
			} else {
				closeToGround = false;

			}
		}
		if (closeToGround) {
			float stayOffGroundForce = 1f;
			forceThisFrame += Vector3.up * stayOffGroundForce;
		}
		
	}


	

	float eatTimeout = 0;
	public override void Eat(GameObject o){
//		return;
//		StartCoroutine (CrossfadeToFlyAfterSeconds(1));
		eatTimeout = 4;
		base.Eat(o);
	}
	

	

	
//	public override void IdleRotate(){
//		//		return;
//		//		if (playerHasTarget) return;
//		base.IdleRotate();
//	}
	
	public override void SeekTarget(){
		base.SeekTarget();
	}
	
	//	override public bool IsValidTarget(Transform t){
	//
	//		if (t.GetComponent<Animal_Bird>()) {
	//			return false;
	//		} else return base.IsValidTarget(t);
	////		return true;
	//	}
	
	//	override public void OnPlayerCollect(GameObject o){
	//		base.OnPlayerCollect(o);
	//	}
	//

	void OnCollisionEnter(Collision hit){
		if (hit.collider.GetComponent<Animal>()) return;
//		// commented Debug.Log("bee hit;"+hit.collider.name);
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")){ //.inst.terrainOnly){
//			// commented Debug.Log("hit terrain.");
			Quaternion r = new Quaternion();
			r.eulerAngles = new Vector3(Random.Range(290,335),transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z);
			targetRot = r;
			transform.rotation = targetRot;
		}
	}
}
