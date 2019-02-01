using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_FactorPlatformCannon : MonoBehaviour {

	public GameObject bulletPrefab;
	bool canFire = false;
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
	public List<GameObject> mynums = new List<GameObject>();


	void Start(){
		GameObject o = new GameObject();
		bucket = o.transform;
	}

	void FixedUpdate(){
		forbidActionForSeconds-=Time.deltaTime;
		timer-=Time.deltaTime;
		if (timer<0){
			timer=interval;
			Fire();
		}

		foreach(Transform t in bucket){
			if (t.GetComponent<Collider>()){

				t.position += -transform.right*fireVelocity * Time.deltaTime;

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

	int firecount = 0;
	void Fire(){
		if (!canFire) return;
		firecount += 1;
//		AudioManager.inst.PlayNumberFire(transform.position,1);
//		AudioManager.inst.PlayPlungerSuck(transform.position,1,false,.5f);
		Vector3 pos = firePos.position;
		GameObject num = (GameObject)Instantiate(bulletPrefab);
		num.transform.position = pos;

		EffectsManager.inst.CreateSmokePuffBig(pos+transform.right*4,firePos.forward*5+Vector3.up,Random.Range(8,15));
		num.transform.localScale=Vector3.one*scale;
//		if (rotate) num.AddComponent<Rotate>();
		Color c;
		num.layer = 25; // collide with player and numbers only.
//		// commented Debug.Log ("number numerator % firecount: " + number.numerator + " % " + firecount + " = " + (number.numerator % firecount));
		if (firecount % number.numerator == 0){
			c = Color.white;
		} else {
			c = Color.black;
			num.GetComponent<Collider>().isTrigger = true;
		}


		TimedObjectDestructor tod = num.AddComponent<TimedObjectDestructor>();
		tod.autoDestruct=false;
		tod.DestroyNow(timeout);
		
		// make cannon shake on each fire
		float jumpForce=120;
//		rigidbody.AddForce(new Vector3(Random.Range(-10,10),Random.Range(30,50),Random.Range(-10,10))*jumpForce);
		StartCoroutine(AddToPoolAfterSeconds(interval/2f,num,c));

	}

	IEnumerator AddToPoolAfterSeconds(float s, GameObject num, Color col){
		float c = 50	;
		Material mat = num.transform.Find ("mesh").GetComponent<Renderer>().material;
		mat.color = new Color(0,0,0,0);


		for (int i=0;i<c;i++){
			yield return new WaitForSeconds(s/c);
			mat.color = Color.Lerp(mat.color,col,5);
		}
		if (num) mynums.Add(num);
		num.transform.parent=bucket.transform;
	}

	public void HaltFiringForSeconds(int s){
		foreach(GameObject o in mynums){
			Destroy (o);
		}
		mynums.Clear();
		timer = s;
	}

	public void ReceiveNumber(GameObject other){
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni.fraction.denominator != 1){
			Destroy (other.gameObject);
		} else {
			canFire = true;
			number = ni.fraction;
			Destroy (other.GetComponent<Rigidbody>());
			Destroy (other.GetComponent<Collider>());
//			ni.StopDying();
			other.transform.parent = transform;
			other.transform.localScale = Vector3.one * 7;
		}
	}

}
