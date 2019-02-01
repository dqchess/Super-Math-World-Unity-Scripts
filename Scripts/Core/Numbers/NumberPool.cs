using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberPool : MonoBehaviour {


	public static NumberPool inst;
	public List<GameObject> pooledNumbers = new List<GameObject>();
	int maxPoolSize = 0;

	public void SetInstance(){
		inst = this;
	}

	int indexX = 0;
	int indexY = 0;
	int indexZ = 0;
	int spacing = 5;
	int maxEditorStackSize = 50;

	public GameObject GetFromPool(){
		if (pooledNumbers.Count == 0) return null;
		GameObject ret = pooledNumbers[0];
		pooledNumbers.Remove(ret);
		return ret;
	}
	float timer = 0;
	float interval = .1f;
	void Update(){
		timer -= Time.deltaTime;
		if (timer < 0){
			timer = interval;
			CreateAndAddNumberToPool(10);

		}
	}

	void CreateAndAddNumberToPool (int qty) {
		for (int i=0;i<qty;i++){
			if (pooledNumbers.Count < maxPoolSize){
				Vector3 p = new Vector3(-10000 - indexX * spacing,-1000 - indexY * spacing,-10000 - indexZ * spacing);
				indexX++;
				if (indexX > maxEditorStackSize){
					indexX = 0;
					indexY ++;
				}
				if (indexY > maxEditorStackSize){
					indexY = 0;
					indexZ ++;
				}
				GameObject poolNum = (GameObject)Instantiate(NumberManager.inst.numberCubePrefab); // NumberManager.inst.CreateNumber(new Fraction(1,1),p,NumberShape.Cube);
				poolNum.transform.position = p;
				pooledNumbers.Add(poolNum);
				poolNum.layer = LayerMask.NameToLayer("HideFromSkyCam");
				poolNum.transform.parent = transform;
			}
		}
	}


//	public void AddToPool(NumberInfo ni){
////		// commented Debug.Log("adding to pool:"+o+", s:"+source);
//		if (pooledNumbers.Contains(ni)){
////			// commented Debug.Log("oops, o already in pool!");
//			return;
//		}
//		ni.transform.parent = transform;
//		pooledNumbers.Add(ni);
//
//		Rigidbody rb = ni.GetComponent<Rigidbody>();
//		if (!rb) rb = ni.gameObject.AddComponent<Rigidbody>();
//		rb.isKinematic = true;
////		rb.useGravity = true;
//
////		// commented Debug.Log("placed:"+indexX+","+indexY+","+indexZ);
//	}
//
//	public NumberInfo CreateCubeNumber(){
//		NumberInfo ret = null;
//		if (pooledNumbers.Count > 0){
//			ret = pooledNumbers[0];
//			pooledNumbers.Remove(ret);
//			if (ret){
//				ret.transform.parent = null;
//				MonsterAIRevertNumber marin = ret.GetComponent<MonsterAIRevertNumber>();
//				if (marin) {
//					if (marin.particles) Destroy(marin.particles); // ugh more "init" stuff we're faking because of reasons.
//					Destroy(marin);
//				}
//				ret.GetComponent<NumberInfo>().destroyedThisFrame = false; // TODO other re-initializations that need to happen (since we're now pulling this from a used "pool" of numbers instead of creating from scratch so 'start' isnt' called, other vars maybe set differently than normal init?
//
//			}
//		}
//
//
////		if (ret != null) // commented Debug.Log("Got cube number from pool:"+ret);
//		return ret;
//	}


//
//	public void DestroyOrPool(NumberInfo ni){
//
//		if (ni.myShape != NumberShape.Cube){
//			Destroy(ni.gameObject);
//			return;
//		}
//		if (pooledNumbers.Count < maxPoolSize){
//			AddToPool(ni);
//			ni.transform.parent = transform;
//			ni.OnDestroy();
//		} else {
//			Destroy(ni.gameObject);
//		}
//	}



}
