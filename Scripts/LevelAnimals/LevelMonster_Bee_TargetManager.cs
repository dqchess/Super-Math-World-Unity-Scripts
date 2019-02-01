using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animal_Bee_TargetManager : MonoBehaviour {

	public Transform[] targets;
	Dictionary<Animal_Bee,int> targetIndices = new Dictionary<Animal_Bee,int>();
	// Use this for initialization
	void Start () {

	}
	bool targetSet = false;
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.B)){
			targetSet = true;
			targetIndices.Clear();
			foreach(Animal_Bee bee in transform.parent.GetComponentsInChildren<Animal_Bee>()){
				targetIndices.Add (bee,0);
			}
		}
		if (targetSet){
//			List<Animal_Bee> toRemove = new List<Animal_Bee>();
			foreach(Animal_Bee bee in transform.parent.GetComponentsInChildren<Animal_Bee>()){
				if (targetIndices.ContainsKey(bee)){
					bee.SetTarget(targets[targetIndices[bee]],TargetType.Home);
					float minDistSqr = 50f;
					if (targetIndices[bee] == targets.Length - 1){
						// for the last target, let them hit farther away
						minDistSqr = 200f;
					}
					if (Vector3.SqrMagnitude(bee.transform.position-targets[targetIndices[bee]].transform.position) < minDistSqr){
						targetIndices[bee] ++;
						if (targetIndices[bee] > targets.Length - 1) {
							targetIndices.Remove(bee);
						}
					}
				}
			}
			if (targetIndices.Count == 0) targetSet = false;
		}
	}
}
