using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMachine_Zone_Square_Roots : MonoBehaviour {

	public GameObject ghostPrefab;
	List<Collider> ghosts = new List<Collider>();


	public void OnTriggerEnter(Collider other){
		ClearDeadGhosts();
		if (ghosts.Contains(other)){
			return;
		}

		if (SMW_GF.inst.ColliderWasNumber(other)){
			NumberInfo ni = other.GetComponentInChildren<NumberInfo>();
			float x = Mathf.Sqrt( (float)ni.fraction.numerator / (float)ni.fraction.denominator);
			// commented Debug.Log ("float of " + ni.fraction.numerator / ni.fraction.denominator + " was " +x);
			// commented Debug.Log ("round == " +Mathf.Round(x));
			if ( Mathf.Round (x) == x){


				FreezeNumber(ni,x); // float. Grabbable, free
				ni.gameObject.AddComponent<CubeWarmer>(); // It becomes a number fire.


			} else {
				// commented Debug.Log (" x was : " +x);
				TurnNumberIntoGhost(ni);

			}
		}
		
	}

	void FreezeNumber(NumberInfo ni,float x){
		ni.GetComponent<Rigidbody>().velocity = Vector3.zero;
		ni.GetComponent<Rigidbody>().useGravity = false;
		ghosts.Add(ni.gameObject.GetComponent<Collider>());
		ni.SetNumber(new Fraction((int)x,1)); // Integer result detected, allow it to stay (set number to its square root result
		ni.gameObject.SendMessage ("StopDying",SendMessageOptions.DontRequireReceiver);
	}

	void TurnNumberIntoGhost(NumberInfo ni){
		// commented Debug.Log("NO");
//		GameObject ghost = (GameObject)Instantiate(ghostPrefab,ni.transform.position,ni.transform.rotation);
//		ghost.transform.parent = transform;
//		ghost.AddComponent<MonsterAIGhost>(); // it becomes a bee / ghost / dumbbell.
//
//		ghosts.Add(ghost.GetComponent<Collider>());
//		ghost.GetComponent<NumberInfo>().SetNumber(ni.fraction);
//		Destroy (ni.gameObject);
	}

	void ClearDeadGhosts(){
		List<Collider> toDel = new List<Collider>();
		foreach(Collider g in ghosts){
			if (g == null) {
				toDel.Add(g);
			}
		}
		foreach(Collider c in toDel){
			ghosts.Remove(c);
		}
	}
}
