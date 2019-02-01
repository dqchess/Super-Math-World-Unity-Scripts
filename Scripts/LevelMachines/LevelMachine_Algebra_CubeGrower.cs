using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_Algebra_CubeGrower : MonoBehaviour {

//	int iterationIndex = 0;
	public CCText equationText;
	public CCText iterationText;
	GameObject heldNumber;
	GameObject exponentBall;
	int exponentReached = 0;
	List<GameObject> xBlocks = new List<GameObject>();
	List<GameObject> yBlocks = new List<GameObject>();
	List<GameObject> zBlocks = new List<GameObject>();
//	List<GameObject> createdNumbers = new List<GameObject>();
	Dictionary<GameObject,Vector3> createdNumbers = new Dictionary<GameObject,Vector3>();

//	List<Transform> grabbedExponnentBalls = new List<Transform>();




	void Reset(){
		
	}

	void OnTriggerEnter(Collider other){
		NumberInfo ni = other.GetComponent<NumberInfo>();
		AlgebraInfo ai = other.GetComponent<AlgebraInfo>();

		if (ai){
			if (heldNumber) {
				Destroy(heldNumber);
				ClearBlocks();
			}
			heldNumber = other.gameObject;
			heldNumber.transform.parent	= transform;
			if (heldNumber.GetComponent<Rigidbody>()) {
				Destroy(heldNumber.GetComponent<Rigidbody>());
			}

//			PopulateBlocks(ai);
		}
		
	}

	bool grabbing = true;
	bool movingNumbersToTargetPos = false;
	float exponentiateTimer = 0f;
	float exponentEatInterval = 3f;
	float timeTillReset = 0f;
	void Update(){
		if (heldNumber){
			if (grabbing){
				float grabSpeed = 1f;
				heldNumber.transform.position = Vector3.Lerp(heldNumber.transform.position,transform.position,grabSpeed * Time.deltaTime);
				if (Vector3.Distance(heldNumber.transform.position,transform.position) < .1f){
					grabbing = false;
					heldNumber.transform.position = transform.position;

				}
				
			}

			if (heldNumber.GetComponent<AlgebraInfo>().exponentBalls.Count > 0){
				exponentiateTimer -= Time.deltaTime;
				if (exponentiateTimer < 0){
					exponentiateTimer = exponentEatInterval;
					PullExponentBallFromAlgebraNumber(heldNumber.GetComponent<AlgebraInfo>());
				}
			}
		}

		if (exponentBall){
			float grabSpeed = 1;
			exponentBall.transform.position = Vector3.Lerp(exponentBall.transform.position,transform.position,Time.deltaTime * grabSpeed);
			if (Vector3.Distance(exponentBall.transform.position,transform.position) < .1f){
				Destroy(exponentBall.gameObject);
				Exponentiate();
			}
		}

		if (movingNumbersToTargetPos){
			float lerpSpeed = 1f;
			bool finished = true;
			float cutoffDist = .1f;
			foreach(KeyValuePair<GameObject,Vector3> kvp in createdNumbers){
				kvp.Key.transform.position = Vector3.Lerp(kvp.Key.transform.position,kvp.Value,Time.deltaTime * lerpSpeed);
				if (Vector3.Distance(kvp.Key.transform.position,kvp.Value) > cutoffDist){
					finished = false;
				}

			}
			if (finished) {
				foreach(KeyValuePair<GameObject,Vector3> kvp in createdNumbers){
					kvp.Key.transform.position = kvp.Value;
				}
				movingNumbersToTargetPos = false;
			}
		}

		if (createdNumbers.Count > 0){
			timeTillReset -= Time.deltaTime;
			if (timeTillReset < 0){
				ClearBlocks();
			}
		}




	}

	void PullExponentBallFromAlgebraNumber(AlgebraInfo ai){
		
		if (ai.exponentBalls.Count > 0 && !exponentBall){
			exponentBall = ai.exponentBalls[0];
			ai.exponentBalls.RemoveAt(0);
		}
	}

	void ClearBlocks() {
		foreach(KeyValuePair<GameObject,Vector3> kvp in createdNumbers){
			Destroy(kvp.Key);
		}
		createdNumbers.Clear();
		exponentReached = 0;
		Destroy(heldNumber);
	}



	void Exponentiate(){
		timeTillReset = 20f;

		movingNumbersToTargetPos = true;
		exponentReached++;
		float numberScale = 3f;
		int num = heldNumber.GetComponent<NumberInfo>().fraction.numerator;
		if (exponentReached == 1){
			for (int i=0;i<num;i++){
				// Make dictionary of ones and targetpositions and lerp them there in a fun way
				GameObject one = NumberManager.inst.CreateNumber(new Fraction(1,1),transform.position,NumberShape.Cube);
				Vector3 targetPos = transform.position + (transform.right * i) * numberScale;
				one.transform.parent = transform;
				Destroy(one.GetComponent<Rigidbody>());
				Destroy(one.GetComponent<PickUppableObject>());
				one.transform.localScale = Vector3.one * numberScale;
				createdNumbers.Add(one,targetPos);
			}
		} else if (exponentReached == 2){
			for (int i=0;i<num;i++){
				for (int j=1;j<num;j++){
					GameObject one = NumberManager.inst.CreateNumber(new Fraction(1,1),transform.position + transform.right * i * numberScale,NumberShape.Cube);
					Vector3 targetPos = transform.position + (transform.right * i + transform.forward * j) * numberScale;
					one.transform.parent = transform;
					Destroy(one.GetComponent<Rigidbody>());
					Destroy(one.GetComponent<PickUppableObject>());
					one.transform.localScale = Vector3.one * numberScale;
					createdNumbers.Add(one,targetPos);
				}
			}
		} else if (exponentReached == 3){
			for (int i=0;i<num;i++){
				for (int j=0;j<num;j++){
					for (int k=1;k<num;k++){
						GameObject one = NumberManager.inst.CreateNumber(new Fraction(1,1),transform.position + (transform.right * i + transform.forward * j) * numberScale,NumberShape.Cube);
						Vector3 targetPos  = transform.position + (transform.right * i + transform.forward * j + transform.up * k) * numberScale;
						one.transform.parent = transform;
						Destroy(one.GetComponent<Rigidbody>());
						Destroy(one.GetComponent<PickUppableObject>());
						one.transform.localScale = Vector3.one * numberScale;
						createdNumbers.Add(one,targetPos);
					}
				}
			}
		}

	}

	void OnPlayerCollect(){
		ClearBlocks();
	}

}
