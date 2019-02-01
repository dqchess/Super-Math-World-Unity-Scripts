using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachinePickup_NumberGenerator : MonoBehaviour {

	public Transform firePos;
	public bool addToPlayerNumber = false;
	public Fraction number=new Fraction(1,1);
	public float interval=3;
	public float fireVelocity = 15;
	public bool faceLeft=false;
	public float scale=2.5f;
	float timer=0;
	Transform bucket;
	float forbidActionForSeconds=1;
	public float timeout= 40;
	public bool numbersAreEnemyRockets=false;
	public List<GameObject> mynums = new List<GameObject>();
	public bool rotate=false;

	void Start(){
		GameObject o = new GameObject();
		bucket = o.transform;
	}
	void Update(){
		forbidActionForSeconds-=Time.deltaTime;
		timer-=Time.deltaTime;
		if (timer<0){
			timer=interval;
			Fire();
		}

		foreach(Transform t in bucket){
			if (t.GetComponent<Collider>()){
				if (t.GetComponent<Rigidbody>()){
					if (!t.GetComponent<Rigidbody>().isKinematic){
						if (SMW_GF.inst.ColliderWasNumber(t.GetComponent<Collider>())) {
						    t.gameObject.GetComponent<Rigidbody>().velocity = -transform.right*fireVelocity;
						}
					}
					NumberInfo ni = t.GetComponent<NumberInfo>();
					if ((ni.fraction.numerator > 0 && faceLeft == true)
						|| (ni.fraction.numerator < 0 && faceLeft == false)){
							t.parent=null; // drop numbers of the wrong sign
//							ni.neverEats=true; // don't let this number interfere with the number line flow
					}
			    }
		    }

		}
	}

	public void PlayerTouched(){

//		Destroy(gameObject);
//		Destroy(collider);
		forbidActionForSeconds = 2f;
		float throwSpacing = 15f;
		transform.position += Camera.main.transform.forward*30;
//		Vector3.OrthoNormalize(transform.position-Player.inst.transform.position)*throwSpacing;
		transform.rotation = Quaternion.identity;
//		transform.LookAt(Player.inst.transform,Vector3.up);
		if (faceLeft){
			transform.Rotate(Vector3.up,180);
//			transform.rotation.eulerAngles = new Vector3(0,180,0);
		}
	}

	void OnPlayerAction(){

	}

	void Fire(){

		AudioManager.inst.PlayNumberFire(transform.position,1);
//		AudioManager.inst.PlayPlungerSuck(transform.position,1,1
		Vector3 pos = firePos.position;
		GameObject num = NumberManager.inst.CreateNumber(number,pos,NumberShape.Cube);

		mynums.Add(num);
		EffectsManager.inst.CreateSmokePuffBig(pos+transform.right*4,firePos.forward*5+Vector3.up,Random.Range(8,15));
		num.transform.localScale=Vector3.one*scale;
		num.GetComponent<Rigidbody>().useGravity=false;
		num.GetComponent<Rigidbody>().isKinematic=false;
		if (rotate) num.AddComponent<Rotate>();
		num.transform.parent=bucket.transform;
		num.GetComponent<Rigidbody>().drag=0;
		num.GetComponent<Rigidbody>().angularDrag=0;
		if (numbersAreEnemyRockets){
//			num.AddComponent<DestroyOnPlayerTouch>();
//			DestroyNumbersInFiringLine d= num.AddComponent<DestroyNumbersInFiringLine>();
//			d.line=this;


			NumberInfo ni = num.GetComponent<NumberInfo>();
//			NumberManager.inst.ApplySpikes(ni,true);
//			ni.pickupFlag=false;
//			ni.killPlayerOnContact=true;
			num.AddComponent<EnemyBrick>();
//			AttackPlayerWithLightning l = num.AddComponent<AttackPlayerWithLightning>();
//			l.range=50;
//			GameObject smokeTrail = (GameObject)Instantiate(EffectsManager.inst.smokeTrail,num.transform.position,Quaternion.identity); // add smoke trail
//			smokeTrail.name = "smokeTrail";
//			smokeTrail.transform.parent = num.transform;
//			num.GetComponent<NumberInfo>().doesExplodeOnImpact=true;
		}

		TimedObjectDestructor tod = num.AddComponent<TimedObjectDestructor>();
		tod.autoDestruct=false;
		tod.DestroyNow(timeout);
		
		// make cannon shake on each fire
		float jumpForce=120;
		GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-10,10),Random.Range(30,50),Random.Range(-10,10))*jumpForce);
		if (addToPlayerNumber) {
//			num.AddComponent<CombineOnPlayerTouch>();
			num.layer = 25; // collide with player and numbers only.
		}
	}

	public void HaltFiringForSeconds(int s){
		foreach(GameObject o in mynums){
			Destroy (o);
		}
		mynums.Clear();
		timer = s;
	}

}
