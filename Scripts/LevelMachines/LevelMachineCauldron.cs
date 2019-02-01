using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CollectedNumber {
	public NumberInfo ni;
	public Vector3 dest;
}


public class LevelMachineCauldron : MonoBehaviour {
//	public Dictionary<NumberInfo,Vector3> collectedNumbers = new Dictionary<NumberInfo,Vector3>();
	[SerializeField] public List<CollectedNumber> collectedNumbers = new List<CollectedNumber>();

	public GameObject explosionPrefab;
	public Transform surfaceCenter;

	void Start(){
		
	}

	void AddNumberToCauldron(NumberInfo ni){
//		Debug.Log("Collecting:"+ni.fraction);
		List<NumberInfo> conflictingNumbers = GetConflictingNumbers(ni);
		if (conflictingNumbers.Count > 0){
			// freeze puzzle
			FindObjectOfType<LevelMachineCauldronManager>().puzzleFrozen = true;

			// destroy other numbers
			foreach(LevelMachineCauldron lmc in FindObjectsOfType<LevelMachineCauldron>()){
				lmc.ConflictDetected(conflictingNumbers);
			}



			// raise two numbers and new conflicting number up
			risingNumbers.AddRange(conflictingNumbers);
			Destroy(ni.GetComponent<Collider>());
			Destroy(ni.GetComponent<Rigidbody>());
			risingNumbers.Add(ni);
			foreach(NumberInfo nii in risingNumbers){
//				nii.transform.localScale = Vector3.one * 3.5f;
				nii.SinGrowOnce();
			}

			// lightning fx
			SMW_GF.inst.CreateLightning(risingNumbers[0].transform,risingNumbers[2].transform,5.5f,true);
			SMW_GF.inst.CreateLightning(risingNumbers[1].transform,risingNumbers[2].transform,5.5f,true);

		
			// both numbers break 
			StartCoroutine(BreakOffendingNumbersAfterSeconds(5.6f));

			// explosion

			// reset cauldron
			StartCoroutine(ResetPuzzleAfterSeconds(5.7f));

		} else {
			float sqrDist= Vector3.SqrMagnitude(ni.transform.position - surfaceCenter.transform.position);
			if (ni.GetComponent<PickUppableObject>()){
				Destroy(ni.GetComponent<PickUppableObject>());
			}
			if (sqrDist > 10){
				// if the number is far away from the cauldron (e.g. was just thrown in at the edge), add a bit to the yvalue to avoid the number clipping through the lip of the caudrlon
				ni.transform.position = new Vector3(ni.transform.position.x,surfaceCenter.position.y + 0.6f,ni.transform.position.z);

			}
//			Debug.Log("name:"+name+" send on col mes:");
			FindObjectOfType<NumberFaucetSequential>().OnNumberCollectedIntoCauldron();
			CollectedNumber cald = new CollectedNumber();
			cald.ni = ni;
			cald.dest = GetRandomTargetPos();
			Destroy(ni.GetComponent<CauldronNumber>());
			ni.transform.localScale = Vector3.one * 1.5f;
			collectedNumbers.Add(cald); //ni,GetRandomTargetPos());
			ni.gameObject.layer = LayerMask.NameToLayer("DontCollideWithPlayer");
			ni.GetComponent<Rigidbody>().useGravity = false;
			ni.GetComponent<Rigidbody>().velocity = Vector3.zero;
			ni.GetComponent<Rigidbody>().drag = 1;
		}
	}

	public Vector3 GetRandomTargetPos(){
		return new Vector3(Random.insideUnitCircle.x,0,Random.insideUnitCircle.y);
	}

	List<NumberInfo> risingNumbers = new List<NumberInfo>();

	float getNewNumberDestTimer = 0;
	void Update(){
		foreach(NumberInfo ni in risingNumbers){
			ni.transform.position = Vector3.Lerp(ni.transform.position, new Vector3(ni.transform.position.x,surfaceCenter.position.y+3.5f,ni.transform.position.z),Time.deltaTime);
		}

		int i=0;
		getNewNumberDestTimer -= Time.deltaTime;
		foreach(CollectedNumber cn in collectedNumbers){
			if (getNewNumberDestTimer < 0){
				float targetDestRadius = 2;
				cn.dest = GetRandomTargetPos() * targetDestRadius;
			}
			float y = Mathf.Sin(Time.time + i)/2f; // just at the surface.

			Vector3 targetPosition = surfaceCenter.position + new Vector3(cn.dest.x,y,cn.dest.z);
			float floatSpeed = 70f;
			Vector3 dir = Vector3.Normalize(targetPosition - cn.ni.transform.position) * floatSpeed;
			cn.ni.GetComponent<Rigidbody>().AddForce(dir);

		}
		if (getNewNumberDestTimer < 0){
			getNewNumberDestTimer = Random.Range(3,5f);
		}
	}

	public void OnSurfaceTriggerEnter(GameObject o){
		NumberInfo ni = o.GetComponent<NumberInfo>();
		CauldronNumber cn = o.GetComponent<CauldronNumber>();
		if (ni && cn){
			AddNumberToCauldron(ni);
		}
	}


	List<NumberInfo> GetConflictingNumbers(NumberInfo ni3){
		List<NumberInfo> ret = new List<NumberInfo>();
		foreach(CollectedNumber cn1 in collectedNumbers){
			foreach(CollectedNumber cn2 in collectedNumbers){
				if (cn1.ni == cn2.ni) continue;
				if (Fraction.Equals(Fraction.Add(cn1.ni.fraction,cn2.ni.fraction),ni3.fraction)) {
					// conflict found where two collectedNumbers add up to new number ni3
					// return the two numbers who formed conflict to highlight them, showing player the error
					ret.Add(cn1.ni);
					ret.Add(cn2.ni);
					return ret;
				}
			}
		}
		return ret;
	}

	public void ResetNumbers(List<NumberInfo> preserveTheseNi){
		foreach(CollectedNumber cn in collectedNumbers){
			if (cn != null && cn.ni && cn.ni.gameObject && !preserveTheseNi.Contains(cn.ni)) Destroy(cn.ni.gameObject);
		}
		collectedNumbers.Clear();
	}

	public void ExplosionFX(){
		GameObject explosion = (GameObject)Instantiate(explosionPrefab,transform.position,Quaternion.identity);
	}
	public void ConflictDetected(List<NumberInfo> preserveTheseNi){
		ExplosionFX();
		ResetNumbers(preserveTheseNi);
	}

	IEnumerator BreakOffendingNumbersAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		foreach(NumberInfo ni in risingNumbers){
			if (ni && ni.gameObject) {
				NumberManager.inst.DestroyOrPool(ni);
				EffectsManager.inst.CreateShards(ni.transform.position);
			}
		}
		risingNumbers.Clear();
	}

	IEnumerator ResetPuzzleAfterSeconds(float s){
		yield return new WaitForSeconds(s);
		FindObjectOfType<LevelMachineCauldronManager>().ResetPuzzle();
		
	}

	public void Reset(){
		risingNumbers.Clear();
		collectedNumbers.Clear();
		// shouldn't be anything else.. right?
	}


}
