using UnityEngine;
using System.Collections;

public class Animal_Bird : Animal {

	#region UserEditable



	// upoffset 	}
	public override void OnLevelBuilderObjectPlaced(){
		GetComponent<Rigidbody>().isKinematic = true;
	}


	public override void OnGameStarted(){ 
		base.OnGameStarted();
		base.Init();
		GetComponent<Rigidbody>().isKinematic = false;
		SetUpPerchPosition();
	}
	#endregion

	GameObject perchPosition;
	GameObject abovePerchPosition;
	public Transform tree;
	public Animator anim;

	// Use this for initialization

//
//	public override void Start () {
//		base.Start();
//
//	}
//
	public void SetUpPerchPosition(){
//		// commented Debug.Log("setting up perch on bird:"+name);
		if (perchPosition) Destroy(perchPosition);
		if (abovePerchPosition) Destroy(abovePerchPosition);
		perchPosition = new GameObject("perchpos");
		abovePerchPosition = new GameObject("above perchpos");
		perchPosition.transform.position = transform.position;
		perchPosition.transform.rotation = transform.rotation;
		abovePerchPosition.transform.position = transform.position + Vector3.up * 15;
		tree.transform.position = transform.position;
		tree.transform.parent = null;
//		// commented Debug.Log("perch position:"+perchPosition.transform.position+", my pos:"+transform.position);

		// Future: Dynamic purch positions.

	}


	bool perched = false;
	override public void StayNearHome() {
		if (!perchPosition){
//			// commented Debug.Log("no perch pos on:"+name);
			return;
		}
		// If we were close to our perch position, maybe end up perching there?
		float perchRadius = 150f;
		float flyAbovePerchRadius = 35f;
		float distToPerchPosition = Vector3.Distance(transform.position,perchPosition.transform.position);
		if (distToPerchPosition < perchRadius){
//			// commented Debug.Log("dist to perch:"+distToPerchPosition+", fly above rad:"+flyAbovePerchRadius);
			if (distToPerchPosition > flyAbovePerchRadius){
				SetTarget(abovePerchPosition.transform,TargetType.Home);
			} else {
				SetTarget(perchPosition.transform,TargetType.Home);
			}
//			// commented Debug.Log ("target set as perch");
		} else {
//			// commented Debug.Log ("base stay near home.");
			base.StayNearHome();
		}
	}

	public override void MoveForwards() {

		if (underwater) forceThisFrame += (moveForce * transform.up);
		base.MoveForwards();
	}

	void Perch(){
//		// commented Debug.Log ("perched!");
		if (LevelBuilder.inst.levelBuilderIsShowing) return;
		GetComponent<Rigidbody>().drag = 0.9f;
		perched = true;
		anim.SetBool("Perched",true);
		LoseTarget ();
//		// commented Debug.Log("perching at;"+perchPosition.transform.position);
		transform.position = perchPosition.transform.position;
//		transform.rotation 	= perchPosition.transform.rotation;

		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().isKinematic = true;
		// anim.Stop();
	}

	void UnPerch(){
		if (perched){
//			// commented Debug.Log("unpurche");
			anim.SetBool("Perched",false);
//			// commented Debug.Log ("unperched");
			// anim.Play();
			perched = false;
			GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	float soarTimer = 0;
	public override void AnimalUpdate () {
		
		if (animalDisabled) return;
		if (GameManager.inst.GameFrozen) return;
		TryUnperch();

		// Pause if just ate
		eatTimeout -= Time.deltaTime;
		if (eatTimeout > 0) return;

		PushUpwardsIfUnderwater();

		if (target != null) {
			if (target.type == TargetType.Home) CheckNearPerch();
		}


	}

	void TryUnperch(){
		if (perched) {
			TryGetTarget();
			CheckTargetActive();
			RotateTowardsTarget(perchPosition.transform.forward);
			if (target != null && target.type != TargetType.Home) {
				UnPerch();
			}
			return;
		}
	}

	void CheckNearPerch(){
//		// commented Debug.Log("checking near perch");
		SetTarget(perchPosition.transform,TargetType.Home);
		GetComponent<Rigidbody>().drag = 3;
		if (target.transform == perchPosition.transform){
			float perchTriggerRadius = 3;
			float d = Vector3.Distance(target.transform.position,transform.position);
//			// commented Debug.Log("d:"+d+", perchtrigraD:"+perchTriggerRadius);
			if (d < perchTriggerRadius){
				Perch();
			}
		} else if (target.transform == abovePerchPosition.transform){
			float flyAbovePerchRadius = 35f;
			float distToPerchPosition = Vector3.Distance(transform.position,perchPosition.transform.position);
			if (distToPerchPosition > flyAbovePerchRadius){
				SetTarget(abovePerchPosition.transform,TargetType.Home);
			} else {
				SetTarget(perchPosition.transform,TargetType.Home);
			}

		}
	}

	void PushUpwardsIfUnderwater(){
		float buoyancyForce = 100;
		if (underwater) GetComponent<Rigidbody>().AddForce(Vector3.up * buoyancyForce);
	}




	float eatTimeout = 0;
	public override void Eat(GameObject o){
		anim.SetTrigger("Eat");
		// anim.clip = squawk;
		// anim.Play();
		base.Eat(o);
	}

	IEnumerator CrossfadeToFlyAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		// anim.CrossFade(fly.name,0.1f);
	}



	public override void IdleRotate(){
//		return;

		base.IdleRotate();
	}

	public override void SeekTarget(){
		base.SeekTarget();
	}

	public override void LoseTarget(string source=""){
//		// commented Debug.Log("bird lose target.");
		base.LoseTarget(source);
		if (!perched){
			if (perchPosition){
				SetTarget(perchPosition.transform,TargetType.Home);
			}
		}
	}

}
