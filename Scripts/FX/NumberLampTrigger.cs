using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumberLampTrigger : MonoBehaviour {

	List<NumberInfo> nums = new List<NumberInfo>();
	public NumberInfo lampNumber;
	void OnTriggerEnter(Collider other){
//		// commented Debug.Log("got;"+other);
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni){
			nums.Add(ni);

		}
	}

	void OnTriggerExit(Collider other){
		NumberInfo ni = other.GetComponent<NumberInfo>();
		if (ni){
			if (nums.Contains(ni)){
				nums.Remove(ni);
				ni.SetColor();
				ni.gameObject.layer = LayerMask.NameToLayer("Default");
				ni.greaterThanCombine = 0; // normal combineability.
				ni.lessThanCombine = 2;
			}
		}
	}

	float interval = 0.3f;
	float t = 0f;
	void Update(){
		t -= Time.deltaTime;
		if (t < 0){
			t = interval + Random.Range(0.1f,0.4f); // add random offset to prevent many lights from checking at one frame
			List<NumberInfo> rm = new List<NumberInfo>();
//			Collider[] cols = Physics.OverlapSphere(transform.position,transform.localScale.x/2f);
//			foreach(Collider col in cols){
//				NumberInfo ni = col.GetComponent<NumberInfo>();
//				if (ni) nums.Add(ni);
//			}
			foreach(NumberInfo ni in nums){
				if (!ni) {
					rm.Add(ni);
				} else {
					if (Fraction.Equals(ni.fraction, lampNumber.fraction)){
						ni.SetColor();
						ni.gameObject.layer = LayerMask.NameToLayer("Default");
//						ni.greaterThanCombine = 1; // don't allow further combinations.
//						ni.lessThanCombine = 0;
						MonsterAIRevertNumber mairn =ni.GetComponent<MonsterAIRevertNumber>();
						if (mairn){
							mairn.SetNumber(lampNumber.fraction); //
							mairn.RevertNumber();

						}

					} else {
						ni.SetColor(true); // transparentify
						ni.gameObject.layer = LayerMask.NameToLayer("TransparentNumber");
					}
				}
			}
			foreach(NumberInfo ni in rm){
				nums.Remove(ni);
			}
		}
	}
}
