using UnityEngine;
using System.Collections;

public class LevelMachineMatrixSeedMaker : MonoBehaviour {

	NumberInfo numberLeft;
	NumberInfo numberRight;
	public Transform startPositionLeft;
	public Transform startPositionRight;
	public Transform endPositionLeft;
	public Transform endPositionRight;
	public Transform seedT;
	public GameObject matrixFloorSeedPrefab;
	bool canAcceptNumbers = true;

	public void CollectNumberRight(GameObject o){
		if (eating || !canAcceptNumbers || !CanCollectNumber(o)) return;
		if (numberRight == null) numberRight = null; // for missing objects.. ugh
		if (numberRight) return;
		NumberInfo ni = o.GetComponent<NumberInfo>();	
		if (ni){
			if (ni.fraction.denominator != 1 || ni.fraction.numerator < 0) return; // pos integers only
			SetNumberRight(ni);
		}
		if (numberRight && numberLeft){
			EatNumbers();
		}
	}

	public void CollectNumberLeft(GameObject o){ // DRY
		if (eating || !canAcceptNumbers || !CanCollectNumber(o)) return;
		if (numberLeft == null) numberLeft = null; // for missing objects.. ugh
		if (numberLeft) return;
		if (seedT.childCount > 0) {
//			if (seedT.GetChild(0).gameObject.activeSelf) return; // if player picked it up, it would be inactive..
			return;
		}
		NumberInfo ni = o.GetComponent<NumberInfo>();
		if (ni){
			if (ni.fraction.denominator != 1 || ni.fraction.numerator < 0) return; // pos integers only
			SetNumberLeft(ni);
		}
		if (numberRight && numberLeft){
			EatNumbers();
		}
	}

	void SetNumberRight(NumberInfo ni){
		numberRight = ni;
		ni.transform.position = startPositionRight.position;
		ni.transform.parent = startPositionRight;
		ni.transform.localScale = Vector3.one * 1.5f;
		ni.transform.GetComponent<Rigidbody>().isKinematic = true;
		ni.gameObject.AddComponent<UseGravityOnPlayerPickup>();
	}

	void SetNumberLeft(NumberInfo ni){ // DRY
		numberLeft = ni;
		ni.transform.position = startPositionLeft.position;
		ni.transform.localScale = Vector3.one * 1.5f;
		ni.transform.parent = startPositionLeft;
		ni.transform.GetComponent<Rigidbody>().isKinematic = true;
		ni.gameObject.AddComponent<UseGravityOnPlayerPickup>();
	}


	bool eating = false;
	float eatTimer = 0f;
	void EatNumbers(){
		eating = true;
		numberLeft.GetComponent<Collider>().enabled = false;
		numberRight.GetComponent<Collider>().enabled = false;
		AudioManager.inst.PlayCartoonEat(transform.position,1,.8f);
		eatTimer = 1.5f;

		
	}


	void Update(){
		if (numberLeft == null) Destroy(numberLeft);
		if (numberRight == null) Destroy(numberRight);
		if (numberLeft){
			if (numberLeft.transform.parent != startPositionLeft){
				numberLeft = null;
			}
		}
		if (numberRight){
			if (numberRight.transform.parent != startPositionRight){
				numberRight = null;
			}
		}
		if (eating && numberLeft && numberRight){
			float eatSpeed = 1f;
			eatTimer -= Time.deltaTime;
			numberRight.transform.position = Vector3.Lerp(numberRight.transform.position,endPositionRight.position,Time.deltaTime * eatSpeed);
			numberLeft.transform.position = Vector3.Lerp(numberLeft.transform.position,endPositionLeft.position,Time.deltaTime * eatSpeed);
			numberRight.transform.localScale = (NumberManager.inst.numberScale * Vector3.one) * Mathf.Min(.6f,eatTimer/2f);
			numberLeft.transform.localScale = (NumberManager.inst.numberScale * Vector3.one) * Mathf.Min(.6f,eatTimer/2f);
			if (eatTimer < 0){
				CreateSeed(numberLeft.fraction.numerator,numberRight.fraction.numerator);
				Destroy(numberLeft.gameObject);
				Destroy(numberRight.gameObject);
				eating = false;
			}
		}
	}
	
	void CreateSeed(int a, int b){
		canAcceptNumbers = false;
//		AudioManager.inst.PlayPlungerSuck(transform.position,1);
		StartCoroutine(MachineSoundAfterSeconds(0.1f));
		StartCoroutine(GenerateSeedAfterSeconds(1.4f,a,b));
	}

	IEnumerator MachineSoundAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		AudioManager.inst.PlayMachineRumble(seedT.position);
	}

	IEnumerator GenerateSeedAfterSeconds(float s, int a, int b){
		yield return new WaitForSeconds(s);
		GameObject seed = (GameObject)Instantiate(matrixFloorSeedPrefab,seedT.position,seedT.rotation);
		seed.transform.parent = seedT;
		MatrixFloorSeed mfs = seed.GetComponentInChildren<MatrixFloorSeed>();
		mfs.SetSizeX(a);
		mfs.SetSizeZ(b);
		mfs.flowerPrefabIndex = Random.Range(0,FlowerManager.inst.flowerPrefabs.Length-1);
		Color randColor = Utils.RandomColor();
		mfs.SetColor(randColor);
		AudioManager.inst.PlayTimerDing();
		canAcceptNumbers = true;
	}

	public bool CanCollectNumber(GameObject o){
		return o.GetComponent<DoesExplodeOnImpact>() == null;
	}
}
